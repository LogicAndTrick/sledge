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

        public Variable(int location, string name, ActiveUniformType type)
        {
            Location = location;
            Name = name;
            Type = type;
        }

        public void Set(float v)
        {
            GL.Uniform1(Location, v);
        }

        public void Set(Matrix4 matrix)
        {
            GL.UniformMatrix4(Location, false, ref matrix);
        }
    }
}