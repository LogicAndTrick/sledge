using OpenTK;
using OpenTK.Graphics;

namespace Sledge.Editor.Rendering.Arrays
{
    public struct MapObjectVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Color4 Colour;
        public float IsSelected;
    }
}