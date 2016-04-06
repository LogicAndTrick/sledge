using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Elements
{
    public class HandleElement : Element
    {
        public enum HandleType
        {
            Square,
            SquareTexture,
            Circle,
            UpTriangle,
            DownTriangle
        }

        internal const string SquareHandleTextureName = "Internal::Elements::HandleElement::SquareTexture";

        public Position Position { get; set; }
        public int LineWidth { get; set; }
        public Color LineColor { get; set; }
        public Color Color { get; set; }
        public int Radius { get; set; }
        public HandleType Type { get; set; }
        public override string ElementGroup { get { return "General"; } }

        public HandleElement(PositionType pType, HandleType hType, Position position, int radius) : base(pType)
        {
            Type = hType;
            Position = position;
            Radius = radius;
            LineColor = Color.Black;
            Color = Color.White;
            LineWidth = 1;
        }

        public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
        {
            return true;
        }

        public override void Validate(IViewport viewport, IRenderer renderer)
        {
            //
        }

        public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
        {
            if (LineWidth <= 0 || Type == HandleType.SquareTexture) yield break;

            var verts = GetVertices().ToList();
            verts.Add(verts[0]); // loop back
            yield return new LineElement(PositionType, LineColor, verts) { DepthTested = DepthTested, ZIndex = ZIndex };
        }

        private Material _texture;

        public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
        {
            if (Type == HandleType.SquareTexture)
            {
                if (_texture == null) _texture = new Material(MaterialType.Textured, Color, SquareHandleTextureName);
                var points = new List<PositionVertex>
                {
                    new PositionVertex(Offset(-Radius, -Radius), 0, 0),
                    new PositionVertex(Offset(+Radius, -Radius), 1, 0),
                    new PositionVertex(Offset(+Radius, +Radius), 1, 1),
                    new PositionVertex(Offset(-Radius, +Radius), 0, 1),
                };
                yield return new FaceElement(PositionType, _texture, points) { DepthTested = DepthTested, ZIndex = ZIndex };
            }
            else
            {
                yield return new FaceElement(PositionType, Material.Flat(Color), GetVertices().Select(x => new PositionVertex(x, 0, 0))) { DepthTested = DepthTested, ZIndex = ZIndex };
            }
        }

        private IEnumerable<Position> GetVertices()
        {
            switch (Type)
            {
                case HandleType.Square:
                    yield return Offset(-Radius, -Radius);
                    yield return Offset(+Radius, -Radius);
                    yield return Offset(+Radius, +Radius);
                    yield return Offset(-Radius, +Radius);
                    break;
                case HandleType.Circle:
                    int sides;
                    if (Radius < 5) sides = 8;
                    else if (Radius < 20) sides = 16;
                    else if (Radius < 50) sides = 24;
                    else if (Radius < 100) sides = 42;
                    else if (Radius < 300) sides = 64;
                    else sides = 128;
                    var diff = (2 * (float) Math.PI) / sides;
                    for (var i = 0; i < sides; i++)
                    {
                        var deg = diff * i;
                        var x = Math.Cos(deg) * Radius;
                        var y = Math.Sin(deg) * Radius;
                        yield return Offset((float) x, (float) y);
                    }
                    break;
                case HandleType.UpTriangle:
                    break;
                case HandleType.DownTriangle:
                    break;
            }
        }

        private Position Offset(float x, float y)
        {
            return new Position(Position.Location) {Offset = Position.Offset + new Vector3(x, y, 0)};
        }
    }
}