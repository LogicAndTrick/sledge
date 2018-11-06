using System.Numerics;
using ImGuiNET;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.Rendering.Overlay
{
    public interface IOverlayRenderable
    {
        void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, ImDrawListPtr im);
        void Render(IViewport viewport, PerspectiveCamera camera, ImDrawListPtr im);
    }
}