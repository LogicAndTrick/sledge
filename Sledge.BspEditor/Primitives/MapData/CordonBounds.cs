using System.ComponentModel.Composition;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapData
{
    public class CordonBounds : IMapData
    {
        public bool AffectsRendering => true;

        public bool Enabled { get; set; }
        public Box Box { get; set; }

        public CordonBounds()
        {
            Enabled = false;
            Box = new Box(Vector3.One * -1024, Vector3.One * 1024);
        }

        public CordonBounds(SerialisedObject obj)
        {
            Enabled = obj.Get<bool>("Enabled");
            var start = obj.Get<Vector3>("Start");
            var end = obj.Get<Vector3>("End");
            Box = new Box(start, end);
        }

        [Export(typeof(IMapElementFormatter))]
        public class CordonBoundsFormatter : StandardMapElementFormatter<CordonBounds> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Meh
        }

        public IMapElement Clone()
        {
            return new CordonBounds
            {
                Box = Box.Clone(),
                Enabled = Enabled
            };
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("CordonBounds");
            so.Set("Enabled", Enabled);
            so.Set("Start", Box.Start);
            so.Set("End", Box.End);
            return so;
        }
    }
}
