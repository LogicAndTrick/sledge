using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public class PointfileHelper : IHelper
    {
        public Document Document { get; set; }
        public bool Is2DHelper { get { return false; } }
        public bool Is3DHelper { get { return false; } }
        public bool IsDocumentHelper { get { return true; } }
        public HelperType HelperType { get { return HelperType.None; } }

        public void AfterRender3D(Viewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void RenderDocument(ViewportBase viewport, Document document)
        {
            if (document.Pointfile == null) return;
            var pf = document.Pointfile;
            var vp2 = viewport as Viewport2D;
            Func<Coordinate, Coordinate> transform = x => x;
            if (vp2 != null) transform = vp2.Flatten;

            TextureHelper.Unbind();
            GL.LineWidth(3);
            GL.Begin(PrimitiveType.Lines);

            var r = 1f;
            var g = 0.5f;
            var b = 0.5f;
            var change = 0.5f / pf.Lines.Count;

            foreach (var line in pf.Lines)
            {
                var start = transform(line.Start);
                var end = transform(line.End);

                GL.Color3(r, g, b);
                GL.Vertex3(start.DX, start.DY, start.DZ);

                r -= change;
                b += change;

                GL.Color3(r, g, b);
                GL.Vertex3(end.DX, end.DY, end.DZ);
            }

            GL.End();
            GL.LineWidth(1);
        }

        public IEnumerable<MapObject> Order(ViewportBase viewport, IEnumerable<MapObject> mapObjects)
        {
            return mapObjects;
        }

        public bool IsValidFor(MapObject o)
        {
            return false;
        }

        public void BeforeRender2D(Viewport2D viewport)
        {
            throw new NotImplementedException();
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            throw new NotImplementedException();
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            throw new NotImplementedException();
        }

        public void BeforeRender3D(Viewport3D viewport)
        {
            throw new NotImplementedException();
        }

        public void Render3D(Viewport3D viewport, MapObject o)
        {
            throw new NotImplementedException();
        }
    }
}