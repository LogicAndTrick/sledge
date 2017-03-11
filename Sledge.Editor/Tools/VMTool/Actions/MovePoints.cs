using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools.VMTool.Actions
{
    /*
    TODO: Try to get VM history working

    VM History attempts, a failed experiment so far
    The MovePoints action worked well for basic movement,
    but couldn't cater for merging.

    The ReplaceSolids action was the next attempt, but
    the current state of the VMPoint and VMSolid classes
    means that dragging a point will update its solid in
    real-time. This is fine, but makes it difficult to keep
    history around.

    One option would be to change the VMPoint/VMSolid 
    classes so that the rendered solid is different to the
    manipulated solid. This would require a third copy
    of each solid in memory which is a bit of a pain.

    A second option could be to stop relying on the solid
    in the first place and have the VMSolid be its own entity
    with its own Face/Vertex classes and render converters.
    This seems pretty heavy handed though.

    Moving points is an easy one, but VM can also do:
     - Merging two points into one
     - Merging all points in a face into one, eliminating a face entirely
     - Split a face, turning one into two and adding vertices
     - Bevel and poke, which can add many faces while removing one (or more)
    */
    public class MovePoints : VMAction
    {
        private class PointState
        {
            public long ObjectID { get; set; }
            public int PointID { get; set; }
            public Coordinate OriginalPosition { get; set; }

            public PointState(VMPoint point)
            {
                ObjectID = point.Solid.Original.ID;
                PointID = point.ID;
                OriginalPosition = point.Position;
            }
        }

        public override string Name { get { return "Move Points"; } }
        public override bool SkipInStack { get { return false; } }
        public override bool ModifiesState { get { return true; } }

        private readonly List<PointState> _points;
        private readonly Coordinate _delta;
        private readonly List<long> _cleanSolids;

        public MovePoints(VMTool tool, IEnumerable<VMPoint> points, Coordinate delta) : base(tool)
        {
            var list = points.SelectMany(x => x.GetStandardPointList()).Distinct().ToList();
            _points = list.Select(x => new PointState(x)).ToList();
            _delta = delta;
            _cleanSolids = list.Select(x => x.Solid).Where(x => !x.IsDirty).Select(x => x.Original.ID).Distinct().ToList();
        }

        private IEnumerable<VMPoint> GetPoints(VMTool tool, IEnumerable<PointState> list)
        {
            return list.Select(x => tool.GetPointByID(x.ObjectID, x.PointID));
        }

        protected override void Reverse(VMTool tool)
        {
            var pts = GetPoints(tool, _points).ToList();
            foreach (var p in pts)
            {
                p.Move(-_delta);
                if (_cleanSolids.Contains(p.Solid.Original.ID)) p.Solid.IsDirty = false;
            }

            var solids = pts.Select(x => x.Solid).Distinct().ToList();
            tool.RefreshPoints(solids);
            tool.Invalidate();
        }

        protected override void Perform(VMTool tool)
        {
            var pts = GetPoints(tool, _points).ToList();
            foreach (var p in pts)
            {
                p.Move(_delta);
                p.Solid.IsDirty = true;
            }

            var solids = pts.Select(x => x.Solid).Distinct().ToList();
            tool.RefreshPoints(solids);
            tool.Invalidate();
        }

        public override void Dispose()
        {
            _points.Clear();
        }
    }

    public class ReplaceSolids : VMAction
    {
        private class ReplacedSolid
        {
            public Solid Original { get; set; }
            public Solid Old { get; set; }
            public List<Coordinate> OldSelectedPoints { get; set; }
            public bool WasDirty { get; set; }
            public Action<VMSolid> Action { get; set; }
        }

        private string _name;
        public override string Name { get { return _name; } }

        public override bool SkipInStack { get { return false; } }
        public override bool ModifiesState { get { return false; } }

        private List<ReplacedSolid> _replacedSolids;

        public ReplaceSolids(VMTool tool, string name) : base(tool)
        {
            _name = name;
            _replacedSolids = new List<ReplacedSolid>();
        }

        public void AddSolid(VMSolid solid, Action<VMSolid> action)
        {
            var rs = new ReplacedSolid
            {
                Original = solid.Original,
                Old = (Solid) solid.Copy.Clone(),
                OldSelectedPoints = solid.Points.Where(x => x.IsSelected).Select(x => x.Position).ToList(),
                WasDirty = solid.IsDirty,
                Action = action
            };
            _replacedSolids.Add(rs);
        }

        protected override void Reverse(VMTool tool)
        {
            var solids = new List<VMSolid>();
            foreach (var replacedSolid in _replacedSolids)
            {
                var solid = tool.GetVmSolid(replacedSolid.Original);
                solids.Add(solid);
                solid.Copy.Unclone(replacedSolid.Old);
                solid.IsDirty = replacedSolid.WasDirty;
            }
            tool.RefreshPoints(solids);
            tool.Invalidate();
        }

        protected override void Perform(VMTool tool)
        {
            var solids = new List<VMSolid>();
            foreach (var replacedSolid in _replacedSolids)
            {
                var solid = tool.GetVmSolid(replacedSolid.Original);
                solids.Add(solid);
                replacedSolid.Action(solid);
                solid.IsDirty = true;
            }
            tool.RefreshPoints(solids);
            tool.Invalidate();
        }

        public override void Dispose()
        {
            _replacedSolids.Clear();
        }
    }
}
