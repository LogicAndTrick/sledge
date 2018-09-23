using System.ComponentModel.Composition;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    public class EntityModel : IMapObjectData, IContentsReplaced, IBoundingBoxProvider
    {
        public string Name { get; }
        public IModel Model { get; }

        public bool ContentsReplaced => Model != null;

        public EntityModel(string name, IModel model)
        {
            Name = name;
            Model = model;
        }

        public EntityModel(SerialisedObject obj)
        {
            Name = obj.Get<string>("Name");
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<EntityModel> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public Box GetBoundingBox(IMapObject obj)
        {
            if (Model == null) return null;

            var origin = obj.Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
            return new Box(Model.Mins + origin, Model.Maxs + origin);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new EntityModel(Name, Model);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject(nameof(EntityModel));
            so.Set(nameof(Name), Name);
            return so;
        }
    }
}