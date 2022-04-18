using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterTweaker
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(CommandHelper))]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

    public class ExamplePlugin : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "WaterTweaker";
        public const string PluginVersion = "1.2.0";

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

            if (RiskOfOptionsCompat.enabled)
            {
                RiskOfOptionsCompat.AddOptionStepSlider(ConfigWetlandWaterOpacity, 0.0f, 1.0f, 0.1f, "Wetland Water Opacity");
                RiskOfOptionsCompat.AddOptionCheckbox(ConfigWetlandWaterPP, "Wetland Post Processing");

                RiskOfOptionsCompat.SetModDescription("Allows you to tweak the graphics settings of Wetland Aspect's water.");
            }

            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            R2API.Utils.CommandHelper.AddToConsoleWhenReady();

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

        [ConCommand(commandName = "watertweaker_opacity", helpText = "Set Opacity of the water in Wetland Aspect. Value must be between 0.0 and 1.0. args[0]=(float)value")]
        private static void CommandOpacity(ConCommandArgs args)
        {
            string arg0 = args.TryGetArgString(0);
            if (!string.IsNullOrWhiteSpace(arg0))
            {
                string sanitizedArg0 = arg0.Trim().Replace(',', '.');
                if (float.TryParse(sanitizedArg0, NumberStyles.Float, CultureInfo.InvariantCulture, out float newValue))
                {
                    if (newValue >= 0.0f && newValue <= 1.0f)
                    {
                        ConfigWetlandWaterOpacity.Value = newValue;
                        Debug.Log($"Water Opacity set to {newValue.ToString(CultureInfo.InvariantCulture)}");
                    }
                    else
                        Debug.LogError("Opacity value out of bounds ! Must be between 0.0 and 1.0");
                }
                else
                    Debug.LogError("Couldn't parse new value as float.");
            }
            else
                Debug.Log($"Current Water Opacity Value: `{ConfigWetlandWaterOpacity.Value.ToString(CultureInfo.InvariantCulture)}`.");
        }

        [ConCommand(commandName = "watertweaker_pp", helpText = "Enables Post processing effects when the camera is under water in Wetland Aspect. Value must be `true`, `false`, `0` or `1`. args[0]=(bool)value")]
        private static void CommandPP(ConCommandArgs args)
        {
            string arg0 = args.TryGetArgString(0);
            if (!string.IsNullOrWhiteSpace(arg0))
            {
                string sanitizedArg0 = arg0.Trim().ToLower();
                if (TryParseBool(sanitizedArg0, out bool newVal))
                {
                    ConfigWetlandWaterPP.Value = newVal;
                    Debug.Log("Water Post Processing effects now " + (newVal ? "enabled." : "disabled."));
                }
                else
                    Debug.LogError("Couldn't parse new value as bool. Value must be `true`, `false`, `0` or `1`");
            }
            else
                Debug.Log($"Post processing effects enabled : `{ConfigWetlandWaterPP.Value}`.");
        }

        internal static bool TryParseBool(string input, out bool result)
        {
            if (bool.TryParse(input, out result))
            {
                return true;
            }

            if (int.TryParse(input, out int val))
            {
                result = val > 0 ? true : false;
                return true;
            }

            return false;
        }
    }
}
