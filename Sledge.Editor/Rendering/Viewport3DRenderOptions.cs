using OpenTK;

namespace Sledge.Editor.Rendering
{
    public class Viewport3DRenderOptions : Viewport2DRenderOptions
    {
        public bool Wireframe { get; set; }
        public bool Textured { get; set; }
        public bool Shaded { get; set; }

        public bool ShowGrid { get; set; }
        public decimal GridSpacing { get; set; }
    }

    public class Viewport2DRenderOptions
    {
        public Matrix4 Viewport { get; set; }
        public Matrix4 Camera { get; set; }
        public Matrix4 ModelView { get; set; }
    }
}