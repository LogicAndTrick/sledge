using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Extensions;
using Sledge.Graphics.Renderables;
using Sledge.Settings;
using Sledge.UI;
using Quaternion = Sledge.DataStructures.Geometric.Quaternion;

namespace Sledge.Editor.Rendering.Renderers
{
    public class DisplayListRenderer : IRenderer
    {
        public string Name { get { return "OpenGL 1.0 Renderer (Display List)"; } }
        public Document Document { get; set; }

        private Matrix _selectionTransformMat;
        private Matrix4 _selectionTransform;

        private readonly int _listUntransformed2D;
        private readonly int _listTransformed2D;
        private readonly int _listUntransformed3D;
        private readonly int _listTransformed3D;
        private readonly int _listUntransformedDecals3D;

        private readonly Dictionary<Model, int> _modelLists;
        private readonly List<Tuple<Entity, Model>> _models;

        public DisplayListRenderer(Document document)
        {
            Document = document;
            _update = true;
            _selectionTransformMat = Matrix.Identity;
            _selectionTransform = Matrix4.Identity;
            _models = new List<Tuple<Entity, Model>>();
            _modelLists = new Dictionary<Model, int>();

            var idx = GL.GenLists(5);
            _listUntransformed2D = idx + 0;
            _listTransformed2D = idx + 1;
            _listUntransformed3D = idx + 2;
            _listTransformed3D = idx + 3;
            _listUntransformedDecals3D = idx + 4;
        }

        public void Dispose()
        {
            GL.DeleteLists(_listUntransformed2D, 6);
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D)
        {
            //
        }

        public void SetSelectionTransform(Matrix4 selTransform)
        {
            _selectionTransform = selTransform;
            _selectionTransformMat = Matrix.FromOpenTKMatrix4(selTransform);
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            UpdateCache();

            RenderGrid(((Viewport2D)context).Zoom);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.MultMatrix(ref modelView);

            GL.CallList(_listUntransformed2D);

            GL.LoadMatrix(ref current);
            GL.MultMatrix(ref modelView);
            GL.MultMatrix(ref _selectionTransform);

            GL.CallList(_listTransformed2D);

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
            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            UpdateCache();

            GL.CallList(_listUntransformed3D);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);

            // Render models
            if (!View.DisableModelRendering)
            {
                foreach (var tuple in _models)
                {
                    var arr = _modelLists[tuple.Item2];
                    var origin = tuple.Item1.Origin;
                    if (tuple.Item1.HideDistance() <= (location - origin).VectorMagnitude())
                    {
                        Immediate.MapObjectRenderer.DrawFilled(tuple.Item1.GetBoxFaces(), Color.Empty);
                    }
                    else
                    {
                        var angles = tuple.Item1.EntityData.GetPropertyCoordinate("angles", Coordinate.Zero);
                        angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X), DMath.DegreesToRadians(angles.Y));
                        if (tuple.Item1.IsSelected)
                        {
                            origin *= _selectionTransformMat;
                        }
                        var tform = Matrix.Rotation(Quaternion.EulerAngles(angles)).Translate(origin).ToOpenTKMatrix4();
                        GL.MultMatrix(ref tform);

                        GL.CallList(arr);
                        GL.LoadMatrix(ref current);
                    }
                }
            }

            GL.MultMatrix(ref _selectionTransform);
            GL.CallList(_listTransformed3D);
            GL.LoadMatrix(ref current);

            GL.CallList(_listUntransformedDecals3D);
        }

        private bool _update;
        
        private void UpdateCache()
        {
            if (!_update) return;

            UpdateModels();

            var all = GetAllVisible(Document.Map.WorldSpawn);
            var cache = CollectFaces(all);
            var unselected = cache.Where(x => !x.IsSelected && (x.Parent == null || !x.Parent.IsSelected) && x.Opacity > 0.1).ToList();
            var selected = cache.Where(x => (x.IsSelected || (x.Parent != null && x.Parent.IsSelected)) && x.Opacity > 0.1).ToList();
            var decals = GetDecals(Document.Map.WorldSpawn).ToList();

            GL.NewList(_listUntransformed2D, ListMode.Compile);

            // Draw unselected stuff
            Immediate.MapObjectRenderer.DrawWireframe(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false);
            Immediate.MapObjectRenderer.DrawWireframe(decals.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), false);
            Immediate.MapObjectRenderer.DrawWireframe(_models.Where(x => !x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), false);

            // Draw selection (untransformed)
            GL.Color4(Color.FromArgb(128, 0, 0));
            Immediate.MapObjectRenderer.DrawWireframe(selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
            Immediate.MapObjectRenderer.DrawWireframe(decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
            Immediate.MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), true);

            GL.EndList();

            GL.NewList(_listTransformed2D, ListMode.Compile);

            // Draw selection (transformed)
            GL.Color4(Color.Red);
            Immediate.MapObjectRenderer.DrawWireframe(selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
            Immediate.MapObjectRenderer.DrawWireframe(decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
            Immediate.MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), true);

            GL.EndList();

            var sel3D = selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D).ToList();
            var sel3DDecals = decals.Where(x => x.IsSelected && !x.IsRenderHidden3D).ToList();


            GL.NewList(_listUntransformed3D, ListMode.Compile);
            // Draw unselected
            Immediate.MapObjectRenderer.DrawFilled(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty);
            // Draw selected (wireframe; untransformed)
            GL.Color4(Color.Yellow);
            Immediate.MapObjectRenderer.DrawWireframe(sel3D, true);
            Immediate.MapObjectRenderer.DrawWireframe(decals.Where(x => x.IsSelected && !x.IsRenderHidden3D).SelectMany(x => x.GetDecalGeometry()), true);
            Immediate.MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden3D).SelectMany(x => x.Item1.GetBoxFaces()), true);
            GL.EndList();

            GL.NewList(_listTransformed3D, ListMode.Compile);
            Immediate.MapObjectRenderer.DrawFilled(sel3D, Color.Empty);
            Immediate.MapObjectRenderer.DrawFilled(sel3DDecals.SelectMany(x => x.GetDecalGeometry()), Color.Empty);
            if (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection)
            {
                Immediate.MapObjectRenderer.DrawFilled(sel3D, Color.FromArgb(64, Color.Red));
                Immediate.MapObjectRenderer.DrawFilled(sel3DDecals.SelectMany(x => x.GetDecalGeometry()), Color.FromArgb(64, Color.Red));
            }
            GL.EndList();

            GL.NewList(_listUntransformedDecals3D, ListMode.Compile);
            Immediate.MapObjectRenderer.DrawFilled(decals.Where(x => !x.IsSelected && !x.IsRenderHidden3D).SelectMany(x => x.GetDecalGeometry()), Color.Empty);
            GL.EndList();

            _update = false;
        }

        private void UpdateModels()
        {
            _models.Clear();
            foreach (var entity in GetModels(Document.Map.WorldSpawn))
            {
                var model = entity.GetModel();
                _models.Add(Tuple.Create(entity, model.Model));
                if (!_modelLists.ContainsKey(model.Model))
                {
                    var list = GL.GenLists(1);
                    GL.NewList(list, ListMode.Compile);
                    Immediate.ModelRenderer.Render(model.Model);
                    GL.EndList();
                    _modelLists.Add(model.Model, list);
                }
            }
            foreach (var kv in _modelLists.Where(x => _models.All(y => y.Item2 != x.Key)).ToList())
            {
                GL.DeleteLists(kv.Value, 1);
                _modelLists.Remove(kv.Key);
            }
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
                if (entity != null && (!entity.HasModel() || View.DisableModelRendering))
                {
                    list.AddRange(entity.GetBoxFaces());
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

        private static IEnumerable<Entity> GetModels(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).OfType<Entity>().Where(x => x.HasModel());
        }

        private static IEnumerable<Entity> GetDecals(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            var results = list.Where(x => !x.IsCodeHidden).OfType<Entity>().Where(x => x.HasDecal()).ToList();
            results.ForEach(x => x.UpdateDecalGeometry());
            return results;
        }

        private static IEnumerable<MapObject> GetAllVisible(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).ToList();
        }

        private static void FindRecursive(ICollection<MapObject> items, MapObject root, Predicate<MapObject> matcher)
        {
            if (!matcher(root)) return;
            items.Add(root);
            root.Children.ForEach(x => FindRecursive(items, x, matcher));
        }
    }
}