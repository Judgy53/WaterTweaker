using BepInEx;
using R2API.Utils;
using RoR2;
using System.Globalization;
using UnityEngine;

namespace WaterTweaker
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(CommandHelper))]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Judgy.WaterTweaker", BepInDependency.DependencyFlags.HardDependency)]

    public class WaterTweaker_R2APIPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Judgy";
        public const string PluginName = "WaterTweaker_R2API";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();

            Logger.LogDebug(nameof(Awake) + " done.");
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
                        WaterTweakerPlugin.ConfigWetlandWaterOpacity.Value = newValue;
                        Debug.Log($"Water Opacity set to {newValue.ToString(CultureInfo.InvariantCulture)}");
                    }
                    else
                        Debug.LogError("Opacity value out of bounds ! Must be between 0.0 and 1.0");
                }
                else
                    Debug.LogError("Couldn't parse new value as float.");
            }
            else
                Debug.Log($"Current Water Opacity Value: `{WaterTweakerPlugin.ConfigWetlandWaterOpacity.Value.ToString(CultureInfo.InvariantCulture)}`.");
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
                    WaterTweakerPlugin.ConfigWetlandWaterPP.Value = newVal;
                    Debug.Log("Water Post Processing effects now " + (newVal ? "enabled." : "disabled."));
                }
                else
                    Debug.LogError("Couldn't parse new value as bool. Value must be `true`, `false`, `0` or `1`");
            }
            else
                Debug.Log($"Post processing effects enabled : `{WaterTweakerPlugin.ConfigWetlandWaterPP.Value}`.");
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
