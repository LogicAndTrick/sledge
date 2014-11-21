using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Actions.MapObjects.Selection;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools.SelectTool
{
    public class SelectTool : BaseDraggableTool
    {
        private BoxDraggableState emptyBox;
        private SelectionBoxDraggableState selectionBox;

        public SelectTool()
        {
            selectionBox = new SelectionBoxDraggableState(this);
            selectionBox.BoxColour = Color.Yellow;
            selectionBox.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.White);
            States.Add(selectionBox);

            emptyBox = new BoxDraggableState(this);
            emptyBox.BoxColour = Color.Yellow;
            emptyBox.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.White);
            States.Add(emptyBox);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Select Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "SelectTool";
        }

        public override void ToolSelected(bool preventHistory)
        {
            //SetCurrentTool(_currentTool);
            IgnoreGroupingChanged();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeStructureChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeObjectsChanged, this);
            Mediator.Subscribe(EditorMediator.IgnoreGroupingChanged, this);

            SelectionChanged();
        }

        #region Selection/document changed
        private void DocumentTreeStructureChanged()
        {
            SelectionChanged();
        }

        private void DocumentTreeObjectsChanged(IEnumerable<MapObject> objects)
        {
            if (objects.Any(x => x.IsSelected))
            {
                SelectionChanged();
            }
        }

        private void SelectionChanged()
        {
            if (Document == null) return;
            UpdateBoxBasedOnSelection();
            //if (State.Action != BoxAction.ReadyToResize && _currentTool != null) SetCurrentTool(null);
            //else if (State.Action == BoxAction.ReadyToResize && _currentTool == null) SetCurrentTool(_lastTool ?? _tools[0]);

            //foreach (var widget in _widgets) widget.SelectionChanged();
        }

        /// <summary>
        /// Updates the box based on the currently selected objects.
        /// </summary>
        private void UpdateBoxBasedOnSelection()
        {
            if (Document.Selection.IsEmpty())
            {
                emptyBox.State.Start = emptyBox.State.End = null;
                emptyBox.State.Action = BoxAction.Idle;
                selectionBox.State.Start = selectionBox.State.End = null;
                selectionBox.State.Action = BoxAction.Idle;
            }
            else
            {
                emptyBox.State.Start = emptyBox.State.End = null;
                emptyBox.State.Action = BoxAction.Idle;

                var box = Document.Selection.GetSelectionBoundingBox();
                selectionBox.State.Start = box.Start;
                selectionBox.State.End = box.End;
                selectionBox.State.Action = BoxAction.Drawn;
            }
        }
        #endregion

        #region Ignore Grouping
        private bool IgnoreGrouping()
        {
            return Document.Map.IgnoreGrouping;
        }

        private void IgnoreGroupingChanged()
        {
            var selected = Document.Selection.GetSelectedObjects().ToList();
            var select = new List<MapObject>();
            var deselect = new List<MapObject>();
            if (Document.Map.IgnoreGrouping)
            {
                deselect.AddRange(selected.Where(x => x.HasChildren));
            }
            else
            {
                var parents = selected.Select(x => x.FindTopmostParent(y => y is Group || y is Entity) ?? x).Distinct();
                foreach (var p in parents)
                {
                    var children = p.FindAll();
                    var leaves = children.Where(x => !x.HasChildren);
                    if (leaves.All(selected.Contains)) select.AddRange(children.Where(x => !selected.Contains(x)));
                    else deselect.AddRange(children.Where(selected.Contains));
                }
            }
            if (deselect.Any() || select.Any())
            {
                Document.PerformAction("Apply group selection", new ChangeSelection(select, deselect));
            }
        }
        #endregion

        #region Perform selection

        /// <summary>
        /// If ignoreGrouping is disabled, this will convert the list of objects into their topmost group or entity.
        /// If ignoreGrouping is enabled, this will remove objects that have children from the list.
        /// </summary>
        /// <param name="objects">The object list to normalise</param>
        /// <param name="ignoreGrouping">True if grouping is being ignored</param>
        /// <returns>The normalised list of objects</returns>
        private static IEnumerable<MapObject> NormaliseSelection(IEnumerable<MapObject> objects, bool ignoreGrouping)
        {
            return ignoreGrouping
                       ? objects.Where(x => !x.HasChildren)
                       : objects.Select(x => x.FindTopmostParent(y => y is Group || y is Entity) ?? x).Distinct().SelectMany(x => x.FindAll());
        }

        /// <summary>
        /// Deselect (first) a list of objects and then select (second) another list.
        /// </summary>
        /// <param name="objectsToDeselect">The objects to deselect</param>
        /// <param name="objectsToSelect">The objects to select</param>
        /// <param name="deselectAll">If true, this will ignore the objectToDeselect parameter and just deselect everything</param>
        /// <param name="ignoreGrouping">If true, object groups will be ignored</param>
        private void SetSelected(IEnumerable<MapObject> objectsToDeselect, IEnumerable<MapObject> objectsToSelect, bool deselectAll, bool ignoreGrouping)
        {
            if (objectsToDeselect == null) objectsToDeselect = new MapObject[0];
            if (objectsToSelect == null) objectsToSelect = new MapObject[0];

            if (deselectAll)
            {
                objectsToDeselect = Document.Selection.GetSelectedObjects();
                // _lastTool = null;
            }

            // Normalise selections
            objectsToDeselect = NormaliseSelection(objectsToDeselect.Where(x => x != null), ignoreGrouping);
            objectsToSelect = NormaliseSelection(objectsToSelect.Where(x => x != null), ignoreGrouping);

            // Don't bother deselecting the objects we're about to select
            objectsToDeselect = objectsToDeselect.Where(x => !objectsToSelect.Contains(x));

            // Perform selections
            var deselected = objectsToDeselect.ToList();
            var selected = objectsToSelect.ToList();

            Document.PerformAction("Selection changed", new ChangeSelection(selected, deselected));
        }

        #endregion

        public override void ToolDeselected(bool preventHistory)
        {
            Mediator.UnsubscribeAll(this);
            // SetCurrentTool(null);
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Selection;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}