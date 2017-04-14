using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.Rendering.Scenes;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class ConvertedScene : IDisposable
    {
        public Sledge.Rendering.Scenes.Scene Scene { get; }
        public MapDocument Document { get; }

        private readonly List<ISceneObjectProvider> _providers;
        private readonly ConcurrentDictionary<ISceneObjectProvider, List<SceneObject>> _sceneObjects;

        public ConvertedScene(MapDocument document)
        {
            Scene = Renderer.Instance.Engine.Renderer.CreateScene();
            Document = document;
            _providers = new List<ISceneObjectProvider>();
            _sceneObjects = new ConcurrentDictionary<ISceneObjectProvider, List<SceneObject>>();
        }

        public async Task AddProvider(ISceneObjectProviderFactory factory)
        {
            var p = factory.MakeProvider(Document);
            if (p != null)
            {
                _providers.Add(p);
                p.SceneObjectsChanged += SceneObjectsChanged;
                await p.Initialise();
            }
        }

        private void SceneObjectsChanged(object sender, SceneObjectsChangedEventArgs e)
        {
            var p = sender as ISceneObjectProvider;
            if (p == null) return;

            if (!_sceneObjects.TryGetValue(p, out List<SceneObject> list))
            {
                list = new List<SceneObject>();
                _sceneObjects[p] = list;
            }

            foreach (var d in e.Deleted.Union(e.Updated))
            {
                list.Remove(d);
                Scene.Remove(d);
            }

            foreach (var c in e.Created.Union(e.Updated))
            {
                list.Add(c);
                Scene.Add(c);
            }
        }

        public void Dispose()
        {
            _providers.ForEach(x => x.SceneObjectsChanged -= SceneObjectsChanged);
            Renderer.Instance.Engine.Renderer.RemoveScene(Scene);
        }
    }
}