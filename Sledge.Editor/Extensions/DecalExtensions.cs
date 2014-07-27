using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Extensions
{
    public static class DecalExtensions
    {
        private const string DecalMetaKey = "Decal";
        private const string DecalNameMetaKey = "DecalName";
        private const string DecalGeometryMetaKey = "DecalGeometry";
        private const string DecalBoundingBoxMetaKey = "DecalBoundingBox";

        public static bool UpdateDecals(this Map map, Document document)
        {
            return UpdateDecals(document, map.WorldSpawn);
        }

        public static bool UpdateDecals(this Map map, Document document, IEnumerable<MapObject> objects)
        {
            var updated = false;
            foreach (var mo in objects) updated |= UpdateDecals(document, mo);
            return updated;
        }

        private static bool UpdateDecals(Document document, MapObject mo)
        {
            var updatedChildren = false;
            foreach (var child in mo.GetChildren()) updatedChildren |= UpdateDecals(document, child);

            var e = mo as Entity;
            if (e == null || !ShouldHaveDecal(e))
            {
                var has = e != null && HasDecal(e);
                if (has) SetDecal(e, null);
                return updatedChildren || has;
            }

            var decal = e.EntityData.Properties.FirstOrDefault(x => x.Key == "texture");
            var existingDecal = e.MetaData.Get<string>(DecalNameMetaKey);
            if (decal == null || String.Equals(decal.Value, existingDecal, StringComparison.InvariantCultureIgnoreCase)) return updatedChildren;

            e.SetDecal(document.GetTexture(decal.Value.ToLowerInvariant()));
            return true;
        }

        public static bool ShouldHaveDecal(this Entity entity)
        {
            return entity.EntityData.Name == "infodecal";
        }

        public static void SetDecal(this Entity entity, ITexture texture)
        {
            if (texture == null)
            {
                entity.MetaData.Unset(DecalMetaKey);
                entity.MetaData.Unset(DecalNameMetaKey);
            }
            else
            {
                entity.MetaData.Set(DecalMetaKey, texture);
                entity.MetaData.Set(DecalNameMetaKey, texture.Name);
            }
            UpdateDecalGeometry(entity);
        }

        public static ITexture GetDecal(this Entity entity)
        {
            return entity.MetaData.Get<ITexture>(DecalMetaKey);
        }

        public static IEnumerable<Face> GetDecalGeometry(this Entity entity)
        {
            return entity.MetaData.Get<List<Face>>(DecalGeometryMetaKey) ?? new List<Face>();
        }

        public static void UpdateDecalGeometry(this Entity entity)
        {
            var decal = GetDecal(entity);
            if (decal == null)
            {
                entity.MetaData.Unset(DecalGeometryMetaKey);
                entity.MetaData.Unset(DecalBoundingBoxMetaKey);
            }
            else
            {
                var geometry = CalculateDecalGeometry(entity, decal);
                entity.MetaData.Set(DecalGeometryMetaKey, geometry);
                entity.MetaData.Set(DecalBoundingBoxMetaKey, geometry.Any() ? new Box(geometry.SelectMany(x => x.Vertices).Select(x => x.Location)) : null);
            }
        }

        public static bool HasDecal(this Entity entity)
        {
            return entity.MetaData.Has<ITexture>(DecalMetaKey);
        }

        private static List<Face> CalculateDecalGeometry(Entity entity, ITexture decal)
        {
            var decalGeometry = new List<Face>();
            if (decal == null || entity.Parent == null) return decalGeometry; // Texture not found

            var boxRadius = Coordinate.One * 4;
            // Decals apply to all faces that intersect within an 8x8x8 bounding box
            // centered at the origin of the decal
            var box = new Box(entity.Origin - boxRadius, entity.Origin + boxRadius);
            var root = MapObject.GetRoot(entity.Parent);
            // Get the faces that intersect with the decal's radius
            var faces = root.GetAllNodesIntersectingWith(box).OfType<Solid>()
                    .SelectMany(x => x.Faces).Where(x => x.IntersectsWithBox(box));
            var idg = new IDGenerator(); // Dummy generator
            foreach (var face in faces)
            {
                // Project the decal onto the face
                var center = face.Plane.Project(entity.Origin);
                var texture = face.Texture.Clone();
                texture.Name = decal.Name;
                texture.Texture = decal;
                texture.XShift = -decal.Width / 2m;
                texture.YShift = -decal.Height / 2m;
                var decalFace = new Face(idg.GetNextFaceID())
                {
                    Colour = entity.Colour,
                    IsSelected = entity.IsSelected,
                    IsHidden = entity.IsCodeHidden,
                    Plane = face.Plane,
                    Texture = texture
                };
                // Re-project the vertices in case the texture axes are not on the face plane
                var xShift = face.Texture.UAxis * face.Texture.XScale * decal.Width / 2;
                var yShift = face.Texture.VAxis * face.Texture.YScale * decal.Height / 2;
                var verts = new[]
                                {
                                    new Vertex(face.Plane.Project(center + xShift - yShift), decalFace), // Bottom Right
                                    new Vertex(face.Plane.Project(center + xShift + yShift), decalFace), // Top Right
                                    new Vertex(face.Plane.Project(center - xShift + yShift), decalFace), // Top Left
                                    new Vertex(face.Plane.Project(center - xShift - yShift), decalFace)  // Bottom Left
                                };

                // Because the texture axes don't have to align to the face, we might have a reversed face here
                // If so, reverse the points to get a valid face for the plane.
                // TODO: Is there a better way to do this?
                var vertPlane = new Plane(verts[0].Location, verts[1].Location, verts[2].Location);
                if (!face.Plane.Normal.EquivalentTo(vertPlane.Normal))
                {
                    Array.Reverse(verts);
                }

                decalFace.Vertices.AddRange(verts);
                decalFace.UpdateBoundingBox();

                // Calculate the X and Y shift bases on the first vertex location (assuming U/V of first vertex is zero) - we dont want these to change
                var vtx = decalFace.Vertices[0];
                decalFace.Texture.XShift = -(vtx.Location.Dot(decalFace.Texture.UAxis)) / decalFace.Texture.XScale;
                decalFace.Texture.YShift = -(vtx.Location.Dot(decalFace.Texture.VAxis)) / decalFace.Texture.YScale;
                decalFace.CalculateTextureCoordinates(true);

                // Next, the decal geometry needs to be clipped to the face so it doesn't spill into the void
                // Create a fake solid out of the decal geometry and clip it against all the brush planes
                var fake = CreateFakeDecalSolid(decalFace);

                foreach (var f in face.Parent.Faces.Except(new[] { face }))
                {
                    Solid back, front;
                    fake.Split(f.Plane, out back, out front, idg);
                    fake = back ?? fake;
                }

                // Extract out the original face
                decalFace = fake.Faces.FirstOrDefault(x => x.Plane.EquivalentTo(face.Plane, 0.05m));
                if (decalFace == null) continue;

                // Add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2m;
                decalFace.Transform(new UnitTranslate(normalAdd), TransformFlags.TextureLock);

                decalFace.IsSelected = entity.IsSelected;
                decalGeometry.Add(decalFace);
            }
            return decalGeometry;
        }

        private static Solid CreateFakeDecalSolid(Face face)
        {
            var s = new Solid(0)
            {
                Colour = face.Colour,
                IsVisgroupHidden = face.IsHidden,
                IsSelected = face.IsSelected
            };
            s.Faces.Add(face);
            var p = face.BoundingBox.Center - face.Plane.Normal * 10; // create a new point underneath the face
            var p1 = face.Vertices[0].Location;
            var p2 = face.Vertices[1].Location;
            var p3 = face.Vertices[2].Location;
            var p4 = face.Vertices[3].Location;
            var faces = new[]
                            {
                                new[] { p2, p1, p},
                                new[] { p3, p2, p},
                                new[] { p4, p3, p},
                                new[] { p1, p4, p}
                            };
            foreach (var ff in faces)
            {
                var f = new Face(-1)
                {
                    Colour = face.Colour,
                    IsSelected = face.IsSelected,
                    IsHidden = face.IsHidden,
                    Plane = new Plane(ff[0], ff[1], ff[2])
                };
                f.Vertices.AddRange(ff.Select(x => new Vertex(x, f)));
                f.UpdateBoundingBox();
                s.Faces.Add(f);
            }
            s.UpdateBoundingBox();
            return s;
        }
    }
}
