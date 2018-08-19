using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;
using Veldrid;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Sledge.Rendering.Overlay
{
    public class ViewportOverlay
    {
        private readonly GraphicsDevice _device;
        private readonly IViewport _viewport;

        private Bitmap _bitmap;

        private int _width;
        private int _height;

        internal ViewportOverlay(GraphicsDevice device, IViewport viewport)
        {
            _device = device;
            _viewport = viewport;
            _width = -1;
            _height = -1;
        }

        private void Resize()
        {
            var vpw = _viewport.Width;
            var vph = _viewport.Height;
            if (_bitmap != null && vpw == _width && vph == _height) return;

            _width = vpw;
            _height = vph;
            _bitmap?.Dispose();
            _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
        }

        public void Build(IEnumerable<IOverlayBuilder> builders)
        {
            Resize();

            var min = Vector3.Zero;
            var max = Vector3.Zero;

            var oc = _viewport.Camera as OrthographicCamera;
            var pc = _viewport.Camera as PerspectiveCamera;

            if (oc != null)
            {
                var tl = oc.ScreenToWorld(Vector3.Zero, _width, _height);
                var br = oc.ScreenToWorld(new Vector3(_width, _height, 0), _width, _height);
                min = Vector3.Min(tl, br);
                max = Vector3.Max(tl, br);
            }

            using (var g = Graphics.FromImage(_bitmap))
            {
                foreach (var b in builders)
                {
                    if (pc != null) b.Render(_viewport, pc, g);
                    if (oc != null) b.Render(_viewport, oc, min, max, g);
                }
            }
        }
    }

    public interface IOverlayBuilder
    {
        void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics);
        void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics);
    }
}
