using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Extensions;
using Sledge.UI;
using System;
using System.Drawing;
using System.Linq;

namespace Sledge.Editor.Rendering.Helpers
{
    public class EntityAngleHelper : IHelper
    {

        public Document Document { get; set; }
        public bool Is2DHelper { get { return Sledge.Settings.View.DrawEntityAngles; } }
        public bool Is3DHelper { get { return false; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Augment; } }

        public bool IsValidFor(MapObject o)
        {
            return o is Entity && !o.HasChildren;
        }

        public void BeforeRender2D(Viewport2D viewport)
        {
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.LineWidth(2);
        }

        protected static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            if (viewport.Zoom < 0.5m) return;

            var entityData = o.GetEntityData();
            if (entityData == null) return;

            var angles = entityData.GetPropertyCoordinate("angles");
            if (angles == null) return;

            angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X), DMath.DegreesToRadians(angles.Y));
            var m = new UnitMatrixMult(Matrix4.CreateRotationX((float)angles.X) * Matrix4.CreateRotationY((float)angles.Y) * Matrix4.CreateRotationZ((float)angles.Z));

            var min = Math.Min(o.BoundingBox.Width, Math.Min(o.BoundingBox.Height, o.BoundingBox.Length));
            var p1 = viewport.Flatten(o.BoundingBox.Center);
            var p2 = p1 + viewport.Flatten(m.Transform(Coordinate.UnitX)) * min * 0.4m;

            var multiplier = 4 / viewport.Zoom;
            var dir = (p2 - p1).Normalise();
            var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();

            GL.Color4(Color.FromArgb(255, o.Colour));

            GL.Begin(PrimitiveType.Lines);
            Coord(p1);
            Coord(p2);
            GL.End();

            GL.Begin(PrimitiveType.Triangles);
            Coord(p2 - (dir * 2 - cp) * multiplier);
            Coord(p2 - (dir * 2 + cp) * multiplier);
            Coord(p2);
            GL.End();
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            GL.LineWidth(1);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
        }

        public void BeforeRender3D(Viewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void Render3D(Viewport3D vp, MapObject o)
        {
            throw new NotImplementedException();
        }

        public void AfterRender3D(Viewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void RenderDocument(ViewportBase viewport, Document document)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MapObject> Order(ViewportBase viewport, IEnumerable<MapObject> mapObjects)
        {
            return mapObjects;
        }
    }
}
