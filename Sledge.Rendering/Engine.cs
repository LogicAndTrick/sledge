using System.Collections.Generic;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes;

namespace Sledge.Rendering
{
    public class Engine
    {
        public List<IViewport> Viewports { get; private set; }
        public IRenderer Renderer { get; private set; }

        public Engine(IRenderer renderer)
        {
            Renderer = renderer;
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