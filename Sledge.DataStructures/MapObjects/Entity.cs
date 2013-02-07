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
        }

        public override MapObject Clone(IDGenerator generator)
        {
            var e = new Entity(generator.GetNextObjectID())
                       {
                           GameData = GameData,
                           EntityData = EntityData.Clone(),
                           Origin = Origin.Clone()
                       };
            CloneBase(e, generator);
            return e;
        }

        public override void Unclone(MapObject o, IDGenerator generator)
        {
            UncloneBase(o, generator);
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
            foreach (var face in faces)
            {
                // Project the decal onto the face
                var center = face.Plane.Project(Origin);
                var decalFace = new Face(int.MinValue) // Use a dummy ID
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
                // Also add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2m;
                var xShift = face.Texture.UAxis * face.Texture.XScale * Decal.Width / 2;
                var yShift = face.Texture.VAxis * face.Texture.YScale * Decal.Height / 2;
                decalFace.Vertices.Add(new Vertex(face.Plane.Project(center + xShift - yShift) + normalAdd, decalFace)); // Bottom Right
                decalFace.Vertices.Add(new Vertex(face.Plane.Project(center + xShift + yShift) + normalAdd, decalFace)); // Top Right
                decalFace.Vertices.Add(new Vertex(face.Plane.Project(center - xShift + yShift) + normalAdd, decalFace)); // Top Left
                decalFace.Vertices.Add(new Vertex(face.Plane.Project(center - xShift - yShift) + normalAdd, decalFace)); // Bottom Left
                // TODO: verify this covers all situations and I don't have to manually calculate the texture coordinates
                decalFace.FitTextureToPointCloud(new Cloud(decalFace.Vertices.Select(x => x.Location)));
                _decalGeometry.Add(decalFace);
            }
        }

        public override void Transform(IUnitTransformation transform)
        {
            Origin = transform.Transform(Origin);
            base.Transform(transform);
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
    }
}
