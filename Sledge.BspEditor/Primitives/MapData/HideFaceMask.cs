using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class HideFaceMask : IMapData
    {
        public bool AffectsRendering => false;

        public bool Hidden { get; set; }

        public HideFaceMask()
        {

        }

        public HideFaceMask(SerialisedObject obj)
        {
            Hidden = obj.Get<bool>("Hidden");
        }

        [Export(typeof(IMapElementFormatter))]
        public class HideFaceMaskFormatter : StandardMapElementFormatter<HideFaceMask> { }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Hidden", Hidden);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("HideFaceMask");
            so.Set("Hidden", Hidden);
            return so;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new HideFaceMask
            {
                Hidden = Hidden
            };
        }
    }
}
