namespace Sledge.Rendering.OpenGL.Arrays
{
    public class PolygonProperties
    {
        public string Texture { get; set; }
        public bool DepthTested { get; set; }

        public PolygonProperties(string texture)
        {
            Texture = texture;
            DepthTested = true;
        }
    }
}