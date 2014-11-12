using OpenTK;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Renderables;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.EditorNew.Rendering
{
    public class RenderManagerRenderable : IRenderable
    {
        private readonly IMapViewport _viewport;
        private readonly RenderManager _manager;
        private readonly bool _is3D;

        public RenderManagerRenderable(IMapViewport viewport, RenderManager manager)
        {
            _viewport = viewport;
            _manager = manager;
            _is3D = viewport.Is3D;
        }

        public void Render(object sender)
        {
            Matrix4 vm = _viewport.GetViewportMatrix(),
                    cm = _viewport.GetCameraMatrix(),
                    mm = _viewport.GetModelViewMatrix();
            if (_viewport.Is3D) _manager.Draw3D((IViewport3D) _viewport, vm, cm, mm);
            else _manager.Draw2D((IViewport2D) _viewport, vm, cm, mm);
        }
    }
}