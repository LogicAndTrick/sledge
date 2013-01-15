using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Shaders
{
    public class Variable
    {
        public int Location { get; private set; }
        public string Name { get; set; }
        public ActiveUniformType Type { get; set; }
        public object Value { get; set; }

        private int _samplerId;

        public Variable(int location, string name, ActiveUniformType type)
        {
            Location = location;
            Name = name;
            Type = type;
        }

        public void Set(float v)
        {
            if (Value != null && Value.Equals(v)) return;
            Value = v;
            GL.Uniform1(Location, v);
        }

        public void Set(Matrix4 matrix)
        {
            if (Value != null && Value.Equals(matrix)) return;
            Value = matrix;
            GL.UniformMatrix4(Location, false, ref matrix);
        }

        public void Set(bool b)
        {
            if (Value != null && Value.Equals(b)) return;
            Value = b;
            var i = b ? 1 : 0;
            GL.Uniform1(Location, i);
        }
    }
}