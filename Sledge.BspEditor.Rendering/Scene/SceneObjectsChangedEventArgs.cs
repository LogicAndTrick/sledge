using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Scenes;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneObjectsChangedEventArgs : EventArgs
    {
        public List<SceneObject> Created { get; }
        public List<SceneObject> Updated { get; }
        public List<SceneObject> Deleted { get; }

        public SceneObjectsChangedEventArgs(IEnumerable<SceneObject> created, IEnumerable<SceneObject> updated, IEnumerable<SceneObject> deleted)
        {
            Created = created.ToList();
            Updated = updated.ToList();
            Deleted = deleted.ToList();
        }

        public void Add(SceneObject obj) => Created.Add(obj);
        public void Add(IEnumerable<SceneObject> obj) => Created.AddRange(obj);
        public void Change(SceneObject obj) => Updated.Add(obj);
        public void Change(IEnumerable<SceneObject> obj) => Updated.AddRange(obj);
        public void Remove(SceneObject obj) => Deleted.Add(obj);
        public void Remove(IEnumerable<SceneObject> obj) => Deleted.AddRange(obj);

        public bool IsEmpty() => !Created.Any() && !Updated.Any() && !Deleted.Any();
    }
}