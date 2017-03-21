using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Rendering.Converters
{
    public class CenterHandlesConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            if (!Sledge.Settings.Select.DrawCenterHandles) return false;
            return obj is Entity || obj is Solid;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var el = new CenterHandleTextElement(obj);
            smo.SceneObjects.Add(new Holder(), el);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            if (smo.SceneObjects.Keys.Any(x => x is Holder))
            {
                var ela = smo.SceneObjects.First(x => x.Key is Holder).Value as CenterHandleTextElement;
                if (ela != null)
                {
                    ela.Update(obj);
                    return true;
                }
            }
            return false;
        }

        private class Holder { }

        private class CenterHandleTextElement : TextElement
        {
            public override string ElementGroup { get { return "CenterHandles"; } }

            public CenterHandleTextElement(MapObject obj) : base(PositionType.World, obj.BoundingBox.Center.ToVector3(), "Å~", Color.FromArgb(192, obj.Colour))
            {

            }

            public void Update(MapObject obj)
            {
                Location = obj.BoundingBox.Center.ToVector3();
                Color = Color.FromArgb(192, obj.Colour);
                ClearValue("Validated");
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                if (Sledge.Settings.Select.CenterHandlesActiveViewportOnly && viewport.IsFocused != GetValue<bool>(viewport, "Focused"))
                {
                    return true;
                }
                return !GetValue<bool>(viewport, "Validated");
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Focused", viewport.IsFocused);
                SetValue(viewport, "Validated", true);
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                if (Sledge.Settings.Select.CenterHandlesActiveViewportOnly && !viewport.IsFocused) return new FaceElement[0];
                return base.GetFaces(viewport, renderer).Select(x =>
                {
                    x.CameraFlags = CameraFlags.Orthographic;
                    return x;
                });
            }
        }
    }
}