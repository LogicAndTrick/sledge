using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering.Arrays;
using Sledge.Editor.Rendering.Shaders;
using Sledge.Editor.UI;
using Sledge.Extensions;
using Sledge.Graphics.Helpers;
using Sledge.UI;
using Matrix = Sledge.DataStructures.Geometric.Matrix;
using Quaternion = Sledge.DataStructures.Geometric.Quaternion;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ModernRenderer : IRenderer
    {
        public string Name { get { return "OpenGL 2.1 Renderer"; } }
        public Document Document { get { return _document; } set { _document = value; } }

        private Document _document;

        private Dictionary<Viewport2D, GridArray> GridArrays { get; set; }
        private readonly MapObjectArray _array;
        private readonly DecalArray _decalArray;
        private readonly Dictionary<Model, ModelArray> _modelArrays;
        private readonly List<Tuple<Entity, Model>> _models;

        private readonly MapObject2DShader _mapObject2DShader;
        private readonly MapObject3DShader _mapObject3DShader;

        private Matrix _selectionTransformMat;
        private Matrix4 _selectionTransform;

        public ModernRenderer(Document document)
        {
            _document = document;

            _array = new MapObjectArray(GetAllVisible(document.Map.WorldSpawn));
            _decalArray = new DecalArray(GetDecals(document.Map.WorldSpawn));
            _modelArrays = new Dictionary<Model, ModelArray>();
            _models = new List<Tuple<Entity, Model>>();

            // Can't use a single grid array as it varies depending on the zoom level (pixel hiding factor)
            GridArrays = ViewportManager.Viewports.OfType<Viewport2D>().ToDictionary(x => x, x => new GridArray());
            UpdateGrid(document.Map.GridSpacing, document.Map.Show2DGrid, document.Map.Show3DGrid, false);

            _selectionTransformMat = Matrix.Identity;
            _selectionTransform = Matrix4.Identity;
            _mapObject2DShader = new MapObject2DShader();
            _mapObject3DShader = new MapObject3DShader();
        }

        public void Dispose()
        {
            _array.Dispose();
            _decalArray.Dispose();
            foreach (var ma in _modelArrays) ma.Value.Dispose();
            _mapObject2DShader.Dispose();
            _mapObject3DShader.Dispose();
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D, bool force)
        {
            foreach (var vp in ViewportManager.Viewports.OfType<Viewport2D>().Where(x => !GridArrays.ContainsKey(x)))
            {
                GridArrays.Add(vp, new GridArray());
            }
            foreach (var vp in GridArrays.Keys.Where(x => !ViewportManager.Viewports.Contains(x)).ToList())
            {
                GridArrays[vp].Dispose();
                GridArrays.Remove(vp);
            }
            foreach (var kv in GridArrays)
            {
                kv.Value.Update(Document.GameData.MapSizeLow, Document.GameData.MapSizeHigh, gridSpacing, kv.Key.Zoom, force);
            }
        }

        public void SetSelectionTransform(Matrix4 selectionTransform)
        {
            _selectionTransform = selectionTransform;
            _selectionTransformMat = Matrix.FromOpenTKMatrix4(selectionTransform);
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            var opts = new Viewport2DRenderOptions
            {
                Viewport = viewport,
                Camera = camera,
                ModelView = Matrix4.Identity // modelView
            };

            _mapObject2DShader.Bind(opts);

            _mapObject2DShader.SelectionTransform = Matrix4.Identity;
            _mapObject2DShader.SelectedOnly = false;
            _mapObject2DShader.SelectedColour = new Vector4(0.5f, 0, 0, 1);

            if (Document.Map.Show2DGrid)
            {
                // Render grid
                var vp2 = (Viewport2D) context;
                if (GridArrays.ContainsKey(vp2)) GridArrays[vp2].Render(context.Context);
            }

            // Render wireframe (untransformed)
            _mapObject2DShader.ModelView = modelView;
            _array.RenderWireframe(context.Context);
            _decalArray.RenderWireframe(context.Context);

            if (Sledge.Settings.View.Draw2DVertices)
            {
                if (Sledge.Settings.View.OverrideVertexColour)
                {
                    var c = Sledge.Settings.View.VertexOverrideColour;
                    _mapObject2DShader.OverrideColour = new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, 1);
                }
                _array.RenderVertices(context.Context, Sledge.Settings.View.VertexPointSize);
                _mapObject2DShader.OverrideColour = Vector4.Zero;
            }

            // Render wireframe (transformed)
            _mapObject2DShader.SelectionTransform = _selectionTransform;
            _mapObject2DShader.SelectedOnly = true;
            _mapObject2DShader.SelectedColour = new Vector4(1, 0, 0, 1);
            _array.RenderWireframe(context.Context);
            _decalArray.RenderWireframe(context.Context);

            if (Sledge.Settings.View.Draw2DVertices)
            {
                if (Sledge.Settings.View.OverrideVertexColour)
                {
                    var c = Sledge.Settings.View.VertexOverrideColour;
                    _mapObject2DShader.OverrideColour = new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, 1);
                }
                _array.RenderVertices(context.Context, Sledge.Settings.View.VertexPointSize);
                _mapObject2DShader.OverrideColour = Vector4.Zero;
            }

            _mapObject2DShader.Unbind();
        }

        public void Draw3D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            var type = ((Viewport3D) context).Type;
            var opts = new Viewport3DRenderOptions
            {
                Viewport = viewport,
                Camera = camera,
                ModelView = modelView,
                ShowGrid = Document.Map.Show3DGrid,
                GridSpacing = Document.Map.GridSpacing,
                Shaded = type == Viewport3D.ViewType.Shaded || type == Viewport3D.ViewType.Textured,
                Textured = type == Viewport3D.ViewType.Textured,
                Wireframe = type == Viewport3D.ViewType.Wireframe
            };

            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            if (!opts.Wireframe)
            {
                _mapObject3DShader.Bind(opts);
                _mapObject3DShader.SelectionTransform = _selectionTransform;
                _mapObject3DShader.SelectionColourMultiplier = Document.Map.HideFaceMask &&
                                                               Document.Selection.InFaceSelection
                                                                   ? new Vector4(1, 1, 1, 1)
                                                                   : new Vector4(1, 0.5f, 0.5f, 1);

                // Render textured polygons
                _array.RenderTextured(context.Context);

                // Render textured models
                if (!Sledge.Settings.View.DisableModelRendering)
                {
                    foreach (var tuple in _models)
                    {
                        var arr = _modelArrays[tuple.Item2];
                        var origin = tuple.Item1.Origin;
                        if (tuple.Item1.HideDistance() <= (location - origin).VectorMagnitude()) continue;

                        var angles = tuple.Item1.EntityData.GetPropertyCoordinate("angles", Coordinate.Zero);
                        angles = new Coordinate(-DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X),
                                                -DMath.DegreesToRadians(angles.Y));
                        if (tuple.Item1.IsSelected)
                        {
                            origin *= _selectionTransformMat;
                            // TODO: rotation/angles
                        }
                        var tform = Matrix.Rotation(Quaternion.EulerAngles(angles)).Translate(origin);
                        _mapObject3DShader.Transformation = tform.ToGLSLMatrix4();
                        arr.RenderTextured(context.Context);
                    }
                    _mapObject3DShader.Transformation = Matrix4.Identity;
                }

                // Render untextured polygons
                _mapObject3DShader.IsTextured = false;
                _array.RenderUntextured(context.Context, location);

                // todo Render sprites

                _mapObject3DShader.Unbind();
            }

            // Render wireframe
            _mapObject2DShader.Bind(opts);
            _mapObject2DShader.SelectedColour = new Vector4(1, 1, 0, 1);

            if (opts.Wireframe)
            {
                _mapObject2DShader.SelectedOnly = false;
                _mapObject2DShader.SelectionTransform = _selectionTransform;
                _array.RenderWireframe(context.Context);
                _decalArray.RenderWireframe(context.Context);
            }

            if (!Document.Selection.InFaceSelection || !Document.Map.HideFaceMask)
            {
                _mapObject2DShader.SelectedOnly = true;
                _mapObject2DShader.SelectionTransform = Matrix4.Identity;
                _array.RenderWireframe(context.Context);
                _decalArray.RenderWireframe(context.Context);
            }

            _mapObject2DShader.Unbind();

            if (!opts.Wireframe)
            {
                // Render transparent
                _mapObject3DShader.Bind(opts);
                _decalArray.RenderTransparent(context.Context, location);
                _array.RenderTransparent(context.Context, x => _mapObject3DShader.IsTextured = x && opts.Textured, location);
                _mapObject3DShader.Unbind();
            }
        }

        public void Update()
        {
            _array.Update(GetAllVisible(Document.Map.WorldSpawn));
            _decalArray.Update(GetDecals(Document.Map.WorldSpawn));
            UpdateModels();
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
            _decalArray.Update(GetDecals(Document.Map.WorldSpawn));
            UpdateModels();
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _array.UpdatePartial(faces);
            _decalArray.Update(GetDecals(Document.Map.WorldSpawn));
            UpdateModels();
        }

        public void UpdateSelection(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
            _decalArray.Update(GetDecals(Document.Map.WorldSpawn, false));
        }

        private void UpdateModels()
        {
            _models.Clear();
            foreach (var entity in GetModels(Document.Map.WorldSpawn))
            {
                var model = entity.GetModel();
                _models.Add(Tuple.Create(entity, model.Model));
                if (!_modelArrays.ContainsKey(model.Model))
                {
                    _modelArrays.Add(model.Model, new ModelArray(model.Model));
                }
            }
            foreach (var kv in _modelArrays.Where(x => _models.All(y => y.Item2 != x.Key)).ToList())
            {
                kv.Value.Dispose();
                _modelArrays.Remove(kv.Key);
            }
        }

        public void UpdateDocumentToggles()
        {
            // Not needed
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
