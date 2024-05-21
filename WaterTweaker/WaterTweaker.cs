using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterTweaker
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

    public class WaterTweakerPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "WaterTweaker";
        public const string PluginVersion = "1.2.0";

        private const string MapWetlandName = "foggyswamp";

        public static ConfigEntry<float> ConfigWetlandWaterOpacity { get; set; }
        public static ConfigEntry<bool> ConfigWetlandWaterPP { get; set; }

        private bool tryApplyTweaks = false;
        private bool loopRunning = false;
        private int applyAttempts = 0;
        
        public void Awake()
        {
            Log.Init(Logger);
            
            ConfigWetlandWaterOpacity = Config.Bind("WaterTweaker", "WetlandWaterOpacity", 1.0f, "Sets the Opacity of the water in Wetland Aspect (between 0.0 and 1.0).");
            ConfigWetlandWaterOpacity.SettingChanged += OnWaterSettingsChanged;

            ConfigWetlandWaterPP = Config.Bind("WaterTweaker", "WetlandPostProcessing", true, "Enables Post Processing effects when the camera goes underwater in Wetland Aspect.");
            ConfigWetlandWaterPP.SettingChanged += OnWaterSettingsChanged;

            if (RiskOfOptionsCompat.Enabled)
            {
                RiskOfOptionsCompat.AddOptionStepSlider(ConfigWetlandWaterOpacity, 0.0f, 1.0f, 0.1f, "Wetland Water Opacity");
                RiskOfOptionsCompat.AddOptionCheckbox(ConfigWetlandWaterPP, "Wetland Post Processing");

                RiskOfOptionsCompat.SetModDescription("Allows you to tweak the graphics settings of Wetland Aspect's water.");
            }

            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            Log.LogInfo(nameof(Awake) + " done.");
        }

        public void Update()
        {
            if(tryApplyTweaks && loopRunning == false)
                StartCoroutine("ApplyTweaksCoroutine");

            /*
            if (Input.GetKeyDown(KeyCode.F2) && Run.instance != null)
                DEBUG_GoToWetlandMap();
            //*/
        }

        private IEnumerator ApplyTweaksCoroutine()
        {
            loopRunning = true;
            tryApplyTweaks = false;

            while (applyAttempts < 10)
            {
                if (TryApplyTweaksWetland())
                    break;

                applyAttempts++;
                yield return new WaitForSeconds(0.5f);
            }

            applyAttempts = 0;
            loopRunning = false;
        }

        private bool TryApplyTweaksWetland()
        {
            IEnumerable<GameObject> waterGOList = Resources.FindObjectsOfTypeAll<GameObject>().Where(IsWaterPlaneObject);
            Log.LogInfo($"Trying to apply Wetland Water Tweaks to {waterGOList.Count()} objects.");

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

        private static bool IsWaterPlaneObject(GameObject go)
        {
            //Log.LogDebug(go == null ? "GO_NULL" : (go.name + ' ' + (go.scene == null ? "SC_NULL" : (go.scene.name ?? "SCNAME_NULL"))));
            if (go == null || go.scene == null || go.scene.name == null || go.name == null) return false;

            return go.scene.name.StartsWith(MapWetlandName) && go.name.StartsWith("water plane", StringComparison.OrdinalIgnoreCase);
        }

        private void OnWaterSettingsChanged(object sender, EventArgs e)
        {
            if(SceneManager.GetActiveScene().name.StartsWith(MapWetlandName))
                tryApplyTweaks = true;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if(newScene.name.StartsWith(MapWetlandName))
                tryApplyTweaks = true;
        }

        //Copypasted and modified from DebugToolkit's `next_stage` command
        private void DEBUG_GoToWetlandMap()
        {
            var scenes = SceneCatalog.allSceneDefs.Where((def) => def.cachedName == MapWetlandName);
            if (!scenes.Any())
                return;

            Run.instance.AdvanceStage(scenes.First());
        }
    }
}
