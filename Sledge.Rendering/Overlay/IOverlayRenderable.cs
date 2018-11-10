using System.Numerics;
using ImGuiNET;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.Rendering.Overlay
{
    public interface IOverlayRenderable
    {
        void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im);
        void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im);
    }
}