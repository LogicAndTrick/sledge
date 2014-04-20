using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.Editor.Extensions;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Helpers;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;

namespace Sledge.Editor.Rendering.Arrays
{
    public class MapObjectArray : VBO<MapObject, MapObjectVertex>
    {
        private const int Textured = 0;
        private const int Transparent = 1;
        private const int Wireframe = 2;

        public MapObjectArray(IEnumerable<MapObject> data)
            : base(data)
        {
        }

        public void RenderTextured(IGraphicsContext context)
        {
            foreach (var subset in GetSubsets<ITexture>(Textured).Where(x => x.Instance != null))
            {
                var tex = (ITexture)subset.Instance;
                tex.Bind();
                Render(context, BeginMode.Triangles, subset);
            }
        }
        public void RenderUntextured(IGraphicsContext context, Coordinate location)
        {
            TextureHelper.Unbind();
            foreach (var subset in GetSubsets<ITexture>(Textured).Where(x => x.Instance == null))
            {
                Render(context, BeginMode.Triangles, subset);
            }
            foreach (var subset in GetSubsets<Entity>(Textured))
            {
                var e = (Entity) subset.Instance;
                if (!Sledge.Settings.View.DisableModelRendering && e.HasModel() && e.HideDistance() > (location - e.Origin).VectorMagnitude()) continue;
                Render(context, BeginMode.Triangles, subset);
            }
        }

        public void RenderTransparent(IGraphicsContext context, Action<bool> isTextured, Coordinate cameraLocation)
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
                isTextured(tex.Texture != null);
                //program.Set("isTextured", tex.Texture != null);
                Render(context, BeginMode.Triangles, subset);
            }
        }

        public void RenderWireframe(IGraphicsContext context)
        {
            foreach (var subset in GetSubsets(Wireframe))
            {
                Render(context, BeginMode.Lines, subset);
            }
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            UpdatePartial(objects.OfType<Solid>().SelectMany(x => x.Faces));
            UpdatePartial(objects.OfType<Entity>().Where(x => !x.HasChildren));
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
            var entities = obj.OfType<Entity>().Where(x => !x.HasChildren).ToList();

            StartSubset(Wireframe);

            // Render solids
            foreach (var group in faces.GroupBy(x => new { x.Texture.Texture, Transparent = HasTransparency(x) }))
            {
                var subset = group.Key.Transparent ? Transparent : Textured;
                if (!group.Key.Transparent) StartSubset(subset);

                foreach (var face in group)
                {
                    if (group.Key.Transparent) StartSubset(subset);

                    PushOffset(face);
                    var index = PushData(Convert(face));
                    if (!face.Parent.IsRenderHidden3D && face.Opacity > 0) PushIndex(subset, index, Triangulate(face.Vertices.Count));
                    if (!face.Parent.IsRenderHidden2D) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));

                    if (group.Key.Transparent) PushSubset(subset, face);
                }

                if (!group.Key.Transparent) PushSubset(subset, group.Key.Texture);
            }

            // Render entities
            foreach (var g in entities.GroupBy(x => x.HasModel()))
            {
                // key = false -> no model, put in the untextured group
                // key = true  -> model, put in the entity group
                if (!g.Key) StartSubset(Textured);
                foreach (var entity in g)
                {
                    if (g.Key) StartSubset(Textured);
                    PushOffset(entity);
                    foreach (var face in entity.GetBoxFaces())
                    {
                        var index = PushData(Convert(face));
                        if (!face.Parent.IsRenderHidden3D) PushIndex(Textured, index, Triangulate(face.Vertices.Count));
                        if (!face.Parent.IsRenderHidden2D) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));
                    }
                    if (g.Key) PushSubset(Textured, entity);
                }
                if (!g.Key) PushSubset(Textured, (ITexture) null);
            }

            PushSubset(Wireframe, (object)null);
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