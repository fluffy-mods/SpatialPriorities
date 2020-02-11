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