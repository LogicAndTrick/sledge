using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapObject2DOverlay))]
    public class EntityNamesOverlay : IMapObject2DOverlay
    {
        public void Render(IViewport viewport, ICollection<IMapObject> objects, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            if (camera.Zoom < 1) return;

            // Escape hatch in case there's too many entities on screen
            var ents = objects.OfType<Entity>().Where(x => x.EntityData != null).Where(x => !x.Data.OfType<IObjectVisibility>().Any(v => v.IsHidden)).ToList();
            if (ents.Count <= 0 || ents.Count > 1000) return;

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

                var size = im.CalcTextSize(FontType.Normal, str);

                var pos = new Vector2(loc.X - size.X / 2, loc.Y - size.Y - 2);

                im.AddText(pos, c, FontType.Normal, str);

                if (renderNames && targetname.Length > 0)
                {
                    var nmms = im.CalcTextSize(FontType.Bold, targetname);
                    im.AddText(new Vector2(loc.X - nmms.X / 2, loc.Y + 2), c, FontType.Bold, targetname);
                }
            }
        }
    }
}