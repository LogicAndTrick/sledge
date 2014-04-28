using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class QuickHideObjects : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipVisibilityInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private List<MapObject> _objects;
        private List<MapObject> _selection;
        private int _added;

        public QuickHideObjects(IEnumerable<MapObject> objects)
        {
            _objects = objects.Where(x => !x.IsVisgroupHidden).ToList();
        }

        public void Dispose()
        {
            _objects = _selection = null;
        }

        public void Reverse(Document document)
        {
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                o.AutoVisgroups.Remove(_added);
                o.Visgroups.Remove(_added);
                o.IsVisgroupHidden = false;
                if (_selection.Contains(o))
                {
                    document.Selection.Select(o);
                }
            }
            _selection = null;
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.SelectionChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public void Perform(Document document)
        {
            var autohide = document.Map.GetAllVisgroups().First(x => x.Name == "Autohide");
            _added = autohide.ID;
            _selection = new List<MapObject>();
            foreach (var mapObject in _objects)
            {
                var o = mapObject;
                if (!o.AutoVisgroups.Contains(_added))
                {
                    o.AutoVisgroups.Add(_added);
                    o.Visgroups.Add(_added);
                }
                o.IsVisgroupHidden = true;
                if (o.IsSelected)
                {
                    _selection.Add(o);
                    document.Selection.Deselect(o);
                }
            }

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.SelectionChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}