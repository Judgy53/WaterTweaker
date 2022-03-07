using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterTweaker
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	
	public class ExamplePlugin : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "WaterTweaker";
        public const string PluginVersion = "1.0.0";

        private const string MapWetlandName = "foggyswamp";

        private static ConfigEntry<float> ConfigWetlandWaterOpacity { get; set; }
        private static ConfigEntry<bool> ConfigWetlandWaterPP { get; set; }

        private bool tryApplyTweaksAgain = false;
        private int applyAttempts = 0;

        public void Awake()
        {
            Log.Init(Logger);

			ConfigWetlandWaterOpacity = Config.Bind<float>("WaterTweaker", "WetlandWaterOpacity", 1.0f, "Sets the Opacity of the water in Wetland Aspect (between 0.0 and 1.0).");
            ConfigWetlandWaterOpacity.SettingChanged += OnWaterSettingsChanged;

            ConfigWetlandWaterPP = Config.Bind<bool>("WaterTweaker", "WetlandPostProcessing", true, "Enables Post Processing effects when the camera goes underwater in Wetland Aspect.");
            ConfigWetlandWaterPP.SettingChanged += OnWaterSettingsChanged;

            SceneManager.activeSceneChanged += OnActiveSceneChanged;

			Log.LogInfo(nameof(Awake) + " done.");
        }

        public void Update()
        {
            if(tryApplyTweaksAgain)
            {
                if (TryApplyTweaksWetland() || applyAttempts >= 60)
                {
                    tryApplyTweaksAgain = false;
                    applyAttempts = 0;
                }
                else
                    applyAttempts++;
            }
        }

        private bool TryApplyTweaksWetland()
        {
            List<GameObject> waterGOList = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.ToLower().StartsWith("water plane")).ToList();
            Log.LogInfo($"Applying Wetland Water Tweaks to {waterGOList.Count} objects.");

            foreach(GameObject go in waterGOList)
            {
                //Post processing effects
                Transform childPP = go.transform.Find("PP");
                if (!childPP)
                    return false;

                childPP.gameObject.SetActive(ConfigWetlandWaterPP.Value);

                //Water Opacity
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                if (!renderer)
                    return false;

                Color c = renderer.material.color;
                renderer.material.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(ConfigWetlandWaterOpacity.Value));
            }

            return true;
        }

        private void OnWaterSettingsChanged(object sender, System.EventArgs e)
        {
            if(SceneManager.GetActiveScene().name.StartsWith(MapWetlandName))
                if (!TryApplyTweaksWetland())
                    tryApplyTweaksAgain = true;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
		{
			if(newScene.name.StartsWith(MapWetlandName))
                if(!TryApplyTweaksWetland())
                    tryApplyTweaksAgain = true;
        }
	}
}
