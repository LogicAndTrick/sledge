using System;
using System.Drawing;
using OpenTK;
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

        public Matrix4 GetBillboardMatrix(Coordinate eye)
        {
            return Matrix4.LookAt(Vector3.Zero, eye.ToVector3(), Vector3.UnitZ).Inverted() * Matrix4.CreateTranslation(Position.ToVector3());

            var dir = (Position - eye).Normalise();
            var right = Coordinate.UnitZ.Cross(dir);
            var up = dir.Cross(right);

            var d = dir.ToVector3();
            var r = right.ToVector3();
            var u = up.ToVector3();
            var p = Position.ToVector3();

            if (false) return new Matrix4(
                    r.X, u.Y, d.X, 0,
                    r.Y, u.Y, d.Y, 0,
                    r.Z, u.Z, d.Z, 0,
                    0, 0, 0, 1
                );

            return new Matrix4(
                    r.X, r.Y, r.Z, 0,
                    u.X, u.Y, u.Z, 0,
                    d.X, d.Y, d.Z, 0,
                    p.X, p.Y, p.Z, 1
                );
        }
    }
}