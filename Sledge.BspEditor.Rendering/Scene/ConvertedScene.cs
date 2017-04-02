using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class ConvertedScene : IDisposable
    {
        public Sledge.Rendering.Scenes.Scene Scene { get; }
        public MapDocument Document { get; }

        private readonly MapObjectConverter _converter;
        private readonly ConcurrentDictionary<IMapObject, SceneMapObject> _sceneObjects;

        public ConvertedScene(MapDocument document, MapObjectConverter converter)
        {
            Scene = Renderer.Instance.Engine.Renderer.CreateScene();
            Document = document;
            _converter = converter;
            _sceneObjects = new ConcurrentDictionary<IMapObject, SceneMapObject>();
        }

        public async Task UpdateAll()
        {
            Scene.Clear();
            _sceneObjects.Clear();
            await Update(Document.Map.Root.FindAll());
        }

        public Task Create(IEnumerable<IMapObject> objects)
        {
            return Update(objects);
        }
        
        public async Task Delete(IEnumerable<IMapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;
                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    SceneMapObject t;
                    _sceneObjects.TryRemove(obj, out t);
                    foreach (var r in rem) Scene.Remove(r);
                }
            }
        }

        public async Task Update(IEnumerable<IMapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;

                if (smo != null && await _converter.Update(smo, Document, obj)) continue;

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    SceneMapObject t;
                    _sceneObjects.TryRemove(obj, out t);
                    foreach (var r in rem) Scene.Remove(r);
                }

                smo = await _converter.Convert(Document, obj);
                if (smo == null) continue;

                foreach (var ro in smo) Scene.Add(ro);
                _sceneObjects[smo.MapObject] = smo;
            }
        }

        public void Dispose()
        {
            Renderer.Instance.Engine.Renderer.RemoveScene(Scene);
        }
    }
}