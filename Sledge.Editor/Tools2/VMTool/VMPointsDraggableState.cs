using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.VMTool.Actions;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMPointsDraggableState : BaseDraggable, IDraggableState
    {
        private VMTool _tool;
        private List<VMPoint> _points;
        private List<VMSolid> _solids;

        public VMPointsDraggableState(VMTool tool)
        {
            _tool = tool;
            _points = new List<VMPoint>();
            _solids = new List<VMSolid>();
        }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return _points;
        }

        public void Commit()
        {
            Commit(_solids);
        }

        private void Commit(IEnumerable<VMSolid> solids)
        {
            var list = solids.ToList();
            if (!list.Any()) return;

            // Reset all changes
            foreach (var s in list)
            {
                _solids.Remove(s);
                _points.RemoveAll(x => x.Solid == s);
                s.Original.IsCodeHidden = s.Copy.IsCodeHidden = false;
                s.Original.Faces.ForEach(x => x.IsSelected = false);
                s.Copy.Faces.ForEach(x => x.IsSelected = false);
            }

            // Commit the changes
            if (list.Any(x => x.IsDirty))
            {
                var dirty = list.Where(x => x.IsDirty).ToList();
                var edit = new ReplaceObjects(dirty.Select(x => x.Original), dirty.Select(x => x.Copy));
                _tool.PerformAndCommitAction("Vertex manipulation on " + dirty.Count + " solid" + (dirty.Count == 1 ? "" : "s"), edit);
            }

            // Notify that we've unhidden some stuff
            if (list.Any(x => !x.IsDirty))
            {
                Mediator.Publish(EditorMediator.SceneObjectsUpdated, list.Where(x => !x.IsDirty).Select(x => x.Original));
            }

            _tool.Invalidate();
        }

        public void Clear()
        {
            _solids.Clear();
            _points.Clear();
            _tool.Invalidate();
        }

        public void SelectionChanged()
        {
            // Find the solids that were selected and now aren't
            var sel = _tool.Document.Selection.GetSelectedObjects().SelectMany(x => x.FindAll()).OfType<Solid>().Distinct().ToList();
            var diff = _solids.Where(x => !sel.Contains(x.Original));

            // Commit the difference
            Commit(diff);
            Clear();

            // Find the solids that are now selected and weren't before
            var newSolids = sel.Where(x => _solids.All(y => y.Original.ID != x.ID)).ToList();
            foreach (var so in newSolids)
            {
                var vmsolid = new VMSolid(this, so);
                so.IsCodeHidden = true;
                _solids.Add(vmsolid);
                _points.AddRange(vmsolid.Points);
            }

            _points.Sort((a, b) => b.IsMidpoint.CompareTo(a.IsMidpoint));

            // Notify if we've changed anything
            if (newSolids.Any())
            {
                Mediator.Publish(EditorMediator.SceneObjectsUpdated, newSolids);
            }

            _tool.Invalidate();
        }

        public void RefreshPoints(IList<VMSolid> solids)
        {
            var newPoints = _points.Except(solids.SelectMany(x => x.Points)).ToList();
            foreach (var solid in solids) solid.RefreshPoints();
            _points = newPoints.Union(solids.SelectMany(x => x.Points)).ToList();
            _points.Sort((a, b) => b.IsMidpoint.CompareTo(a.IsMidpoint));
        }

        public VMPoint GetPointByID(long objectId, int pointId)
        {
            return _points.FirstOrDefault(x => x.ID == pointId && x.Solid.Original.ID == objectId);
        }

        /*
         * Selection in 2D:
         * MouseDown:
         *   If ctrl is not down, deselect all
         *   Points <- all vertices under cursor
         *   If any Points is standard Points <- Points where standard
         *   Topmost <- closest Points to viewport
         *   If shift is down, Points <- { Topmost }
         *   Points <- Points where Solid = Topmost.Solid
         *   Val <- true
         *   If ctrl is down, Val <- !TopMost.IsSelected
         *   Topmost:
         *     If null, try 2D object selection instead
         *     Otherwise, Points each IsSelected <- Val
         * 
         */

        public List<VMPoint> GetPoints(MapViewport viewport, Coordinate position, bool allowMixed, bool topmostOnly, bool oneSolidOnly)
        {
            var p = viewport.Flatten(position);
            var d = 5 / (decimal) viewport.Zoom; // Tolerance value = 5 pixels

            // Order by the unused coordinate in the view (which is the up axis) descending to get the "closest" point
            var points = (from pp in _points
                let c = viewport.Flatten(pp.Position)
                where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                let unused = viewport.GetUnusedCoordinate(pp.Position)
                orderby unused.X + unused.Y + unused.Z
                select pp).ToList();

            if (!allowMixed && points.Any(x => !x.IsMidpoint)) points.RemoveAll(x => x.IsMidpoint);
            if (points.Count <= 0) return points;

            var first = points[0];
            if (topmostOnly) points = new List<VMPoint> {first};
            if (oneSolidOnly) points.RemoveAll(x => x.Solid != first.Solid);

            return points;
        }

        private bool _selectOnClick;

        public void PointMouseDown(MapViewport viewport, VMPoint point)
        {
            // todo box cancel?
            var vtxs = GetPoints(viewport, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;

            _selectOnClick = true;
            if (!vtxs.Any(x => x.IsSelected))
            {
                Select(vtxs, KeyboardState.Ctrl);
                _selectOnClick = false;
            }
        }

        public void PointClick(MapViewport viewport, VMPoint point)
        {
            if (!_selectOnClick) return;
            _selectOnClick = false;

            var vtxs = GetPoints(viewport, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;
            Select(vtxs, KeyboardState.Ctrl);
        }

        private void Select(List<VMPoint> points, bool toggle)
        {
            if (!points.Any()) return;
            if (!toggle) _points.ForEach(x => x.IsSelected = false);
            var first = points[0];
            var val = !toggle || !first.IsSelected;
            points.ForEach(x => x.IsSelected = val);

            _tool.Invalidate();
        }

        public void DeselectAll()
        {
            _points.ForEach(x => x.IsSelected = false);

            _tool.Invalidate();
        }

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            DeselectAll();
        }

        private Coordinate _pointDragStart;

        public void StartPointDrag(MapViewport viewport, ViewportEvent e, Coordinate startLocation)
        {
            foreach (var p in _points.Where(x => x.IsSelected))
            {
                p.IsDragging = true;
            }
            _pointDragStart = viewport.Flatten(startLocation);

            _tool.Invalidate();
        }

        public void PointDrag(MapViewport viewport, ViewportEvent viewportEvent, Coordinate lastPosition, Coordinate position)
        {
            var delta = viewport.Expand(position - _pointDragStart);

            var selected = _points.Where(x => x.IsSelected).Distinct().SelectMany(x => x.GetStandardPointList()).ToList();
            selected.ForEach(x => x.DragMove(delta));

            foreach (var midpoint in selected.Select(x => x.Solid).Distinct().SelectMany(x => x.Points.Where(p => p.IsMidpoint)))
            {
                var p1 = midpoint.MidpointStart.IsDragging ? midpoint.MidpointStart.DraggingPosition : midpoint.MidpointStart.Position;
                var p2 = midpoint.MidpointEnd.IsDragging ? midpoint.MidpointEnd.DraggingPosition : midpoint.MidpointEnd.Position;
                midpoint.DraggingPosition = midpoint.Position = (p1 + p2) / 2;
            }

            _tool.Invalidate();
        }

        public void EndPointDrag(MapViewport viewport, ViewportEvent e, Coordinate endLocation)
        {
            var delta = viewport.Expand(endLocation - _pointDragStart);

            var selected = _points.Where(x => x.IsSelected).ToList();
            selected.ForEach(x => x.IsDragging = false);

            var act = new MovePoints(_tool, this, selected, delta);
            _tool.PerformAction(act);
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return false;
        }

        public override void Highlight(MapViewport viewport)
        {
            
        }

        public override void Unhighlight(MapViewport viewport)
        {

        }

        public override IEnumerable<SceneObject> GetSceneObjects()
        {
            return _solids.SelectMany(x => x.Copy.Convert(_tool.Document));
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield break;
        }
    }
}
