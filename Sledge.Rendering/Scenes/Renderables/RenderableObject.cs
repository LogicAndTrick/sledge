using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Renderables
{
    public abstract class RenderableObject : SceneObject, IBounded
    {
        public Material Material { get; set; }
        public Coordinate Origin { get { return BoundingBox.Center; } }
        public Box BoundingBox { get; set; }
        public LightingType Lighting { get; set; }
        public bool IsWireframe { get; set; }
        public byte Opacity { get; set; }
    }
}