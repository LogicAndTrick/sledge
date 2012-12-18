using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Vertex
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
