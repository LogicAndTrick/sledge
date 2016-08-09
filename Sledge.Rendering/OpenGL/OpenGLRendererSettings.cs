using System.Drawing;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLRendererSettings : IRendererSettings
    {
        private readonly IRenderer _renderer;

        public OpenGLRendererSettings(IRenderer renderer)
        {
            _renderer = renderer;
        }

        public bool DisableTextureTransparency
        {
            get { return !((TextureStorage)_renderer.Textures).EnableTransparency; }
            set { ((TextureStorage)_renderer.Textures).EnableTransparency = !value; }
        }

        public bool DisableTextureFiltering
        {
            get { return ((TextureStorage)_renderer.Textures).DisableTextureFiltering; }
            set { ((TextureStorage)_renderer.Textures).DisableTextureFiltering = value; }
        }

        public bool ForcePowerOfTwoTextureSizes
        {
            get { return ((TextureStorage)_renderer.Textures).ForceNonPowerOfTwoResize; }
            set { ((TextureStorage)_renderer.Textures).ForceNonPowerOfTwoResize = value; }
        }

        public Color PerspectiveBackgroundColour { get; set; }
        public Color OrthographicBackgroundColour { get; set; }
        public float PerspectiveGridSpacing { get; set; }
        public bool ShowPerspectiveGrid { get; set; }
        public float PointSize { get; set; }
    }
}