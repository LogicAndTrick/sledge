using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Rendering.OpenGL.Lists
{
    public class ElementListCollection : IDisposable
    {
        private readonly DisplayListRenderer _renderer;
        private readonly IViewport _viewport;
        private readonly Dictionary<string, ElementList> _lists;
        private readonly Dictionary<string, int> _counts;

        public ElementListCollection(DisplayListRenderer renderer, IViewport viewport)
        {
            _renderer = renderer;
            _viewport = viewport;
            _lists = new Dictionary<string, ElementList>();
            _counts = new Dictionary<string, int>();
        }

        public void Render(DisplayListRenderer renderer, IViewport viewport)
        {
            foreach (var array in _lists.Values)
            {
                array.Render(renderer, viewport);
            }
        }

        public void Update(IEnumerable<Element> elements)
        {
            var keys = _lists.Keys.ToList();

            foreach (var group in elements.GroupBy(x => x.ElementGroup))
            {
                UpdateGroup(group.Key, group);
                keys.Remove(group.Key);
            }

            // Remove any groups that aren't around anymore
            foreach (var key in keys)
            {
                _lists[key].Dispose();
                _lists.Remove(key);
                _counts.Remove(key);
            }
        }

        private void UpdateGroup(string group, IEnumerable<Element> elements)
        {
            var els = elements.ToList();

            if (!_lists.ContainsKey(group))
            {
                _lists.Add(group, new ElementList(_renderer, _viewport, new Element[0]));
                _lists[group].Update(els);
                _counts[group] = els.Count;
                els.ForEach(x => x.Validate(_viewport, _renderer));
            }
            else if (_counts[group] != els.Count || els.Any(x => x.RequiresValidation(_viewport, _renderer)))
            {
                _lists[group].Update(els);
                _counts[group] = els.Count;
                els.ForEach(x => x.Validate(_viewport, _renderer));
            }
        }

        public void Dispose()
        {
            foreach (var array in _lists.Values)
            {
                array.Dispose();
            }
            _lists.Clear();
        }
    }
}
