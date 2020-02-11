using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace SpatialPriority
{
    public class Manager : Mod
    {
        public Manager( ModContentPack content ) : base( content )
        {
            LongEventHandler.ExecuteWhenFinished( Initialize );
        }

        public static bool DrawPriorityGrid = false;

        public static IEnumerable<SpatialPriority> PriorityLevels =
            Enum.GetValues( typeof( SpatialPriority ) ).Cast<SpatialPriority>();

        private static void Initialize()
        {
            // run harmony patches
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            var harmony = HarmonyInstance.Create("fluffy.spatialpriority");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // hack in designator
            var catDef = DefDatabase<DesignationCategoryDef>.GetNamed( "Zone" );
            catDef.specialDesignatorClasses.Add( typeof( Designator_SpatialPriority ) );
            catDef.ResolveReferences(); // recaches designators.
        }

        public static PriorityGrid PriorityGrid( Map map )
        {
            var comp = map.GetComponent<PriorityGrid>();
            if ( comp == null )
            {
                comp = new PriorityGrid( map );
                map.components.Add( comp );
            }
            return comp;
        }
    }
}
