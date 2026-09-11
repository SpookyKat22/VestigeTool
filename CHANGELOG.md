# Changelogs

## [v1.4.2] - 2026-09-10
### Changed
- Pixel priority is Map (#FFF700), Evade (#FF0000), Attack (#394ACE), then 8-second Pixel (#537559).
- Map presses the Macro Key immediately and approximately every 300 ms, and double-clicks approximately every 500 ms.
- Evade searches only the centered quarter-width by quarter-height area and presses the Macro Key immediately, then approximately every 300 ms.
- Attack continuously double-clicks, yielding to the timed pixel after ten seconds with a stationary cursor. Attack takes priority again when detected during the timed rule.
- The 8-second Pixel continuously double-clicks and sends the Macro Key once after eight seconds of uninterrupted detection. Losing the target, a failed click, or another active rule resets its timer.
- A valid scan with no matching enabled pixels sends the Macro Key immediately, then approximately every 300 ms.
- One random left click occurs every ten seconds, left of center within the centered quarter-width by quarter-height area, including while pixel rules are active.
- Scanning starts at the client center and expands outward. Pixel clicks use real mouse input, stay at the target, and require the selected client to be in front.
- Pixel Macro and its rules start unchecked. The tool can be minimized but cannot be maximized or resized.

### Removed
- Beep on Detect and Test Detection controls.

### Fixed
- Failed scans cannot trigger the Macro Key. Stopping the macro releases any mouse button pressed by the tool.
- Matching regions use a matching target point when their bounding-box center is empty.


## v[1.0.0] - 25/02/2022 (First Stable Version)
- Autopot (HP/SP with delay customization)
- Autobuff Status (Only for Poison, Silence, Blind, Confusion, Curse, Hallucinationwalk)
- Autoclick (Skill Spammer with delay customization) 
- Profiles Support (How many profiles you want)
- Ingame ON/OFF Button (End Key), you don't need to minimize your game screen.



## [v1.1.0] - 2022-02-26
### Added
- Auto-Refresh Spammer
- Auto status. Removes Poison, silence, blind, confusion, curse and hallucination automatically using Panacea, Green Potion or Royal Jelly
- New UI to manager profiles

### Fixed
- Fixed a bug where AHK was consuming more than 5% of CPU. Now the consumer of the CPU is under of 1%.
- Always request execution level as administrator


## [v1.2.0] - 2022-02-28
### Added
- Autobuff for Stuffs
- Added new keys (Space, Page UP, Page Down, Insert, Delete)

### Fixed
- On change profile not start threads anymore.
- App don't crash anymore with a invalid json profile.
