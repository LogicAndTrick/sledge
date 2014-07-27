using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Tools;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class ChangeFaceSelection : IAction
    {
        public bool SkipInStack { get { return Sledge.Settings.Select.SkipSelectionInUndoStack; } }
        public bool ModifiesState { get { return false; } }

        private Dictionary<long, long> _selected;
        private Dictionary<long, long> _deselected;

        public ChangeFaceSelection(IEnumerable<Face> selected, IEnumerable<Face> deselected)
        {
            _selected = selected.Where(x => x.Parent != null).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First().Parent.ID);
            _deselected = deselected.Where(x => x.Parent != null).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First().Parent.ID);
        }

        public void Dispose()
        {
            _selected = _deselected = null;
        }

        private Face FindFace(Document document, long faceId, long parentId)
        {
            var par = document.Map.WorldSpawn.FindByID(parentId) as Solid;
            if (par == null) return null;
            return par.Faces.FirstOrDefault(x => x.ID == faceId);
        }

        public void Reverse(Document document)
        {
            var desel = _deselected.Select(x => FindFace(document, x.Key, x.Value)).Where(x => x != null).ToList();
            var sel = _selected.Select(x => FindFace(document, x.Key, x.Value)).Where(x => x != null).ToList();

            document.Selection.Select(desel);
            document.Selection.Deselect(sel);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, sel.Union(desel));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }

        public void Perform(Document document)
        {
            var desel = _deselected.Select(x => FindFace(document, x.Key, x.Value)).Where(x => x != null).ToList();
            var sel = _selected.Select(x => FindFace(document, x.Key, x.Value)).Where(x => x != null).ToList();

            document.Selection.Deselect(desel);
            document.Selection.Select(sel);

            Mediator.Publish(EditorMediator.DocumentTreeSelectedFacesChanged, sel.Union(desel));
            Mediator.Publish(EditorMediator.SelectionChanged);
        }
    }
}