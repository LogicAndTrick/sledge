using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Scenes.Elements
{
    public class FaceElement : Element
    {
        public RenderFlags RenderFlags { get; set; }
        public Material Material { get; set; }
        public Color AccentColor { get; set; }
        public List<PositionVertex> Vertices { get; set; }

        public FaceElement(Material material, List<PositionVertex> vertices)
        {
            Material = material;
            Vertices = vertices;
            AccentColor = material.Color;
            CameraFlags = CameraFlags.All;
            RenderFlags = RenderFlags.Polygon;
        }

        public override IEnumerable<LineElement> GetLines()
        {
            yield break;
        }

        public override IEnumerable<FaceElement> GetFaces()
        {
            yield return this;
        }
    }

    public class HandleElement : Element
    {
        public enum HandleType
        {
            Square,
            Circle,
            UpTriangle,
            DownTriangle
        }

        public Position Position { get; set; }
        public int LineWidth { get; set; }
        public Color LineColor { get; set; }
        public Color Color { get; set; }
        public int Radius { get; set; }
        public HandleType Type { get; set; }

        public HandleElement(HandleType type, Position position, int radius)
        {
            Type = type;
            Position = position;
            Radius = radius;
            LineColor = Color.Black;
            Color = Color.White;
            LineWidth = 1;
        }

        public override IEnumerable<LineElement> GetLines()
        {
            if (LineWidth <= 0) yield break;

            var verts = GetVertices().ToList();
            verts.Add(verts[0]); // loop back
            yield return new LineElement(LineColor, verts);
        }

        public override IEnumerable<FaceElement> GetFaces()
        {
            yield return new FaceElement(Material.Flat(Color), GetVertices().Select(x => new PositionVertex(x, 0, 0)).ToList());
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
            return new Position(Position.Type, Position.Location) {Offset = Position.Offset + new Vector3(x, y, 0)};
        }
    }
}