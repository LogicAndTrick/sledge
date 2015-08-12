using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Rendering
{
    public class ConvertedScene
    {
        private Document _document;
        private Dictionary<MapObject, SceneMapObject> _sceneObjects;
        private ReferenceCounter<string> _usedTextures;

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

                if (smo != null && obj.Update(smo, _document)) continue;

                if (smo != null)
                {
                    var rem = _sceneObjects[obj];
                    _sceneObjects.Remove(obj);
                    foreach (var r in rem) _document.Scene.Remove(r);
                    _usedTextures.Decrement(rem.UsedTextures);
                }

                smo = obj.Convert(_document);
                if (smo == null) continue;

                foreach (var ro in smo) _document.Scene.Add(ro);
                _sceneObjects[smo.MapObject] = smo;
                _usedTextures.Increment(smo.UsedTextures);
            }
        }
    }
}