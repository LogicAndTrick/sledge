using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.MapObjects;
using Sledge.UI;

namespace Sledge.Editor.Tools.DisplacementTools
{
    public class SelectTool : DisplacementSubTool
    {
        public SelectTool()
        {
            Control = new SelectControl();
        }

        public override string GetName()
        {
            return "Select";
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            return;

            var vp = viewport as Viewport3D;
            if (vp == null || (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)) return;

            var ray = vp.CastRayFromScreen(e.X, e.Y);
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray).OfType<Solid>();
            var clickedFace = hits.SelectMany(f => f.Faces)
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();
            // TODO: Select
            //TextureItem itemToSelect = null;
            // if ((behaviour == SelectBehaviour.Select || behaviour == SelectBehaviour.LiftSelect) && !KeyboardState.Ctrl)
            // {
            //     Selection.Clear();
            // }
            //if (clickedFace != null)
            //{
            //    var faces = new List<Face>();
            //    if (KeyboardState.Shift) faces.AddRange(clickedFace.Parent.Faces);
            //    else faces.Add(clickedFace);
            //    if (behaviour == SelectBehaviour.Select || behaviour == SelectBehaviour.LiftSelect)
            //    {
            //        foreach (var face in faces)
            //        {
            //            if (face.IsSelected) Selection.Deselect(face);
            //            else Selection.Select(face);
            //        }
            //    }
            //    if (behaviour == SelectBehaviour.Lift || behaviour == SelectBehaviour.LiftSelect)
            //    {
            //        var tex = faces.Where(face => face.Texture.Texture != null).FirstOrDefault();
            //        itemToSelect = tex != null ? TexturePackage.GetItem(tex.Texture.Name) : null;
            //    }
            //}
            //_form.SelectionChanged();
            //if (itemToSelect != null)
            //{
            //    _form.SelectTexture(itemToSelect);
            //}
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
        {
            //
        }

        public override void Render(ViewportBase viewport)
        {
            //
        }
    }
}
