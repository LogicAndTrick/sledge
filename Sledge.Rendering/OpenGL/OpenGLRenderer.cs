using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLRenderer : IRenderer
    {
        private readonly Dictionary<IViewport, ViewportData> _viewportData;

        public OpenGLRenderer()
        {
            _viewportData = new Dictionary<IViewport, ViewportData>();
        }

        public IViewport CreateViewport()
        {
            var view = new OpenGLViewport();
            view.Render += RenderViewport;
            return view;
        }

        private struct Thing
        {
            public Vector3 Position;
        }

        private void RenderViewport(IViewport viewport, Frame frame)
        {
            var vBuffer = new OpenGLVertexBuffer<Thing>();
            vBuffer.Update(Enumerable.Range(0, 5000).Select(x => new Thing { Position = new Vector3(x, x, 0) }), new Thing[0]);

            var elementBuffer = new OpenGLElementArray<Thing>(vBuffer);
            elementBuffer.AddGroup(1, PrimitiveType.Points, t => t.GroupBy(x => (object)(x.Position.X)));
            elementBuffer.Update();
            
            var data = GetViewportData(viewport);

            // Set up FBO
            data.Framebuffer.Size = new Size(viewport.Control.Width, viewport.Control.Height);
            data.Framebuffer.Bind();

            // Set up camera
            GL.LoadIdentity();
            GL.Viewport(0, 0, viewport.Control.Width, viewport.Control.Height);

            GL.MatrixMode(MatrixMode.Projection);
            var vpMatrix = viewport.Camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            GL.LoadMatrix(ref vpMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            var camMatrix = viewport.Camera.GetCameraMatrix();
            GL.LoadMatrix(ref camMatrix);

            // Do actual render
            var colours = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < colours.Length; i++)
            {
                var a = i * 10;
                var b = (i + 1) * 10;
                GL.Color3(colours[i]);
                GL.Vertex3(a, 0, 0);
                GL.Vertex3(b, 0, 0);
                GL.Vertex3(0, a, 0);
                GL.Vertex3(0, b, 0);
                GL.Vertex3(0, 0, a);
                GL.Vertex3(0, 0, b);
            }
            GL.End();

            // Blit FBO
            data.Framebuffer.Unbind();
            data.Framebuffer.Render();

            elementBuffer.Dispose();
            vBuffer.Dispose();
        }

        private ViewportData GetViewportData(IViewport viewport)
        {
            if (!_viewportData.ContainsKey(viewport))
            {
                var data = new ViewportData(new Framebuffer(viewport.Control.Width, viewport.Control.Height));
                _viewportData.Add(viewport, data);
            }
            return _viewportData[viewport];
        }

        private class ViewportData
        {
            public Framebuffer Framebuffer { get; set; }

            public ViewportData(Framebuffer framebuffer)
            {
                Framebuffer = framebuffer;
            }
        }
    }
}