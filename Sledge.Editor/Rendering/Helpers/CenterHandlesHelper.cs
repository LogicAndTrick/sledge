using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public class CenterHandlesHelper : IHelper
    {
        public Document Document { get; set; }
        public bool Is2DHelper { get { return Sledge.Settings.Select.DrawCenterHandles; } }
        public bool Is3DHelper { get { return false; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Augment; } }

        public bool IsValidFor(MapObject o)
        {
            return (o is Entity || o is Solid) && !o.HasChildren;
        }

        private double _offset;
        private double _fadeDistance;
        private Coordinate _mousePos;
        public void BeforeRender2D(Viewport2D viewport)
        {
            _offset = 3 / (double)viewport.Zoom;
            _fadeDistance = 200 / (double)viewport.Zoom;
            var mp = viewport.PointToClient(Control.MousePosition);
            _mousePos = viewport.ScreenToWorld(new Coordinate(mp.X, viewport.Height - mp.Y, 0));
            GL.Enable(EnableCap.LineSmooth);
            GL.Begin(BeginMode.Lines);
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            if (Sledge.Settings.Select.CenterHandlesActiveViewportOnly && !viewport.IsFocused) return;
            var center = viewport.Flatten(o.BoundingBox.Center);
            double a = 192;
            if (Sledge.Settings.Select.CenterHandlesFollowCursor)
            {
                var dist = (double) (center - _mousePos).VectorMagnitude();
                if (dist >= _fadeDistance) return;
                a = 192 * ((_fadeDistance - dist) / _fadeDistance);
            }
            GL.Color4(Color.FromArgb((int) a, o.Colour));
            GL.Vertex2(center.DX - _offset, center.DY - _offset);
            GL.Vertex2(center.DX + _offset, center.DY + _offset);
            GL.Vertex2(center.DX - _offset, center.DY + _offset);
            GL.Vertex2(center.DX + _offset, center.DY - _offset);
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            GL.End();
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