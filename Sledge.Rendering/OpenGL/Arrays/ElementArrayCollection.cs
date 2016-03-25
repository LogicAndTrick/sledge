using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ElementArrayCollection : IDisposable
    {
        private readonly IRenderer _renderer;
        private readonly IViewport _viewport;
        private readonly Dictionary<string, ElementVertexArray> _arrays;
        private readonly Dictionary<string, int> _counts;

        public ElementArrayCollection(IRenderer renderer, IViewport viewport)
        {
            _renderer = renderer;
            _viewport = viewport;
            _arrays = new Dictionary<string, ElementVertexArray>();
            _counts = new Dictionary<string, int>();
        }

        public void Render(IRenderer renderer, Passthrough shader, IViewport viewport)
        {
            foreach (var array in _arrays.Values)
            {
                array.Render(renderer, shader, viewport);
            }
        }

        public void Update(IEnumerable<Element> elements)
        {
            var keys = _arrays.Keys.ToList();

            foreach (var group in elements.GroupBy(x => x.ElementGroup))
            {
                UpdateGroup(group.Key, group);
                keys.Remove(group.Key);
            }

            // Remove any groups that aren't around anymore
            foreach (var key in keys)
            {
                _arrays[key].Dispose();
                _arrays.Remove(key);
                _counts.Remove(key);
            }
        }

        private void UpdateGroup(string group, IEnumerable<Element> elements)
        {
            var els = elements.ToList();

            if (!_arrays.ContainsKey(group))
            {
                _arrays.Add(group, new ElementVertexArray(_renderer, _viewport, new Element[0]));
                _arrays[group].Update(els);
                _counts[group] = els.Count;
                els.ForEach(x => x.Validate(_viewport, _renderer));
            }
            else if (_counts[group] != els.Count || els.Any(x => x.RequiresValidation(_viewport, _renderer)))
            {
                _arrays[group].Update(els);
                _counts[group] = els.Count;
                els.ForEach(x => x.Validate(_viewport, _renderer));
            }
        }

        public void Dispose()
        {
            foreach (var array in _arrays.Values)
            {
                array.Dispose();
            }
            _arrays.Clear();
        }
    }
}
