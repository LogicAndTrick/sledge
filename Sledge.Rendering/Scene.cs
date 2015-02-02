using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sledge.Rendering
{
    public class Scene
    {
        public Engine Engine { get; private set; }

        private readonly List<Light> _lights;
        private readonly List<RenderableObject> _renderables;

        public bool Dirty { get; private set; }

        public Scene(Engine engine)
        {
            Engine = engine;
            _lights = new List<Light>();
            _renderables = new List<RenderableObject>();
        }

        public void Add(Light light)
        {
            _lights.Add(light);
            Dirty = true;
        }

        public void Add(RenderableObject renderable)
        {
            _renderables.Add(renderable);
            Dirty = true;
        }

        public void Remove(Light light)
        {
            _lights.Remove(light);
            Dirty = true;
        }

        public void Remove(RenderableObject renderable)
        {
            _renderables.Remove(renderable);
            Dirty = true;
        }
    }
}