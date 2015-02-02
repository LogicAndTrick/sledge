using System.Collections.Generic;

namespace Sledge.Rendering
{
    public class Engine
    {
        public Scene Scene { get; private set; }
        public List<IViewport> Viewports { get; private set; }
        public IRenderer Renderer { get; private set; }

        public Engine(IRenderer renderer)
        {
            Renderer = renderer;
            Scene = new Scene(this);
            Viewports = new List<IViewport>();
        }

        public IViewport CreateViewport(Camera camera)
        {
            var vp = Renderer.CreateViewport();
            vp.Camera = camera;
            Viewports.Add(vp);
            vp.Run();
            return vp;
        }
    }
}