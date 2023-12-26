# GlobalGoopTracker
A mod for Loddlenaut to track all goop and litter globally. Only tested on Windows.

## Installation
Install BepInEx 5 by following the [installation instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html). You should run the game once to make sure it works and to generate the required files/folders. Then get the dll for this mod from the [releases](https://github.com/paperman5/GlobalGoopTracker/releases) page and put it in `Gameroot/BepInEx/plugins/GlobalGoopTracker`. If it worked you should see the version number on the title screen.

## Usage
This mod works by adding another `BiomeManager` that tracks all the litter/goop that's not in any other biome. The map in the menu has been altered to show the global cleanup percentage as well as the litter & goop left to collect from non-biome locations - this information will show when there is no other biome selected.
