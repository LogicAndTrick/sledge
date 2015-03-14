using System;
using System.Drawing;
using OpenTK;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Cameras;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Sprite : RenderableObject
    {
        private Vector3 _position;
        private float _width;
        private float _height;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                var square = Vector3.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Position");
            }
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                var square = Vector3.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Width");
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                var square = Vector3.One * Math.Max(_width, _height);
                BoundingBox = new Box(_position - square, _position + square);
                OnPropertyChanged("Height");
            }
        }

        public Sprite(Vector3 position, Material material, float width, float height)
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

        public Matrix4 GetBillboardMatrix(Vector3 eye)
        {
            //return Matrix4.LookAt(Vector3.Zero, eye, Vector3.UnitZ).Inverted() * Matrix4.CreateTranslation(Position);
            return Matrix4.LookAt(Position, eye, Vector3.UnitZ).Inverted();
        }
    }
}