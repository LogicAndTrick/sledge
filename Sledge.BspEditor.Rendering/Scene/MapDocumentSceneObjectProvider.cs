using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
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
            Oy.Subscribe<Change>("MapDocument:Changed", OnChange);
            await Update(Document.Map.Root.FindAll(), new IMapObject[0], new IMapObject[0]);
        }

        private async Task OnChange(Change change)
        {
            if (change.Document == Document)
            {
                await Update(change.Added, change.Updated, change.Removed);

                // Document objects are attached to the root
                if (change.HasDataChanges)
                {
                    await Update(new IMapObject[0], new[] {change.Document.Map.Root}, new IMapObject[0]);
                }
            }
        }

        private async Task Update(IEnumerable<IMapObject> create, IEnumerable<IMapObject> update, IEnumerable<IMapObject> delete)
        {
            var created = new List<SceneObject>();
            var updated = new List<SceneObject>();
            var deleted = new List<SceneObject>();

            foreach (var obj in delete)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;
                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    _sceneObjects.TryRemove(obj, out SceneMapObject _);
                    deleted.AddRange(rem);
                }
            }

            foreach (var obj in create.Union(update))
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

            if (created.Any() || updated.Any() || deleted.Any())
            {
                var ea = new SceneObjectsChangedEventArgs(created, updated, deleted);
                SceneObjectsChanged?.Invoke(this, ea);
            }
        }
    }
}