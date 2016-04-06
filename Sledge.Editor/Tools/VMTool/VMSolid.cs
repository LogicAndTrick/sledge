using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools.VMTool
{
    public class VMSolid
    {
        private readonly VMTool _tool;
        public Solid Copy { get; set; }
        public Solid Original { get; set; }
        public bool IsDirty { get; set; }
        public List<VMPoint> Points { get; set; }

        public VMSolid(VMTool tool, Solid original)
        {
            _tool = tool;
            Original = original;

            Copy = (Solid) original.Clone();
            Copy.IsSelected = false;
            Copy.IsCodeHidden = false;
            foreach (var f in Copy.Faces) f.IsSelected = false;

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
                Points.Add(new VMPoint(_tool, this)
                {
                    ID = verts.IndexOf(group.First()) + 1,
                    Position = group.First().Location.Round(2),
                    Vertices = group.ToList(),
                    IsSelected = selected.Any(x => !x.IsMidpoint && x.Position == group.First().Location.Round(2))
                });
            }

            // Add midpoints
            foreach (var group in Copy.Faces.SelectMany(x => x.GetLines()).GroupBy(x => new { Start = x.Start.Round(2), End = x.End.Round(2) }))
            {
                var s = group.Key.Start;
                var e = group.Key.End;
                var coord = (s + e) / 2;
                var mpStart = Points.First(x => !x.IsMidpoint && x.Position == s);
                var mpEnd = Points.First(x => !x.IsMidpoint && x.Position == e);
                Points.Add(new VMPoint(_tool, this)
                {
                    Position = coord,
                    IsMidpoint = true,
                    MidpointStart = mpStart,
                    MidpointEnd = mpEnd,
                    IsSelected = selected.Any(x => x.IsMidpoint && x.MidpointStart.Position == mpStart.Position && x.MidpointEnd.Position == mpEnd.Position)
                });
            }
        }

        public void UpdatePoints()
        {
            foreach (var point in Points)
            {
                if (point.IsMidpoint)
                {
                    point.Position = (point.MidpointStart.Position + point.MidpointEnd.Position) / 2;
                }
            }
        }

        public IEnumerable<VMError> GetErrors()
        {
            foreach (var g in Copy.GetCoplanarFaces().GroupBy(x => x.Plane))
            {
                yield return new VMError("Coplanar faces", Copy, g);
            }
            foreach (var f in Copy.GetBackwardsFaces(0.5m))
            {
                yield return new VMError("Backwards face", Copy, new[] { f });
            }
            foreach (var f in Copy.Faces)
            {
                var np = f.GetNonPlanarVertices(0.5m).ToList();
                var found = false;
                if (np.Any())
                {
                    yield return new VMError("Nonplanar vertex", Copy, new[] { f }, np);
                    found = true;
                }
                foreach (var g in f.Vertices.GroupBy(x => x.Location.Round(2)).Where(x => x.Count() > 1))
                {
                    yield return new VMError("Overlapping vertices", Copy, new[] { f }, g);
                    found = true;
                }
                if (!f.IsConvex() && !found)
                {
                    yield return new VMError("Concave face", Copy, new[] { f });
                }
            }
        }
    }
}