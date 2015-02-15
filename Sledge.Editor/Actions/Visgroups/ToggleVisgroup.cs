using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class ToggleVisgroup : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipVisibilityInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private readonly int _visgroupId;
        private readonly bool _hide;
        private List<MapObject> _changed;
        private List<MapObject> _deselected;

        public ToggleVisgroup(int visgroupId, bool visible)
        {
            // Visible makes more sense as a parameter, but hide makes more sense in implementation, so flip it in the ctor.
            _visgroupId = visgroupId;
            _hide = !visible;
        }

        public void Dispose()
        {
            _changed = null;
            _deselected = null;
        }

        public void Reverse(Document document)
        {
            _changed.ForEach(x => x.IsVisgroupHidden = !_hide);
            var vg = document.Map.Visgroups.FirstOrDefault(x => x.ID == _visgroupId);
            if (vg != null) vg.Visible = _hide;

            if (_deselected != null)
            {
                document.Selection.Select(_deselected);
                Mediator.Publish(EditorMediator.SelectionChanged);
            }

            Mediator.Publish(EditorMediator.VisgroupVisibilityChanged, _visgroupId);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);

            _changed = null;
            _deselected = null;
        }

        public void Perform(Document document)
        {
            _changed = document.Map.WorldSpawn
                .Find(x => x.IsInVisgroup(_visgroupId, true), true)
                .Where(x => x.IsVisgroupHidden != _hide).ToList();
            _changed.ForEach(x => x.IsVisgroupHidden = _hide);
            var vg = document.Map.Visgroups.FirstOrDefault(x => x.ID == _visgroupId);
            if (vg != null) vg.Visible = !_hide;

            if (_hide)
            {
                _deselected = _changed.Where(x => x.IsSelected).ToList();
                document.Selection.Deselect(_deselected);
                Mediator.Publish(EditorMediator.SelectionChanged);
            }

            Mediator.Publish(EditorMediator.VisgroupVisibilityChanged, _visgroupId);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}
