//// Patch_PotentialWorkCellsGlobal.cs
//// Copyright Karel Kroeze, 2018-2018
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
//    public class Patch_PotentialWorkCellsGlobal
//    {
//        static IEnumerable<MethodBase> TargetMethods()
//        {
//            foreach ( var scanner in typeof( WorkGiver_Scanner ).AllSubclasses() )
//            {
//                var potentialCells = scanner.GetMethod( "PotentialWorkCellsGlobal" );
//                if ( potentialCells?.DeclaringType == scanner )
//                    yield return potentialCells;
//            }
//        }
//
//        static void Postfix( Pawn pawn, ref IEnumerable<IntVec3> __result, WorkGiver_Scanner __instance )
//        {
//            var cells = __result.ToList();
//            if (!cells.Any())
//                return;
//
//            var priority = Manager.PriorityGrid(pawn.Map);
//            var traverseParams = TraverseParms.For( pawn, __instance.MaxPathDanger( pawn ) );
//
//            var cellsByPriority = cells
//                .GroupBy(c => priority[c])
//                .OrderByDescending(g => g.Key);
//
//            foreach (var group in cellsByPriority)
//            {
//                var anyApplicable = group.Any( c => Valid( c, pawn, __instance, traverseParams ) );
//                if ( anyApplicable )
//                {
//                    __result = group;
//                    return;
//                }
//            }
//            __result = null;
//        }
//
//        private static bool Valid( IntVec3 cell, Pawn pawn, WorkGiver_Scanner scanner, TraverseParms traverseParams )
//        {
//            return !cell.IsForbidden( pawn )
//                   && scanner.HasJobOnCell( pawn, cell )
//                   && ( scanner.AllowUnreachable ||
//                        pawn.Map.reachability.CanReach( pawn.Position, cell, scanner.PathEndMode, traverseParams ) );
//        }
//    }
//}