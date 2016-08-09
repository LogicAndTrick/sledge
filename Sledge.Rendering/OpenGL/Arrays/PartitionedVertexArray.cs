using System.Collections.Generic;
using OpenTK;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class PartitionedVertexArray : RenderableVertexArray, IBounded
    {
        public Vector3 Origin { get { return BoundingBox.Center; } }
        public Box BoundingBox { get; private set; }

        public PartitionedVertexArray(Box boundingBox, ICollection<RenderableObject> data) : base(data)
        {
            BoundingBox = boundingBox;
        }
    }
}