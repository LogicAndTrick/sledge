using System.Collections.Generic;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL
{
    public class PartitionedVertexArray : RenderableVertexArray
    {
        public Box BoundingBox { get; private set; }

        public PartitionedVertexArray(Box boundingBox, ICollection<RenderableObject> data) : base(data)
        {
            BoundingBox = boundingBox;
        }
    }
}