using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Vertex : ISerializable
    {
        private decimal _textureU;
        private decimal _textureV;

        private double _dTextureU;
        private double _dTextureV;

        public decimal TextureU
        {
            get { return _textureU; }
            set
            {
                _textureU = value;
                _dTextureU = (double) value;
            }
        }

        public decimal TextureV
        {
            get { return _textureV; }
            set
            {
                _textureV = value;
                _dTextureV = (double)value;
            }
        }

        public double DTextureU
        {
            get { return _dTextureU; }
            set
            {
                _dTextureU = value;
                _textureU = (decimal) value;
            }
        }

        public double DTextureV
        {
            get { return _dTextureV; }
            set
            {
                _dTextureV = value;
                _textureV = (decimal) value;
            }
        }

        public Coordinate Location { get; set; }

        public Face Parent { get; set; }

        public Vertex(Coordinate location, Face parent)
        {
            Location = location;
            Parent = parent;
            TextureV = TextureU = 0;
        }

        protected Vertex(SerializationInfo info, StreamingContext context)
        {
            TextureU = info.GetDecimal("TextureU");
            TextureV = info.GetDecimal("TextureV");
            Location = (Coordinate) info.GetValue("Location", typeof (Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TextureU", TextureU);
            info.AddValue("TextureV", TextureV);
            info.AddValue("Location", Location);
        }

        public Vertex Clone()
        {
            return new Vertex(Location.Clone(), Parent)
                       {
                           _textureU = _textureU,
                           _textureV = _textureV,
                           _dTextureU = _dTextureU,
                           _dTextureV = _dTextureV
                       };
        }
    }
}
