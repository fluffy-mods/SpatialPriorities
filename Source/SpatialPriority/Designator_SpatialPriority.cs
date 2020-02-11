// Designator_SpatialPriority.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpatialPriority
{
    public class Designator_SpatialPriority: Designator
    {
        public Designator_SpatialPriority()
        {
            icon = ContentFinder<Texture2D>.Get( "UI/Icons/SpatialPriority" );
            defaultLabel = "Priority";
            _instance = this;
        }

        private static Designator_SpatialPriority _instance;

        public static Designator_SpatialPriority Instance => _instance;

        public SpatialPriority priority = SpatialPriority.Normal;
        public PriorityGrid PriorityGrid => Manager.PriorityGrid( Map );

        public override AcceptanceReport CanDesignateCell( IntVec3 cell )
        {
            if ( !cell.InBounds( Map ) )
                return false;
            return priority != PriorityGrid[cell];
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
            PriorityGrid.MarkForDraw();
        }

        public override void DesignateSingleCell( IntVec3 c )
        {
            PriorityGrid[c] = priority;
        }

        public override int DraggableDimensions => 2;

        public override void ProcessInput( Event ev )
        {
            base.ProcessInput( ev );
            var options = new List<FloatMenuOption>();
            foreach ( var _priority in Manager.PriorityLevels )
            {
                var __priority = _priority;
                options.Add(new FloatMenuOption(__priority.ToString(), () => priority = __priority));
            }
            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        public override void RenderHighlight( List<IntVec3> dragCells )
        {
            DesignatorUtility.RenderHighlightOverSelectableCells( this, dragCells );
        }

        public override void DrawMouseAttachments()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Rect    mouseRect     = new Rect( mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f );
            Find.WindowStack.ImmediateWindow( 34003428, mouseRect, WindowLayer.Super,
                                              ( () =>
                                              {
                                                  GUI.color = PriorityGrid.Colour( priority );
                                                  GUI.DrawTexture( mouseRect.AtZero(), PriorityGrid.Icon( priority ) );
                                                  GUI.color = Color.white;
                                              } ), false,
                                              false, 0.0f );
        }
    }
}