using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.Scenes.Lights
{
    public class PointLight : Light
    {
        public Coordinate Position { get; set; }
        public float Distance { get; set; }
    }
}