using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapObject2DOverlay))]
    public class EntityNamesOverlay : IMapObject2DOverlay
    {
        public void Render(IViewport viewport, ICollection<IMapObject> objects, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (camera.Zoom < 1) return;

            var font = SystemFonts.DefaultFont;
            var nameFont = new Font(font, FontStyle.Bold);

            // Escape hatch in case there's too many entities on screen
            var ents = objects.OfType<Entity>().Where(x => x.EntityData != null).ToList();
            if (ents.Count <= 0 || ents.Count > 100) return;

            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            var renderNames = camera.Zoom > 2 && ents.Count < 50;

            foreach (var ed in ents)
            {
                var c = ed.Color?.Color ?? Color.White;

                var loc = camera.WorldToScreen(ed.BoundingBox.Center);

                var box = ed.BoundingBox;
                var dim = camera.Flatten(box.Dimensions / 2);
                loc.Y -= camera.UnitsToPixels(dim.Y);

                var str = ed.EntityData.Name;
                var targetname = ed.EntityData.Get<string>("targetname")?.Trim() ?? "";

                var size = graphics.MeasureString(str, font);

                var pos = new PointF(loc.X - size.Width / 2, loc.Y - size.Height - 2);
                var rect = new RectangleF(pos, size);

                using (var b = new SolidBrush(c))
                {
                    graphics.DrawString(str, font, b, rect.X, rect.Y);

                    if (renderNames && targetname.Length > 0)
                    {
                        var nmms = graphics.MeasureString(targetname, nameFont);
                        graphics.DrawString(targetname, nameFont, b, loc.X - nmms.Width / 2, loc.Y + 2);
                    }
                }
            }

            nameFont.Dispose();
        }
    }
}