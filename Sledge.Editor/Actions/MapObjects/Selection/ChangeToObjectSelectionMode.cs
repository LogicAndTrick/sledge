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
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipSelectionInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private readonly Type _toolType;
        private readonly Dictionary<long, long> _selection;

        public ChangeToObjectSelectionMode(Type toolType, IEnumerable<Face> selection)
        {
            _toolType = toolType;
            _selection = selection.Where(x => x.Parent != null).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First().Parent.ID);
        }

        public void Dispose()
        {
            _selection.Clear();
        }

        private Face FindFace(Document document, long faceId, long parentId)
        {
            var par = document.Map.WorldSpawn.FindByID(parentId) as Solid;
            if (par == null) return null;
            return par.Faces.FirstOrDefault(x => x.ID == faceId);
        }

        public void Reverse(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToFaceSelection();
            var seln = document.Selection.GetSelectedFaces();
            document.Selection.Clear();

            var sel = _selection.Select(x => FindFace(document, x.Key, x.Value)).Where(x => x != null).ToList();
            document.Selection.Select(sel);

            ToolManager.Activate(_toolType, true);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, sel.Union(seln));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            ToolManager.Deactivate(true);

            document.Selection.SwitchToObjectSelection();

            ToolManager.Activate(HotkeyTool.Selection, true);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, _selection);
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}