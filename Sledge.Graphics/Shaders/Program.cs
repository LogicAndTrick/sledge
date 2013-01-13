using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Shaders
{
    public class Program : IDisposable
    {
        public int ID { get; private set; }
        public List<Variable> Variables { get; private set; }
        public List<Shader> Shaders { get; private set; }

        public Program(List<Shader> shaders)
        {
            Shaders = shaders;
            ID = CreateProgram(shaders);
            int c;
            GL.GetProgram(ID, ProgramParameter.ActiveUniforms, out c);
            for (var i = 0; i < c; i++)
            {
                int len, size;
                ActiveUniformType type;
                var sb = new StringBuilder();
                GL.GetActiveUniform(ID, i, 256, out len, out size, out type, sb);
                var loc = GL.GetUniformLocation(ID, sb.ToString());
                var v = new Variable(loc, sb.ToString(), type);
            }
        }

        public void Dispose()
        {
            Shaders.ForEach(x => GL.DetachShader(ID, x.ID));
            Shaders.ForEach(x => x.Dispose());
            GL.DeleteProgram(ID);
        }

        static int CreateProgram(List<Shader> shaders)
        {
            var program = GL.CreateProgram();
            shaders.ForEach(x => GL.AttachShader(program, x.ID));
            GL.LinkProgram(program);

            // Program error logging
            int status;
            GL.GetProgram(program, ProgramParameter.LinkStatus, out status);
            if (status == 0)
            {
                var err = GL.GetProgramInfoLog(program);
                throw new Exception("Error linking program: " + err);
            }

            return program;
        }
    }
}