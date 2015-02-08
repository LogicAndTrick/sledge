using System.Collections.Generic;

namespace Sledge.Rendering.Scenes
{
    public class SceneChangeSet
    {
        public HashSet<SceneObject> Added { get; private set; }
        public HashSet<SceneObject> Removed { get; private set; }
        public HashSet<SceneObject> Updated { get; private set; }
        public HashSet<SceneObject> Replaced { get; private set; }

        public SceneChangeSet()
        {
            Added = new HashSet<SceneObject>();
            Removed = new HashSet<SceneObject>();
            Updated = new HashSet<SceneObject>();
            Replaced = new HashSet<SceneObject>();
        }

        public void Add(SceneObject obj)
        {
            Added.Add(obj);
        }

        public void Update(SceneObject obj)
        {
            Updated.Add(obj);
        }

        public void Replace(SceneObject obj)
        {
            Replaced.Add(obj);
        }

        public void Remove(SceneObject obj)
        {
            Removed.Add(obj);
        }
    }
}