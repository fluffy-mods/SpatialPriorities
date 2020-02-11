[![RimWorld Alpha 1](https://img.shields.io/badge/RimWorld-Alpha%201-brightgreen.svg)](http://rimworldgame.com/)

Some spaces are more important than others

# IMPORTANT
This mod is a work-in-progress. You may find more bugs than you're used to in my mods. You may find that I have suddenly changed the save format, breaking your save games. You have been warned.

In particular, it is also currently INCOMPATIBLE WITH THESE MODS:
 - Hospitality (visitors will not do work, which is probably fine)
 - Prison Labour (prisoners will not do work, which rather defeats the purpose)
And any other mod that relies on altering `JobGiver_Work`, as this mod completely overrides it.

# Features
Pawns follow the priorities you set in the Work tab, but will choose to do jobs in high priority areas (e.g. your medicine crops) before jobs in low priority areas (e.g. the hay fields). You can set priorities with the designator in the "Zone" category of the architect, or with the 1-5 buttons above the game speed indicators. 

In the vanilla game, the priority of jobs is decided by (in order);
  1) Priority of the work type,
  2) Left-right order of the work type,
  3) 'natural' priority of the task (WorkGivers, normally invisible),
  4) Distance to the target.

This mod changes that to;  
  1) Priority of the work type (Work Tab support for workgiver level priorities is planned)
  2) Left-right order of the work type,
  3) Priority of the target area,
  4) 'natural' priority of the task,
  5) Distance to the target.

In other words, we evaluate all workgivers in the same worktype, with the same priority, at the same time. Within this 'batch' of potential jobs, we try to find a job in the area that has the highest priority. 

# Future plans
I plan to add a bunch of tweaking options, primarily in deciding how big the 'batches' of potential jobs will be. That may mean batching all potential jobs with the same priority in all worktypes together, or even considering the priority of the area before looking at the priority of jobs. I'll need playtesting data and suggestions, which is why I chose to release this work in progress on steam.

# Notes
Adds a MapComponent; 
 - Can be safely added to existing games.
 - Can be safely removed from existing games (although you will get an error when you load the game, the error goes away after saving without this mod active).

# Performance
Performance will vary, depending on how much you use priorities, and how often work is done in high priority areeas. I'd love to hear feedback on how it runs in your game.

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
This is version 0.5.101, for RimWorld 1.0.2408.