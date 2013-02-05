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

        public IEnumerable<Face> GetFaces()
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
            return GetFaces().Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }
    }
}
