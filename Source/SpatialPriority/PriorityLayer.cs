// PriorityLayer.cs
// Copyright Karel Kroeze, 2018-2018

using UnityEngine;
using Verse;

namespace SpatialPriority
{
    public class PriorityLayer: ICellBoolGiver
    {
        private SpatialPriority _priority;
        private PriorityGrid _grid;
        private CellBoolDrawer _drawer;

        public PriorityLayer( PriorityGrid grid, SpatialPriority priority, Color color )
        {
            _priority = priority;
            _grid = grid;
            Color = color;
            _drawer = new CellBoolDrawer( this, grid.map.Size.x, grid.map.Size.z );
            Debug.Log( $"Creating layer {priority} ({color})"  );
        }

        public bool GetCellBool( int index )
        {
            return _grid[index] == _priority;
        }

        public Color GetCellExtraColor( int index )
        {
            return Color.white;
        }

        public Color Color { get; }

        public void MarkDirty()
        {
            _drawer.SetDirty();
        }

        public void MarkForDraw()
        {
            _drawer.MarkForDraw();
        }

        public void Update()
        {
            _drawer.CellBoolDrawerUpdate();
        }
    }
}