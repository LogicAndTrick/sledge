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
    public class ChangeToObjectSelectionMode : IAction
    {
        private readonly Type _toolType;
        private readonly List<Face> _selection;

        public ChangeToObjectSelectionMode(Type toolType, IEnumerable<Face> selection)
        {
            _toolType = toolType;
            _selection = new List<Face>(selection);
        }

        public void Dispose()
        {
            _selection.Clear();
        }

        public void Reverse(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToFaceSelection();
            var seln = document.Selection.GetSelectedFaces();
            document.Selection.Clear();
            document.Selection.Select(_selection);

            ToolManager.Activate(_toolType, true);

            Mediator.Publish(EditorMediator.DocumentTreeFacesChanged, _selection.Union(seln));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToObjectSelection();

            ToolManager.Activate(HotkeyTool.Selection, true);

            Mediator.Publish(EditorMediator.DocumentTreeFacesChanged, _selection);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}