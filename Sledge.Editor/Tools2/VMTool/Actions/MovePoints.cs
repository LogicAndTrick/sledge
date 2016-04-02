using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Actions;
using Sledge.Editor.Documents;
using Sledge.Editor.History;

namespace Sledge.Editor.Tools2.VMTool.Actions
{

    public abstract class VMAction : IHistoryItem
    {
        private readonly VMTool _tool;

        public abstract string Name { get; }
        public abstract bool SkipInStack { get; }
        public abstract bool ModifiesState { get; }

        protected VMAction(VMTool tool)
        {
            _tool = tool;
        }

        public void Undo(Document document)
        {
            Reverse(_tool);
        }

        public void Redo(Document document)
        {
            Perform(_tool);
        }

        protected abstract void Reverse(VMTool tool);
        protected abstract void Perform(VMTool tool);
        public abstract void Dispose();
    }

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

        public override string Name { get { return "VM: Move Points"; } }
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

            foreach (var face in pts.SelectMany(x => x.Vertices.Select(v => v.Parent)).Distinct())
            {
                face.CalculateTextureCoordinates(true);
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

            foreach (var face in pts.SelectMany(x => x.Vertices.Select(v => v.Parent)).Distinct())
            {
                face.CalculateTextureCoordinates(true);
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
}
