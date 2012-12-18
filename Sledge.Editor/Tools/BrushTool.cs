using System.Drawing;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Editor.Editing;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Helpers;
using Sledge.UI;
using Sledge.Editor.Brushes;

namespace Sledge.Editor.Tools
{
    public class BrushTool : BaseBoxTool
    {
        private Box _lastBox;

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "Brush Tool";
        }

        protected override Color BoxColour
        {
            get { return Color.Turquoise; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(64, Color.Green); }
        }

        public override void ToolSelected()
        {
            var sel = Selection.GetSelectedObjects().OfType<Solid>();
            if (sel.Any())
            {
                _lastBox = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (_lastBox == null)
            {
                _lastBox = new Box(Coordinate.Zero, new Coordinate(Document.GridSpacing, Document.GridSpacing, Document.GridSpacing));
            }
        }

        protected override void LeftMouseDownToDraw(Viewport2D viewport, System.Windows.Forms.MouseEventArgs e)
        {
            base.LeftMouseDownToDraw(viewport, e);
            if (_lastBox == null) return;
            State.BoxStart += viewport.GetUnusedCoordinate(_lastBox.Start);
            State.BoxEnd += viewport.GetUnusedCoordinate(_lastBox.End);
        }

        private void CreateBrush(Box bounds)
        {
            var temp = new BlockBrush();
            var brush = temp.Create(bounds, TextureHelper.Get("AAATRIGGER"));
            foreach (var obj in brush)
            {
                obj.Parent = Document.Map.WorldSpawn;
                Document.Map.WorldSpawn.Children.Add(obj);
            }
            Document.UpdateDisplayLists();
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            var box = new Box(State.BoxStart, State.BoxEnd);
            if (box.Start.X != box.End.X && box.Start.Y != box.End.Y && box.Start.Z != box.End.Z)
            {
                CreateBrush(box);
                _lastBox = box;
            }
            base.BoxDrawnConfirm(viewport);
        }

        public override void BoxDrawnCancel(ViewportBase viewport)
        {
            _lastBox = new Box(State.BoxStart, State.BoxEnd);
            base.BoxDrawnCancel(viewport);
        }

        protected override void Render3D(Viewport3D viewport)
        {
            base.Render3D(viewport);
        }
    }
}
