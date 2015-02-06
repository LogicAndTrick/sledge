using System.Collections.Generic;

namespace Sledge.Rendering.Scenes
{
    public class Scene
    {
        private readonly List<SceneObject> _objects;
        private bool _updateInProgress;
        private bool _trackChanges;
        private SceneChangeSet _changeSet;

        public bool TrackChanges
        {
            get { return _trackChanges; }
            set
            {
                if (_trackChanges == value) return;
                _trackChanges = value;
                _changeSet = null; // If we're just turning tracking on, nothing has changed yet
                _updateInProgress = false;
            }
        }

        public SceneChangeSet CurrentChangeSet
        {
            get { return _updateInProgress ? null : _changeSet; }
        }

        public bool HasChanges
        {
            get { return !_updateInProgress && _changeSet != null; }
        }

        public IEnumerable<SceneObject> Objects
        {
            get { return _objects; }
        }

        public Scene()
        {
            _objects = new List<SceneObject>();
        }

        public void ClearChanges()
        {
            _changeSet = null;
            _updateInProgress = false;
        }

        public void StartUpdate()
        {
            _updateInProgress = true;
        }

        public void EndUpdate()
        {
            _updateInProgress = false;
        }

        public void Add(SceneObject obj)
        {
            if (_trackChanges && _changeSet == null) _changeSet = new SceneChangeSet();
            if (_trackChanges) _changeSet.Add(obj);
            _objects.Add(obj);
        }

        public void Remove(SceneObject obj)
        {
            if (_trackChanges && _changeSet == null) _changeSet = new SceneChangeSet();
            if (_trackChanges) _changeSet.Remove(obj);
            _objects.Remove(obj);
        }
    }
}