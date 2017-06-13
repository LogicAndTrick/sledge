using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Common.Shell.Settings;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    [Export(typeof(ISettingsContainer))]
    public class CenterHandlesConverter : IMapObjectSceneConverter, ISettingsContainer
    {
        // Settings

        [Setting("DrawCenterHandles")] private bool _drawCenterHandles = true;
        [Setting("CenterHandlesActiveViewportOnly")] private bool _centerHandlesActiveViewportOnly = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Rendering.Converters.CenterHandlesConverter";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Rendering", "DrawCenterHandles", typeof(bool));
            yield return new SettingKey("Rendering", "CenterHandlesActiveViewportOnly", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            if (!_drawCenterHandles) return false;
            return obj is Entity || obj is Solid;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var el = new CenterHandleTextElement(obj, _centerHandlesActiveViewportOnly);
            smo.SceneObjects.Add(new Holder(), el);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
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
            private readonly bool _activeViewportOnly;
            public override string ElementGroup => "CenterHandles";

            public CenterHandleTextElement(IMapObject obj, bool activeViewportOnly) : base(PositionType.World, obj.BoundingBox.Center.ToVector3(), "Å~", Color.FromArgb(192, obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White))
            {
                _activeViewportOnly = activeViewportOnly;
            }

            public void Update(IMapObject obj)
            {
                Location = obj.BoundingBox.Center.ToVector3();
                Color = Color.FromArgb(192, Color.FromArgb(192, obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White));
                ClearValue("Validated");
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                if (_activeViewportOnly && viewport.IsFocused != GetValue<bool>(viewport, "Focused"))
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
                if (_activeViewportOnly && !viewport.IsFocused) return new FaceElement[0];
                return base.GetFaces(viewport, renderer).Select(x =>
                {
                    x.CameraFlags = CameraFlags.Orthographic;
                    return x;
                });
            }
        }
    }
}