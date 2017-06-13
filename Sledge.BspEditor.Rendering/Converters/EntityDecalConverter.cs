using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.BspEditor.Primitives.MapObjectData.Face;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class EntityDecalConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity && GetDecalName((Entity)obj) != null;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var entity = (Entity) obj;
            var decalName = GetDecalName(entity);

            var fakeSolid = new Solid(0) { IsSelected = entity.IsSelected, Data = { entity.Color }};

            var tc = await document.Environment.GetTextureCollection();
            if (tc != null)
            {
                var tex = await tc.GetTextureItem(decalName);
                if (tex != null)
                {
                    var geo = CalculateDecalGeometry(entity, tex, document);
                    foreach (var face in geo)
                    {
                        var f = await DefaultSolidConverter.ConvertFace(fakeSolid, face, document);
                        f.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
                        smo.SceneObjects.Add(face, f);
                    }
                }
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        private static string GetDecalName(Entity entity)
        {
            if (entity.EntityData.Name != "infodecal") return null;

            var decal = entity.EntityData.Properties.Where(x => x.Key == "texture").Select(x => x.Value).FirstOrDefault();

            return decal;
        }

        private static IEnumerable<IMapObject> GetBoxIntersections(MapDocument document, Box box)
        {
            return document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren
            );
        }

        private static IEnumerable<Face> CalculateDecalGeometry(Entity entity, TextureItem decal, MapDocument document)
        {
            if (decal == null || entity.Hierarchy.Parent == null) yield break; // Texture not found

            var boxRadius = Coordinate.One * 4;

            // Decals apply to all faces that intersect within an 8x8x8 bounding box
            // centered at the origin of the decal
            var box = new Box(entity.Origin - boxRadius, entity.Origin + boxRadius);

            // Get the faces that intersect with the decal's radius
            var lines = box.GetBoxLines().ToList();
            var faces = GetBoxIntersections(document, box)
                .OfType<Solid>()
                .SelectMany(x => x.Faces.Select(f => new { Solid = x, Face = f}))
                .Where(x =>
                {
                    var p = new Polygon(x.Face.Vertices);
                    return lines.Any(l => p.GetIntersectionPoint(l, true) != null);
                });

            foreach (var sf in faces)
            {
                var solid = sf.Solid;
                var face = sf.Face;

                // Project the decal onto the face
                var center = face.Plane.Project(entity.Origin);
                var texture = face.Texture.Clone();
                texture.Name = decal.Name;
                texture.XShift = -decal.Width / 2m;
                texture.YShift = -decal.Height / 2m;
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
                // Create a fake solid out of the decal geometry and clip it against all the brush planes
                var poly = new Polygon(decalFace.Vertices);
                var fake = CreateFakeDecalSolid(poly);

                foreach (var f in solid.Faces.Except(new[] { decalFace }))
                {
                    Polyhedron back, front;
                    fake.Split(f.Plane, out back, out front);
                    fake = back ?? fake;
                }

                // Extract out the original face
                var newFace = fake.Polygons.FirstOrDefault(x => x.GetPlane().EquivalentTo(face.Plane, 0.05m));
                if (newFace == null) continue;

                decalFace.Vertices.Clear();
                decalFace.Vertices.AddRange(newFace.Vertices);

                // Add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2m;
                decalFace.Transform(Matrix.Translation(normalAdd));

                yield return decalFace;
            }
        }

        private static Polyhedron CreateFakeDecalSolid(Polygon face)
        {
            var list = new List<Polygon> {face};

            var bbox = new Box(face.Vertices);
            var plane = face.GetPlane();

            var p = bbox.Center - plane.Normal * 10; // create a new point underneath the face
            var p1 = face.Vertices[0];
            var p2 = face.Vertices[1];
            var p3 = face.Vertices[2];
            var p4 = face.Vertices[3];
            var faces = new[]
            {
                new[] { p2, p1, p},
                new[] { p3, p2, p},
                new[] { p4, p3, p},
                new[] { p1, p4, p}
            };

            list.AddRange(faces.Select(ff => new Polygon(ff)));
            
            return new Polyhedron(list);
        }
    }
}