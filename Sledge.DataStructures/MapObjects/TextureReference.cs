using System;
using System.Runtime.Serialization;
using Sledge.Common;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class TextureReference : ISerializable
    {
        public string Name { get; set; }
        private ITexture _texture;
        public ITexture Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                Name = _texture == null ? Name : _texture.Name;
            }
        }

        public decimal Rotation { get; set; }

        private Coordinate _uAxis;
        public Coordinate UAxis
        {
            get { return _uAxis; }
            set { _uAxis = value.Normalise(); }
        }

        private Coordinate _vAxis;
        public Coordinate VAxis
        {
            get { return _vAxis; }
            set { _vAxis = value.Normalise(); }
        }

        public decimal XShift { get; set; }
        public decimal XScale { get; set; }

        public decimal YShift { get; set; }
        public decimal YScale { get; set; }

        public TextureReference()
        {
            Name = "";
            Texture = null;
            Rotation = 0;
            _uAxis = -Coordinate.UnitZ;
            _vAxis = Coordinate.UnitX;
            XShift = YShift = 0;
            XScale = YScale = 1;
        }

        protected TextureReference(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Rotation = info.GetInt32("Rotation");
            _uAxis = (Coordinate) info.GetValue("UAxis", typeof (Coordinate));
            _vAxis = (Coordinate) info.GetValue("VAxis", typeof (Coordinate));
            XShift = info.GetDecimal("XShift");
            XScale = info.GetDecimal("XScale");
            YShift = info.GetDecimal("YShift");
            YScale = info.GetDecimal("YScale");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Rotation", Rotation);
            info.AddValue("UAxis", _uAxis);
            info.AddValue("VAxis", _vAxis);
            info.AddValue("XShift", XShift);
            info.AddValue("XScale", XScale);
            info.AddValue("YShift", YShift);
            info.AddValue("YScale", YScale);
        }

        public Coordinate GetNormal()
        {
            return UAxis.Cross(VAxis).Normalise();
        }

        public TextureReference Clone()
        {
            return new TextureReference
                       {
                           Name = Name,
                           Texture = Texture,
                           Rotation = Rotation,
                           UAxis = UAxis.Clone(),
                           VAxis = VAxis.Clone(),
                           XShift = XShift,
                           XScale = XScale,
                           YShift = YShift,
                           YScale = YScale
                       };
        }
    }
}
