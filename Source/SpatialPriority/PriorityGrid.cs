// PriorityGrid.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SpatialPriority
{
    [StaticConstructorOnStartup]
    public class PriorityGrid: MapComponent
    {
        public PriorityGrid( Map map ) : base( map )
        {
            grid = new SpatialPriority[map.Size.x * map.Size.z];
            mapSizeX = map.Size.x;

            // initialize grid at normal.
            for ( int i = 0; i < grid.Length; i++ )
                grid[i] = SpatialPriority.Normal;

            // create layers
            var priorities = Enum.GetValues(typeof(SpatialPriority)).Cast<SpatialPriority>();
            _layers = new List<PriorityLayer>();
            foreach ( var priority in priorities )
                _layers.Add( new PriorityLayer( this, priority,
                                                priority == SpatialPriority.Normal
                                                    ? new Color( 0, 0, 0, 0 )
                                                    : Colour( priority ) ) );
        }

        public static Color Color255( int r, int g, int b, float a = 1f) => new Color( r / 255f, g / 255f, b / 255f, a );

        static PriorityGrid()
        {
            _colors = new[]
            {
                // http://colorbrewer2.org/?type=diverging&scheme=RdBu&n=7 , using the middle 5
                Color255( 239, 138, 98 ),
                Color255( 253, 219, 199 ),
                Color255( 255, 255, 255 ),
                Color255( 209, 229, 240 ),
                Color255( 103, 169, 172 )
            };
            _priorityIcons = ContentFinder<Texture2D>.GetAllInFolder( "UI/Icons/Priorities" ).ToArray();
        }

        private SpatialPriority[] grid;
        private int mapSizeX;
        private List<PriorityLayer> _layers;
        private static Color[] _colors;
        private static Texture2D[] _priorityIcons;

        public static Color Colour( SpatialPriority priority ) => _colors[(int) priority];
        public static Texture2D Icon( SpatialPriority priority ) => _priorityIcons[(int) priority];

        public SpatialPriority this[IntVec3 c]
        {
            get => grid[CellIndicesUtility.CellToIndex(c, mapSizeX)];
            set
            {
                int num = CellIndicesUtility.CellToIndex(c, mapSizeX);
                grid[num] = value;
                SetDirty();
            }
        }

        public SpatialPriority this[int index]
        {
            get => grid[index];
            set {
                grid[index] = value;
                SetDirty();
            }
        }

        public SpatialPriority this[int x, int z]
        {
            get => grid[CellIndicesUtility.CellToIndex(x, z, mapSizeX)];
            set
            {
                grid[CellIndicesUtility.CellToIndex( x, z, mapSizeX )] = value;
                SetDirty();
            }
        }

        public int CellsCount => grid.Length;
        
        public void MarkForDraw()
        {
            _layers.ForEach( l => l.MarkForDraw() );
        }

        public void SetDirty()
        {
            _layers.ForEach( l => l.MarkDirty() );
        }

        public override void MapComponentUpdate()
        {
            if (Find.CurrentMap == map )
                _layers.ForEach( l => l.Update() );
        }

        private byte[] _priorityGridScribeHelper;
        public override void ExposeData()
        {
            if ( Scribe.mode == LoadSaveMode.Saving )
                _priorityGridScribeHelper = grid.Select( p => (byte) p ).ToArray();
            DataExposeUtility.ByteArray( ref _priorityGridScribeHelper, "priorityGrid" );
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                grid = _priorityGridScribeHelper.Select( p => (SpatialPriority) p ).ToArray();
                SetDirty();
            }

            base.ExposeData();
        }
    }
}
