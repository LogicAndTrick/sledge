using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Rendering
{
    public class ConvertedScene
    {
        private readonly Document _document;
        private readonly Dictionary<MapObject, SceneMapObject> _sceneObjects;
        private readonly ReferenceCounter<string> _usedTextures;

        public ConvertedScene(Document document)
        {
            _document = document;
            _sceneObjects = new Dictionary<MapObject, SceneMapObject>();
            _usedTextures = new ReferenceCounter<string>();
        }

        public void UpdateAll()
        {
            _document.Scene.Clear();
            _sceneObjects.Clear();
            Update(_document.Map.WorldSpawn.FindAll());
        }

        public void Create(IEnumerable<MapObject> objects)
        {
            Update(objects);
        }

        public void Delete(IEnumerable<MapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;
                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    _sceneObjects.Remove(obj);
                    foreach (var r in rem) _document.Scene.Remove(r);
                    _usedTextures.Decrement(rem.UsedTextures);
                }
            }
        }

        public void Update(IEnumerable<MapObject> objects)
        {
            foreach (var obj in objects)
            {
                var smo = _sceneObjects.ContainsKey(obj) ? _sceneObjects[obj] : null;

                if (smo != null && MapObjectConverter.Update(smo, _document, obj)) continue;

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    _sceneObjects.Remove(obj);
                    foreach (var r in rem) _document.Scene.Remove(r);
                    _usedTextures.Decrement(rem.UsedTextures);
                }

                smo = MapObjectConverter.Convert(_document, obj);
                if (smo == null) continue;

                foreach (var ro in smo) _document.Scene.Add(ro);
                _sceneObjects[smo.MapObject] = smo;
                _usedTextures.Increment(smo.UsedTextures);
            }
        }
    }
}