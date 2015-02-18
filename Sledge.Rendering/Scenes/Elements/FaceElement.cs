using System.Collections.Generic;
using System.Drawing;
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
}