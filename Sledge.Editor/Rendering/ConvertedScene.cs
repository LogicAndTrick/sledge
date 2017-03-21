using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.Common.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Rendering
{
    public class ConvertedScene
    {
        private readonly Document _document;
        private readonly ConcurrentDictionary<MapObject, SceneMapObject> _sceneObjects;
        private readonly ReferenceCounter<string> _usedTextures;
        private readonly TaskQueue _queue;

        public ConvertedScene(Document document)
        {
            _document = document;
            _sceneObjects = new ConcurrentDictionary<MapObject, SceneMapObject>();
            _usedTextures = new ReferenceCounter<string>();
            _queue = new TaskQueue();
        }

        public void UpdateAll()
        {
            _document.Scene.Clear();
            _sceneObjects.Clear();
            _queue.Enqueue(UpdateInternal(_document.Map.WorldSpawn.FindAll()));
        }

        public void Create(IEnumerable<MapObject> objects)
        {
            _queue.Enqueue(UpdateInternal(objects));
        }

        public void Delete(IEnumerable<MapObject> objects)
        {
            _queue.Enqueue(DeleteInternal(objects));
        }

        private async Task DeleteInternal(IEnumerable<MapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;
                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    SceneMapObject t;
                    _sceneObjects.TryRemove(obj, out t);
                    foreach (var r in rem) _document.Scene.Remove(r);
                    _usedTextures.Decrement(rem.UsedTextures);
                }
            }
        }

        public void Update(IEnumerable<MapObject> objects)
        {
            _queue.Enqueue(UpdateInternal(objects));
        }

        private async Task UpdateInternal(IEnumerable<MapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;

                if (smo != null && await MapObjectConverter.Update(smo, _document, obj)) continue;

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    SceneMapObject t;
                    _sceneObjects.TryRemove(obj, out t);
                    foreach (var r in rem) _document.Scene.Remove(r);
                    _usedTextures.Decrement(rem.UsedTextures);
                }

                smo = await MapObjectConverter.Convert(_document, obj);
                if (smo == null) continue;

                foreach (var ro in smo) _document.Scene.Add(ro);
                _sceneObjects[smo.MapObject] = smo;
                _usedTextures.Increment(smo.UsedTextures);
            }
        }
    }
}