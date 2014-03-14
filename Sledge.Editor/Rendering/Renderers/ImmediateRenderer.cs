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
using Sledge.Editor.Rendering.Immediate;
using Sledge.Extensions;
using Sledge.Graphics.Renderables;
using Sledge.Settings;
using Sledge.UI;
using Quaternion = Sledge.DataStructures.Geometric.Quaternion;

namespace Sledge.Editor.Rendering.Renderers
{
    public class ImmediateRenderer : IRenderer
    {
        public string Name { get { return "OpenGL 1.0 Renderer (Immediate Mode)"; } }
        public Document Document { get; set; }

        private Matrix _selectionTransformMat;
        private Matrix4 _selectionTransform;

        public ImmediateRenderer(Document document)
        {
            Document = document;
            _cache = null;
            _selectionTransformMat = Matrix.Identity;
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

            // Draw unselected stuff
            MapObjectRenderer.DrawWireframe(_unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false);
            MapObjectRenderer.DrawWireframe(_decals.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), false);
            MapObjectRenderer.DrawWireframe(_models.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), false);

            // Draw selection (untransformed)
            GL.Color4(Color.FromArgb(128, 0, 0));
            MapObjectRenderer.DrawWireframe(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
            MapObjectRenderer.DrawWireframe(_decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
            MapObjectRenderer.DrawWireframe(_models.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), true);

            GL.LoadMatrix(ref current);
            GL.MultMatrix(ref modelView);
            GL.MultMatrix(ref _selectionTransform);

            // Draw selection (transformed)
            GL.Color4(Color.Red);
            MapObjectRenderer.DrawWireframe(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
            MapObjectRenderer.DrawWireframe(_decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
            MapObjectRenderer.DrawWireframe(_models.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), true);

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
            UpdateCache();

            var type = ((Viewport3D)context).Type;
            var cam = ((Viewport3D)context).Camera.Location;
            var location = new Coordinate((decimal)cam.X, (decimal)cam.Y, (decimal)cam.Z);

            bool shaded = type == Viewport3D.ViewType.Shaded || type == Viewport3D.ViewType.Textured,
                 textured = type == Viewport3D.ViewType.Textured,
                 wireframe = type == Viewport3D.ViewType.Wireframe;

            if (shaded) MapObjectRenderer.EnableLighting();
            if (!textured) GL.Disable(EnableCap.Texture2D);
            else GL.Enable(EnableCap.Texture2D);

            Matrix4 current;
            GL.GetFloat(GetPName.ModelviewMatrix, out current);
            GL.MatrixMode(MatrixMode.Modelview);

            var sel3D = _selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D).ToList();
            var sel3DDecals = _decals.Where(x => x.IsSelected && !x.IsRenderHidden3D).ToList();

            if (!wireframe)
            {
                // Draw unselected
                MapObjectRenderer.DrawFilled(_unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden3D), Color.Empty, textured);
                // Draw selected (wireframe; untransformed)
                GL.Color4(Color.Yellow);
                MapObjectRenderer.DrawWireframe(sel3D, true);
                MapObjectRenderer.DrawWireframe(_decals.Where(x => x.IsSelected && !x.IsRenderHidden3D).SelectMany(x => x.GetDecalGeometry()), true);
                MapObjectRenderer.DrawWireframe(_models.Where(x => x.IsSelected && !x.IsRenderHidden3D).SelectMany(x => x.GetBoxFaces()), true);

                // Draw models
                if (!View.DisableModelRendering)
                {
                    foreach (var entity in _models)
                    {
                        var origin = entity.Origin;
                        if (entity.HideDistance() <= (location - origin).VectorMagnitude())
                        {
                            MapObjectRenderer.DrawFilled(entity.GetBoxFaces(), Color.Empty, textured);
                        }
                        else
                        {
                            var angles = entity.EntityData.GetPropertyCoordinate("angles", Coordinate.Zero);
                            angles = new Coordinate(DMath.DegreesToRadians(angles.Z), DMath.DegreesToRadians(angles.X), DMath.DegreesToRadians(angles.Y));
                            if (entity.IsSelected)
                            {
                                origin *= _selectionTransformMat;
                            }
                            var tform = Matrix.Rotation(Quaternion.EulerAngles(angles)).Translate(origin).ToOpenTKMatrix4();
                            GL.MultMatrix(ref tform);
                            ModelRenderer.Render(entity.GetModel().Model);
                            GL.LoadMatrix(ref current);
                        }
                    }
                }
            }
            else
            {
                // Draw unselected stuff
                MapObjectRenderer.DrawWireframe(_unselected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), false);
                MapObjectRenderer.DrawWireframe(_decals.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), false);
                MapObjectRenderer.DrawWireframe(_models.Where(x => !x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), false);

                // Draw selection (untransformed)
                GL.Color4(Color.FromArgb(128, 0, 0));
                MapObjectRenderer.DrawWireframe(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
                MapObjectRenderer.DrawWireframe(_decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
                MapObjectRenderer.DrawWireframe(_models.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), true);
            }

            GL.MultMatrix(ref _selectionTransform);

            if (!wireframe)
            {
                // Draw selection
                MapObjectRenderer.DrawFilled(sel3D, Color.Empty, textured);
                MapObjectRenderer.DrawFilled(sel3DDecals.SelectMany(x => x.GetDecalGeometry()), Color.Empty, textured);
                if (!Document.Map.HideFaceMask || !Document.Selection.InFaceSelection)
                {
                    MapObjectRenderer.DrawFilled(sel3D, Color.FromArgb(255, 255, 128, 128), textured);
                    MapObjectRenderer.DrawFilled(sel3DDecals.SelectMany(x => x.GetDecalGeometry()), Color.FromArgb(255, 255, 128, 128), textured);
                }
            }
            else
            {
                // Draw selection (transformed)
                GL.Color4(Color.Red);
                MapObjectRenderer.DrawWireframe(_selected.Where(x => x.Parent == null || !x.Parent.IsRenderHidden2D), true);
                MapObjectRenderer.DrawWireframe(_decals.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetDecalGeometry()), true);
                MapObjectRenderer.DrawWireframe(_models.Where(x => x.IsSelected && !x.IsRenderHidden2D).SelectMany(x => x.GetBoxFaces()), true);
            }

            GL.LoadMatrix(ref current);

            if (!wireframe)
            {
                // Draw unselected decals
                MapObjectRenderer.DrawFilled(_decals.Where(x => !x.IsSelected && !x.IsRenderHidden3D).SelectMany(x => x.GetDecalGeometry()), Color.Empty, textured);
            }

            MapObjectRenderer.DisableLighting();
            GL.Disable(EnableCap.Texture2D);
        }

        private List<Face> _cache;
        private List<Face> _unselected;
        private List<Face> _selected;
        private List<Entity> _decals;
        private List<Entity> _models;
        
        private void UpdateCache()
        {
            if (_cache != null) return;
            var all = GetAllVisible(Document.Map.WorldSpawn);
            _cache = CollectFaces(all);
            _unselected = _cache.Where(x => !x.IsSelected && (x.Parent == null || !x.Parent.IsSelected) && x.Opacity > 0.1).ToList();
            _selected = _cache.Where(x => (x.IsSelected || (x.Parent != null && x.Parent.IsSelected)) && x.Opacity > 0.1).ToList();

            _decals = GetDecals(Document.Map.WorldSpawn).ToList();
            _models = GetModels(Document.Map.WorldSpawn).ToList();
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

        public void UpdateSelection(IEnumerable<MapObject> objects)
        {
            _cache = null;
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
            root.Children.ForEach(x => FindRecursive(items, x, matcher));
        }
    }
}