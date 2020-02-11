//// Patch_PotentialWorkCellsGlobal.cs
//// Copyright Karel Kroeze, 2018-2018
//
//// Patching potential work things/cells was an alternative approach to patching/replacing the jobGiver_work. 
//
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Harmony;
//using RimWorld;
//using Verse;
//
//namespace SpatialPriority
//{
//    public class Patch_PotentialWorkThingsGlobal
//    {
//        static IEnumerable<MethodBase> TargetMethods()
//        {
//            foreach ( var scanner in typeof( WorkGiver_Scanner ).AllSubclasses() )
//            {
//                var potentialThings = scanner.GetMethod( "PotentialWorkThingsGlobal" );
//                if ( potentialThings?.DeclaringType == scanner )
//                    yield return potentialThings;
//            }
//        }
//
//        static void Postfix(Pawn pawn, ref IEnumerable<Thing> __result, WorkGiver_Scanner __instance)
//        {
//            var things = __result.ToList();
//            if (!things.Any())
//                return;
//
//            var priority = Manager.PriorityGrid(pawn.Map);
//            var traverseParams = TraverseParms.For(pawn, __instance.MaxPathDanger(pawn));
//
//            var thingsByPriority = things
//                .GroupBy(t => priority[t.Position])
//                .OrderByDescending(g => g.Key);
//
//            foreach (var group in thingsByPriority)
//            {
//                var anyApplicable = group.Any(t => Valid(t, pawn, __instance, traverseParams));
//                if ( anyApplicable )
//                {
//                    __result = group;
//                    return;
//                }
//            }
//
//            // return empty list instead of null to stop ClosestThingReachable from trying to get stuff from the ThingRequest.
//            // note that I technically consider this a bug in vanilla, but it normally never occurs - because these workgivers 
//            // would always include the pawn currently looking for a job (or only include that pawn).
//            // We return an empty set to stop the game from doing extra work, if we were lazy we could just return the original set.
//            if ( __instance.PotentialWorkThingRequest.IsUndefined )
//                __result = new List<Thing>();
//            else
//                __result = null;
//        }
//
//        private static bool Valid(Thing thing, Pawn pawn, WorkGiver_Scanner scanner, TraverseParms traverseParams)
//        {
//            var result = !thing.IsForbidden(pawn)
//                   && scanner.HasJobOnThing(pawn, thing)
//                   && (scanner.AllowUnreachable ||
//                       pawn.Map.reachability.CanReach(pawn.Position, thing, scanner.PathEndMode, traverseParams));
//            return result;
//        }
//    }
//}