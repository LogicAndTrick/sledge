using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    public class GridLine
    {
        public GridLineType Type { get; private set; }
        public Line Line { get; set; }

        public GridLine(GridLineType type, Coordinate start, Coordinate end)
        {
            Type = type;
            Line = new Line(start, end);
        }
    }
}
