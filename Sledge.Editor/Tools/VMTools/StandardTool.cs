using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.UI;

namespace Sledge.Editor.Tools.VMTools
{
    public class StandardTool : VMSubTool
    {
        private enum VMState
        {
            None,
            Down,
            Moving
        }

        private VMState _state;

        public StandardTool(VMTool mainTool) : base(mainTool)
        {
            Control = new StandardControl();
        }

        public override string GetName()
        {
            return "Standard Mode";
        }

        public override void ToolSelected()
        {
            _state = VMState.None;
        }

        public override void ToolDeselected()
        {
            _state = VMState.None;
        }

        public override List<VMPoint> GetVerticesAtPoint(int x, int y, Viewport2D viewport)
        {
            return MainTool.GetVerticesAtPoint(x, y, viewport);
        }

        public override void DragStart(List<VMPoint> clickedPoints)
        {
            _state = VMState.Down;
            Editor.Instance.CaptureAltPresses = true;
        }

        public override void DragMove(Coordinate distance)
        {
            // Move each selected point by the delta value
            foreach (var p in MainTool.Points.Where(x => !x.IsMidPoint && x.IsSelected))
            {
                p.Move(distance);
            }
            MainTool.Dirty = true;
        }

        public override void DragEnd()
        {
            if (_state == VMState.Moving)
            {
                CheckMergedVertices();
            }
            _state = VMState.None;
            Editor.Instance.CaptureAltPresses = false;
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {

        }

        private void CheckMergedVertices()
        {
            // adjacent points with the same solid and coordinate need to be merged (erp)
            foreach (var group in MainTool.Points.Where(x => !x.IsMidPoint).GroupBy(x => new { x.Solid, x.Coordinate }).Where(x => x.Count() > 1))
            {
                var allFaces = group.SelectMany(x => x.Vertices).Select(x => x.Parent).Distinct().ToList();
                foreach (var face in allFaces)
                {
                    var distinctVerts = face.Vertices.GroupBy(x => x.Location).Select(x => x.First()).ToList();
                    if (distinctVerts.Count < 3) group.Key.Solid.Faces.Remove(face); // Remove face
                    else face.Vertices.RemoveAll(x => !distinctVerts.Contains(x)); // Remove duped verts
                }
                // ... this is hard :(
            }
            MainTool.RefreshPoints();
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            var nudge = GetNudgeValue(e.KeyCode);
            var vp = viewport as Viewport2D;
            if (nudge != null && vp != null && _state == VMState.None && MainTool.Points.Any(x => x.IsSelected))
            {
                var translate = vp.Expand(nudge);
                foreach (var p in MainTool.Points.Where(x => !x.IsMidPoint && x.IsSelected))
                {
                    p.Move(translate);
                }
                MainTool.RefreshMidpoints(false);
                MainTool.Dirty = true;
            }
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void UpdateFrame(ViewportBase viewport)
        {

        }

        public override void Render(ViewportBase viewport)
        {

        }

        public override void Render2D(Viewport2D viewport)
        {

        }

        public override void Render3D(Viewport3D viewport)
        {

        }

        public override void SelectionChanged()
        {
            
        }
    }
}
