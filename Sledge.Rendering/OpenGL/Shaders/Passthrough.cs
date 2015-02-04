using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Shaders;

namespace Sledge.Rendering.OpenGL.Shaders
{
    public class Passthrough : ShaderBase
    {
        public Matrix4 ViewportMatrix { set { Shader.Set("viewportMatrix", value); } }
        public Matrix4 CameraMatrix { set { Shader.Set("cameraMatrix", value); } }

        private const string ShaderName = "Passthrough";

        public Passthrough() : base()
        {

        }

        public override string Name
        {
            get { return "Passthrough"; }
        }

        public override IEnumerable<ShaderType> GetShaderTypes()
        {
            yield return ShaderType.VertexShader;
            yield return ShaderType.FragmentShader;
        }
    }
}
