using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Common;
using Sledge.Common.Shell.Settings;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    [Export(typeof(ISettingsContainer))]
    public class EntityAngleConverter : IMapObjectSceneConverter, ISettingsContainer
    {
        // Settings

        [Setting("DrawEntityAngles")] private bool _drawEntityAngles = true;

        string ISettingsContainer.Name => "Sledge.BspEditor.Rendering.Converters.EntityAngleConverter";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Rendering", "DrawEntityAngles", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            if (!_drawEntityAngles) return false;
            if (obj is Entity && !obj.Hierarchy.HasChildren)
            {
                var ed = obj.Data.GetOne<EntityData>();
                if (ed != null)
                {
                    return ed.GetCoordinate("angles") != null;
                }
            }
            return false;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var el = new EntityAnglesLineElement(obj);
            smo.SceneObjects.Add(new Holder(), el);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            if (smo.SceneObjects.Keys.Any(x => x is Holder))
            {
                var ela = smo.SceneObjects.First(x => x.Key is Holder).Value as EntityAnglesLineElement;
                if (ela != null)
                {
                    ela.Update(obj);
                    return true;
                }
            }
            return false;
        }

        private class Holder { }
        
        private class EntityAnglesLineElement : LineElement
        {
            public override string ElementGroup => "Entity";

            public EntityAnglesLineElement(IMapObject obj) : base(PositionType.World, obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White, new List<Position>
                                                            {
                                                                new Position(obj.BoundingBox.Center.ToVector3()),
                                                                new Position(GetEndPoint(obj))
                                                            })
            {
                CameraFlags = CameraFlags.Orthographic;
            }

            public void Update(IMapObject obj)
            {
                Vertices[0].Location = obj.BoundingBox.Center.ToVector3();
                Vertices[1].Location = GetEndPoint(obj);
                ClearValue("Validated");
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                return !GetValue<bool>(viewport, "Validated") || GetValue(viewport, "Zoomed", viewport.Camera.Zoom >= 0.5) != viewport.Camera.Zoom >= 0.5;
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Zoomed", viewport.Camera.Zoom >= 0.5);
                SetValue(viewport, "Validated", true);
            }

            public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
            {
                // Don't render if we're zoomed out
                var ortho = viewport.Camera as OrthographicCamera;
                if (ortho != null && ortho.Zoom < 0.5) return new LineElement[0];

                return base.GetLines(viewport, renderer);
            }

            private static Vector3 GetEndPoint(IMapObject obj)
            {
                var angles = obj.Data.GetOne<EntityData>()?.GetCoordinate("angles") ?? obj.BoundingBox.Center;
                angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X),
                    DMath.DegreesToRadians(angles.Y));
                var m =
                    new UnitMatrixMult(Matrix4.CreateRotationX((float)angles.X) * Matrix4.CreateRotationY((float)angles.Y) *
                                       Matrix4.CreateRotationZ((float)angles.Z));
                var min = Math.Min(obj.BoundingBox.Width, Math.Min(obj.BoundingBox.Height, obj.BoundingBox.Length));
                var transformed = obj.BoundingBox.Center + m.Transform(Coordinate.UnitX) * min * 0.4m;
                return transformed.ToVector3();
            }
        }
    }
}