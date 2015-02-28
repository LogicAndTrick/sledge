namespace Sledge.Rendering.Scenes.Elements
{
    public class PositionVertex
    {
        public Position Position { get; set; }
        public float TextureU { get; set; }
        public float TextureV { get; set; }

        public PositionVertex(Position position, float textureU, float textureV)
        {
            Position = position;
            TextureU = textureU;
            TextureV = textureV;
        }

        public PositionVertex Clone()
        {
            return new PositionVertex(Position.Clone(), TextureU, TextureV);
        }
    }
}