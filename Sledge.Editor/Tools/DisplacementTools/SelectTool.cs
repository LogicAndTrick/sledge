using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Rendering;
using Sledge.Rendering;

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

        public override void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            return;

            var vp = viewport as MapViewport;
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

        public override void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(MapViewport viewport, Frame frame)
        {
            //
        }

        public void Render(MapViewport viewport)
        {
            //
        }
    }
}
