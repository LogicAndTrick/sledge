using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering.Immediate;
using Sledge.Extensions;
using Sledge.Settings;
using Sledge.UI;
using Matrix = Sledge.DataStructures.Geometric.Matrix;
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

        private readonly int _listUntransformed3DTextured;
        private readonly int _listTransformed3DTextured;

        private readonly int _listUntransformed3DFlat;
        private readonly int _listTransformed3DFlat;

        private readonly List<Face> _transparentFaces;
        private readonly Dictionary<Model, int> _modelLists;
        private readonly List<Tuple<Entity, Model>> _models;

        public DisplayListRenderer(Document document)
        {
            Document = document;
            _update = true;
            _selectionTransformMat = Matrix.Identity;
            _selectionTransform = Matrix4.Identity;
            _transparentFaces = new List<Face>();
            _models = new List<Tuple<Entity, Model>>();
            _modelLists = new Dictionary<Model, int>();

            var idx = GL.GenLists(7);
            _listUntransformed2D = idx + 0;
            _listTransformed2D = idx + 1;
            _listUntransformed3D = idx + 2;
            _listUntransformed3DTextured = idx + 3;
            _listUntransformed3DFlat = idx + 4;
            _listTransformed3DTextured = idx + 5;
            _listTransformed3DFlat = idx + 6;
        }

        public void Dispose()
        {
            GL.DeleteLists(_listUntransformed2D, 6);
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D, bool force)
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

            if (Document.Map.Show2DGrid)
            {
                RenderGrid(((Viewport2D) context).Zoom);
            }

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
            GL.Begin(PrimitiveType.Lines);
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
            var type = ((Viewport3D) context).Type;
            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            UpdateCache();

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);

            bool shaded = type == Viewport3D.ViewType.Shaded || type == Viewport3D.ViewType.Textured,
                 textured = type == Viewport3D.ViewType.Textured,
                 wireframe = type == Viewport3D.ViewType.Wireframe;

            if (shaded) MapObjectRenderer.EnableLighting();
            if (!textured) GL.Disable(EnableCap.Texture2D);
            else GL.Enable(EnableCap.Texture2D);

            if (!wireframe)
            {
                GL.CallList(textured ? _listUntransformed3DTextured : _listUntransformed3DFlat);
                
                if (!Document.Selection.InFaceSelection || !Document.Map.HideFaceMask)
                {
                    GL.CallList(_listUntransformed3D);
                }

                // Render models
                if (!View.DisableModelRendering)
                {
                    foreach (var tuple in _models)
                    {
                        var arr = _modelLists[tuple.Item2];
                        var origin = tuple.Item1.Origin;
                        if (tuple.Item1.HideDistance() <= (location - origin).VectorMagnitude())
                        {
                            MapObjectRenderer.DrawFilled(tuple.Item1.GetBoxFaces(), Color.Empty, textured);
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
            }
            else
            {
                GL.CallList(_listUntransformed2D);
            }

            GL.MultMatrix(ref _selectionTransform);
            if (!wireframe)
            {
                GL.CallList(textured ? _listTransformed3DTextured : _listTransformed3DFlat);
            }
            else
            {
                GL.CallList(_listTransformed2D);
            }
            GL.LoadMatrix(ref current);

            if (!wireframe)
            {
                foreach (var face in _transparentFaces.OrderByDescending(x => (location - x.BoundingBox.Center).LengthSquared()))
                {
                    var sel = (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection) && (face.IsSelected || (face.Parent != null && face.Parent.IsSelected));
                    if (sel) GL.MultMatrix(ref _selectionTransform);
                    MapObjectRenderer.DrawFilled(new[] {face}, sel ? Color.FromArgb(255, 255, 128, 128) : Color.Empty, true);
                    GL.LoadMatrix(ref current);
                }
            }
            else
            {
                if (Document.Map.HideFaceMask && Document.Selection.InFaceSelection)
                {
                    MapObjectRenderer.DrawWireframe(_transparentFaces, false, true);
                }
                else
                {
                    MapObjectRenderer.DrawWireframe(_transparentFaces.Where(x => !x.IsSelected && (x.Parent == null || !x.Parent.IsSelected)), false, true);
                    GL.Color4(Color.Red);
                    MapObjectRenderer.DrawWireframe(_transparentFaces.Where(x => x.IsSelected || (x.Parent != null && x.Parent.IsSelected)), true, true);
                }
            }

            MapObjectRenderer.DisableLighting();
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
            MapObjectRenderer.DrawWireframe(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false, true);
            MapObjectRenderer.DrawWireframe(decals.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), false, true);
            MapObjectRenderer.DrawWireframe(_models.Where(x => !x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), false, true);

            // Draw selection (untransformed)
            GL.Color4(Color.FromArgb(128, 0, 0));
            MapObjectRenderer.DrawWireframe(selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true, true);
            MapObjectRenderer.DrawWireframe(decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true, true);
            MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), true, true);

            GL.EndList();

            GL.NewList(_listTransformed2D, ListMode.Compile);

            // Draw selection (transformed)
            GL.Color4(Color.Red);
            MapObjectRenderer.DrawWireframe(selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true, true);
            MapObjectRenderer.DrawWireframe(decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true, true);
            MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden2D).SelectMany(x => x.Item1.GetBoxFaces()), true, true);

            GL.EndList();

            _transparentFaces.Clear();
            _transparentFaces.AddRange(selected.Where(x => x.Opacity < 0.9 || (x.Texture.Texture != null && x.Texture.Texture.HasTransparency())));
            _transparentFaces.AddRange(unselected.Where(x => x.Opacity < 0.9 || (x.Texture.Texture != null && x.Texture.Texture.HasTransparency())));
            _transparentFaces.AddRange(decals.SelectMany(x => x.GetDecalGeometry()));

            selected.RemoveAll(_transparentFaces.Contains);
            unselected.RemoveAll(_transparentFaces.Contains);

            var sel3D = selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D).ToList();

            // Draw unselected
            GL.NewList(_listUntransformed3DTextured, ListMode.Compile);
            MapObjectRenderer.DrawFilled(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty, true);
            GL.EndList();

            GL.NewList(_listUntransformed3DFlat, ListMode.Compile);
            MapObjectRenderer.DrawFilled(unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty, false);
            GL.EndList();

            GL.NewList(_listUntransformed3D, ListMode.Compile);
            // Draw selected (wireframe; untransformed)
            GL.Color4(Color.Yellow);
            MapObjectRenderer.DrawWireframe(sel3D, true, false);
            MapObjectRenderer.DrawWireframe(_models.Where(x => x.Item1.IsSelected && !x.Item1.IsRenderHidden3D).SelectMany(x => x.Item1.GetBoxFaces()), true, false);
            GL.EndList();

            GL.NewList(_listTransformed3DTextured, ListMode.Compile);
            MapObjectRenderer.DrawFilled(sel3D, Color.Empty, true);
            if (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection)
            {
                MapObjectRenderer.DrawFilled(sel3D, Color.FromArgb(255, 255, 128, 128), true);
            }
            GL.EndList();

            GL.NewList(_listTransformed3DFlat, ListMode.Compile);
            MapObjectRenderer.DrawFilled(sel3D, Color.Empty, false);
            if (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection)
            {
                MapObjectRenderer.DrawFilled(sel3D, Color.FromArgb(255, 255, 128, 128), false);
            }
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
                    ModelRenderer.Render(model.Model);
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

        public void UpdateSelection(IEnumerable<MapObject> objects)
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

        private static IEnumerable<Entity> GetDecals(MapObject root, bool update = true)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            var results = list.Where(x => !x.IsCodeHidden).OfType<Entity>().Where(x => x.HasDecal()).ToList();
            if (update) results.ForEach(x => x.UpdateDecalGeometry()); // TODO This is a big performance hit, can it be moved elsewhere?
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
            foreach (var mo in root.GetChildren())
            {
                FindRecursive(items, mo, matcher);
            }
        }
    }
}