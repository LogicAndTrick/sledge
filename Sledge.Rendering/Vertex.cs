using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering
{
    public class Vertex
    {
        public Coordinate Position { get; set; }
        public decimal TextureU { get; set; }
        public decimal TextureV { get; set; }

        public Vertex(Coordinate position, decimal textureU, decimal textureV)
        {
            Position = position;
            TextureU = textureU;
            TextureV = textureV;
        }
    }
}