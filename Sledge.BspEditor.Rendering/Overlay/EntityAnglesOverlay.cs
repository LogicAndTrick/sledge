using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    [Export(typeof(IMapObject2DOverlay))]
    public class EntityAnglesOverlay : IMapObject2DOverlay
    {
        public void Render(IViewport viewport, ICollection<IMapObject> objects, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (camera.Zoom < 0.5f) return;

            foreach (var ed in objects.OfType<Entity>().Where(x => x.EntityData != null))
            {
                var ang = ed.EntityData.GetVector3("angles");
                if (!ang.HasValue) continue;

                var c = ed.Color?.Color ?? Color.White;

                var angRad = ang.Value * (float) Math.PI / 180f;
                var min = Math.Min(ed.BoundingBox.Width, Math.Min(ed.BoundingBox.Height, ed.BoundingBox.Length));
                var tform = Matrix4x4.CreateFromYawPitchRoll(angRad.X, angRad.Z, angRad.Y);

                var origin = ed.BoundingBox.Center;

                var start = camera.WorldToScreen(origin, camera.Width, camera.Height);
                var end = camera.WorldToScreen(origin + Vector3.Transform(Vector3.UnitX, tform) * 0.4f * min, camera.Width, camera.Height);

                using (var p = new Pen(c, 2))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawLine(p, start.X, start.Y, end.X, end.Y);
                    graphics.SmoothingMode = SmoothingMode.Default;
                }
            }
        }
    }
}