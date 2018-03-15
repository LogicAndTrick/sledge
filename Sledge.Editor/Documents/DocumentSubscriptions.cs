using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;

namespace Sledge.Editor.Documents
{
    /// <summary>
    /// A simple container to separate out the document mediator listeners from the document itself.
    /// </summary>
    public class DocumentSubscriptions : IMediatorListener
    {
        public void QuickHideSelected()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Selection.GetSelectedObjects();
            _document.PerformAction("Hide objects", new QuickHideObjects(objects));
        }

        public void QuickHideUnselected()
        {
            if (_document.Selection.InFaceSelection) return;

            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Map.WorldSpawn.FindAll().Except(_document.Selection.GetSelectedObjects()).Where(x => !(x is World) && !(x is Group));
            _document.PerformAction("Hide objects", new QuickHideObjects(objects));
        }

        public void QuickHideShowAll()
        {
            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Map.WorldSpawn.Find(x => x.IsInVisgroup(autohide.ID, true));
            _document.PerformAction("Show hidden objects", new QuickShowObjects(objects));
        }

        public void RotateClockwise()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;
            var focused = ViewportManager.GetActiveViewport();
            if (focused == null) return;

            var center = new Box(_document.Selection.GetSelectedObjects().Select(x => x.BoundingBox).Where(x => x != null)).Center;
            var axis = focused.GetUnusedCoordinate(Coordinate.One);
            var transform = new UnitRotate(DMath.DegreesToRadians(90), new Line(center, center + axis));
            var selected = _document.Selection.GetSelectedParents();
            _document.PerformAction("Transform selection", new Edit(selected, new TransformEditOperation(transform, _document.Map.GetTransformFlags())));
        }

        public void RotateCounterClockwise()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;
            var focused = ViewportManager.GetActiveViewport();
            if (focused == null) return;

            var center = new Box(_document.Selection.GetSelectedObjects().Select(x => x.BoundingBox).Where(x => x != null)).Center;
            var axis = focused.GetUnusedCoordinate(Coordinate.One);
            var transform = new UnitRotate(DMath.DegreesToRadians(-90), new Line(center, center + axis));
            var selected = _document.Selection.GetSelectedParents();
            _document.PerformAction("Transform selection", new Edit(selected, new TransformEditOperation(transform, _document.Map.GetTransformFlags())));
        }

        private void FlipObjects(Coordinate scale)
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();
            var box = _document.Selection.GetSelectionBoundingBox();

            var transform = new UnitScale(scale, box.Center);
            _document.PerformAction("Flip Objects", new Edit(selected, new TransformEditOperation(transform, _document.Map.GetTransformFlags())));
        }

        public void FlipX()
        {
            FlipObjects(new Coordinate(-1, 1, 1));
        }

        public void FlipY()
        {
            FlipObjects(new Coordinate(1, -1, 1));
        }

        public void FlipZ()
        {
            FlipObjects(new Coordinate(1, 1, -1));
        }

        public void ShowLogicalTree()
        {
            var mtw = new MapTreeWindow(_document);
            mtw.Show(Editor.Instance);
        }

        public void CheckForProblems()
        {
            using (var cfpd = new CheckForProblemsDialog(_document))
            {
                cfpd.ShowDialog(Editor.Instance);
            }
        }
    }
}