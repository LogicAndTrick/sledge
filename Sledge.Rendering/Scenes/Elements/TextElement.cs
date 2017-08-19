using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Elements
{
    public class TextElement : Element
    {
        public Vector3 Location { get; set; }
        public string Text { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public float AnchorX { get; set; }
        public float AnchorY { get; set; }
        public Color Color { get; set; }
        public Color BackgroundColor { get; set; }
        public bool ClampToViewport { get; set; }
        public Vector3 ScreenOffset { get; set; }
        public override string ElementGroup { get { return "General"; } }

        public TextElement(PositionType positionType, Vector3 location, string text, Color color) : base(positionType)
        {
            Location = location;
            Text = text;
            Color = color;
            BackgroundColor = Color.Transparent;
            FontName = StringTextureManager.DefaultFontKey.Name;
            FontSize = StringTextureManager.DefaultFontKey.Size;
            FontStyle = StringTextureManager.DefaultFontKey.Style;
            AnchorX = AnchorY = 0.5f;
            ClampToViewport = false;
            ScreenOffset = Vector3.Zero;
        }

        public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
        {
            var v = GetValue<StringTextureManager.StringTextureValue>(viewport, "LastValue");
            return v == null || v.IsRemoved;
        }

        public override void Validate(IViewport viewport, IRenderer renderer)
        {
            SetValue(viewport, "LastValue", renderer.StringTextureManager.GetTextureValue(Text, FontName, FontSize, FontStyle));
        }

        public Size GetSize(IRenderer renderer)
        {
            return renderer.StringTextureManager.GetSize(Text, FontName, FontSize, FontStyle);
        }

        public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
        {
            yield break;
        }

        public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
        {
            var el = renderer.StringTextureManager.GetElement(Text, Color, PositionType, Location, AnchorX, AnchorY, FontName, FontSize, FontStyle);
            foreach (var v in el.Vertices) v.Position.Offset += ScreenOffset;

            if (ClampToViewport && Viewport != null)
            {
                var rec = new Rectangle(0, 0, Viewport.Control.Width, Viewport.Control.Height);

                Vector3 ConvertToScreen(Vector3 x) => PositionType == PositionType.Screen ? x : Viewport.Camera.WorldToScreen(x, rec.Width, rec.Height);
                Vector3 ConvertToWorld(Vector3 x) => PositionType == PositionType.Screen ? x : Viewport.Camera.Expand(new Vector3(Viewport.Camera.PixelsToUnits(x.X), -Viewport.Camera.PixelsToUnits(x.Y), Viewport.Camera.PixelsToUnits(x.Z)));

                var xvals = el.Vertices.Select(x => ConvertToScreen(x.Position.Location).X + x.Position.Offset.X).ToArray();
                var minX = xvals.Min();
                var maxX = xvals.Max();
                if (minX < rec.Left) el.Vertices.ForEach(x => x.Position.Location += ConvertToWorld(new Vector3(rec.Left - minX, 0, 0)));
                else if (maxX > rec.Right) el.Vertices.ForEach(x => x.Position.Location += ConvertToWorld(new Vector3(rec.Right - maxX, 0, 0)));

                var yvals = el.Vertices.Select(x => ConvertToScreen(x.Position.Location).Y + x.Position.Offset.Y).ToArray();
                var minY = yvals.Min();
                var maxY = yvals.Max();
                if (minY < rec.Top) el.Vertices.ForEach(x => x.Position.Location += ConvertToWorld(new Vector3(0, rec.Top - minY, 0)));
                else if (maxY > rec.Bottom) el.Vertices.ForEach(x => x.Position.Location += ConvertToWorld(new Vector3(0, rec.Bottom - maxY, 0)));
            }
            if (BackgroundColor.A > 0)
            {
                yield return new FaceElement(el.PositionType, Material.Flat(BackgroundColor), el.Vertices.Select(x => x.Clone()));
            }
            yield return el;
        }
    }
}