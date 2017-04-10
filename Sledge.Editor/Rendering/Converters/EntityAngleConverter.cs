using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Rendering.Converters
{
    public class EntityAngleConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            if (!Sledge.Settings.View.DrawEntityAngles) return false;
            if (obj is Entity && !obj.HasChildren)
            {
                var ed = obj.GetEntityData();
                if (ed != null)
                {
                    var angles = ed.GetPropertyCoordinate("angles");
                    return angles != null;
                }
            }
            return false;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var el = new EntityAnglesLineElement(obj);
            smo.SceneObjects.Add(new Holder(), el);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
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
            public override string ElementGroup { get { return "Entity"; } }

            public EntityAnglesLineElement(MapObject obj) : base(PositionType.World, obj.Colour, new List<Position>
                                                            {
                                                                new Position(obj.BoundingBox.Center.ToVector3()),
                                                                new Position(GetEndPoint(obj))
                                                            })
            {
                CameraFlags = CameraFlags.Orthographic;
            }

            public void Update(MapObject obj)
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

            private static Vector3 GetEndPoint(MapObject obj)
            {
                var angles = obj.GetEntityData().GetPropertyCoordinate("angles");
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