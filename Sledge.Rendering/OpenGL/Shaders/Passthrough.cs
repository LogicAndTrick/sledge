using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Shaders
{
    public class Passthrough : ShaderBase
    {
        public Matrix4 ViewportMatrix { set { Shader.Set("viewportMatrix", value); } }
        public Matrix4 CameraMatrix { set { Shader.Set("cameraMatrix", value); } }
        public bool Wireframe { set { Shader.Set("wireframe", value); } }
        public bool Orthographic { set { Shader.Set("orthographic", value); } }

        private const string ShaderName = "Passthrough";

        public Passthrough()
        {
            Shader.Set("currentTexture", 0);
            Shader.Set("wireframe", false);
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
