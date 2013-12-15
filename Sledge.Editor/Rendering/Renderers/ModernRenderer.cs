using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering.Arrays;
using Sledge.Editor.Rendering.Shaders;
using Sledge.Editor.UI;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ModernRenderer : IRenderer
    {
        public string Name { get { return "OpenGL 2.1 Renderer"; } }
        public Document Document { get { return _document; } set { _document = value; } }

        private Document _document;

        private readonly MapObjectArray _array;
        private readonly DecalArray _decalArray;
        private readonly Dictionary<Model, ModelArray> _modelArrays;
        private readonly List<Tuple<Entity, Model>> _models;

        private readonly MapObject2DShader _mapObject2DShader;
        private readonly MapObject3DShader _mapObject3DShader;
        private Matrix4 _selectionTransform;


        private Dictionary<ViewportBase, GridRenderable> GridRenderables { get; set; }

        public ModernRenderer(Document document)
        {
            _document = document;

            _array = new MapObjectArray(GetAllVisible(document.Map.WorldSpawn));
            _decalArray = new DecalArray(GetDecals(document.Map.WorldSpawn));
            _modelArrays = new Dictionary<Model, ModelArray>();
            _models = new List<Tuple<Entity, Model>>();

            GridRenderables = ViewportManager.Viewports.OfType<Viewport2D>().ToDictionary(x => (ViewportBase)x, x => new GridRenderable(_document));

            _selectionTransform = Matrix4.Identity;
            _mapObject2DShader = new MapObject2DShader();
            _mapObject3DShader = new MapObject3DShader();
        }

        public void Dispose()
        {
            _mapObject2DShader.Dispose();
            _mapObject3DShader.Dispose();
        }

        public void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D)
        {
            foreach (var kv in GridRenderables)
            {
                kv.Value.RebuildGrid(((Viewport2D)kv.Key).Zoom);
            }
        }

        public void SetSelectionTransform(Matrix4 selectionTransform)
        {
            _selectionTransform = selectionTransform;
        }

        public void Draw2D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            if (GridRenderables.ContainsKey(context)) GridRenderables[context].Render(context);

            var opts = new Viewport2DRenderOptions
            {
                Viewport = viewport,
                Camera = camera,
                ModelView = modelView
            };

            _mapObject2DShader.Bind(opts);

            // Render wireframe (untransformed)
            _mapObject2DShader.SelectionTransform = Matrix4.Identity;
            _mapObject2DShader.SelectedOnly = false;
            _mapObject2DShader.SelectedColour = new Vector4(0.5f, 0, 0, 1);
            _array.RenderWireframe(context.Context);

            // Render wireframe (transformed)
            _mapObject2DShader.SelectionTransform = _selectionTransform;
            _mapObject2DShader.SelectedOnly = true;
            _mapObject2DShader.SelectedColour = new Vector4(1, 0, 0, 1);
            _array.RenderWireframe(context.Context);

            _mapObject2DShader.Unbind();
        }

        public void Draw3D(ViewportBase context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            var opts = new Viewport3DRenderOptions
            {
                Viewport = viewport,
                Camera = camera,
                ModelView = modelView,
                ShowGrid = Document.Map.Show3DGrid,
                GridSpacing = Document.Map.GridSpacing,
                Lit = true,
                Textured = true,
                Wireframe = false
            };

            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            _mapObject3DShader.Bind(opts);
            _mapObject3DShader.SelectionTransform = _selectionTransform;
            _mapObject3DShader.SelectionColourMultiplier = Document.Map.HideFaceMask && Document.Selection.InFaceSelection
                ? new Vector4(1, 1, 1, 1) : new Vector4(1, 0.5f, 0.5f, 1);

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
                    _mapObject3DShader.Translation = new Vector4((float) origin.X, (float) origin.Y, (float) origin.Z, 0);
                    arr.RenderTextured(context.Context);
                }
                _mapObject3DShader.Translation = Vector4.Zero;
            }

            // Render untextured polygons
            _mapObject3DShader.IsTextured = false;
            _array.RenderUntextured(context.Context, location);
            
            // todo Render sprites

            _mapObject3DShader.Unbind();

            // Render wireframe
            _mapObject2DShader.Bind(opts);
            _mapObject2DShader.SelectedOnly = true;
            _mapObject2DShader.SelectedColour = new Vector4(1, 1, 0, 1);
            _mapObject2DShader.SelectionTransform = Matrix4.Identity;
            _array.RenderWireframe(context.Context);
            _decalArray.RenderWireframe(context.Context);
            _mapObject2DShader.Unbind();

            // Render transparent
            _mapObject3DShader.Bind(opts);
            _decalArray.RenderTransparent(context.Context, location);
            _array.RenderTransparent(context.Context, x => _mapObject3DShader.IsTextured = x, location);
            _mapObject3DShader.Unbind();
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
