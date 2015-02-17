using System.Collections.Generic;
using System.ComponentModel;
using Sledge.Rendering.Scenes.Renderables;

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

        private void ObjectUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (!_trackChanges) return;
            if (_changeSet == null) _changeSet = new SceneChangeSet();

            var obj = (SceneObject) sender;
            if (e.PropertyName == "RenderCritical")
            {
                _changeSet.Replace(obj);
            }
            else
            {
                _changeSet.Update(obj);
            }
        }

        public void Add(SceneObject obj)
        {
            if (_trackChanges && _changeSet == null) _changeSet = new SceneChangeSet();
            if (_trackChanges) _changeSet.Add(obj);
            _objects.Add(obj);
            obj.PropertyChanged += ObjectUpdated;
        }

        public void Remove(SceneObject obj)
        {
            if (_trackChanges && _changeSet == null) _changeSet = new SceneChangeSet();
            if (_trackChanges) _changeSet.Remove(obj);
            _objects.Remove(obj);
            obj.PropertyChanged -= ObjectUpdated;
        }
    }
}