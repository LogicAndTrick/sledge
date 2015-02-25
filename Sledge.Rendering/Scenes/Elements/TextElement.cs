using System.Collections.Generic;
using System.Drawing;
using OpenTK;
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

        public TextElement(PositionType positionType, Vector3 location, string text, Color color) : base(positionType)
        {
            Location = location;
            Text = text;
            Color = color;
            FontName = StringTextureManager.DefaultFontKey.Name;
            FontSize = StringTextureManager.DefaultFontKey.Size;
            FontStyle = StringTextureManager.DefaultFontKey.Style;
            AnchorX = AnchorY = 0.5f;
        }

        public override IEnumerable<LineElement> GetLines(IRenderer renderer)
        {
            yield break;
        }

        public override IEnumerable<FaceElement> GetFaces(IRenderer renderer)
        {
            yield return renderer.StringTextureManager.GetElement(Text, Color, PositionType, Location, AnchorX, AnchorY, FontName, FontSize, FontStyle);
        }
    }
}