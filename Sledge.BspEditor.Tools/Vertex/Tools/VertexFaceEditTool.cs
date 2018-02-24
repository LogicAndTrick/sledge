using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Vertex.Controls;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Shell.Input;
using Face = Sledge.BspEditor.Primitives.MapObjectData.Face;
using SceneFace = Sledge.Rendering.Scenes.Renderables.Face;
using SceneVertex = Sledge.Rendering.Scenes.Renderables.Vertex;
using Line = Sledge.DataStructures.Geometric.Line;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    [AutoTranslate]
    [Export(typeof(VertexSubtool))]
    public class VertexFaceEditTool : VertexSubtool
    {
        [Import] private VertexEditFaceControl _control;

        public override string OrderHint => "F";
        public override string GetName() => "Face editing";
        public override Control Control => _control;
        
        private readonly List<SolidFace> _selectedFaces;
        
        public VertexFaceEditTool()
        {
            _selectedFaces = new List<SolidFace>();
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<string>("VertexTool:DeselectAll", _ => ClearSelection());
            yield return Oy.Subscribe<int>("VertexEditFaceTool:Poke", v => Poke(v));
            yield return Oy.Subscribe<int>("VertexEditFaceTool:Bevel", v => Bevel(v));
        }
        
        private void Poke(int num)
        {
            foreach (var solidFace in _selectedFaces)
            {
                PokeFace(solidFace, num);
            }
            UpdateSolids(_selectedFaces.Select(x => x.Solid).ToList());
        }

        private void Bevel(int num)
        {
            foreach (var solidFace in _selectedFaces)
            {
                BevelFace(solidFace, num);
            }
            UpdateSolids(_selectedFaces.Select(x => x.Solid).ToList());
        }

        private IList<Face> GetSelectedFaces()
        {
            return _selectedFaces.Select(x => x.Face).ToList();
        }

        private void ClearSelection()
        {
            _selectedFaces.Clear();
            Invalidate();
        }

        #region Edit faces

        private void PokeFace(SolidFace solidFace, int num)
        {
            var face = solidFace.Face;
            var solid = solidFace.Solid.Copy;

            // Remove the face
            solid.Data.Remove(face);

            var center = face.Origin + face.Plane.Normal * num;
            foreach (var edge in face.GetEdges())
            {
                var v1 = face.Vertices.First(x => x == edge.Start);
                var v2 = face.Vertices.First(x => x == edge.End);
                var verts = new[] { v1, v2, center };
                var f = new Face(Document.Map.NumberGenerator.Next("Face"))
                {
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => x.Clone()));
                solid.Data.Add(f);
            }
            solid.DescendantsChanged();
        }

        private void BevelFace(SolidFace solidFace, int num)
        {
            var face = solidFace.Face;
            var solid = solidFace.Solid.Copy;

            // Remember the original positions
            var vertexCoordinates = face.Vertices.Select(x => x.Clone()).ToList();

            // Scale the face a bit and move it away by the bevel distance
            var origin = face.Origin;
            face.Transform(Matrix.Translation(origin) * Matrix.Scale(Coordinate.One * 0.9m) * Matrix.Translation(-origin));
            face.Transform(Matrix.Translation(face.Plane.Normal * num));

            // Create a face for each new edge -> old edge
            foreach (var edge in face.GetEdges())
            {
                var startIndex = face.Vertices.IndexOf(edge.Start);
                var endIndex = face.Vertices.IndexOf(edge.End);
                var verts = new[] { vertexCoordinates[startIndex], vertexCoordinates[endIndex], edge.End, edge.Start };
                var f = new Face(Document.Map.NumberGenerator.Next("Face"))
                {
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => x.Clone()));
                solid.Data.Add(f);
            }
            solid.DescendantsChanged();
        }

        #endregion
        
        #region 3D interaction
        
        private Coordinate GetIntersectionPoint(Face face, Line line)
        {
            return face.ToPolygon().GetIntersectionPoint(line);
        }
        
        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;

            e.Handled = true;

            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Selection.Where(x => x.Copy.BoundingBox.IntersectsWith(ray));

            // Sort the list of intersecting elements by distance from ray origin
            var clickedFace = hits
                .SelectMany(x => x.Copy.Faces.Select(f => new { Solid = x, Face = f}))
                .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x.Face, ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();
            

            var faces = new List<SolidFace>();
            if (clickedFace != null)
            {
                if (KeyboardState.Shift) faces.AddRange(clickedFace.Solid.Copy.Faces.Select(x => new SolidFace(clickedFace.Solid, x)));
                else faces.Add(new SolidFace(clickedFace.Solid, clickedFace.Face));
            }
            
            if (!KeyboardState.Ctrl) ClearSelection();
            _selectedFaces.AddRange(faces);

            Invalidate();
        }

        #endregion

        public override async Task SelectionChanged()
        {
            ClearSelection();
        }

        public override async Task ToolSelected()
        {
            ClearSelection();
            await base.ToolSelected();
        }

        public override async Task ToolDeselected()
        {
            ClearSelection();
            await base.ToolDeselected();
        }

        public override void Update()
        {
            ClearSelection();
        }

        private void UpdateSolids(List<VertexSolid> solids)
        {
            if (!solids.Any()) return;

            foreach (var solid in solids)
            {
                solid.IsDirty = true;
                solid.Copy.DescendantsChanged();
            }

            Invalidate();
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var mat = Material.Flat(Color.FromArgb(128, Color.White));

            var objects = new List<SceneObject>();
            foreach (var solidFace in _selectedFaces)
            {
                var verts = solidFace.Face.Vertices.Select(x => new SceneVertex(x.ToVector3(), 0, 0)).ToList();
                var sf = new SceneFace(mat, verts)
                {
                    TintColor = Color.OrangeRed
                };
                objects.Add(sf);
            }
            objects.AddRange(base.GetSceneObjects());
            return objects;
        }

        private class SolidFace
        {
            public VertexSolid Solid { get; set; }
            public Face Face { get; set; }

            public SolidFace(VertexSolid solid, Face face)
            {
                Solid = solid;
                Face = face;
            }
        }
    }
}