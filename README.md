# Aether Compass

> <img src="./res/img/icon_by_arkfrostlumas.png" width="96"/>
> Have you ever run into something when looking at your compass ... oooops!

_Aether Compass_ is an FFXIV Dalamud plugin that has a compass (a set of compasses, actually) 
that automatically detects certain objects/NPCs such as Aether Currents nearby and shows where they are.

Detected objects/NPCs are displayed in various ways, 
notably through markers on your screen that point to the locations of these objects/NPCs.
Optionally, it can notify you through Chat and/or Toast messages.

Currently supports detecting:
- Aether Currents
- Mob Hunt Elite Marks (Notorious Monsters)
- Gathering Points
- Eureka Elementals (by apetih)
- *\[Experimental\] Quest-related NPCs/Objects*
- *\[Experimental\] Island Sanctuary Gathering Objects and Animals*

**NOTE:** Because most objects/NPCs are not loaded 
when they are too faraway or when there are too many entities nearby (such as too many player characters), 
they will not be detected in this case.


## Installation

[XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) is required to install and run the plugin.

If you cannot find a release version (i.e. not a testing version) of it,
then the plugin is either only available as a testing plugin, or it is not updated at all.

In the former case, you can find it by enabling the setting "Get plugin testing builds" in Dalamud.

:warning: **Please be cautious in doing so, as testing plugins are expected to be unstable.
There could be major bugs that may even crash your game client in some worse cases.**
In addition, by enabling this setting you might also receive unstable test builds of other plugins.

## Special Thanks

- [yomishino](https://github.com/yomishino) - For creating Aether Compass
- [apetih](https://github.com/apetih) - For making the Eureka Elementals compass
- [Lumas Arkfrost](https://github.com/ArkfrostLumas) - For the plugin icon
- And thanks to all who have contributed to bug fixes
