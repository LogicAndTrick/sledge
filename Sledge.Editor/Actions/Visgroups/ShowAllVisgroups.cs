using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class ShowAllVisgroups : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipVisibilityInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private List<MapObject> _shown;

        public void Dispose()
        {
            _shown = null;
        }

        public void Reverse(Document document)
        {
            _shown.ForEach(x => x.IsVisgroupHidden = true);

            Mediator.Publish(EditorMediator.VisgroupsChanged);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);

            _shown = null;
        }

        public void Perform(Document document)
        {
            _shown = document.Map.WorldSpawn.FindAll()
                .Where(x => x.IsVisgroupHidden).ToList();
            _shown.ForEach(x => x.IsVisgroupHidden = false);

            Mediator.Publish(EditorMediator.VisgroupsChanged);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}