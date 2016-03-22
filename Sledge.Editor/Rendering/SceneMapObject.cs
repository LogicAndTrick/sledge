using System.Collections;
using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Rendering.Scenes;

namespace Sledge.Editor.Rendering
{
    public class SceneMapObject : IEnumerable<SceneObject>
    {
        public MapObject MapObject { get; set; }
        public Dictionary<object, SceneObject> SceneObjects { get; private set; }
        public HashSet<string> UsedTextures { get; set; }

        public Dictionary<string, object> MetaData { get; set; }

        public SceneMapObject(MapObject mapObject)
        {
            MapObject = mapObject;
            SceneObjects = new Dictionary<object, SceneObject>();
            UsedTextures = new HashSet<string>();
            MetaData = new Dictionary<string, object>();
        }

        public IEnumerator<SceneObject> GetEnumerator()
        {
            return SceneObjects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}