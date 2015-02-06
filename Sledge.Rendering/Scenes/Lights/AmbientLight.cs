using System.Drawing;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.Scenes.Lights
{
    public class AmbientLight : Light
    {
        public Coordinate Direction { get; set; }

        public AmbientLight(Color color, Coordinate direction, float intensity)
        {
            Color = color;
            Direction = direction.Normalise();
            Intensity = intensity;
        }
    }
}