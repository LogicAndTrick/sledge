using System.Collections.Generic;
using System.Drawing;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Scenes.Elements
{
    public class LineElement : Element
    {
        public bool Stippled { get; set; }
        public float Width { get; set; }
        public Color Color { get; private set; }
        public Color PointColor { get; set; }
        public List<Position> Vertices { get; set; }
        public override string ElementGroup { get { return "General"; } }

        public LineElement(PositionType type, Color color, List<Position> vertices) : base(type)
        {
            Color = color;
            PointColor = Color.Transparent;
            Vertices = vertices;
            Width = 1;
            Smooth = true;
            DepthTested = false;
            Stippled = false;
            CameraFlags = CameraFlags.All;
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
            yield return this;
        }

        public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
        {
            yield break;
        }
    }
}