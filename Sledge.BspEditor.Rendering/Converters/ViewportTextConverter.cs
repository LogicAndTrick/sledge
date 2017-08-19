using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class ViewportTextConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            smo.SceneObjects.Add(new Holder(), new ViewportLabelTextElement());
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return true;
        }

        private class Holder { }

        private class ViewportLabelTextElement : TextElement
        {
            public ViewportLabelTextElement() : base(PositionType.Screen, new Vector3(0, 0, 0), "", Color.White)
            {
                BackgroundColor = Color.FromArgb(128, Color.Red);
                AnchorX = AnchorY = 0;
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                return !GetValue<bool>(viewport, "Validated") || base.RequiresValidation(viewport, renderer);
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Validated", true);
                base.Validate(viewport, renderer);
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                var text = "";
                if (viewport.Camera.Flags.HasFlag(CameraFlags.Perspective))
                {
                    text = "3D";
                    if (viewport.Camera.RenderOptions.RenderFacePolygonTextures) text += " Textured";
                    else if (viewport.Camera.RenderOptions.RenderFacePolygons) text += " Polygons";
                    else if (viewport.Camera.RenderOptions.RenderFaceWireframe) text += " Wireframe";
                }
                else
                {
                    var oc = (OrthographicCamera) viewport.Camera;
                    text = "2D " + oc.Type;
                }
                var el = renderer.StringTextureManager.GetElement(text, Color, PositionType, Location, AnchorX, AnchorY, FontName, FontSize, FontStyle);
                el.ZIndex = 11;
                if (BackgroundColor.A > 0)
                {
                    yield return new FaceElement(el.PositionType, Material.Flat(BackgroundColor), el.Vertices.Select(x => x.Clone()))
                    {
                        ZIndex = 10
                    };
                }
                yield return el;
            }
        }
    }
}