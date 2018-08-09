using System.Collections;
using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Scenes;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneMapObject : IEnumerable<SceneObject>
    {
        public IMapObject MapObject { get; set; }
        public Dictionary<object, SceneObject> SceneObjects { get; private set; }

        public Dictionary<string, object> MetaData { get; set; }

        public SceneMapObject(IMapObject mapObject)
        {
            MapObject = mapObject;
            SceneObjects = new Dictionary<object, SceneObject>();
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