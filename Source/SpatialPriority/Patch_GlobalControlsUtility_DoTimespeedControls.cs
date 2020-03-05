// PriorityQuickMenu.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SpatialPriority
{
    [HarmonyPatch( typeof( GlobalControlsUtility ), nameof( GlobalControlsUtility.DoTimespeedControls ) )]
    [StaticConstructorOnStartup]
    public class Patch_GlobalControlsUtility_DoTimespeedControls
    {
        public static float size = 20f;
        public static IEnumerable<SpatialPriority> priorities = Enum.GetValues( typeof(SpatialPriority ) ).Cast<SpatialPriority>();

        public static void Postfix( float leftX, float width, ref float curBaseY )
        {
            var iconRect = new Rect( leftX + width - size - 8, curBaseY - size - 8, size, size );
            var active = Find.DesignatorManager.SelectedDesignator == Designator_SpatialPriority.Instance;

            foreach ( var priority in priorities )
            {
                var colour = PriorityGrid.Colour( priority );
                if ( Widgets.ButtonImage( iconRect, PriorityGrid.Icon( priority ), active ? colour : Color.white, colour ) )
                {
                    Designator_SpatialPriority.Instance.priority = priority;
                    Find.DesignatorManager.Select( Designator_SpatialPriority.Instance );
                }

                iconRect.x -= size + 4f;
            }

            GUI.color = Color.white;
            curBaseY -= size + 3 * 4;
        }
    }
}