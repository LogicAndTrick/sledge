using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
            else if (MetaData.Has<Box>("BoundingBox"))
            {
                var box = MetaData.Get<Box>("BoundingBox");
                BoundingBox = box.Clone().Transform(new UnitTranslate(Origin - box.Center + new Coordinate(0, 0, box.Height / 2)));
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
                BoundingBox = new Box(GetChildren().SelectMany(x => new[] {x.BoundingBox.Start, x.BoundingBox.End}));
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
                    var behav = GameData.Behaviours.LastOrDefault(x => x.Name == "color");
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
            var dummySolid = new Solid(-1)
                                 {
                                     IsCodeHidden = IsCodeHidden,
                                     IsRenderHidden2D = IsRenderHidden2D,
                                     IsSelected = IsSelected,
                                     IsRenderHidden3D = IsRenderHidden3D,
                                     IsVisgroupHidden = IsVisgroupHidden
                                 };
            foreach (var ca in box)
            {
                var face = new Face(0)
                               {
                                   Plane = new Plane(ca[0], ca[1], ca[2]),
                                   Colour = Colour,
                                   IsSelected = IsSelected,
                                   Parent = dummySolid
                               };
                face.Vertices.AddRange(ca.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                faces.Add(face);
            }
            return faces;
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
            var faces = GetBoxFaces().Union(MetaData.GetAll<List<Face>>().SelectMany(x => x));
            return faces.Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        public override Box GetIntersectionBoundingBox()
        {
            return new Box(new[] {BoundingBox}.Union(MetaData.GetAll<Box>()));
        }

        public override EntityData GetEntityData()
        {
            return EntityData;
        }
    }
}
