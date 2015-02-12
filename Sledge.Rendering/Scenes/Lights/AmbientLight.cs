using System.Drawing;
using OpenTK;

namespace Sledge.Rendering.Scenes.Lights
{
    public class AmbientLight : Light
    {
        public Vector3 Direction { get; set; }

        public AmbientLight(Color color, Vector3 direction, float intensity)
        {
            Color = color;
            Direction = direction.Normalized();
            Intensity = intensity;
        }
    }
}