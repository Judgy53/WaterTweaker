# WaterTweaker
 
Various graphics tweaks on Wetland Aspect water. Default configuration doesn't do anything. Use console commands (requires R2API) or [RiskOfOptions](https://thunderstore.io/package/Rune580/Risk_Of_Options/) to edit configuration in game.

# Commands (requires R2API)

- `watertweaker_opacity <newValue>` : Sets the Opacity of the water in Wetland Aspect (between 0.0 and 1.0). Omitting `newValue` argument outputs the current value. (Default: `1.0`)
- `watertweaker_pp <newValue>` : Enable/Disable Post Processing effects when the camera goes underwater in Wetland Aspect. Omitting `newValue` argument outputs the current value. (Default: `true`)

# Configuration

- `WetlandWaterOpacity` : Sets the Opacity of the water in Wetland Aspect (between 0.0 and 1.0). (Default: `1.0`)
- `WetlandPostProcessing` : Enables Post Processing effects when the camera goes underwater in Wetland Aspect. (Default: `true`)

# Changelog

- 1.2.0
    - Added Risk of Options integration.
    - R2API is now an optional dependency.
- 1.1.1
	- Fix internal version number to avoid potential dependency issues.
- 1.1.0
	- Add console commands.
	- Now requires R2API dependency.
- 1.0.0
	- Initial Release.