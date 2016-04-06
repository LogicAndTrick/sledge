using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.VMTool.Controls;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.VMTool.SubTools
{
    public class VMScaleTool : VMSubTool
    {
        private class ScaleOrigin : DraggableCoordinate
        {
            private readonly VMScaleTool _vmScaleTool;

            public ScaleOrigin(VMScaleTool vmScaleTool)
            {
                _vmScaleTool = vmScaleTool;
                Width = 10;
            }

            public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
            {
                Position = _vmScaleTool.SnapIfNeeded(viewport.Expand(position) + viewport.GetUnusedCoordinate(Position));
                OnDragMoved();
            }

            public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
            {
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 8)
                {
                    Color = Color.Transparent,
                    LineColor = Color.AliceBlue,
                    ZIndex = 10
                };
                yield return new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(Position.ToVector3()), 4)
                {
                    Color = Color.Transparent,
                    LineColor = Color.AliceBlue,
                    ZIndex = 10
                };
            }
        }

        public override Control Control { get { return _control;} }

        private ScaleControl _control;
        private ScaleOrigin _origin;
        private decimal _prevValue;
        private Dictionary<VMPoint, Coordinate> _originals;

        public VMScaleTool(VMTool tool) : base(tool)
        {
            _control = new ScaleControl();
            _control.ValueChanged += ValueChanged;
            _control.ValueReset += ValueReset;
            _control.ResetOrigin += ResetOrigin;
            _origin = new ScaleOrigin(this);
        }

        private void ValueChanged(object sender, decimal value)
        {
            MovePoints(value);
            _prevValue = value;
        }

        private void ValueReset(object sender, decimal value)
        {
            _prevValue = value;
            _originals = _tool.GetVisiblePoints().ToDictionary(x => x, x => x.Position);
        }

        private void ResetOrigin(object sender)
        {
            var points = _tool.GetVisiblePoints().Where(x => x.IsSelected).Select(x => x.Position).ToList();
            if (!points.Any()) points = _tool.GetVisiblePoints().Where(x => !x.IsMidpoint).Select(x => x.Position).ToList();
            if (!points.Any()) _origin.Position = Coordinate.Zero;
            else _origin.Position = points.Aggregate(Coordinate.Zero, (a, b) => a + b) / points.Count;
            _tool.Invalidate();
        }

        public override void SelectionChanged()
        {
            _control.ResetValue();
            if (_tool.GetVisiblePoints().Any(x => x.IsSelected)) ResetOrigin(null);
        }

        private void MovePoints(decimal value)
        {
            var o = _origin.Position;
            var solids = new List<VMSolid>();
            // Move each selected point by the computed offset from the origin
            foreach (var p in _tool.GetVisiblePoints().Where(x => x.IsSelected).SelectMany(x => x.GetStandardPointList()).Distinct())
            {
                if (!solids.Contains(p.Solid)) solids.Add(p.Solid);
                var orig = _originals[p];
                var diff = orig - o;
                var move = o + diff * value / 100;
                p.Move(move - p.Position);
            }
            _tool.UpdateSolids(solids, false);
        }

        public override void ToolSelected(bool preventHistory)
        {
            ValueReset(null, 0);
            ResetOrigin(null);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            _originals = null;
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            return _tool.GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0).OfType<IDraggable>().Union(new[] {_origin});
        }

        public override bool CanDragPoint(VMPoint point)
        {
            return false;
        }

        public override string GetName()
        {
            return "Scale";
        }

        public override string GetContextualHelp()
        {
            return
                @"*Click* a vertex to select all points under the cursor.
 - Hold *control* to select multiple points.
 - Hold *shift* to only select the topmost point.
Move the origin point around by *clicking and dragging* it.";
        }
    }
}