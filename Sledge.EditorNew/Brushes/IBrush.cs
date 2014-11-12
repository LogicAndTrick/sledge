using System.Collections.Generic;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Brushes.Controls;

namespace Sledge.EditorNew.Brushes
{
    public interface IBrush
    {
        string Name { get; }
        bool CanRound { get; }
        IEnumerable<BrushControl> GetControls();
        IEnumerable<MapObject> Create(IDGenerator generator, Box box, ITexture texture, int roundDecimals);
    }
}
