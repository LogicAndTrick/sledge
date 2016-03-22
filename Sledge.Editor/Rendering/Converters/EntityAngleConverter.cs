using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Extensions;
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

        public bool Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var cen = obj.BoundingBox.Center.ToVector3();

            var transformed = GetEndPoint(obj);

            var el = new EntityAnglesLineElement(PositionType.World, obj.Colour, new List<Position>
            {
                new Position(cen),
                new Position(transformed.ToVector3())
            });
            smo.SceneObjects.Add(new Holder(), el);
            return true;
        }

        private static Coordinate GetEndPoint(MapObject obj)
        {
            var angles = obj.GetEntityData().GetPropertyCoordinate("angles");
            angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X),
                DMath.DegreesToRadians(angles.Y));
            var m =
                new UnitMatrixMult(Matrix4.CreateRotationX((float) angles.X) * Matrix4.CreateRotationY((float) angles.Y) *
                                   Matrix4.CreateRotationZ((float) angles.Z));
            var min = Math.Min(obj.BoundingBox.Width, Math.Min(obj.BoundingBox.Height, obj.BoundingBox.Length));
            var transformed = obj.BoundingBox.Center + m.Transform(Coordinate.UnitX) * min * 0.4m;
            return transformed;
        }

        public bool Update(SceneMapObject smo, Document document, MapObject obj)
        {
            if (smo.SceneObjects.Keys.Any(x => x is Holder))
            {
                var ela = smo.SceneObjects.First(x => x.Key is Holder).Value as EntityAnglesLineElement;
                if (ela != null)
                {
                    ela.Vertices[0].Location = obj.BoundingBox.Center.ToVector3();
                    ela.Vertices[1].Location = GetEndPoint(obj).ToVector3();
                    return true;
                }
            }
            return false;
        }

        private class Holder { }
        
        private class EntityAnglesLineElement : LineElement
        {
            public EntityAnglesLineElement(PositionType type, Color color, List<Position> vertices) : base(type, color, vertices)
            {
                CameraFlags = CameraFlags.Orthographic;
            }

            public override IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer)
            {
                // Don't render if we're zoomed out
                var ortho = viewport.Camera as OrthographicCamera;
                if (ortho != null && ortho.Zoom < 0.5) return new LineElement[0];

                return base.GetLines(viewport, renderer);
            }
        }
    }
}