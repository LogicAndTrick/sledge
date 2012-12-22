using System.Collections.Generic;
using System.Linq;
using Sledge.Graphics.Renderables;
using Enumerable = System.Linq.Enumerable;

namespace Sledge.Graphics
{
    public class RenderContext
    {
        private static int _numLists = 0;

        private string ListName { get; set; }
        private List<IRenderable> Renderables { get; set; }
        private bool Changed { get; set; }

        public RenderContext()
        {
            _numLists++;
            Renderables = new List<IRenderable>();
            Changed = true;
            ListName = "RenderContext." + _numLists + ".GeometryList";
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
            return Renderables.Where(r => typeof (T).Equals(r.GetType())).Select(r => (T)r).FirstOrDefault();
        }
    }
}
