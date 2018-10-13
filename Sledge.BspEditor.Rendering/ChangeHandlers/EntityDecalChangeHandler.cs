using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.ChangeHandling;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    [Export(typeof(IMapDocumentChangeHandler))]
    public class EntityDecalChangeHandler : IMapDocumentChangeHandler
    {
        public string OrderHint => "M";

        public async Task Changed(Change change)
        {
            var tc = await change.Document.Environment.GetTextureCollection();

            // Update any decal entities that have been changed
            var changedEntities = change.Added.Union(change.Updated).OfType<Entity>().ToHashSet();
            
            // Also update any decals had geometry for a changed solid
            var documentEntities = change.Document.Map.Root
                .Find(x => x is Entity e && !String.IsNullOrWhiteSpace(GetDecalName(e))).OfType<Entity>()
                .Except(changedEntities) // don't bother checking entities already in the change
                .Select(x => new
                {
                    Entity = x,
                    Box = new Box(x.Origin - Vector3.One * 4, x.Origin + Vector3.One * 4),
                    Decal = x.Data.GetOne<EntityDecal>()
                })
                .ToList();

            // Include removed solids in the test so we can delete decal geometry for deleted solids
            var changedSolids = change.Added.Union(change.Updated).Union(change.Removed).OfType<Solid>().ToList();
            foreach (var cs in changedSolids)
            {
                // Get decals that have geometry for the solid or decals that intersect with the solid
                var intersects = documentEntities.Where(x => x.Decal?.SolidIDs.Contains(cs.ID) == true || x.Box.IntersectsWith(cs.BoundingBox))
                    .Select(x => x.Entity)
                    .ToHashSet();

                // Add the intersecting decals to the change set
                change.UpdateRange(intersects);
                changedEntities.UnionWith(intersects);
            }

            // Perform the update
            foreach (var entity in changedEntities)
            {
                var sn = GetDecalName(entity);
                var dd = sn == null ? null : await CreateDecalData(entity, change.Document, tc, sn);
                if (dd == null) entity.Data.Remove(x => x is EntityDecal);
                else entity.Data.Replace(dd);
                entity.DescendantsChanged();
            }
        }

        private static string GetDecalName(Entity entity)
        {
            if (entity.EntityData.Name != "infodecal") return null;
            var decal = entity.EntityData.Properties.Where(x => x.Key == "texture").Select(x => x.Value).FirstOrDefault();
            return string.IsNullOrWhiteSpace(decal) ? null : decal;
        }

        private static async Task<EntityDecal> CreateDecalData(Entity entity, MapDocument doc, TextureCollection tc, string name)
        {
            var texture = await tc.GetTextureItem(name);
            if (texture == null) return null;

            var solidIds = new List<long>();
            var geometry = CalculateDecalGeometry(entity, texture, doc, solidIds).ToList();
            return new EntityDecal(name, solidIds, geometry);
        }

        private static IEnumerable<IMapObject> GetBoxIntersections(MapDocument document, Box box)
        {
            return document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren
            );
        }

        private static IEnumerable<Face> CalculateDecalGeometry(Entity entity, TextureItem decal, MapDocument document, ICollection<long> solidIds)
        {
            if (decal == null || entity.Hierarchy.Parent == null) yield break; // Texture not found

            var boxRadius = Vector3.One * 4;

            // Decals apply to all faces that intersect within an 8x8x8 bounding box
            // centered at the origin of the decal
            var box = new Box(entity.Origin - boxRadius, entity.Origin + boxRadius);

            // Get the faces that intersect with the decal's radius
            var lines = box.GetBoxLines().ToList();
            var faces = GetBoxIntersections(document, box)
                .OfType<Solid>()
                .SelectMany(x => x.Faces.Select(f => new { Solid = x, Face = f }))
                .Where(x =>
                {
                    var p = new Polygon(x.Face.Vertices);
                    return lines.Any(l => p.GetIntersectionPoint(l, true) != null);
                });

            foreach (var sf in faces)
            {
                var solid = sf.Solid;
                var face = sf.Face;
                solidIds.Add(solid.ID);

                // Project the decal onto the face
                var center = face.Plane.Project(entity.Origin);
                var texture = face.Texture.Clone();
                texture.Name = decal.Name;
                texture.XShift = -decal.Width / 2f;
                texture.YShift = -decal.Height / 2f;
                var decalFace = new Face(0)
                {
                    Plane = face.Plane,
                    Texture = texture
                };
                // Re-project the vertices in case the texture axes are not on the face plane
                var xShift = face.Texture.UAxis * face.Texture.XScale * decal.Width / 2;
                var yShift = face.Texture.VAxis * face.Texture.YScale * decal.Height / 2;
                var verts = new[]
                {
                    face.Plane.Project(center + xShift - yShift), // Bottom Right
                    face.Plane.Project(center + xShift + yShift), // Top Right
                    face.Plane.Project(center - xShift + yShift), // Top Left
                    face.Plane.Project(center - xShift - yShift)  // Bottom Left
                };

                // Because the texture axes don't have to align to the face, we might have a reversed face here
                // If so, reverse the points to get a valid face for the plane.
                // TODO: Is there a better way to do this?
                var vertPlane = new Plane(verts[0], verts[1], verts[2]);
                if (!face.Plane.Normal.EquivalentTo(vertPlane.Normal))
                {
                    Array.Reverse(verts);
                }

                decalFace.Vertices.AddRange(verts);

                // Calculate the X and Y shift bases on the first vertex location (assuming U/V of first vertex is zero) - we dont want these to change
                var vtx = decalFace.Vertices[0];
                decalFace.Texture.XShift = -(vtx.Dot(decalFace.Texture.UAxis)) / decalFace.Texture.XScale;
                decalFace.Texture.YShift = -(vtx.Dot(decalFace.Texture.VAxis)) / decalFace.Texture.YScale;

                // Next, the decal geometry needs to be clipped to the face so it doesn't spill into the void
                var poly = new Polygon(decalFace.Vertices).ToPrecisionPolygon();

                foreach (var f in solid.Faces.Except(new[] { decalFace }))
                {
                    poly.Split(f.Plane.ToPrecisionPlane(), out var back, out _);
                    poly = back ?? poly;
                }

                var newFace = poly.ToStandardPolygon();

                decalFace.Vertices.Clear();
                decalFace.Vertices.AddRange(newFace.Vertices);

                // Add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2f;
                decalFace.Transform(Matrix4x4.CreateTranslation(normalAdd));

                yield return decalFace;
            }
        }
    }
}
