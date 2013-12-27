using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Shaders
{
    public class Shader : IDisposable
    {
        public ShaderType Type { get; private set; }
        public string ShaderCode { get; private set; }
        public int ID { get; private set; }
        private Dictionary<int, string> _attribLocations;

        public Shader(ShaderType type, string shaderCode)
        {
            Type = type;
            ShaderCode = shaderCode;
            ReplaceLayoutLocations();
            ID = CreateShader(type, ShaderCode);
        }

        public void Dispose()
        {
            GL.DeleteShader(ID);
        }

        private void ReplaceLayoutLocations()
        {
            _attribLocations = new Dictionary<int, string>();
            // 300 -> layout(location = 0) in vec3 position;
            // 130 -> in vec3 position
            // 120 -> attribute vec3 position
            var regex = new Regex(@"layout\s*\(\s*location\s*=\s*(\d+)\s*\)\s*in\s*([^\s]*)\s*([^\s*]*)\s*;");
            ShaderCode = regex.Replace(ShaderCode, x =>
                                                       {
                                                           _attribLocations.Add(int.Parse(x.Groups[1].Value), x.Groups[3].Value);
                                                           return "attribute " + x.Groups[2].Value + " " + x.Groups[3].Value + ";";
                                                       });
            if (!ShaderCode.Trim().StartsWith("#version 120"))
            {
                throw new Exception("Please use #version 120 for shaders to keep support for older hardware.");
            }
        }

        public void BindAttribLocations(int program)
        {
            if (_attribLocations == null) return;
            foreach (var kv in _attribLocations)
            {
                GL.BindAttribLocation(program, kv.Key, kv.Value);
            }
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