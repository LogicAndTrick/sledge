using System.Collections.Generic;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Brush
{
    public interface IBrush
    {
        string Name { get; }
        bool CanRound { get; }
        IEnumerable<BrushControl> GetControls();
        IEnumerable<IMapObject> Create(UniqueNumberGenerator idGenerator, Box box, string texture, int roundDecimals);
    }
}
