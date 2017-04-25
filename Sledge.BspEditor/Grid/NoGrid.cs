using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// A grid that does nothing
    /// </summary>
    [Export(typeof(IGrid))]
    public class NoGrid : IGrid
    {
        public int Spacing
        {
            get { return 1; }
            set { }
        }

        public Coordinate Snap(Coordinate coordinate)
        {
            return coordinate;
        }

        public Coordinate AddStep(Coordinate coordinate, Coordinate add)
        {
            return coordinate + add;
        }

        public IEnumerable<GridLine> GetLines(Coordinate normal, decimal scale, Coordinate worldMinimum, Coordinate worldMaximum)
        {
            yield break;
        }
    }
}