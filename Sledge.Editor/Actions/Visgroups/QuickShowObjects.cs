using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class QuickShowObjects : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipVisibilityInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private List<MapObject> _objects;
        private int _removed;

        public QuickShowObjects(IEnumerable<MapObject> objects)
        {
            _objects = objects.Where(x => x.IsVisgroupHidden).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                if (!o.AutoVisgroups.Contains(_removed))
                {
                    o.AutoVisgroups.Add(_removed);
                    o.Visgroups.Add(_removed);
                }
                o.IsVisgroupHidden = true;
            }
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public void Perform(Document document)
        {
            var autohide = document.Map.GetAllVisgroups().First(x => x.Name == "Autohide");
            _removed = autohide.ID;
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                o.AutoVisgroups.Remove(_removed);
                o.Visgroups.Remove(_removed);
                o.IsVisgroupHidden = false;
            }
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}