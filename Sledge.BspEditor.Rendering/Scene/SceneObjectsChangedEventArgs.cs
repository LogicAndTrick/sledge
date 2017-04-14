using System;
using System.Collections.Generic;
using Sledge.Rendering.Scenes;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneObjectsChangedEventArgs : EventArgs
    {
        public IEnumerable<SceneObject> Created { get; }
        public IEnumerable<SceneObject> Updated { get; }
        public IEnumerable<SceneObject> Deleted { get; }

        public SceneObjectsChangedEventArgs(IEnumerable<SceneObject> created, IEnumerable<SceneObject> updated, IEnumerable<SceneObject> deleted)
        {
            Created = created;
            Updated = updated;
            Deleted = deleted;
        }
    }
}