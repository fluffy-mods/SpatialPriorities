[![RimWorld Alpha 1](https://img.shields.io/badge/RimWorld-Alpha%201-brightgreen.svg)](http://rimworldgame.com/)

Some spaces are more important than others

# Features
Adds a designator in the "Zone" menu that allows you to apply priorities to areas. Pawns follow the priorities you set in the Work tab, but will choose to do jobs in high priority areas (e.g. your medicine crops) before jobs in low priority areas (e.g. the hay fields).

In the core game, the priority of jobs is decided by (in order);
  1) Priority of the work type,
  2) Left-right order of the work type,
  3) Left-right order of the task (WorkGivers, normally invisible),
  4) Distance to the target.

This mod changes that to;  
  1) Priority of the work type (Work Tab support for workgiver level priorities is planned)
  2) Left-right order of the work type,
  3) Priority of the target area,
  4) Left-right order of the task,
  5) Distance to the target.

In other words, we evaluate all workgivers in the same worktype, with the same priority, at the same time. Within this 'batch' of potential jobs, we try to find the highest priority job.

# Notes
Adds a MapComponent; 
 - Can be safely added to existing games.
 - Can be safely removed from existing games (although you will get an error when you load the game, the error goes away after saving without this mod active).

# Performance
Performance will vary, depending on how much you use priorities, and how often work is done in high priority areeas. I'd love to hear feedback on how it runs in your game.

# Compatibility
This mod completely replaces `JobGiver_Work`, by replacing it in all `ThinkTreeDefs`. Any mods that rely on patching this class will be incompatible, but at the moment, I'm not aware of any mods that do this (I haven't looked very hard though).

# Think you found a bug? 
Please read [this guide](http://steamcommunity.com/sharedfiles/filedetails/?id=725234314) before creating a bug report,
 and then create a bug report [here](https://github.com/FluffierThanThou/SpatialPriorities/issues)

# Older versions
All current and past versions of this mod can be downloaded from [GitHub](https://github.com/FluffierThanThou/SpatialPriorities/releases).

# License
All original code in this mod is licensed under the [MIT license](https://opensource.org/licenses/MIT). Do what you want, but give me credit. 
All original content (e.g. text, imagery, sounds) in this mod is licensed under the [CC-BY-SA 4.0 license](http://creativecommons.org/licenses/by-sa/4.0/).

Parts of the code in this mod, and some content may be licensed by their original authors. If this is the case, the original author & license will either be given in the source code, or be in a LICENSE file next to the content. Please do not decompile my mods, but use the original source code available on [GitHub](https://github.com/FluffierThanThou/SpatialPriorities/), so license information in the source code is preserved.

# Are you enjoying my mods?
Show your appreciation by buying me a coffee (or contribute towards a nice single malt).

[![Buy Me a Coffee](http://i.imgur.com/EjWiUwx.gif)](https://ko-fi.com/fluffymods)

[![I Have a Black Dog](https://i.ibb.co/ss59Rwy/New-Project-2.png)](https://www.youtube.com/watch?v=XiCrniLQGYc)

# Version
This is version 0.3.99, for RimWorld 1.0.2408.