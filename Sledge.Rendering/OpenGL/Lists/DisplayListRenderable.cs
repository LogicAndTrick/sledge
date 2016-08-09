using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.OpenGL.Lists
{
    public class DisplayListRenderable : IDisposable
    {
        private const int DisplayListCount = 20;
        private readonly int _firstListIndex;

        private int UntransformedForcedPolygonsTextured { get { return _firstListIndex + 0; } }
        private int UntransformedForcedPolygonsFlat { get { return _firstListIndex + 1; } }
        private int UntransformedPolygonsTextured { get { return _firstListIndex + 2; } }
        private int UntransformedPolygonsFlat { get { return _firstListIndex + 3; } }
        private int UntransformedForcedWireframe { get { return _firstListIndex + 4; } }
        private int UntransformedFaceWireframe { get { return _firstListIndex + 5; } }
        private int UntransformedLineWireframe { get { return _firstListIndex + 6; } }
        private int UntransformedForcedPoints { get { return _firstListIndex + 7; } }
        private int UntransformedFacePoints { get { return _firstListIndex + 8; } }
        private int UntransformedLinePoints { get { return _firstListIndex + 9; } }

        private int TransformedForcedPolygonsTextured { get { return _firstListIndex + 10; } }
        private int TransformedForcedPolygonsFlat { get { return _firstListIndex + 11; } }
        private int TransformedPolygonsTextured { get { return _firstListIndex + 12; } }
        private int TransformedPolygonsFlat { get { return _firstListIndex + 13; } }
        private int TransformedForcedWireframe { get { return _firstListIndex + 14; } }
        private int TransformedFaceWireframe { get { return _firstListIndex + 15; } }
        private int TransformedLineWireframe { get { return _firstListIndex + 16; } }
        private int TransformedForcedPoints { get { return _firstListIndex + 17; } }
        private int TransformedFacePoints { get { return _firstListIndex + 18; } }
        private int TransformedLinePoints { get { return _firstListIndex + 19; } }

        public HashSet<RenderableObject> Items { get; private set; }
        public HashSet<RenderableObject> SecondPassItems { get; private set; }

        public DisplayListRenderable(DisplayListRenderer renderer, IEnumerable<RenderableObject> data)
        {
            _firstListIndex = GL.GenLists(DisplayListCount);
            Update(renderer, data);
        }

        public void Render(DisplayListRenderer renderer, IViewport viewport)
        {
            var camera = viewport.Camera;
            var options = camera.RenderOptions;

            var vpMatrix = camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            var camMatrix = camera.GetCameraMatrix();
            var mvMatrix = camera.GetModelMatrix();
            var selTransform = renderer.SelectionTransform;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref vpMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camMatrix);
            GL.MultMatrix(ref mvMatrix);

            // ----- Unselected objects -----

            // Polygons
            GL.CallList(options.RenderFacePolygonTextures ? UntransformedForcedPolygonsTextured : UntransformedForcedPolygonsFlat);
            if (options.RenderFacePolygons)
            {
                GL.CallList(options.RenderFacePolygonTextures ? UntransformedPolygonsTextured : UntransformedPolygonsFlat);
            }

            // Wireframe
            GL.CallList(UntransformedForcedWireframe);
            if (options.RenderFaceWireframe) GL.CallList(UntransformedFaceWireframe);
            if (options.RenderLineWireframe) GL.CallList(UntransformedLineWireframe);

            // Points
            GL.CallList(UntransformedForcedPoints);
            if (options.RenderFacePoints) GL.CallList(UntransformedFacePoints);
            if (options.RenderLinePoints) GL.CallList(UntransformedLinePoints);

            // ----- Selected objects -----

            GL.MultMatrix(ref selTransform);

            // Polygons
            GL.CallList(options.RenderFacePolygonTextures ? TransformedForcedPolygonsFlat : TransformedForcedPolygonsTextured);
            if (options.RenderFacePolygons)
            {
                GL.CallList(options.RenderFacePolygonTextures ? TransformedPolygonsFlat : TransformedPolygonsTextured);
            }

            // Wireframe
            GL.CallList(TransformedForcedWireframe);
            if (options.RenderFaceWireframe) GL.CallList(TransformedFaceWireframe);
            if (options.RenderLineWireframe) GL.CallList(TransformedLineWireframe);

            // Points
            GL.CallList(TransformedForcedPoints);
            if (options.RenderFacePoints) GL.CallList(TransformedFacePoints);
            if (options.RenderLinePoints) GL.CallList(TransformedLinePoints);
        }

        public void RenderTransparent(DisplayListRenderer renderer, IViewport viewport)
        {

        }

        public void Update(DisplayListRenderer renderer, IEnumerable<RenderableObject> data)
        {
            Items = new HashSet<RenderableObject>(data);
            SecondPassItems = new HashSet<RenderableObject>(Items.Where(x => x is Sprite || x is Model || (x.Material != null && x.Material.HasTransparency)));

            var processableItems = Items.Where(x => x.RenderFlags != RenderFlags.None || x.ForcedRenderFlags != RenderFlags.None)
                    .Where(x => (x is Line || x is Face) && (x.Material == null || !x.Material.HasTransparency))
                    .ToList();

            var forcedPolygons = new List<RenderableObject>();
            var regularPolygons = new List<RenderableObject>();
            var forcedWireframe = new List<RenderableObject>();
            var faceWireframe = new List<RenderableObject>();
            var lineWireframe = new List<RenderableObject>();
            var forcedPoints = new List<RenderableObject>();
            var facePoints = new List<RenderableObject>();
            var linePoints = new List<RenderableObject>();

            foreach (var item in processableItems.Where(x => x.CameraFlags.HasFlag(CameraFlags.Perspective)))
            {
                if (item is Face)
                {
                    if (item.ForcedRenderFlags.HasFlag(RenderFlags.Polygon)) forcedPolygons.Add(item);
                    else if (item.RenderFlags.HasFlag(RenderFlags.Polygon)) regularPolygons.Add(item);

                    if (item.ForcedRenderFlags.HasFlag(RenderFlags.Wireframe)) forcedWireframe.Add(item);
                    else if (item.RenderFlags.HasFlag(RenderFlags.Wireframe)) faceWireframe.Add(item);

                    if (item.ForcedRenderFlags.HasFlag(RenderFlags.Point)) forcedPoints.Add(item);
                    else if (item.RenderFlags.HasFlag(RenderFlags.Point)) facePoints.Add(item);
                }
                else
                {
                    if (item.ForcedRenderFlags.HasFlag(RenderFlags.Wireframe)) forcedWireframe.Add(item);
                    else if (item.RenderFlags.HasFlag(RenderFlags.Wireframe)) lineWireframe.Add(item);

                    if (item.ForcedRenderFlags.HasFlag(RenderFlags.Point)) forcedPoints.Add(item);
                    else if (item.RenderFlags.HasFlag(RenderFlags.Point)) linePoints.Add(item);
                }
            }

            // Forced polygons - textured and flat
            MakeLists(UntransformedForcedPolygonsTextured, TransformedForcedPolygonsTextured, forcedPolygons.OfType<Face>(), x => RenderFacesTextured(renderer, x));
            MakeLists(UntransformedForcedPolygonsFlat, TransformedForcedPolygonsFlat, forcedPolygons.OfType<Face>(), x => RenderFacesFlat(renderer, x));

            // Regular polygons - textured and flat
            MakeLists(UntransformedPolygonsTextured, TransformedPolygonsTextured, regularPolygons.OfType<Face>(), x => RenderFacesTextured(renderer, x));
            MakeLists(UntransformedPolygonsFlat, TransformedPolygonsFlat, regularPolygons.OfType<Face>(), x => RenderFacesFlat(renderer, x));

            // Wireframe
            MakeLists(UntransformedForcedWireframe, TransformedForcedWireframe, forcedWireframe, x => RenderWireframe(renderer, x));
            MakeLists(UntransformedFaceWireframe, TransformedFaceWireframe, faceWireframe, x => RenderWireframe(renderer, x));
            MakeLists(UntransformedLineWireframe, TransformedLineWireframe, lineWireframe, x => RenderWireframe(renderer, x));

            // Points
            MakeLists(UntransformedForcedPoints, TransformedForcedPoints, forcedPoints, x => RenderPoints(renderer, x));
            MakeLists(UntransformedFacePoints, TransformedFacePoints, facePoints, x => RenderPoints(renderer, x));
            MakeLists(UntransformedLinePoints, TransformedLinePoints, linePoints, x => RenderPoints(renderer, x));
        }

        private void MakeLists<T>(int untransformedList, int transformedList, IEnumerable<T> objects, Action<IEnumerable<T>> action) where T : RenderableObject
        {
            var g = objects.GroupBy(x => x.IsSelected);
            foreach (var group in g)
            {
                GL.NewList(group.Key ? transformedList : untransformedList, ListMode.Compile);
                action(group);
                GL.EndList();
            }
        }

        private void RenderFacesTextured(DisplayListRenderer renderer, IEnumerable<Face> faces)
        {
            foreach (var textureGroup in faces.GroupBy(x => x.Material.UniqueIdentifier))
            {
                var list = textureGroup.ToList();
                renderer.Materials.Bind(textureGroup.Key);

                GL.Begin(PrimitiveType.Triangles);
                foreach (var face in list)
                {
                    GL.Color4(Blend(face.Material.Color, face.TintColor));
                    foreach (var vertex in Triangulate(face.Vertices))
                    {
                        GL.TexCoord2(vertex.TextureU, vertex.TextureV);
                        GL.Normal3(face.Plane.Normal);
                        GL.Vertex3(vertex.Position);
                    }
                }
                GL.End();
            }
        }

        private void RenderFacesFlat(DisplayListRenderer renderer, IEnumerable<Face> faces)
        {
            var texture = Material.Flat(Color.White);
            renderer.Materials.Bind(texture.UniqueIdentifier);

            GL.Begin(PrimitiveType.Triangles);
            foreach (var face in faces)
            {
                GL.Color4(Blend(face.TintColor, face.AccentColor));
                foreach (var vertex in Triangulate(face.Vertices))
                {
                    GL.TexCoord2(vertex.TextureU, vertex.TextureV);
                    GL.Normal3(face.Plane.Normal);
                    GL.Vertex3(vertex.Position);
                }
            }
            GL.End();
        }

        private void RenderWireframe(DisplayListRenderer renderer, IEnumerable<RenderableObject> objects)
        {
            var texture = Material.Flat(Color.White);
            renderer.Materials.Bind(texture.UniqueIdentifier);

            GL.Begin(PrimitiveType.Lines);
            foreach (var obj in objects)
            {
                var verts = obj is Line ? ((Line)obj).Vertices : ((Face)obj).Vertices.Select(x => x.Position).ToList();
                GL.Color4(obj.AccentColor);
                foreach (var vertex in Linearise(verts))
                {
                    GL.Vertex3(vertex);
                }
            }
            GL.End();
        }

        private void RenderPoints(DisplayListRenderer renderer, IEnumerable<RenderableObject> objects)
        {
            var texture = Material.Flat(Color.White);
            renderer.Materials.Bind(texture.UniqueIdentifier);

            GL.Begin(PrimitiveType.Points);
            foreach (var obj in objects)
            {
                var verts = obj is Line ? ((Line)obj).Vertices : ((Face)obj).Vertices.Select(x => x.Position).ToList();
                GL.Color4(obj.PointColor);
                foreach (var vertex in verts)
                {
                    GL.Vertex3(vertex);
                }
            }
            GL.End();
        }

        private IEnumerable<Vertex> Triangulate(IList<Vertex> polygon)
        {
            for (var i = 1; i < polygon.Count - 1; i++)
            {
                yield return polygon[0];
                yield return polygon[i];
                yield return polygon[i + 1];
            }
        }

        protected static IEnumerable<Vector3> Linearise(IList<Vector3> loop)
        {
            for (var i = 0; i < loop.Count; i++)
            {
                var ni = (i + 1) % loop.Count;
                yield return loop[i];
                yield return loop[ni];
            }
        }

        private static Color Blend(Color color, Color other)
        {
            return Color.FromArgb(
                (byte)((color.A) / 255f * (other.A / 255f) * 255),
                (byte)((color.R) / 255f * (other.R / 255f) * 255),
                (byte)((color.G) / 255f * (other.G / 255f) * 255),
                (byte)((color.B) / 255f * (other.B / 255f) * 255)
                );
        }

        public void Dispose()
        {
            GL.DeleteLists(_firstListIndex, DisplayListCount);
        }
    }
}