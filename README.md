# FloatTool
[![Build status](https://ci.appveyor.com/api/projects/status/pbudy8wm35ybxpuf?svg=true)](https://ci.appveyor.com/project/Prevter/floattool)
[![Discord](https://img.shields.io/discord/852481556019019786?label=discord&logo=discord)](https://discord.gg/RM9VrzMfhP)

![Program Working](https://github.com/Prevter/FloatTool-GUI/blob/master/doc/program.png?raw=true)  
Utility to create any float for your CS:GO skins.

## Description

This app allows you to quickly search combinations to craft skins in CS:GO with (almost) any float you want. All you have to do is set some search settings, wait a bit and all you have to do next is to buy skins from marketplace, that the app found to be needed.  
This is probably the fastest app you can find, as it can achieve more than 10,000,000 combinations per second on a i9-9900K.

## Getting Started

### Dependencies

* Windows 10 or greater recommended. Windows 7 can have some minor issues with connection and ui.
* .NET Framework 4.7.2

### Installing

* Proceed to releases page
* Download latest release archive
* Unzip to any folder
* Run **FloatTool.exe**

### Setting up search

* TO-DO

## Help

If you have any issues with this software, you can post an issue or contact me:

## Contacts

[Steam](https://steamcommunity.com/id/prevter)  
[Discord Server](https://discord.gg/RM9VrzMfhP)  
Discord: Prevter#4666  
[Telegram](https://t.me/prevter)

## Version History
* 0.8.0
    * Speed have been increased x3 times
    * Fixed some minor bugs
    * Changed platform to x64 only
    * App will auto reload _itemData.json_ if deleted
    * Fixed crash if couldn't check for updates
    * If you press F2, you will get some utilities to calculate floats
    * Compacted build files
    * Added testing OpenCL kernel. You can test your GPU by entering a cheat code `gpupower` and pressing a button that appeared
* 0.7.2
    * Benchmark now uses themes
    * You can now see IEEE754 of the float
    * Some UI tweaks
    * Added craft range to see what you can craft from your input
    * Float downloading is now faster
    * Minor optimizations
* 0.7.1
    * Increased search speed
    * Fixed duplicate combinations
    * Fixed some minor bugs
* 0.7.0
    * Increased search speed
    * Added some features to benchmark
    * Added custom theme loader
    * Update checker have been improved to autoupdate
    * Added "Find one" button
    * Logger now saves crashreports
    * Thread count now saves previous settings
* 0.6.1
    * Code refactoring
    * Added logging system
    * Fixed light mode
    * Added 6 new collections
    * Changed font
    * If you press F1, a table containing drop chances will open
* 0.6.0
    * Rewamped found combinations UI
    * Speed have been increased
    * You can now see full price of a craft
    * You can disable Discord RPC
    * You can change your currency
    * Fixed a bug with PP-Bizon that was not showing skins
    * Code refactoring
    * Registry autofixes itself if deleted
* 0.5.2
    * Hotfix. Csgofloat changed some API
* 0.5.1
    * Search speed increased 10x times
    * Added outcome selector
    * Added benchmark
    * Added settings window
    * You can now search floats less or greater than selected
    * You can now see stats about current speed
    * Remade progress bar
    * Remade checkboxes
    * Thread count setting is now set automatically
    * Added update check
    * Default float is now 0.25000000032783
    * Removed white border
    * Added maximize/minimize button
    * Sound have been changed
* 0.4.2
    * Fixed StatTrack search
    * Added new icon
* 0.4.1
    * Added multithreading
* 0.3.3
    * You can now change to light mode
* 0.3.2
    * Small precision fix
* 0.3.1
    * Reworked UI to a more modern design
* 0.2.1
    * Added some English localization
    * Added Discord Rich Presence
    * Fixed a memory leak
* 0.2.0
    * Added button to check a skin for validity
    * Added sound when combination was found
    * Added about menu
    * Added settings where you can disable/enable sound
* 0.1.1.1
    * Some UI fixes
* 0.1.0
    * Float precision have been fixed, so it will calculate floats a lot better
* 0.0.2
    * Hotfix. The app was using one thread, and it froze when you started searching.
    * Styled window a bit
* 0.0.1
    * Initial Release

## License

This project is licensed under the GPL v3 License - see the LICENSE.md file for details
