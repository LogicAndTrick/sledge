using System.Drawing;

namespace Sledge.Rendering.Scenes.Lights
{
    public abstract class Light : SceneObject
    {
        public Color Color { get; set; }
        public float Intensity { get; set; }
    }
}