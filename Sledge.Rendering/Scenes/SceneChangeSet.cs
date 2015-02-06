using System.Collections.Generic;

namespace Sledge.Rendering.Scenes
{
    public class SceneChangeSet
    {
        public List<SceneObject> Added { get; private set; }
        public List<SceneObject> Removed { get; private set; }
        public List<SceneObject> Updated { get; private set; }

        public SceneChangeSet()
        {
            Added = new List<SceneObject>();
            Removed = new List<SceneObject>();
            Updated = new List<SceneObject>();
        }

        public void Add(SceneObject obj)
        {
            Added.Add(obj);
        }

        public void Remove(SceneObject obj)
        {
            Removed.Add(obj);
        }
    }
}