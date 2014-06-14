using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools.VMTool
{
    public class VMPoint
    {
        public Coordinate Coordinate { get; set; }
        public Solid Solid { get; set; }
        public List<Vertex> Vertices { get; set; }

        /// <summary>
        /// Midpoints are somewhat "virtual" points in that they only facilitate the moving and selection of two points at once.
        /// </summary>
        public bool IsMidPoint { get; set; }
        public VMPoint MidpointStart { get; set; }
        public VMPoint MidpointEnd { get; set; }
        public bool IsSelected { get; set; }

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

        public bool IsMidPointFor(VMPoint start, VMPoint end)
        {
            return IsMidPoint
                   && ((start == MidpointStart && end == MidpointEnd)
                       || (end == MidpointStart && start == MidpointEnd));
        }

        public IEnumerable<Face> GetAdjacentFaces()
        {
            if (IsMidPoint)
            {
                return MidpointStart.GetAdjacentFaces()
                    .Intersect(MidpointEnd.GetAdjacentFaces());
            }
            return Vertices.Select(x => x.Parent).Distinct();
        }
    }
}