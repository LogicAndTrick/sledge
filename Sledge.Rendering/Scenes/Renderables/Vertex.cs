using OpenTK;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Vertex
    {
        public Vector3 Position { get; set; }
        public float TextureU { get; set; }
        public float TextureV { get; set; }

        public Vertex(Vector3 position, float textureU, float textureV)
        {
            Position = position;
            TextureU = textureU;
            TextureV = textureV;
        }
    }
}