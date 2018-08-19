using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapDocumentOverlayRenderable))]
    public class ViewportTextOverlay : IMapDocumentOverlayRenderable
    {
        public void SetActiveDocument(MapDocument doc)
        {
            //
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            var str = $"2D {camera.ViewType}";
            var size = graphics.MeasureString(str, SystemFonts.DefaultFont);
            graphics.DrawString(str, SystemFonts.DefaultFont, Brushes.White, 0, 0);
            using (var b = new SolidBrush(Color.FromArgb(128, Color.Pink))) graphics.FillRectangle(b, 0, 0, size.Width, size.Height);
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            var size = graphics.MeasureString("3D View", SystemFonts.DefaultFont);
            graphics.DrawString("3D View", SystemFonts.DefaultFont, Brushes.White, 0, 0);
            using (var b = new SolidBrush(Color.FromArgb(128, Color.Pink))) graphics.FillRectangle(b, 0, 0, size.Width, size.Height);
        }
    }
}
