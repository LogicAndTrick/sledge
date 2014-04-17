using OpenTK;
using OpenTK.Graphics;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Extensions;
using Sledge.UI;
using System;
using System.Collections.Generic;
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
            return o is Entity && !o.Children.Any();
        }

        private List<Coordinate> arrow;

        public EntityAngleHelper()
        {
            var end = new Coordinate(1.0m, 0.0m, 0.0m);
            arrow = new List<Coordinate>{
                new Coordinate(0m, 0m, 0m), end,
                new Coordinate(0.6m, 0.2m, 0m), end,
                new Coordinate(0.6m, -0.2m, 0m), end,
                new Coordinate(0.6m, 0m, 0.2m), end,
                new Coordinate(0.6m, 0m, -0.2m), end
            };
        }

        public void BeforeRender2D(Viewport2D viewport)
        {
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(2);
            GL.Begin(BeginMode.Lines);
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            var entityData = o.GetEntityData();
            if (entityData == null) return;

            var angles = entityData.GetPropertyCoordinate("angles");
            if (angles == null) return;

            angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X), DMath.DegreesToRadians(angles.Y));
            var center = viewport.Flatten(o.BoundingBox.Center);
            double scale = (double)o.BoundingBox.Width * 0.48d;
            UnitMatrixMult m = new UnitMatrixMult(Matrix4.CreateRotationX((float)angles.X) 
                                                * Matrix4.CreateRotationY((float)angles.Y) 
                                                * Matrix4.CreateRotationZ((float)angles.Z));

            GL.Color4(Color.FromArgb(255, o.Colour));
            foreach (Coordinate c in arrow)
            {
                var v = m.Transform(c);
                v = viewport.Flatten(v);
                GL.Vertex3(center.DX + v.DX * scale, center.DY + v.DY * scale, center.DZ + v.DZ * scale);
            }
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            GL.End();
            GL.LineWidth(1);
            GL.Disable(EnableCap.LineSmooth);
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
    }
}
