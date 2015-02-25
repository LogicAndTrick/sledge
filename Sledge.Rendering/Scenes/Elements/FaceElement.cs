using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
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

        public FaceElement(PositionType type, Material material, IEnumerable<PositionVertex> vertices)
            : base(type)
        {
            Material = material;
            Vertices = vertices.ToList();
            AccentColor = material.Color;
            CameraFlags = CameraFlags.All;
            RenderFlags = RenderFlags.Polygon;
        }

        public override IEnumerable<LineElement> GetLines(IRenderer renderer)
        {
            yield break;
        }

        public override IEnumerable<FaceElement> GetFaces(IRenderer renderer)
        {
            yield return this;
        }
    }
}