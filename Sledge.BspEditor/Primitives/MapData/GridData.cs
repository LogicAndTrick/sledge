using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Grid;

namespace Sledge.BspEditor.Primitives.MapData
{
    public class GridData : IMapData
    {
        public bool SnapToGrid { get; set; }
        public IGrid Grid { get; set; }

        public GridData(IGrid grid)
        {
            Grid = grid;
            SnapToGrid = true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Meh
        }

        public IMapData Clone()
        {
            return new GridData(Grid) {SnapToGrid = SnapToGrid};
        }
    }
}
