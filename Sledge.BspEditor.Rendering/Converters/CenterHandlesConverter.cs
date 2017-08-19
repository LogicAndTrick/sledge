using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Common.Shell.Settings;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

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

        private class CenterHandleTextElement : Element
        {
            private const string Name = "Sledge.BspEditor.Rendering.CenterHandlesConverter::x";
            private static bool _initialised;

            static void Init(IRenderer renderer)
            {
                if (_initialised) return;
                _initialised = true;

                using (var bmp = new Bitmap(5, 5))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.DrawLine(Pens.White, 0, 0, 4, 4);
                        g.DrawLine(Pens.White, 4, 0, 0, 4);
                    }
                    renderer.Textures.Create(Name, bmp, bmp.Width, bmp.Height, TextureFlags.PixelPerfect);
                }

                renderer.Materials.Add(Material.Texture(Name, false));
            }

            private Vector3 _location;
            private Color _color;
            private readonly bool _activeViewportOnly;
            public override string ElementGroup => "CenterHandles";

            public CenterHandleTextElement(IMapObject obj, bool activeViewportOnly) : base(PositionType.World)
            {
                _activeViewportOnly = activeViewportOnly;
                _color = Color.FromArgb(192, obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White);
                _location = obj.BoundingBox.Center.ToVector3();
            }

            public void Update(IMapObject obj)
            {
                _location = obj.BoundingBox.Center.ToVector3();
                _color = Color.FromArgb(192, Color.FromArgb(192, obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White));
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

            public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
            {
                yield break;
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                if (_activeViewportOnly && !viewport.IsFocused) yield break;
                Init(renderer);

                var mat = new Material(MaterialType.Textured, _color, Name);
                yield return new FaceElement(PositionType, mat, new[]
                {
                    new PositionVertex(new Position(_location) { Offset = new Vector3(-2.5f, -2.5f, 0) }, 0, 0),
                    new PositionVertex(new Position(_location) { Offset = new Vector3(+2.5f, -2.5f, 0) }, 1, 0),
                    new PositionVertex(new Position(_location) { Offset = new Vector3(+2.5f, +2.5f, 0) }, 1, 1),
                    new PositionVertex(new Position(_location) { Offset = new Vector3(-2.5f, +2.5f, 0) }, 0, 1)
                })
                {
                    AccentColor = _color,
                    RenderFlags = RenderFlags.Polygon,
                    CameraFlags = CameraFlags.Orthographic
                };
            }
        }
    }
}