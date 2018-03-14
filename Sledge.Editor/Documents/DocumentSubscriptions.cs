using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.QuickForms;
using Sledge.QuickForms.Items;
using Sledge.Rendering.Cameras;
using Sledge.Settings;

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

        public void SnapSelectionToGrid()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();

            var box = _document.Selection.GetSelectionBoundingBox();
            var transform = GetSnapTransform(box);

            _document.PerformAction("Snap to grid", new Edit(selected, new TransformEditOperation(transform, _document.Map.GetTransformFlags())));
        }

        public void SnapSelectionToGridIndividually()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();

            _document.PerformAction("Snap to grid individually", new Edit(selected, new SnapToGridEditOperation(_document.Map.GridSpacing, _document.Map.GetTransformFlags())));
        }

        private void AlignObjects(AlignObjectsEditOperation.AlignAxis axis, AlignObjectsEditOperation.AlignDirection direction)
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();
            var box = _document.Selection.GetSelectionBoundingBox();

            _document.PerformAction("Align Objects", new Edit(selected, new AlignObjectsEditOperation(box, axis, direction, _document.Map.GetTransformFlags())));
        }

        public void AlignXMax()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.X, AlignObjectsEditOperation.AlignDirection.Max);
        }

        public void AlignXMin()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.X, AlignObjectsEditOperation.AlignDirection.Min);
        }

        public void AlignYMax()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.Y, AlignObjectsEditOperation.AlignDirection.Max);
        }

        public void AlignYMin()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.Y, AlignObjectsEditOperation.AlignDirection.Min);
        }

        public void AlignZMax()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.Z, AlignObjectsEditOperation.AlignDirection.Max);
        }

        public void AlignZMin()
        {
            AlignObjects(AlignObjectsEditOperation.AlignAxis.Z, AlignObjectsEditOperation.AlignDirection.Min);
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

        public void GoToCoordinates()
        {
            using (var qf = new QuickForm("Enter Coordinates") { LabelWidth = 50, UseShortcutKeys = true }
                .TextBox("X", "0")
                .TextBox("Y", "0")
                .TextBox("Z", "0")
                .OkCancel())
            {
                qf.ClientSize = new Size(180, qf.ClientSize.Height);
                if (qf.ShowDialog() != DialogResult.OK) return;

                decimal x, y, z;
                if (!Decimal.TryParse(qf.String("X"), out x)) return;
                if (!Decimal.TryParse(qf.String("Y"), out y)) return;
                if (!Decimal.TryParse(qf.String("Z"), out z)) return;

                var coordinate = new Coordinate(x, y, z);

                ViewportManager.Viewports.ForEach(vp => vp.FocusOn(coordinate));
            }
        }

        public void GoToBrushID()
        {
            using (var qf = new QuickForm("Enter Brush ID") { LabelWidth = 100, UseShortcutKeys = true }
                .TextBox("Brush ID")
                .OkCancel())
            {
                qf.ClientSize = new Size(230, qf.ClientSize.Height);

                if (qf.ShowDialog() != DialogResult.OK) return;

                long id;
                if (!long.TryParse(qf.String("Brush ID"), out id)) return;

                var obj = _document.Map.WorldSpawn.FindByID(id);
                if (obj == null) return;

                // Select and go to the brush
                _document.PerformAction("Select brush ID " + id, new ChangeSelection(new[] { obj }, _document.Selection.GetSelectedObjects()));
                ViewportManager.Viewports.ForEach(x => x.FocusOn(obj.BoundingBox));
            }
        }

        public void ShowSelectedBrushID()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selectedIds = _document.Selection.GetSelectedObjects().Select(x => x.ID);
            var idString = String.Join(", ", selectedIds);

            MessageBox.Show("Selected Object IDs: " + idString);
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