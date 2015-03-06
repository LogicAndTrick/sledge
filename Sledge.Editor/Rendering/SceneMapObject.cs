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

        public SceneMapObject(MapObject mapObject)
        {
            MapObject = mapObject;
            SceneObjects = new Dictionary<object, SceneObject>();
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