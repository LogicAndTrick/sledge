using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Rendering.Helpers
{
    public class EntityTextHelper : IHelper
    {
        public Document Document { get; set; }
        public bool Is2DHelper { get { return Sledge.Settings.View.DrawEntityNames; } }
        public bool Is3DHelper { get { return false; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Augment; } }

        public bool IsValidFor(MapObject o)
        {
            return o is Entity;
        }

        private readonly TextPrinter _printer;
        private readonly Font _printerFont;

        public EntityTextHelper()
        {
            _printer = new TextPrinter(TextQuality.Low);
            _printerFont = new Font(FontFamily.GenericSansSerif, 12, GraphicsUnit.Pixel);
        }

        public void BeforeRender2D(IViewport2D viewport)
        {
            _printer.Begin();
        }

        public void Render2D(IViewport2D viewport, MapObject o)
        {
            if (viewport.Zoom < 1) return;

            var entityData = o.GetEntityData();
            if (entityData == null) return;

            var start = viewport.WorldToScreen(viewport.Flatten(o.BoundingBox.Start));
            var end = viewport.WorldToScreen(viewport.Flatten(o.BoundingBox.End));
            if (start.X >= viewport.Width || end.X <= 0 || start.Y >= viewport.Height || end.Y <= 0) return;

            var text = entityData.Name;
            var nameProp = entityData.GetPropertyValue("targetname");
            if (!String.IsNullOrWhiteSpace(nameProp)) text += ": " + nameProp;

            // Center the text horizontally
            var wid = _printer.Measure(text, _printerFont, new RectangleF(0, 0, viewport.Width, viewport.Height));
            var cx = (float)(start.X + (end.X - start.X) / 2);
            var bounds = new RectangleF(cx - wid.BoundingBox.Width / 2, viewport.Height - (float)end.Y - _printerFont.Height - 6, viewport.Width, viewport.Height);

            _printer.Print(text, _printerFont, o.Colour, bounds);
        }

        public void AfterRender2D(IViewport2D viewport)
        {
            _printer.End();
        }

        public void BeforeRender3D(IViewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void Render3D(IViewport3D vp, MapObject o)
        {
            throw new NotImplementedException();
        }

        public void AfterRender3D(IViewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void RenderDocument(IMapViewport viewport, Document document)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MapObject> Order(IMapViewport viewport, IEnumerable<MapObject> mapObjects)
        {
            return mapObjects;
        }
    }
}