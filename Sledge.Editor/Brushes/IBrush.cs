using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Common;

namespace Sledge.Editor.Brushes
{
    public interface IBrush
    {
        IEnumerable<MapObject> Create(Box box, ITexture texture);
    }
}
