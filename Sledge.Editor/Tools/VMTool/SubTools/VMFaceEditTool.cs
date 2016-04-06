using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.VMTool.Controls;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools.VMTool.SubTools
{
    public class VMFaceEditTool : VMSubTool
    {
        public override Control Control { get { return _control; } }

        private EditFaceControl _control;

        public VMFaceEditTool(VMTool tool) : base(tool)
        {
            _control = new EditFaceControl();
            _control.Poke += Poke;
            _control.Bevel += Bevel;
        }
        
        private void Poke(object sender, int num)
        {
            foreach (var face in GetSelectedFaces())
            {
                PokeFace(face, num);
            }
        }

        private void Bevel(object sender, int num)
        {
            foreach (var face in GetSelectedFaces())
            {
                BevelFace(face, num);
            }
        }

        private IList<Face> GetSelectedFaces()
        {
            return _tool.GetSolids().SelectMany(x => x.Copy.Faces).Where(x => x.IsSelected).ToList();
        }

        private void ClearSelection()
        {
            foreach (var face in _tool.GetSolids().SelectMany(x => x.Copy.Faces))
            {
                face.IsSelected = false;
            }
        }

        #region Edit faces

        private void PokeFace(Face face, int num)
        {
            var solid = face.Parent;
            // Remove the face
            solid.Faces.Remove(face);
            face.Parent = null;
            face.IsSelected = false;

            var center = face.BoundingBox.Center + face.Plane.Normal * num;
            foreach (var edge in face.GetEdges())
            {
                var v1 = face.Vertices.First(x => x.Location == edge.Start);
                var v2 = face.Vertices.First(x => x.Location == edge.End);
                var verts = new[] { v1.Location, v2.Location, center };
                var f = new Face(Document.Map.IDGenerator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(verts[0], verts[1], verts[2]),
                    Colour = solid.Colour,
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => new Vertex(x, face)));
                f.UpdateBoundingBox();
                f.AlignTextureToFace();
                solid.Faces.Add(f);
                f.IsSelected = true;
            }
            solid.UpdateBoundingBox();

            _tool.UpdateSolids(_tool.GetSolids().ToList(), true);
        }

        private void BevelFace(Face face, int num)
        {
            var solid = face.Parent;
            var vertexCoordinates = face.Vertices.ToDictionary(x => x, x => x.Location);
            // Scale the face a bit and move it away by the bevel distance
            face.Transform(new UnitScale(Coordinate.One * 0.9m, face.BoundingBox.Center), TransformFlags.TextureLock);
            face.Transform(new UnitTranslate(face.Plane.Normal * num), TransformFlags.TextureLock);
            foreach (var edge in face.GetEdges())
            {
                var v1 = face.Vertices.First(x => x.Location == edge.Start);
                var v2 = face.Vertices.First(x => x.Location == edge.End);
                var verts = new[] { vertexCoordinates[v1], vertexCoordinates[v2], v2.Location, v1.Location };
                var f = new Face(Document.Map.IDGenerator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(verts[0], verts[1], verts[2]),
                    Colour = solid.Colour,
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => new Vertex(x, face)));
                f.UpdateBoundingBox();
                f.AlignTextureToFace();
                solid.Faces.Add(f);
                f.IsSelected = true;
            }
            solid.UpdateBoundingBox();

            _tool.UpdateSolids(_tool.GetSolids().ToList(), true);
        }

        #endregion

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;

            // Do selection
            e.Handled = true;
            var ray = viewport.CastRayFromScreen(e.X, e.Y);
            var hits = _tool.GetSolids().Select(x => x.Copy).Where(x => x.BoundingBox.IntersectsWith(ray));
            var clickedFace = hits.SelectMany(f => f.Faces)
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            var faces = new List<Face>();
            if (clickedFace != null)
            {
                if (KeyboardState.Shift) faces.AddRange(clickedFace.Parent.Faces);
                else faces.Add(clickedFace);
            }
            
            if (!KeyboardState.Ctrl) ClearSelection();
            faces.ForEach(x => x.IsSelected = true);

            _tool.Invalidate();
        }

        public override string GetName()
        {
            return "Edit Face";
        }

        public override string GetContextualHelp()
        {
            return "*Click* a face in the 3D view to select it.\n" +
                   "Hold *control* to select multiple faces.";
        }

        public override void ToolSelected(bool preventHistory)
        {
            ClearSelection();
        }

        public override void ToolDeselected(bool preventHistory)
        {
            ClearSelection();
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            yield break;
        }

        public override bool CanDragPoint(VMPoint point)
        {
            return false;
        }
    }
}
