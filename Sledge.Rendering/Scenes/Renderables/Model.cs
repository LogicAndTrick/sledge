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

        public Model(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
    }
}