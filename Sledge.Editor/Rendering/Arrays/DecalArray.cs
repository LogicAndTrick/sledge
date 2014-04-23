using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
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
    public class DecalArray : VBO<MapObject, MapObjectVertex>
    {
        private const int Transparent = 0;
        private const int Wireframe = 1;

        public DecalArray(IEnumerable<MapObject> data)
            : base(data)
        {
        }

        public void RenderTransparent(IGraphicsContext context, Coordinate cameraLocation)
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
                tex.Texture.Bind();
                Render(context, PrimitiveType.Triangles, subset);
            }
        }

        public void RenderWireframe(IGraphicsContext context)
        {
            foreach (var subset in GetSubsets(Wireframe))
            {
                Render(context, PrimitiveType.Lines, subset);
            }
        }

        protected override void CreateArray(IEnumerable<MapObject> objects)
        {
            var entities = objects.OfType<Entity>().Where(x => x.HasDecal()).ToList();

            StartSubset(Wireframe);

            var decals = new List<Tuple<Entity, Face>>();
            foreach (var entity in entities.Where(x => x.HasDecal()))
            {
                decals.AddRange(entity.GetDecalGeometry().Select(x => Tuple.Create(entity, x)));
            }

            // Render decals
            foreach (var entity in entities)
            {
                foreach (var face in entity.GetDecalGeometry())
                {
                    StartSubset(Transparent);
                    face.IsSelected = entity.IsSelected;
                    var index = PushData(Convert(face));
                    if (!entity.IsRenderHidden3D) PushIndex(Transparent, index, Triangulate(face.Vertices.Count));
                    if (!entity.IsRenderHidden2D) PushIndex(Wireframe, index, Linearise(face.Vertices.Count));

                    PushSubset(Transparent, face);
                }
            }

            PushSubset(Wireframe, (object)null);
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