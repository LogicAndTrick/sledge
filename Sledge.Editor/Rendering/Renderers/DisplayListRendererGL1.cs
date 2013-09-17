using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Renderers
{
    public class DisplayListRendererGL1 : IRenderer
    {
        public string Name { get { return "OpenGL 1.0 Renderer (Display List)"; } }
        public Document Document { get; set; }

        private Matrix4 _selectionTransform;

        private readonly int _listUnselectedWireframe2D;
        private readonly int _listSelectedWireframe2D;
        private readonly int _listUnselectedFilled;
        private readonly int _listSelectedWireframe3D;
        private readonly int _listSelectedFilled;
        private readonly int _listSelectedFilledHighlight;

        public DisplayListRendererGL1(Document document)
        {
            Document = document;
            _update = true;
            _selectionTransform = Matrix4.Identity;
            var idx = GL.GenLists(6);
            _listUnselectedWireframe2D = idx + 0;
            _listSelectedWireframe2D = idx + 1;
            _listUnselectedFilled = idx + 2;
            _listSelectedWireframe3D = idx + 3;
            _listSelectedFilled = idx + 4;
            _listSelectedFilledHighlight = idx + 5;
        }

        public void Dispose()
        {
            GL.DeleteLists(_listUnselectedWireframe2D, 6);
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

            GL.CallList(_listUnselectedWireframe2D);

            GL.LoadMatrix(ref current);
            GL.MultMatrix(ref modelView);
            GL.MultMatrix(ref _selectionTransform);

            GL.CallList(_listSelectedWireframe2D);

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

            GL.CallList(_listUnselectedFilled);
            GL.CallList(_listSelectedWireframe3D);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.MultMatrix(ref _selectionTransform);

            GL.CallList(_listSelectedFilled);
            GL.CallList(_listSelectedFilledHighlight);

            GL.LoadMatrix(ref current);
        }

        private bool _update;
        
        private void UpdateCache()
        {
            if (!_update) return;

            var all = GetAllVisible(Document.Map.WorldSpawn);
            var cache = CollectFaces(all);
            var unselected = cache.Where(x => !x.IsSelected && (x.Parent == null || !x.Parent.IsSelected) && x.Opacity > 0.1).ToList();
            var selected = cache.Where(x => (x.IsSelected || (x.Parent != null && x.Parent.IsSelected)) && x.Opacity > 0.1).ToList();

            GL.NewList(_listUnselectedWireframe2D, ListMode.Compile);
            DataStructures.Rendering.Rendering.DrawWireframe(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false);
            GL.EndList();

            GL.NewList(_listSelectedWireframe2D, ListMode.Compile);
            GL.Color4(Color.Red);
            DataStructures.Rendering.Rendering.DrawWireframe(selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
            GL.EndList();

            GL.NewList(_listUnselectedFilled, ListMode.Compile);
            DataStructures.Rendering.Rendering.DrawFilled(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty);
            GL.EndList();

            var sel3D = selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D).ToList();

            GL.NewList(_listSelectedWireframe3D, ListMode.Compile);
            GL.Color4(Color.Yellow);
            DataStructures.Rendering.Rendering.DrawWireframe(sel3D, true);
            GL.EndList();

            GL.NewList(_listSelectedFilled, ListMode.Compile);
            DataStructures.Rendering.Rendering.DrawFilled(sel3D, Color.Empty);
            GL.EndList();

            GL.NewList(_listSelectedFilledHighlight, ListMode.Compile);
            if (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection)
            {
                DataStructures.Rendering.Rendering.DrawFilled(sel3D, Color.FromArgb(64, Color.Red));
            }
            GL.EndList();

            _update = false;
        }

        private IEnumerable<MapObject> GetAllVisible(MapObject root)
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
            _update = true;
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _update = true;
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _update = true;
        }

        public void UpdateDocumentToggles()
        {
            _update = true;
        }
    }
}