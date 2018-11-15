using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    public class EntityDecal : IMapObjectData, IBoundingBoxProvider
    {
        public string Name { get; set; }
        public IReadOnlyCollection<long> SolidIDs { get; }
        public List<Face> Geometry { get; set; }

        public EntityDecal(string name, IEnumerable<long> solidIds, IEnumerable<Face> geometry)
        {
            Name = name;
            SolidIDs = solidIds.ToList();
            Geometry = geometry.ToList();
        }

        public EntityDecal(SerialisedObject obj)
        {
            Name = obj.Get<string>("Name");
            SolidIDs = new List<long>();
            Geometry = new List<Face>();
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<EntityDecal> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public Box GetBoundingBox(IMapObject obj)
        {
            if (string.IsNullOrWhiteSpace(Name)) return null;
            if (!Geometry.Any()) return null;
            return new Box(Geometry.SelectMany(x => x.Vertices));
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new EntityDecal(Name, SolidIDs.ToList(), Geometry.Select(x => (Face) x.Clone()));
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject(nameof(EntityDecal));
            so.Set(nameof(Name), Name);
            return so;
        }
    }
}