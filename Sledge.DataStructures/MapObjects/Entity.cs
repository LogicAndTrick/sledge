using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.MapObjects
{
    public class Entity : MapObject
    {
        public GameDataObject GameData { get; set; }
        public EntityData EntityData { get; set; }
        public Coordinate Origin { get; set; }
        public ITexture Sprite { get; set; }
        public ITexture Decal { get; set; }

        public Entity(long id) : base(id)
        {
            Origin = new Coordinate(0, 0, 0);
            EntityData = new EntityData();
        }

        public override MapObject Copy(IDGenerator generator)
        {
            var e = new Entity(generator.GetNextObjectID())
                       {
                           GameData = GameData,
                           EntityData = EntityData.Clone(),
                           Origin = Origin.Clone()
                       };
            CopyBase(e, generator);
            return e;
        }

        public override void Paste(MapObject o, IDGenerator generator)
        {
            PasteBase(o, generator);
            var e = o as Entity;
            if (e == null) return;
            GameData = e.GameData;
            Origin = e.Origin.Clone();
            EntityData = e.EntityData.Clone();
        }

        public override MapObject Clone()
        {
            var e = new Entity(ID) {GameData = GameData, EntityData = EntityData.Clone(), Origin = Origin.Clone()};
            CopyBase(e, null, true);
            return e;
        }

        public override void Unclone(MapObject o)
        {
            PasteBase(o, null, true);
            var e = o as Entity;
            if (e == null) return;
            GameData = e.GameData;
            Origin = e.Origin.Clone();
            EntityData = e.EntityData.Clone();
        }

        public override void UpdateBoundingBox(bool cascadeToParent = true)
        {
            if (GameData == null && !Children.Any())
            {
                var sub = new Coordinate(-16, -16, -16);
                var add = new Coordinate(16, 16, 16);
                BoundingBox = new Box(Origin + sub, Origin + add);
            }
            else if (GameData != null && GameData.ClassType == ClassType.Point)
            {
                var sub = new Coordinate(-16, -16, -16);
                var add = new Coordinate(16, 16, 16);
                var behav = GameData.Behaviours.SingleOrDefault(x => x.Name == "size");
                if (behav != null && behav.Values.Count >= 6)
                {
                    sub = behav.GetCoordinate(0);
                    add = behav.GetCoordinate(1);
                }
                else if (GameData.Name == "infodecal")
                {
                    sub = Coordinate.One * -4;
                    add = Coordinate.One * 4;
                }
                BoundingBox = new Box(Origin + sub, Origin + add);
            }
            else if (Children.Any())
            {
                BoundingBox = new Box(Children.SelectMany(x => new[] {x.BoundingBox.Start, x.BoundingBox.End}));
            }
            else
            {
                BoundingBox = new Box(Origin, Origin);
            }
            base.UpdateBoundingBox(cascadeToParent);
        }

        public new Color Colour
        {
            get
            {
                if (GameData != null && GameData.ClassType == ClassType.Point)
                {
                    var behav = GameData.Behaviours.SingleOrDefault(x => x.Name == "color");
                    if (behav != null && behav.Values.Count == 3)
                    {
                        return behav.GetColour(0);
                    }
                }
                return base.Colour;
            }
            set { base.Colour = value; }
        }

        public IEnumerable<Face> GetBoxFaces()
        {
            var faces = new List<Face>();
            if (Children.Any()) return faces;

            var box = BoundingBox.GetBoxFaces();
            foreach (var ca in box)
            {
                var face = new Face(0)
                               {
                                   Plane = new Plane(ca[0], ca[1], ca[2]),
                                   Colour = Colour,
                                   IsSelected = IsSelected
                               };
                face.Vertices.AddRange(ca.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                faces.Add(face);
            }
            return faces;
        }

        private List<Face> _decalGeometry;

        public IEnumerable<Face> GetTexturedFaces()
        {
            return _decalGeometry ?? (_decalGeometry = new List<Face>());
        }

        public void CalculateDecalGeometry()
        {
            _decalGeometry = new List<Face>();
            if (Decal == null) return; // Texture not found

            var boxRadius = Coordinate.One * 4;
            // Decals apply to all faces that intersect within an 8x8x8 bounding box
            // centered at the origin of the decal
            var box = new Box(Origin - boxRadius, Origin + boxRadius);
            var root = GetRoot(Parent);
            // Get the faces that intersect with the decal's radius
            var faces = root.GetAllNodesIntersectingWith(box).OfType<Solid>()
                .SelectMany(x => x.Faces).Where(x => x.IntersectsWithBox(box));
            var idg = new IDGenerator(); // Dummy generator
            foreach (var face in faces)
            {
                // Project the decal onto the face
                var center = face.Plane.Project(Origin);
                var decalFace = new Face(idg.GetNextFaceID())
                                    {
                                        Colour = Colour,
                                        IsSelected = IsSelected,
                                        IsHidden = IsCodeHidden,
                                        Plane = face.Plane,
                                        Texture =
                                            {
                                                Name = Decal.Name,
                                                Texture = Decal,
                                                UAxis = face.Texture.UAxis,
                                                VAxis = face.Texture.VAxis,
                                                XScale = face.Texture.XScale,
                                                YScale = face.Texture.YScale,
                                                XShift = -Decal.Width / 2m,
                                                YShift = -Decal.Height / 2m
                                            }
                                    };
                // Re-project the vertices in case the texture axes are not on the face plane
                var xShift = face.Texture.UAxis * face.Texture.XScale * Decal.Width / 2;
                var yShift = face.Texture.VAxis * face.Texture.YScale * Decal.Height / 2;
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
                // TODO: verify this covers all situations and I don't have to manually calculate the texture coordinates
                decalFace.FitTextureToPointCloud(new Cloud(decalFace.Vertices.Select(x => x.Location)));
                
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
                decalFace = fake.Faces.First(x => x.Plane.EquivalentTo(face.Plane, 0.05m));

                // Add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2m;
                decalFace.Transform(new UnitTranslate(normalAdd), TransformFlags.None);

                _decalGeometry.Add(decalFace);
            }
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

        public override void Transform(IUnitTransformation transform, TransformFlags flags)
        {
            Origin = transform.Transform(Origin);
            base.Transform(transform, flags);
        }

        /// <summary>
        /// Returns the intersection point closest to the start of the line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>The closest intersecting point, or null if the line doesn't intersect.</returns>
        public override Coordinate GetIntersectionPoint(Line line)
        {
            var faces = GetBoxFaces();
            if (_decalGeometry != null) faces = faces.Union(_decalGeometry);
            return faces.Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        public override EntityData GetEntityData()
        {
            return EntityData;
        }
    }
}
