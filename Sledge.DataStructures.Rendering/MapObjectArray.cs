using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering
{
    public class MapObjectArray : VBO<MapObject, MapObjectArray.MapObjectVertex>
    {
        public struct MapObjectVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 Texture;
            public Color4 Colour;
            public float IsSelected;
        }

        private const int Textured = 0;
        private const int Transparent = 1;
        private const int Wireframe = 2;

        public MapObjectArray(IEnumerable<MapObject> data)
            : base(data)
        {
        }

        public void RenderTextured(IGraphicsContext context, ShaderProgram program)
        {
            foreach (var subset in GetSubsets<ITexture>(Textured))
            {
                var tex = (ITexture) subset.Instance;
                if (tex != null) tex.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tex != null);
                Render(context, BeginMode.Triangles, subset);
            }
        }

        public void RenderTransparent(IGraphicsContext context, ShaderProgram program, Coordinate cameraLocation)
        {
            var sorted =
                from subset in GetSubsets<Face>(Transparent)
                let face = subset.Instance as Face
                where face != null
                orderby (cameraLocation - face.BoundingBox.Center).LengthSquared() descending
                select subset;
            foreach (var subset in sorted)
            {
                var tex = ((Face) subset.Instance).Texture;
                if (tex.Texture != null) tex.Texture.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tex.Texture != null);
                Render(context, BeginMode.Triangles, subset);
            }
        }

        public void RenderWireframe(IGraphicsContext context, ShaderProgram program)
        {
            foreach (var subset in GetSubsets(Wireframe))
            {
                Render(context, BeginMode.Lines, subset);
            }
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            UpdatePartial(objects.OfType<Solid>().SelectMany(x => x.Faces));
            UpdatePartial(objects.OfType<Entity>().Where(x => x.Children.Count == 0));
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            foreach (var face in faces)
            {
                var offset = GetOffset(face);
                if (offset < 0) continue;
                var conversion = Convert(face);
                Update(offset, conversion);
            }
        }

        public void UpdatePartial(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                var offset = GetOffset(entity);
                if (offset < 0) continue;
                var conversion = entity.GetBoxFaces().SelectMany(Convert);
                Update(offset, conversion);
            }
        }

        protected override void CreateArray(IEnumerable<MapObject> objects)
        {
            var obj = objects.Where(x => !x.IsVisgroupHidden && !x.IsCodeHidden).ToList();
            var faces = obj.OfType<Solid>().SelectMany(x => x.Faces).ToList();
            var entities = obj.OfType<Entity>().Where(x => x.Children.Count == 0).ToList();

            StartSubset(Wireframe, (object)null);

            // Render solids
            foreach (var group in faces.GroupBy(x => new { x.Texture.Texture, Transparent = HasTransparency(x) }))
            {
                var subset = group.Key.Transparent ? Transparent : Textured;
                if (!group.Key.Transparent) StartSubset(subset, group.Key.Texture);

                foreach (var face in group)
                {
                    if (group.Key.Transparent) StartSubset(subset, face);

                    PushOffset(face);
                    var index = PushData(Convert(face));
                    if (!face.Parent.IsRenderHidden3D) PushIndex(subset, index, Triangulate(face.Vertices.Count));
                    if (!face.Parent.IsRenderHidden2D) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));

                    if (group.Key.Transparent) PushSubset(subset);
                }

                if (!group.Key.Transparent) PushSubset(subset);
            }

            // Render entities
            StartSubset<ITexture>(Textured, null);
            foreach (var entity in entities)
            {
                PushOffset(entity);
                foreach (var face in entity.GetBoxFaces())
                {
                    var index = PushData(Convert(face));
                    if (!face.Parent.IsRenderHidden3D) PushIndex(Textured, index, Triangulate(face.Vertices.Count));
                    if (!face.Parent.IsRenderHidden2D) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));
                }
            }
            PushSubset(Textured);

            PushSubset(Wireframe);
        }

        private bool HasTransparency(Face face)
        {
            return face.Opacity < 0.95
                   || (face.Texture.Texture != null && face.Texture.Texture.HasTransparency);
        }

        protected IEnumerable<MapObjectVertex> Convert(Face face)
        {
            float nx = (float)face.Plane.Normal.DX,
              ny = (float)face.Plane.Normal.DY,
              nz = (float)face.Plane.Normal.DZ;
            float r = face.Colour.R / 255f,
                  g = face.Colour.G / 255f,
                  b = face.Colour.B / 255f,
                  a = face.Opacity;
            return face.Vertices.Select(vert => new MapObjectVertex
            {
                Position = new Vector3((float)vert.Location.DX, (float)vert.Location.DY, (float)vert.Location.DZ),
                Normal = new Vector3(nx, ny, nz),
                Texture = new Vector2((float)vert.TextureU, (float)vert.TextureV),
                Colour = new Color4(r, g, b, a),
                IsSelected = face.IsSelected || (face.Parent != null && face.Parent.IsSelected) ? 1 : 0
            });
        }
    }
}