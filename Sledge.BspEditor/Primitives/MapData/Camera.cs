using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class Camera : IMapData, ISerializable
    {
        public Coordinate EyePosition { get; set; }
        public Coordinate LookPosition { get; set; }
        public bool IsActive { get; set; }

        public Camera()
        {
            EyePosition = new Coordinate(0, 0, 0);
            LookPosition = new Coordinate(0, 0, 0);
            IsActive = false;
        }

        public Camera(SerialisedObject obj)
        {
            EyePosition = obj.Get<Coordinate>("EyePosition");
            LookPosition = obj.Get<Coordinate>("LookPosition");
            IsActive = obj.Get<bool>("IsActive");
        }

        [Export(typeof(IMapElementFormatter))]
        public class EntityFormatter : StandardMapElementFormatter<Camera> { }

        protected Camera(SerializationInfo info, StreamingContext context)
        {
            EyePosition = (Coordinate)info.GetValue("EyePosition", typeof(Coordinate));
            LookPosition = (Coordinate)info.GetValue("LookPosition", typeof(Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("EyePosition", EyePosition);
            info.AddValue("LookPosition", LookPosition);
        }

        public IMapElement Clone()
        {
            return new Camera
            {
                EyePosition = EyePosition,
                LookPosition = LookPosition,
                IsActive = IsActive
            };
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Camera");
            so.Set("EyePosition", EyePosition);
            so.Set("LookPosition", LookPosition);
            so.Set("IsActive", IsActive);
            return so;
        }

        public decimal Length
        {
            get => (LookPosition - EyePosition).VectorMagnitude();
            set => LookPosition = EyePosition + Direction * value;
        }

        public Coordinate Direction
        {
            get => (LookPosition - EyePosition).Normalise();
            set => LookPosition = EyePosition + value.Normalise() * Length;
        }
    }
}