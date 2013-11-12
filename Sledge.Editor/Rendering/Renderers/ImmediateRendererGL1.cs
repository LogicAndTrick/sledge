using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Documents;
using Sledge.Graphics.Renderables;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ImmediateRendererGL1 : IRenderer
    {
        public string Name { get { return "OpenGL 1.0 Renderer (Immediate Mode)"; } }
        public Document Document { get; set; }

        private Matrix4 _selectionTransform;

        public ImmediateRendererGL1(Document document)
        {
            Document = document;
            _cache = null;
            _selectionTransform = Matrix4.Identity;
        }

        public void Dispose()
        {
            _cache = null;
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D)
        {
            //
        }

        public void SetSelectionTransform(Matrix4 selTransform)
        {
            _selectionTransform = selTransform;
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            UpdateCache();

            RenderGrid(((Viewport2D)context).Zoom);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.MultMatrix(ref modelView);

            DataStructures.Rendering.Rendering.DrawWireframe(_unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false);
            GL.Color4(Color.Red);

            GL.LoadMatrix(ref current);
            GL.MultMatrix(ref modelView);
            GL.MultMatrix(ref _selectionTransform);
            DataStructures.Rendering.Rendering.DrawWireframe(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);

            GL.LoadMatrix(ref current);
        }

        private void RenderGrid(decimal zoom)
        {
            var lower = Document.GameData.MapSizeLow;
            var upper = Document.GameData.MapSizeHigh;
            var step = Document.Map.GridSpacing;
            var actualDist = step * zoom;
            if (Grid.HideSmallerOn)
            {
                while (actualDist < Grid.HideSmallerThan)
                {
                    step *= Grid.HideFactor;
                    actualDist *= Grid.HideFactor;
                }
            }
            GL.Begin(BeginMode.Lines);
            for (decimal i = lower; i <= upper; i += step)
            {
                var c = Grid.GridLines;
                if (i == 0) c = Grid.ZeroLines;
                else if (i % Grid.Highlight2UnitNum == 0 && Grid.Highlight2On) c = Grid.Highlight2;
                else if (i % (step * Grid.Highlight1LineNum) == 0 && Grid.Highlight1On) c = Grid.Highlight1;
                var ifloat = (float)i;
                GL.Color3(c);
                GL.Vertex2(lower, ifloat);
                GL.Vertex2(upper, ifloat);
                GL.Vertex2(ifloat, lower);
                GL.Vertex2(ifloat, upper);
            }
            GL.Color3(Grid.BoundaryLines);
            // Top
            GL.Vertex2(lower, upper);
            GL.Vertex2(upper, upper);
            // Left
            GL.Vertex2(lower, lower);
            GL.Vertex2(lower, upper);
            // Right
            GL.Vertex2(upper, lower);
            GL.Vertex2(upper, upper);
            // Bottom
            GL.Vertex2(lower, lower);
            GL.Vertex2(upper, lower);
            GL.End();
        }

        public void Draw3D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            UpdateCache();
            DataStructures.Rendering.Rendering.DrawFilled(_unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty);
            GL.Color4(Color.Yellow);
            DataStructures.Rendering.Rendering.DrawWireframe(_selected, true);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.MultMatrix(ref _selectionTransform);

            DataStructures.Rendering.Rendering.DrawFilled(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty);
            if (!Document.Selection.InFaceSelection || !Document.Map.HideFaceMask)
            {
                DataStructures.Rendering.Rendering.DrawFilled(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.FromArgb(64, Color.Red));
            }

            GL.LoadMatrix(ref current);
        }

        private List<Face> _cache;
        private List<Face> _unselected;
        private List<Face> _selected;
        
        private void UpdateCache()
        {
            if (_cache != null) return;
            var all = GetAllVisible(Document.Map.WorldSpawn);
            _cache = CollectFaces(all);
            _unselected = _cache.Where(x => !x.IsSelected && (x.Parent == null || !x.Parent.IsSelected) && x.Opacity > 0.1).ToList();
            _selected = _cache.Where(x => (x.IsSelected || (x.Parent != null && x.Parent.IsSelected)) && x.Opacity > 0.1).ToList();
        }

        private IList<MapObject> GetAllVisible(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).ToList();
        }

        private void FindRecursive(ICollection<MapObject> items, MapObject root, Predicate<MapObject> matcher)
        {
            if (!matcher(root)) return;
            items.Add(root);
            root.Children.ForEach(x => FindRecursive(items, x, matcher));
        }

        private List<Face> CollectFaces(IEnumerable<MapObject> all)
        {
            var list = new List<Face>();
            foreach (var mo in all)
            {
                var solid = mo as Solid;
                var entity = mo as Entity;

                if (solid != null)
                {
                    list.AddRange(solid.Faces);
                }
                if (entity != null)
                {
                    list.AddRange(entity.GetBoxFaces());
                    list.AddRange(entity.GetTexturedFaces());
                }
            }
            return list;
        }

        public void Update()
        {
            _cache = null;
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _cache = null;
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _cache = null;
        }

        public IRenderable CreateRenderable(Model model)
        {
            throw new NotImplementedException();
        }

        public void UpdateDocumentToggles()
        {
            // Not needed
        }
    }
}