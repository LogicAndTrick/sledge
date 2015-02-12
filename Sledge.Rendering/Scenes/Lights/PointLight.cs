using OpenTK;

namespace Sledge.Rendering.Scenes.Lights
{
    public class PointLight : Light
    {
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
    }
}