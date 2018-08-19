using System.Drawing;
using System.Numerics;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.Rendering.Overlay
{
    public interface IOverlayRenderable
    {
        void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics);
        void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics);
    }
}