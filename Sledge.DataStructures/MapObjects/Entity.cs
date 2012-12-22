using System;
using System.Linq;
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

        public override MapObject Clone()
        {
            var e = new Entity
                       {
                           GameData = GameData,
                           EntityData = EntityData.Clone(),
                           Origin = Origin.Clone()
                       };
            CloneBase(e);
            return e;
        }

        public override void Unclone(MapObject o)
        {
            UncloneBase(o);
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
            else
            {
                BoundingBox = new Box(Children.SelectMany(x => new[] {x.BoundingBox.Start, x.BoundingBox.End}));
            }
            base.UpdateBoundingBox(cascadeToParent);
        }

        public override void Transform(IUnitTransformation transform)
        {
            Origin = transform.Transform(Origin);
            base.Transform(transform);
        }
    }
}
