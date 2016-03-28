using System;
using OpenTK;
using Sledge.Rendering.DataStructures;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Model : RenderableObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Vector3 Position
        {
            get { return Origin; }
            set { BoundingBox = new Box(value, value); }
        }

        private Vector3 _rotation;
        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; BoundingBox = new Box(Origin, Origin); }
        }

        public Model(string name)
        {
            Name = name;
        }

        public Matrix4 GetTransform()
        {
            return Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Origin);
        }
    }
}