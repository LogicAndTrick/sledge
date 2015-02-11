using System;
using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Cameras;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Sprite : RenderableObject
    {
        private Coordinate _position;
        private decimal _width;
        private decimal _height;

        public Coordinate Position
        {
            get { return _position; }
            set
            {
                _position = value;
                var square = Coordinate.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Position");
            }
        }

        public decimal Width
        {
            get { return _width; }
            set
            {
                _width = value;
                var square = Coordinate.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Width");
            }
        }

        public decimal Height
        {
            get { return _height; }
            set
            {
                _height = value;
                var square = Coordinate.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Height");
            }
        }

        public Sprite(Coordinate position, Material material, decimal width, decimal height)
        {
            Position = position;
            Width = width;
            Height = height;
            Material = material;
            AccentColor = Color.White;
            TintColor = Color.White;
            CameraFlags = CameraFlags.Perspective;
            RenderFlags = RenderFlags.Polygon;
            ForcedRenderFlags = RenderFlags.Polygon;
        }
    }
}