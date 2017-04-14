using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Rendering.Scenes;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class MapDocumentSceneObjectProvider : ISceneObjectProvider
    {
        public MapDocument Document { get; }

        private readonly MapObjectConverter _converter;
        private readonly ConcurrentDictionary<IMapObject, SceneMapObject> _sceneObjects;

        public MapDocumentSceneObjectProvider(MapDocument document, MapObjectConverter converter)
        {
            Document = document;
            _converter = converter;

            _sceneObjects = new ConcurrentDictionary<IMapObject, SceneMapObject>();
        }

        public event EventHandler<SceneObjectsChangedEventArgs> SceneObjectsChanged;

        public async Task Initialise()
        {
            await Update(Document.Map.Root.FindAll());
        }

        private async Task Update(IEnumerable<IMapObject> objects)
        {
            var created = new List<SceneObject>();
            var updated = new List<SceneObject>();
            var deleted = new List<SceneObject>();

            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;

                if (smo != null && await _converter.Update(smo, Document, obj))
                {
                    updated.AddRange(smo);
                    continue;
                }

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    _sceneObjects.TryRemove(obj, out SceneMapObject _);
                    deleted.AddRange(rem);
                }

                smo = await _converter.Convert(Document, obj);
                if (smo == null) continue;

                created.AddRange(smo);
                _sceneObjects[smo.MapObject] = smo;
            }

            var ea = new SceneObjectsChangedEventArgs(created, updated, deleted);
            SceneObjectsChanged?.Invoke(this, ea);
        }
    }
}