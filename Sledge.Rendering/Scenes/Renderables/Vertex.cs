using OpenTK;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Vertex
    {
        public Vector3 Position { get; set; }
        public decimal TextureU { get; set; }
        public decimal TextureV { get; set; }

        public Vertex(Vector3 position, decimal textureU, decimal textureV)
        {
            Position = position;
            TextureU = textureU;
            TextureV = textureV;
        }
    }
}