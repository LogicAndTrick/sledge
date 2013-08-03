using System.Collections.Generic;
using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools.VMTools
{
    public class VMPoint
    {
        private bool _isSelected;
        public Coordinate Coordinate { get; set; }
        public Solid Solid { get; set; }
        public List<Vertex> Vertices { get; set; }

        /// <summary>
        /// Midpoints are somewhat "virtual" points in that they only facilitate the moving and selection of two points at once.
        /// </summary>
        public bool IsMidPoint { get; set; }
        public VMPoint MidpointStart { get; set; }
        public VMPoint MidpointEnd { get; set; }
        public bool IsSelected
        {
            get
            {
                if (IsMidPoint) return MidpointStart.IsSelected && MidpointEnd.IsSelected;
                return _isSelected;
            }
            set
            {
                if (IsMidPoint) MidpointStart.IsSelected = MidpointEnd.IsSelected = value;
                else _isSelected = value;
            }
        }

        public void Move(Coordinate delta)
        {
            Coordinate += delta;
            if (!IsMidPoint)
            {
                Vertices.ForEach(x => x.Location += delta);
            }
        }

        public Color GetColour()
        {
            // Midpoints are selected = Pink, deselected = orange
            // Vertex points are selected = red, deselected = white
            if (IsMidPoint) return IsSelected ? Color.DeepPink : Color.Orange;
            return IsSelected ? Color.Red : Color.White;
        }
    }
}