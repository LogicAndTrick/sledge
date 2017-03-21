using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Editor.Rendering.Converters
{
    public class EntityDecalConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is Entity && GetDecalName((Entity)obj) != null;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var entity = (Entity) obj;

            var decalName = GetDecalName(entity);
            var tex = await document.TextureCollection.GetTextureItem(decalName);
            if (tex != null)
            {
                var geo = CalculateDecalGeometry(entity, tex, document);
                foreach (var face in geo)
                {
                    var f = await DefaultSolidConverter.ConvertFace(face, document);
                    f.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
                    smo.SceneObjects.Add(face, f);
                }
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
        }

        private static string GetDecalName(Entity entity)
        {
            if (entity.EntityData.Name != "infodecal") return null;

            var decal = entity.EntityData.Properties.FirstOrDefault(x => x.Key == "texture");
            if (decal == null) return null;

            return decal.Value;
        }
        private static IEnumerable<DataStructures.MapObjects.Face> CalculateDecalGeometry(Entity entity, TextureItem decal, Document document)
        {
            var decalGeometry = new List<DataStructures.MapObjects.Face>();
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
                texture.XShift = -decal.Width / 2m;
                texture.YShift = -decal.Height / 2m;
                var decalFace = new DataStructures.MapObjects.Face(idg.GetNextFaceID())
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
                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center + xShift - yShift), decalFace), // Bottom Right
                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center + xShift + yShift), decalFace), // Top Right
                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center - xShift + yShift), decalFace), // Top Left
                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center - xShift - yShift), decalFace)  // Bottom Left
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

        private static Solid CreateFakeDecalSolid(DataStructures.MapObjects.Face face)
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
                var f = new DataStructures.MapObjects.Face(-1)
                {
                    Colour = face.Colour,
                    IsSelected = face.IsSelected,
                    IsHidden = face.IsHidden,
                    Plane = new Plane(ff[0], ff[1], ff[2])
                };
                f.Vertices.AddRange(ff.Select(x => new DataStructures.MapObjects.Vertex(x, f)));
                f.UpdateBoundingBox();
                s.Faces.Add(f);
            }
            s.UpdateBoundingBox();
            return s;
        }
    }
}