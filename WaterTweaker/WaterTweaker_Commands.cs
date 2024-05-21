using RoR2;
using System.Globalization;
using UnityEngine;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]

namespace WaterTweaker
{
    public static class WaterTweaker_Commands
    {
        [ConCommand(commandName = "watertweaker_opacity", helpText = "Set Opacity of the water in Wetland Aspect. Value must be between 0.0 and 1.0. args[0]=(float)value")]
        public static void CommandOpacity(ConCommandArgs args)
        {
            string argValue = args.TryGetArgString(0);

            if (string.IsNullOrWhiteSpace(argValue))
            {
                Debug.Log($"Current Water Opacity Value: `{WaterTweakerPlugin.ConfigWetlandWaterOpacity.Value.ToString(CultureInfo.InvariantCulture)}`.");
                return;
            }

            argValue = argValue.Trim().Replace(',', '.');
            if (!float.TryParse(argValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float newValue))
            {
                Debug.LogError("Couldn't parse new value as float.");
                return;
            }

            if (newValue >= 0.0f && newValue <= 1.0f)
            {
                WaterTweakerPlugin.ConfigWetlandWaterOpacity.Value = newValue;
                Debug.Log($"Water Opacity set to {newValue.ToString(CultureInfo.InvariantCulture)}");
            }
            else
                Debug.LogError("Opacity value out of bounds ! Must be between 0.0 and 1.0");
                  
        }

        [ConCommand(commandName = "watertweaker_pp", helpText = "Enables Post processing effects when the camera is under water in Wetland Aspect. Value must be `true`, `false`, `0` or `1`. args[0]=(bool)value")]
        public static void CommandPP(ConCommandArgs args)
        {
            string argEnabled = args.TryGetArgString(0);
            if (string.IsNullOrWhiteSpace(argEnabled))
            {
                Debug.Log($"Post processing effects enabled : `{WaterTweakerPlugin.ConfigWetlandWaterPP.Value}`.");
                return;
            }

            argEnabled = argEnabled.Trim().ToLower();
            if (!TryParseBool(argEnabled, out bool newVal))
            {
                Debug.LogError("Couldn't parse new value as bool. Value must be `true`, `false`, `0` or `1`");
                return;
            }

            WaterTweakerPlugin.ConfigWetlandWaterPP.Value = newVal;
            Debug.Log("Water Post Processing effects now " + (newVal ? "enabled." : "disabled."));
        }

        private static bool TryParseBool(string input, out bool result)
        {
            if (bool.TryParse(input, out result))
                return true;

            if (int.TryParse(input, out int val))
            {
                result = val > 0;
                return true;
            }

            return false;
        }
    }
}
