using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Scenes
{
    public class Scene
    {
        private readonly HashSet<SceneObject> _objects;
        private bool _trackChanges;
        private SceneChangeSet _changeSet;
        private readonly object _lock = new object();

        public bool TrackChanges
        {
            get => _trackChanges;
            set
            {
                if (_trackChanges == value) return;
                lock (_lock)
                {
                    _trackChanges = value;
                    _changeSet = null; // If we're just turning tracking on, nothing has changed yet
                }
            }
        }

        public bool HasChanges
        {
            get
            {
                lock (_lock)
                {
                    return _changeSet != null;
                }
            }
        }

        public IEnumerable<SceneObject> Objects => _objects;

        public Scene()
        {
            _objects = new HashSet<SceneObject>();
        }

        public SceneChangeSet ClearChanges()
        {
            lock (_lock)
            {
                var cs = _changeSet;
                _changeSet = null;
                return cs;
            }
        }

        private void ObjectUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (!_trackChanges) return;
            lock (_lock)
            {
                if (_changeSet == null)
                {
                    _changeSet = new SceneChangeSet();
                }

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
        }

        public void Add(SceneObject obj)
        {
            if (_trackChanges)
            {
                lock (_lock)
                {
                    if (_changeSet == null) _changeSet = new SceneChangeSet();
                    _changeSet.Add(obj);
                }
            }
            _objects.Add(obj);
            obj.PropertyChanged += ObjectUpdated;
        }

        public void Remove(SceneObject obj)
        {
            if (_trackChanges)
            {
                lock (_lock)
                {
                    if (_changeSet == null) _changeSet = new SceneChangeSet();
                    _changeSet.Remove(obj);
                }
            }
            _objects.Remove(obj);
            obj.PropertyChanged -= ObjectUpdated;
        }

        public void Clear()
        {
            if (_trackChanges)
            {
                lock (_lock)
                {
                    if (_changeSet == null) _changeSet = new SceneChangeSet();
                    foreach (var o in _objects)
                    {
                        _changeSet.Remove(o);
                    }
                }
            }
            foreach (var o in _objects)
            {
                o.PropertyChanged -= ObjectUpdated;
            }
            _objects.Clear();
        }
    }
}