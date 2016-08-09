using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Lists
{
    public class PartitionedDisplayList : DisplayListRenderable, IBounded
    {
        public Vector3 Origin { get { return BoundingBox.Center; } }
        public Box BoundingBox { get; private set; }

        public PartitionedDisplayList(DisplayListRenderer renderer, Box boundingBox, IEnumerable<RenderableObject> data) : base(renderer, data)
        {
            BoundingBox = boundingBox;
        }
    }
}
