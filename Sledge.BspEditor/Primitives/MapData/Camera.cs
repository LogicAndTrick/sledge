using System;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class Camera : IMapData, ISerializable
    {
        public bool AffectsRendering => false;

        public Vector3 EyePosition { get; set; }
        public Vector3 LookPosition { get; set; }
        public bool IsActive { get; set; }

        public Camera()
        {
            EyePosition = new Vector3(0, 0, 0);
            LookPosition = new Vector3(0, 0, 0);
            IsActive = false;
        }

        public Camera(SerialisedObject obj)
        {
            EyePosition = obj.Get<Vector3>("EyePosition");
            LookPosition = obj.Get<Vector3>("LookPosition");
            IsActive = obj.Get<bool>("IsActive");
        }

        [Export(typeof(IMapElementFormatter))]
        public class EntityFormatter : StandardMapElementFormatter<Camera> { }

        protected Camera(SerializationInfo info, StreamingContext context)
        {
            EyePosition = (Vector3)info.GetValue("EyePosition", typeof(Vector3));
            LookPosition = (Vector3)info.GetValue("LookPosition", typeof(Vector3));
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

        public float Length
        {
            get => (LookPosition - EyePosition).Length();
            set => LookPosition = EyePosition + Direction * value;
        }

        public Vector3 Direction
        {
            get => (LookPosition - EyePosition).Normalise();
            set => LookPosition = EyePosition + value.Normalise() * Length;
        }
    }
}