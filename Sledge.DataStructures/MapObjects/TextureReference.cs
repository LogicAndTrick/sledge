using Sledge.Common;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class TextureReference
    {
        public string Name { get; set; }
        private ITexture _texture;
        public ITexture Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                Name = _texture == null ? string.Empty : _texture.Name;
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

        public TextureReference Clone()
        {
            return new TextureReference
                       {
                           Name = Name,
                           Texture = Texture,
                           Rotation = Rotation,
                           _uAxis = _uAxis.Clone(),
                           _vAxis = _vAxis.Clone(),
                           XShift = XShift,
                           XScale = XScale,
                           YShift = YShift,
                           YScale = YScale
                       };
        }
    }
}
