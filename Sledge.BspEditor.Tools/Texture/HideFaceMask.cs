using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Tools.Texture
{
    [Serializable]
    public class HideFaceMask : IMapData
    {
        public bool Hidden { get; set; }

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
