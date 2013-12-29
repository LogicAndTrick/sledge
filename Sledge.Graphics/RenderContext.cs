using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Graphics.Renderables;
using Enumerable = System.Linq.Enumerable;

namespace Sledge.Graphics
{
    public class RenderContext : IDisposable
    {
        private List<IRenderable> Renderables { get; set; }
        private bool Changed { get; set; }

        public RenderContext()
        {
            Renderables = new List<IRenderable>();
            Changed = true;
        }

        public void Add(IRenderable r)
        {
            Renderables.Add(r);
            Changed = true;
        }

        public void Remove(IRenderable r)
        {
            Renderables.Remove(r);
            Changed = true;
        }

        public void Clear()
        {
            Renderables.Clear();
            Changed = true;
        }

        public void Render(object sender)
        {
            Renderables.ForEach(x => x.Render(sender));
        }

        public T FindRenderable<T>() where T : IRenderable
        {
            return Renderables.Where(r => typeof (T) == r.GetType()).Select(r => (T)r).FirstOrDefault();
        }

        public void Dispose()
        {
            Renderables.OfType<IDisposable>().ToList().ForEach(x => x.Dispose());
            Clear();
        }
    }
}
