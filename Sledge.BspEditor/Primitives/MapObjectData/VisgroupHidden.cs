using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class VisgroupHidden : IMapObjectData, IObjectVisibility
    {
        public bool IsHidden { get; set; }

        public VisgroupHidden(bool isHidden)
        {
            IsHidden = isHidden;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsHidden", IsHidden);
        }

        public IMapElement Clone()
        {
            return new VisgroupHidden(IsHidden);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("VisgroupHidden");
            so.Set("IsHidden", IsHidden);
            return so;
        }
    }
}