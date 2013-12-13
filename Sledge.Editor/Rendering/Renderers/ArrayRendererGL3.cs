using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering.Shaders;
using Sledge.Editor.UI;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ArrayRendererGL3 : IRenderer
    {
        public string Name { get { return "OpenGL 3.0 Renderer"; } }
        public Document Document { get { return _document; } set { _document = value; } }

        private Document _document;
        private readonly MapObjectArray _array;
        private readonly MapObject2DShader _mapObject2DShader;
        private readonly MapObject3DShader _mapObject3DShader;
        private Matrix4 _selectionTransform;


        private Dictionary<ViewportBase, GridRenderable> GridRenderables { get; set; }

        public ArrayRendererGL3(Document document)
        {
            _document = document;

            var all = GetAllVisible(document.Map.WorldSpawn);
            _array = new MapObjectArray(all);

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

            // Render textured polygons
            _array.RenderTextured(context.Context);

            // Render untextured polygons
            _mapObject3DShader.IsTextured = false;
            _array.RenderUntextured(context.Context);

            //todo decals
            // Render sprites
            // Render models

            _mapObject3DShader.Unbind();

            // Render wireframe
            _mapObject2DShader.Bind(opts);
            _mapObject2DShader.SelectedOnly = true;
            _mapObject2DShader.SelectedColour = new Vector4(1, 1, 0, 1);
            _mapObject2DShader.SelectionTransform = Matrix4.Identity;
            _array.RenderWireframe(context.Context);
            _mapObject2DShader.Unbind();

            // Render transparent
            _mapObject3DShader.Bind(opts);
            _array.RenderTransparent(context.Context, x => _mapObject3DShader.IsTextured = x, location);
            _mapObject3DShader.Unbind();
        }

        public void Update()
        {
            var all = GetAllVisible(Document.Map.WorldSpawn);
            _array.Update(all);
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _array.UpdatePartial(faces);
        }

        public IRenderable CreateRenderable(Model model)
        {
            throw new NotImplementedException();
        }

        public void UpdateDocumentToggles()
        {
            // Not needed
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
