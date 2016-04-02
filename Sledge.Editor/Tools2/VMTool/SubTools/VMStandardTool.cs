using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.VMTool.Actions;
using Sledge.Editor.Tools2.VMTool.Controls;
using Sledge.Editor.UI;

namespace Sledge.Editor.Tools2.VMTool.SubTools
{
    public class VMStandardTool : VMSubTool
    {
        private StandardControl _control;

        public override Control Control { get { return _control; } }

        public VMStandardTool(VMTool tool) : base(tool)
        {
            var sc = new StandardControl();
            //sc.Merge += Merge;
            //sc.Split += Split;
            _control = sc;
        }

        /*
        private bool AutomaticallyMerge()
        {
            return _control.AutomaticallyMerge;
        }

        private void UpdateSplitEnabled()
        {
            _control.SplitEnabled = CanSplit();
        }

        private bool CanSplit()
        {
            return GetSplitFace() != null;
        }

        private Face GetSplitFace()
        {
            if (_tool.Points == null) return null;

            var selected = _tool.Points.Where(x => x.IsSelected).ToList();

            // Must have two points selected
            if (selected.Count != 2) return null;

            // Selection must share a face
            var commonFace = selected[0].GetAdjacentFaces().Intersect(selected[1].GetAdjacentFaces()).ToList();
            if (commonFace.Count != 1) return null;

            var face = commonFace[0];
            var s = selected[0].Position;
            var e = selected[1].Position;
            var edges = face.GetEdges();

            // The points cannot be adjacent
            return edges.Any(x => (x.Start == s && x.End == e) || (x.Start == e && x.End == s))
                       ? null
                       : face;
        }

        private void Merge(object sender)
        {
            //CheckMergedVertices();
        }

        private void Split(object sender)
        {
            var face = GetSplitFace();
            if (face == null) return;

            var solid = face.Parent;

            var sel = _tool.Points.Where(x => x.IsSelected).ToList();
            var p1 = sel[0];
            var p2 = sel[1];

            if (p1.IsMidpoint) AddAdjacentPoint(face, p1);
            if (p2.IsMidpoint) AddAdjacentPoint(face, p2);

            var polygon = new Polygon(face.Vertices.Select(x => x.Location));
            var clip = new Plane(p1.Position, p2.Position, p1.Position + face.Plane.Normal * 10);
            Polygon back, front;
            polygon.Split(clip, out back, out front);
            if (back == null || front == null) return;

            solid.Faces.Remove(face);
            face.Parent = null;

            CreateFace(back, solid, face);
            CreateFace(front, solid, face);

            solid.UpdateBoundingBox();

            //_tool.SetDirty(true, true);
        }

        private void VMSplitFace()
        {
            if (CanSplit())
            {
                Split(null);
            }
        }

        private void CreateFace(Polygon polygon, Solid parent, Face original)
        {
            var verts = polygon.Vertices;
            var f = new Face(Document.Map.IDGenerator.GetNextFaceID())
            {
                Parent = parent,
                Plane = new Plane(verts[0], verts[1], verts[2]),
                Colour = parent.Colour,
                Texture = original.Texture.Clone()
            };
            f.Vertices.AddRange(verts.Select(x => new Vertex(x, f)));
            f.UpdateBoundingBox();
            f.CalculateTextureCoordinates(true);
            parent.Faces.Add(f);
        }

        private void AddAdjacentPoint(Face face, VMPoint point)
        {
            var solid = face.Parent;
            var s = point.MidpointStart.Position;
            var e = point.MidpointEnd.Position;

            foreach (var f in solid.Faces.Where(x => x != face))
            {
                foreach (var edge in f.GetEdges())
                {
                    if (edge.Start == s && edge.End == e)
                    {
                        var idx = f.Vertices.FindIndex(x => x.Location == e);
                        f.Vertices.Insert(idx, new Vertex(point.Position, f));
                        return;
                    }
                    if (edge.Start == e && edge.End == s)
                    {
                        var idx = f.Vertices.FindIndex(x => x.Location == s);
                        f.Vertices.Insert(idx, new Vertex(point.Position, f));
                        return;
                    }
                }
            }
        }
        */

        public override IEnumerable<IDraggable> GetDraggables()
        {
            return _tool.GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0);
        }

        public override bool CanDragPoint(VMPoint point)
        {
            return true;
        }
        
        #region Point dragging - 2D

        private Coordinate _pointDragStart;
        private Coordinate _pointDragGridOffset;

        public override void StartPointDrag(MapViewport viewport, ViewportEvent e, Coordinate startLocation)
        {
            foreach (var p in GetVisiblePoints().Where(x => x.IsSelected))
            {
                p.IsDragging = true;
            }
            _pointDragStart = viewport.ZeroUnusedCoordinate(startLocation);
            _pointDragGridOffset = SnapIfNeeded(viewport.ZeroUnusedCoordinate(startLocation)) - viewport.ZeroUnusedCoordinate(startLocation);

            Invalidate();
        }

        public override void PointDrag(MapViewport viewport, ViewportEvent viewportEvent, Coordinate lastPosition, Coordinate position)
        {
            var delta = viewport.ZeroUnusedCoordinate(position) - _pointDragStart;
            if (KeyboardState.Shift && !KeyboardState.Alt) delta -= _pointDragGridOffset;

            var selected = GetVisiblePoints().Where(x => x.IsSelected).Distinct().SelectMany(x => x.GetStandardPointList()).ToList();
            selected.ForEach(x => x.DragMove(delta));

            foreach (var face in selected.SelectMany(x => x.Vertices.Select(v => v.Parent)).Distinct())
            {
                face.CalculateTextureCoordinates(true);
            }

            foreach (var midpoint in selected.Select(x => x.Solid).Distinct().SelectMany(x => x.Points.Where(p => p.IsMidpoint)))
            {
                var p1 = midpoint.MidpointStart.IsDragging ? midpoint.MidpointStart.DraggingPosition : midpoint.MidpointStart.Position;
                var p2 = midpoint.MidpointEnd.IsDragging ? midpoint.MidpointEnd.DraggingPosition : midpoint.MidpointEnd.Position;
                midpoint.DraggingPosition = midpoint.Position = (p1 + p2) / 2;
            }

            Invalidate();
        }

        public override void EndPointDrag(MapViewport viewport, ViewportEvent e, Coordinate endLocation)
        {
            var delta = viewport.ZeroUnusedCoordinate(endLocation) - _pointDragStart;
            if (KeyboardState.Shift && !KeyboardState.Alt) delta -= _pointDragGridOffset;

            var selected = GetVisiblePoints().Where(x => x.IsSelected).ToList();
            selected.ForEach(x => x.IsDragging = false);

            var act = new MovePoints(_tool, selected, delta);
            PerformAction(act);
        }

        #endregion

        public override string GetName()
        {
            return "Standard";
        }

        public override string GetContextualHelp()
        {
            return
@"*Click* a vertex to select all points under the cursor.
 - Hold *control* to select multiple points.
 - Hold *shift* to only select the topmost point.
Drag vertices to move them around.

Select two (non-adjacent) points on a face to enable splitting.";
        }
    }
}
