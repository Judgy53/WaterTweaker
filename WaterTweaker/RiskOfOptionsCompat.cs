﻿using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using BepInEx.Configuration;

namespace WaterTweaker
{
    public static class RiskOfOptionsCompat
    {
        private static bool? _enabled;

        public static bool Enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_enabled;
            }
        }
        public static void AddOptionStepSlider(ConfigEntry<float> configEntry, float min, float max, float increment, string name)
        {
            ModSettingsManager.AddOption(new StepSliderOption(configEntry, new StepSliderConfig() { min = min, max = max, increment = increment, name = name }), WaterTweakerPlugin.PluginGUID, WaterTweakerPlugin.PluginName);
        }

        public static void AddOptionCheckbox(ConfigEntry<bool> configEntry, string name)
        {
            ModSettingsManager.AddOption(new CheckBoxOption(configEntry, new CheckBoxConfig() { name = name }), WaterTweakerPlugin.PluginGUID, WaterTweakerPlugin.PluginName);
        }

        public static void SetModDescription(string desc)
        {
            ModSettingsManager.SetModDescription(desc, WaterTweakerPlugin.PluginGUID, WaterTweakerPlugin.PluginName);
        }
    }
}