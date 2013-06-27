using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class ToggleVisgroup : IAction
    {
        private readonly int _visgroupId;
        private readonly bool _hide;
        private List<MapObject> _changed;

        public ToggleVisgroup(int visgroupId, bool visible)
        {
            // Visible makes more sense as a parameter, but hide makes more sense in implementation, so flip it in the ctor.
            _visgroupId = visgroupId;
            _hide = !visible;
        }

        public void Dispose()
        {
            _changed = null;
        }

        public void Reverse(Document document)
        {
            _changed.ForEach(x => x.IsVisgroupHidden = !_hide);

            Mediator.Publish(EditorMediator.VisgroupVisibilityChanged, _visgroupId);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);

            _changed = null;
        }

        public void Perform(Document document)
        {
            _changed = document.Map.WorldSpawn.Find(x => x.IsInVisgroup(_visgroupId), true)
                .Where(x => x.IsVisgroupHidden != _hide).ToList();
            _changed.ForEach(x => x.IsVisgroupHidden = _hide);

            Mediator.Publish(EditorMediator.VisgroupVisibilityChanged, _visgroupId);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}
