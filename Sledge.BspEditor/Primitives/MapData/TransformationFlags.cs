using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class TransformationFlags : IMapData
    {
        public bool AffectsRendering => false;

        public bool TextureLock { get; set; } = true;
        public bool TextureScaleLock { get; set; }

        public TransformationFlags()
        {
        }

        public TransformationFlags(SerialisedObject obj)
        {
            TextureLock = obj.Get<bool>("TextureLock");
            TextureScaleLock = obj.Get<bool>("TextureScaleLock");
        }

        [Export(typeof(IMapElementFormatter))]
        public class HideFaceMaskFormatter : StandardMapElementFormatter<TransformationFlags> { }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TextureLock", TextureLock);
            info.AddValue("TextureScaleLock", TextureScaleLock);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("TransformationFlags");
            so.Set("TextureLock", TextureLock);
            so.Set("TextureScaleLock", TextureScaleLock);
            return so;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new TransformationFlags
            {
                TextureLock = TextureLock,
                TextureScaleLock = TextureScaleLock
            };
        }
    }
}
