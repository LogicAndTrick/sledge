using OpenTK;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    public class RenderManagerRenderable : IRenderable
    {
        private readonly ViewportBase _viewport;
        private readonly RenderManager _manager;
        private readonly bool _is3D;

        public RenderManagerRenderable(ViewportBase viewport, RenderManager manager)
        {
            _viewport = viewport;
            _manager = manager;
            _is3D = viewport is Viewport3D;
        }

        public void Render(object sender)
        {
            Matrix4 vm = _viewport.GetViewportMatrix(),
                    cm = _viewport.GetCameraMatrix(),
                    mm = _viewport.GetModelViewMatrix();
            if (_is3D) _manager.Draw3D(_viewport, vm, cm, mm);
            else _manager.Draw2D(_viewport, vm, cm, mm);
        }
    }
}