using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;
using Sledge.Editor.Brushes.Controls;

namespace Sledge.Editor.Brushes
{
    public interface IBrush
    {
        string Name { get; }
        IEnumerable<BrushControl> GetControls();
        IEnumerable<MapObject> Create(Box box, ITexture texture);
    }
}
