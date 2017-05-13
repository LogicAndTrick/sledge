using System;
using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    public interface ITextured
    {
        Texture Texture { get; }
    }
}