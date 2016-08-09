using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Rendering.OpenGL.Lists
{
    public class ElementList : IDisposable
    {
        private IRenderer _renderer;
        private IViewport _viewport;

        public ElementList(DisplayListRenderer renderer, IViewport viewport, IEnumerable<Element> elements)
        {
            _renderer = renderer;
            _viewport = viewport;

            // something with elements...
        }

        public void Render(DisplayListRenderer renderer, IViewport viewport)
        {
            //
        }

        public void Dispose()
        {
            // 
        }

        public void Update(List<Element> elements)
        {
            // 
        }
    }
}
