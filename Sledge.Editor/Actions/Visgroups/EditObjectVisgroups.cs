using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class EditObjectVisgroups : IAction
    {
        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private List<MapObject> _objects;
        private List<int> _add;
        private List<int> _remove;
        private Dictionary<long, List<int>> _originals;

        public EditObjectVisgroups(IEnumerable<MapObject> objects, IEnumerable<int> add, IEnumerable<int> remove)
        {
            _objects = objects.ToList();
            _add = add.ToList();
            _remove = remove.ToList();
        }

        public void Dispose()
        {
            _objects = null;
            _add = _remove = null;
            _originals = null;
        }

        public void Reverse(Document document)
        {
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                o.Visgroups.Clear();
                o.Visgroups.AddRange(_originals[o.ID]);
            }
            _originals = null;
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public void Perform(Document document)
        {
            _originals = new Dictionary<long, List<int>>();
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                _originals.Add(o.ID, new List<int>(o.Visgroups));
                o.Visgroups.RemoveAll(x => _remove.Contains(x));
                o.Visgroups.AddRange(_add.Where(i => !o.Visgroups.Contains(i)).Distinct());
            }
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}