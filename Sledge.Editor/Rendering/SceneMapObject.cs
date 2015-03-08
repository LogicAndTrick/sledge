using System.Collections;
using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Providers.Texture;
using Sledge.Rendering.Scenes;

namespace Sledge.Editor.Rendering
{
    public class SceneMapObject : IEnumerable<SceneObject>
    {
        public MapObject MapObject { get; set; }
        public Dictionary<object, SceneObject> SceneObjects { get; private set; }
        public List<TextureItem> UsedTextures { get; set; }

        public SceneMapObject(MapObject mapObject)
        {
            MapObject = mapObject;
            SceneObjects = new Dictionary<object, SceneObject>();
            UsedTextures = new List<TextureItem>();
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