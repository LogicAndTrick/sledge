using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Shaders
{
    public abstract class ShaderBase : IDisposable
    {
        protected ShaderProgram Shader { get; private set; }
        public abstract string Name { get; }
        public abstract IEnumerable<ShaderType> GetShaderTypes();

        public ShaderBase()
        {
            Shader = new ShaderProgram(GetShaderTypes().Select(GetShader).ToArray());
        }

        public void Bind()
        {
            Shader.Bind();
        }

        public void Unbind()
        {
            Shader.Unbind();
        }

        public void Dispose()
        {
            Shader.Dispose();
        }

        private Shader GetShader(ShaderType type)
        {
            string ext;
            switch (type)
            {
                case ShaderType.VertexShader:
                    ext = "vert";
                    break;
                case ShaderType.FragmentShader:
                    ext = "frag";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var ty = GetType();
            using (var s = ty.Assembly.GetManifestResourceStream(ty, Name + "." + ext))
            {
                if (s == null) throw new Exception("Shader file not found.");
                using (var r = new StreamReader(s))
                {
                    return new Shader(type, r.ReadToEnd());
                }
            }
        }
    }
}