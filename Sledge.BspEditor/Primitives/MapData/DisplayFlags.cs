using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class DisplayFlags : IMapData
    {
        public bool HideNullTextures { get; set; } = false;
        public bool HideDisplacementSolids { get; set; } = false;

        [Export(typeof(IMapElementFormatter))]
        public class DisplayFlagsFormatter : StandardMapElementFormatter<DisplayFlags> { }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HideNullTextures", HideNullTextures);
            info.AddValue("HideDisplacementSolids", HideDisplacementSolids);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("DisplayFlags");
            so.Set("HideNullTextures", HideNullTextures);
            so.Set("HideDisplacementSolids", HideDisplacementSolids);
            return so;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new DisplayFlags
            {
                HideNullTextures = HideNullTextures,
                HideDisplacementSolids = HideDisplacementSolids
            };
        }
    }
}
