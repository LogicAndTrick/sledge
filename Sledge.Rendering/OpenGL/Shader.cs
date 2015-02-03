using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL
{
    public class Shader : IDisposable
    {
        public ShaderType Type { get; private set; }
        public string ShaderCode { get; private set; }
        public int ID { get; private set; }

        public Shader(ShaderType type, string shaderCode)
        {
            Type = type;
            ShaderCode = shaderCode;
            ID = CreateShader(type, ShaderCode);
        }

        public void Dispose()
        {
            GL.DeleteShader(ID);
        }

        static int CreateShader(ShaderType shaderType, string shaderCode)
        {
            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, shaderCode);
            GL.CompileShader(shader);

            // Shader error logging
            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                var err = GL.GetShaderInfoLog(shader);
                throw new Exception("Error compiling " + shaderType + " shader: " + err);
            }

            return shader;
        }
    }
}