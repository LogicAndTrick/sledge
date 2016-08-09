using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.OpenGL.Shaders;
using Sledge.Rendering.OpenGL.Vertices;
using Sledge.Rendering.Scenes.Renderables;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class RenderableVertexArray : VertexArray<RenderableObject, SimpleVertex>
    {
        private const int FacePolygons = 0;
        private const int FaceWireframe = 1;
        private const int FacePoints = 2;
        private const int FaceTransparentPolygons = 3;

        private const int LineWireframe = 11;
        private const int LinePoints = 12;

        private const int ForcedPolygons = 20;
        private const int ForcedWireframe = 21;
        private const int ForcedPoints = 22;
        private const int ForcedTransparentPolygons = 23;

        public HashSet<RenderableObject> Items { get; private set; }
        private Dictionary<Type, HashSet<RenderableObject>> _typedItems;
        private Dictionary<int, SetCache> _setCaches;

        private class SetCache
        {
            public object[] State { get; private set; }
            public List<Subset> Sets { get; private set; }

            public SetCache(object[] state, List<Subset> sets)
            {
                State = state;
                Sets = sets;
            }

            public bool Matches(object[] state)
            {
                if (state == null && State == null) return true;
                if (state == null || State == null) return false;
                if (state.Length != State.Length) return false;
                return !state.Where((t, i) => !Equals(t, State[i])).Any();
            }
        }

        private IEnumerable<Subset> HitCache(int key, Func<IEnumerable<Subset>> generator, params object[] state)
        {
            if (_setCaches.ContainsKey(key))
            {
                if (!_setCaches[key].Matches(state)) _setCaches.Remove(key);
            }
            if (!_setCaches.ContainsKey(key))
            {
                _setCaches[key] = new SetCache(state, generator().ToList());
            }
            return _setCaches[key].Sets;
        }

        public RenderableVertexArray(IEnumerable<RenderableObject> data) : base(data)
        {

        }

        public IEnumerable<string> GetMaterials()
        {
            return GetSubsets<string>(FacePolygons).Where(x => x.Instance != null).Select(x => x.Instance).OfType<string>();
        }

        public void Render(OpenGLRenderer renderer, IViewport viewport)
        {
            var camera = viewport.Camera;

            var vpMatrix = camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            var camMatrix = camera.GetCameraMatrix();
            var mvMatrix = camera.GetModelMatrix();

            var options = camera.RenderOptions;

            var shader = renderer.StandardShader;
            var modelShader = renderer.ModelShader;

            shader.Bind();
            shader.SelectionTransform = renderer.SelectionTransform;
            shader.ModelMatrix = mvMatrix;
            shader.CameraMatrix = camMatrix;
            shader.ViewportMatrix = vpMatrix;
            shader.Orthographic = camera.Flags.HasFlag(CameraFlags.Orthographic);
            shader.UseAccentColor = !options.RenderFacePolygonTextures;
            shader.UsePointColor = false;
            shader.Viewport = new Vector2(viewport.Control.Width, viewport.Control.Height);
            shader.Zoom = camera.Zoom;

            // todo
            // options.RenderFacePolygonLighting

            // Render non-transparent polygons
            string last = null;

            var sets = HitCache(FacePolygons, () =>
            {
                var ss = GetSubsets<string>(ForcedPolygons).ToList();
                if (options.RenderFacePolygons) ss.AddRange(GetSubsets<string>(FacePolygons));
                return ss.Where(x => x.Instance != null).OrderBy(x => (string) x.Instance);
            }, options.RenderFacePolygons);

            foreach (var subset in sets)
            {
                var mat = (string) subset.Instance;
                if (mat != last) renderer.Materials.Bind(mat);
                last = mat;

                Render(PrimitiveType.Triangles, subset);
            }

            shader.UseAccentColor = true;

            // Render wireframe
            sets = HitCache(FaceWireframe, () =>
            {
                var ss = GetSubsets(ForcedWireframe).ToList();
                if (options.RenderFaceWireframe) ss.AddRange(GetSubsets(FaceWireframe));
                if (options.RenderLineWireframe) ss.AddRange(GetSubsets(LineWireframe));
                return ss;
            }, options.RenderFaceWireframe, options.RenderLineWireframe);

            foreach (var subset in sets)
            {
                Render(PrimitiveType.Lines, subset);
            }

            shader.UsePointColor = true;

            // Render points
            sets = HitCache(FacePoints, () =>
            {
                var ss = GetSubsets(ForcedPoints).ToList();
                if (options.RenderFacePoints) ss.AddRange(GetSubsets(FacePoints));
                if (options.RenderLinePoints) ss.AddRange(GetSubsets(LinePoints));
                return ss;
            }, options.RenderFacePoints, options.RenderLinePoints);

            foreach (var subset in sets)
            {
                Render(PrimitiveType.Points, subset);
            }

            shader.UsePointColor = false;

            shader.Unbind();

            if (_typedItems.ContainsKey(typeof (Model)))
            {
                foreach (var o in _typedItems[typeof (Model)])
                {
                    var model = (Model) o;
                    var array = renderer.Models.GetArray(model.Name);
                    array.Render(renderer, modelShader, viewport, model.GetTransform());
                }
            }
        }

        public void RenderTransparent(OpenGLRenderer renderer, IViewport viewport)
        {
            var camera = viewport.Camera;

            var vpMatrix = camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            var camMatrix = camera.GetCameraMatrix();
            var mvMatrix = camera.GetModelMatrix();

            var eye = camera.EyeLocation;
            var options = camera.RenderOptions;

            var shader = renderer.StandardShader;

            shader.Bind();
            shader.SelectionTransform = renderer.SelectionTransform;
            shader.ModelMatrix = mvMatrix;
            shader.CameraMatrix = camMatrix;
            shader.ViewportMatrix = vpMatrix;
            shader.Orthographic = camera.Flags.HasFlag(CameraFlags.Orthographic);
            shader.UseAccentColor = !options.RenderFacePolygonTextures;
            shader.Viewport = new Vector2(viewport.Control.Width, viewport.Control.Height);
            shader.Zoom = camera.Zoom;

            GL.DepthMask(false);

            // Render transparent polygons, sorted back-to-front
            // todo: it may be worth doing per-face culling for transparent objects
            // todo: can I just turn off depth writing instead of sorting?
            string last = null;
            var lastMat = Matrix4.Identity;
            var lastAcc = true;
            var tsets = GetSubsets(ForcedTransparentPolygons).ToList();
            if (options.RenderFacePolygons) tsets.AddRange(GetSubsets(FaceTransparentPolygons));
            var sorted =
                from subset in tsets
                where subset.Instance != null
                let obj = subset.Instance as RenderableObject
                where obj != null
                orderby (eye - obj.Origin).LengthSquared descending
                select subset;
            foreach (var subset in sorted)
            {
                var mat = ((RenderableObject)subset.Instance).Material.UniqueIdentifier;
                if (mat != last) renderer.Materials.Bind(mat);
                last = mat;

                var mult = subset.Instance is Sprite ? ((Sprite) subset.Instance).GetBillboardMatrix(eye) : Matrix4.Identity;
                if (mult != lastMat) shader.ModelMatrix = mult;
                lastMat = mult;

                var acc = !options.RenderFacePolygonTextures && !(subset.Instance is Sprite);
                if (acc != lastAcc) shader.UseAccentColor = acc;
                lastAcc = acc;

                Render(PrimitiveType.Triangles, subset);
            }

            GL.DepthMask(true);
        }

        public void UpdatePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var obj in objects)
            {
                var offset = GetOffset(obj);
                if (offset < 0) continue;
                if (obj is Face) Update(offset, Convert((Face)obj));
                if (obj is Line) Update(offset, Convert((Line)obj));
                if (obj is Sprite) Update(offset, Convert((Sprite)obj));
            }
        }

        public void DeletePartial(IEnumerable<RenderableObject> objects)
        {
            foreach (var obj in objects)
            {
                var offset = GetOffset(obj);
                if (offset < 0) continue;
                if (obj is Face) Update(offset, Convert((Face)obj, VertexFlags.InvisibleOrthographic | VertexFlags.InvisiblePerspective));
                if (obj is Line) Update(offset, Convert((Line)obj, VertexFlags.InvisibleOrthographic | VertexFlags.InvisiblePerspective));
                if (obj is Sprite) Update(offset, Convert((Sprite)obj, VertexFlags.InvisibleOrthographic | VertexFlags.InvisiblePerspective));
            }
        }

        protected override void CreateArray(IEnumerable<RenderableObject> data)
        {
            Items = new HashSet<RenderableObject>(data);
            _typedItems = Items.GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => new HashSet<RenderableObject>(x));
            _setCaches = new Dictionary<int, SetCache>();

            StartSubset(LineWireframe);
            StartSubset(LinePoints);
            StartSubset(FaceWireframe);
            StartSubset(FacePoints);
            StartSubset(ForcedWireframe);
            StartSubset(ForcedPoints);

            var items = Items.Where(x => x.RenderFlags != RenderFlags.None || x.ForcedRenderFlags != RenderFlags.None).ToList();

            // Push faces (grouped by material)
            foreach (var g in items.OfType<Face>().GroupBy(x => x.Material.UniqueIdentifier))
            {
                StartSubset(FacePolygons);
                StartSubset(ForcedPolygons);

                foreach (var face in g)
                {
                    PushOffset(face);
                    var index = PushData(Convert(face));

                    var transparent = face.Material.HasTransparency;
                    if (transparent)
                    {
                        StartSubset(FaceTransparentPolygons);
                        StartSubset(ForcedTransparentPolygons);
                    }

                    if (face.ForcedRenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(transparent ? ForcedTransparentPolygons : ForcedPolygons, index, Triangulate(face.Vertices.Count));
                    else if (face.RenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(transparent ? FaceTransparentPolygons : FacePolygons, index, Triangulate(face.Vertices.Count));

                    if (face.ForcedRenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(ForcedWireframe, index, Linearise(face.Vertices.Count));
                    else if (face.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(FaceWireframe, index, Linearise(face.Vertices.Count));

                    if (face.ForcedRenderFlags.HasFlag(RenderFlags.Point)) PushIndex(ForcedPoints, index, new[] { 0u });
                    else if (face.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(FacePoints, index, new[] { 0u });

                    if (transparent)
                    {
                        PushSubset(FaceTransparentPolygons, face);
                        PushSubset(ForcedTransparentPolygons, face);
                    }
                }

                PushSubset(FacePolygons, g.Key);
                PushSubset(ForcedPolygons, g.Key);
            }

            // Push lines (no grouping)
            foreach (var line in items.OfType<Line>())
            {
                PushOffset(line);
                var index = PushData(Convert(line));

                if (line.ForcedRenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(ForcedWireframe, index, Linearise(line.Vertices.Count));
                else if (line.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(LineWireframe, index, Linearise(line.Vertices.Count));

                if (line.ForcedRenderFlags.HasFlag(RenderFlags.Point)) PushIndex(ForcedPoints, index, new[] { 0u });
                else if (line.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(LinePoints, index, new[] { 0u });
            }

            // Push sprites (grouped by material)
            foreach (var sprite in items.OfType<Sprite>())
            {
                StartSubset(FaceTransparentPolygons);

                PushOffset(sprite);
                var index = PushData(Convert(sprite));

                if (sprite.ForcedRenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(FaceTransparentPolygons, index, Triangulate(4));
                else if (sprite.RenderFlags.HasFlag(RenderFlags.Polygon)) PushIndex(FaceTransparentPolygons, index, Triangulate(4));

                if (sprite.ForcedRenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(ForcedWireframe, index, Linearise(4));
                else if (sprite.RenderFlags.HasFlag(RenderFlags.Wireframe)) PushIndex(FaceWireframe, index, Linearise(4));

                if (sprite.ForcedRenderFlags.HasFlag(RenderFlags.Point)) PushIndex(ForcedPoints, index, new[] { 0u });
                else if (sprite.RenderFlags.HasFlag(RenderFlags.Point)) PushIndex(FacePoints, index, new[] { 0u });

                PushSubset(FaceTransparentPolygons, sprite);
            }

            // Push displacements (grouped by material, then by displacement)
            // Push models (grouped by model, then by material)

            PushSubset(LineWireframe, (object)null);
            PushSubset(LinePoints, (object)null);
            PushSubset(FaceWireframe, (object)null);
            PushSubset(FacePoints, (object)null);
            PushSubset(ForcedWireframe, (object)null);
            PushSubset(ForcedPoints, (object)null);
        }

        private VertexFlags ConvertVertexFlags(RenderableObject obj)
        {
            var flags = VertexFlags.None;
            if (!obj.CameraFlags.HasFlag(CameraFlags.Orthographic)) flags |= VertexFlags.InvisibleOrthographic;
            if (!obj.CameraFlags.HasFlag(CameraFlags.Perspective)) flags |= VertexFlags.InvisiblePerspective;
            if (obj.IsSelected) flags |= VertexFlags.Selected;
            return flags;
        }

        private IEnumerable<SimpleVertex> Convert(Face face, VertexFlags flags = VertexFlags.None)
        {
            return face.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x.Position,
                Normal = face.Plane.Normal,
                Texture = new Vector2(x.TextureU, x.TextureV),
                MaterialColor = face.Material.Color.ToAbgr(),
                AccentColor = face.AccentColor.ToAbgr(),
                PointColor = face.PointColor.ToAbgr(),
                TintColor = face.TintColor.ToAbgr(),
                Flags = ConvertVertexFlags(face) | flags
            });
        }

        private IEnumerable<SimpleVertex> Convert(Line line, VertexFlags flags = VertexFlags.None)
        {
            return line.Vertices.Select((x, i) => new SimpleVertex
            {
                Position = x,
                Normal = Vector3.UnitZ,
                Texture = Vector2.Zero,
                MaterialColor = line.Material.Color.ToAbgr(),
                AccentColor = line.AccentColor.ToAbgr(),
                PointColor = line.PointColor.ToAbgr(),
                TintColor = line.TintColor.ToAbgr(),
                Flags = ConvertVertexFlags(line) | flags
            });
        }

        private IEnumerable<SimpleVertex> Convert(Sprite sprite, VertexFlags flags = VertexFlags.None)
        {
            var p = sprite.Position;
            var w = sprite.Width / 2;
            var h = sprite.Height / 2;
            var verts = new[]
                        {
                            new Vertex(new Vector3(-w, -h, 0), 0, 1),
                            new Vertex(new Vector3(+w, -h, 0), 1, 1),
                            new Vertex(new Vector3(+w, +h, 0), 1, 0),
                            new Vertex(new Vector3(-w, +h, 0), 0, 0),
                        };

            return verts.Select((x, i) => new SimpleVertex
            {
                Position = x.Position,
                Normal = Vector3.UnitZ,
                Texture = new Vector2(x.TextureU, x.TextureV),
                MaterialColor = sprite.Material.Color.ToAbgr(),
                AccentColor = sprite.AccentColor.ToAbgr(),
                PointColor = sprite.PointColor.ToAbgr(),
                TintColor = sprite.TintColor.ToAbgr(),
                Flags = ConvertVertexFlags(sprite) | flags
            });
        }
    }
}