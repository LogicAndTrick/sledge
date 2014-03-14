using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Easings;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Tools.DisplacementTools
{
    public class GeometryTool : DisplacementSubTool
    {
        private DisplacementPoint _currentPoint;
        private bool _mouseDown;
        private decimal _multiplier;
        private bool _needsRedraw;
        private Point _mousePos;
        private int _moveCount;

        public GeometryTool()
        {
            var gc = new GeometryControl();
            gc.ResetAllPoints += ResetAllPoints;
            Control = gc;
            _mousePos = new Point(0, 0);
        }

        private void ResetAllPoints(object sender)
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            return "Geometry";
        }

        public override void ToolSelected(bool preventHistory)
        {
            //
        }

        public override void ToolDeselected(bool preventHistory)
        {
            _currentPoint = null;
        }

        private Coordinate GetNormal(Viewport3D vp, DisplacementPoint point, GeometryControl.Axis axis)
        {
            switch (axis)
            {
                case GeometryControl.Axis.XAxis:
                    return Coordinate.UnitX;
                case GeometryControl.Axis.YAxis:
                    return Coordinate.UnitY;
                case GeometryControl.Axis.ZAxis:
                    return Coordinate.UnitZ;
                case GeometryControl.Axis.FaceNormal:
                    return point.Parent.Plane.Normal;
                case GeometryControl.Axis.PointNormal:
                    return point.Displacement.Normal;
                case GeometryControl.Axis.TowardsViewport:
                    var v = vp.Camera.LookAt;
                    return new Coordinate((decimal) v.X, (decimal) v.Y, (decimal) v.Z).Normalise();
                default:
                    throw new ArgumentOutOfRangeException("axis");
            }
        }

        private void CollectJoinedDisplacements(List<Displacement> collection, Displacement startingPoint, List<Displacement> searchList)
        {
            if (collection.Contains(startingPoint)) return;
            collection.Add(startingPoint);
            searchList.Remove(startingPoint);
            var sewable = startingPoint.GetSewableDisplacements(searchList).ToList();
            sewable.ForEach(x => CollectJoinedDisplacements(collection, x, searchList));
        }

        private void ApplyEffect(decimal distance, Coordinate normal,
            DisplacementPoint point, GeometryControl.Effect effect, GeometryControl.Brush brush,
            decimal spatialRadius, int pointSize, Easing softEdgeFunc)
        {
            var disps = new List<Displacement>();
            //CollectJoinedDisplacements(disps, point.Parent, Selection.GetSelectedFaces().OfType<Displacement>().ToList());
            disps.AddRange(Document.Selection.GetSelectedFaces().OfType<Displacement>());
            var list = new List<Tuple<DisplacementPoint, decimal>>();

            if (brush == GeometryControl.Brush.Point)
            {
                // If we're in point brush mode, select the points by location in the displacement grid
                list.Add(Tuple.Create(_currentPoint, 1m));
                var ratio = 1m / pointSize;
                var cx = _currentPoint.XIndex;
                var cy = _currentPoint.YIndex;
                for (var i = 1; i < pointSize; i++)
                {
                    var edge = 1 - softEdgeFunc.Evaluate(ratio * i);// softEdge ? 1 - (ratio * i) : 1;

                    var points = new List<DisplacementPoint>();
                    for (var j = -i; j <= i; j++)
                    {
                        // Get the points in a box around the point, distance i
                       // points.Add(disp.GetPoint(cx - i, cy + j));
                       // points.Add(disp.GetPoint(cx + i, cy + j));
                       // if (j == -i || j == i) continue;
                       // points.Add(disp.GetPoint(cx + j, cy - i));
                       // points.Add(disp.GetPoint(cx + j, cy + i));
                    }

                    list.AddRange(points.Where(x => x != null).Select(x => Tuple.Create(x, edge)));
                }
            }
            else if (brush == GeometryControl.Brush.Spatial)
            {
                // For spatial brush mode, select the points by distance from the current point
                var points = disps.SelectMany(x => x.GetPoints())
                    .Select(x => new
                                     {
                                         Point = x,
                                         Distance = (x.Location - _currentPoint.Location).VectorMagnitude()
                                     })
                    .Where(x => x.Distance <= spatialRadius);
                // list.AddRange(points.Select(x => Tuple.Create(x.Point, softEdge ? (spatialRadius - x.Distance) / spatialRadius : 1)));
                list.AddRange(points.Select(x => Tuple.Create(x.Point, 1 - softEdgeFunc.Evaluate(1 - (spatialRadius - x.Distance) / spatialRadius))));
            }

            if (!list.Any()) return;

            switch (effect)
            {
                case GeometryControl.Effect.RelativeDistance:
                    list.ForEach(
                        x => x.Item1.CurrentPosition.Location +=
                             normal * (distance * x.Item2));
                    break;
                case GeometryControl.Effect.AbsoluteDistance:
                    list.ForEach(
                        x => x.Item1.CurrentPosition.Location =
                             x.Item1.InitialPosition + (x.Item1.Parent.Plane.Normal * distance));
                    break;
                case GeometryControl.Effect.SmoothPoints:
                    var avg = list.Select(x => x.Item1.Location).Aggregate(Coordinate.Zero, (x, y) => x + y) / list.Count;
                    foreach (var pt in list)
                    {
                        var dist = (pt.Item1.Location - avg).Dot(normal);
                        var mult = -Math.Sign(dist) * Math.Min(Math.Abs(dist), Math.Abs(distance));
                        pt.Item1.CurrentPosition.Location += mult * pt.Item2 * normal;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("effect");
            }
        }

        private void PaintCurrentPoint(Viewport3D vp, int applyCount)
        {
            if (_currentPoint == null) return;
            var c = (GeometryControl) Control;
            var normal = GetNormal(vp, _currentPoint, c.SelectedAxis);
            ApplyEffect(c.Distance * _multiplier * applyCount, normal, _currentPoint, c.SelectedEffect, c.SelectedBrush,
                        c.SpatialBrushRadius, c.PointBrushSize, c.SoftEdgeEasing);
            // TODO: c.AutoSew;
            _needsRedraw = true;
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            _mouseDown = false;
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            if (!(viewport is Viewport3D) || (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)) return;
            _multiplier = e.Button == MouseButtons.Left ? 1 : -1;
            //PaintCurrentPoint((Viewport3D) viewport);
            _mouseDown = true;
            _moveCount = 0;
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
            _moveCount = 0;
            _mouseDown = false;
            Document.Selection.GetSelectedFaces().OfType<Displacement>().ToList().ForEach(x => x.CalculateNormals());
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            _mousePos.X = e.X;
            _mousePos.Y = e.Y;

            if (_currentPoint == null) return;

            if (_mouseDown)
            {
                _moveCount++;
                //PaintCurrentPoint(vp);
            }
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
            var vp = viewport as Viewport3D;
            if (vp == null) return;

            if (_moveCount > 0)
            {
                PaintCurrentPoint(vp, _moveCount);
                _moveCount = 0;
            }

            // Check the current intersecting point
            var ray = vp.CastRayFromScreen(_mousePos.X, _mousePos.Y);
            var selectedFaces = Document.Selection.GetSelectedFaces().OfType<Displacement>();
            var closestPoints = selectedFaces.Select(x => x.GetClosestDisplacementPoint(ray));
            _currentPoint = closestPoints
                .OrderBy(x => (x.Location - ray.ClosestPoint(x.Location)).VectorMagnitude())
                .FirstOrDefault();

            if (_needsRedraw)
            {
                // update display lists?
                _needsRedraw = false;
            }
        }

        public override void Render(ViewportBase viewport)
        {
            if (_currentPoint != null)
            {
                var sub = new Coordinate(40, 40, 40);
                var pointBox = new Box(_currentPoint.CurrentPosition.Location - sub, _currentPoint.CurrentPosition.Location + sub);

                TextureHelper.Unbind();
                GL.LineWidth(3);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1f, 1, 0);
                foreach (var line in pointBox.GetBoxLines())
                {
                    GL.Vertex3(line.Start.DX, line.Start.DY, line.Start.DZ);
                    GL.Vertex3(line.End.DX, line.End.DY, line.End.DZ);
                }
                GL.End();
                GL.LineWidth(1);
            }
        }
    }
}
