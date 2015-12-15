using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMSolid
    {
        private readonly VMPointsDraggableState _state;
        public Solid Copy { get; set; }
        public Solid Original { get; set; }
        public bool IsDirty { get; set; }
        public List<VMPoint> Points { get; set; }

        public VMSolid(VMPointsDraggableState state, Solid original)
        {
            _state = state;
            Original = original;

            Copy = (Solid) original.Clone();
            Copy.IsSelected = false;
            Copy.IsCodeHidden = false;
            foreach (var f in Copy.Faces)
            {
                f.IsSelected = true;
            }

            Points = new List<VMPoint>();
            RefreshPoints();
        }

        public void RefreshPoints()
        {
            var selected = Points.Where(x => x.IsSelected).ToList();
            Points.Clear();

            var verts = Copy.Faces.SelectMany(x => x.Vertices).ToList();

            // Add vertex points
            foreach (var group in verts.GroupBy(x => x.Location.Round(2)))
            {
                Points.Add(new VMPoint(_state, this)
                {
                    ID = verts.IndexOf(group.First()) + 1,
                    Position = group.First().Location,
                    Vertices = group.ToList(),
                    IsSelected = selected.Any(x => !x.IsMidpoint && x.Position == group.First().Location)
                });
            }

            // Add midpoints
            foreach (var group in Copy.Faces.SelectMany(x => x.GetLines()).GroupBy(x => new { x.Start, x.End }))
            {
                var s = group.Key.Start;
                var e = group.Key.End;
                var coord = (s + e) / 2;
                var mpStart = Points.First(x => !x.IsMidpoint && x.Position == s);
                var mpEnd = Points.First(x => !x.IsMidpoint && x.Position == e);
                Points.Add(new VMPoint(_state, this)
                {
                    Position = coord,
                    IsMidpoint = true,
                    MidpointStart = mpStart,
                    MidpointEnd = mpEnd,
                    IsSelected = selected.Any(x => x.IsMidpoint && x.MidpointStart.Position == mpStart.Position && x.MidpointEnd.Position == mpEnd.Position)
                });
            }
        }
    }
}