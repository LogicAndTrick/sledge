using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Tools;
using Sledge.Settings;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeToFaceSelectionMode : IAction
    {
        public bool SkipInStack { get { return true; } } //todo
        public bool ModifiesState { get { return false; } }

        private readonly Type _toolType;
        private readonly List<MapObject> _selection;

        public ChangeToFaceSelectionMode(Type toolType, IEnumerable<MapObject> selection)
        {
            _toolType = toolType;
            _selection = new List<MapObject>(selection);
        }

        public void Dispose()
        {
            _selection.Clear();
        }

        public void Reverse(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToObjectSelection();
            document.Selection.Clear();
            document.Selection.Select(_selection);

            ToolManager.Activate(HotkeyTool.Selection, true);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedObjectsChanged, _selection.Union(document.Selection.GetSelectedObjects()));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToFaceSelection();

            ToolManager.Activate(_toolType, true);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, document.Selection.GetSelectedFaces());
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}