using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Shaders
{
    public class ModelShader : ShaderBase
    {
        public Matrix4 SelectionTransform { set { Shader.Set("selectionTransform", value); } }
        public Matrix4 ModelMatrix { set { Shader.Set("modelMatrix", value); } }
        public Matrix4 ViewportMatrix { set { Shader.Set("viewportMatrix", value); } }
        public Matrix4 CameraMatrix { set { Shader.Set("cameraMatrix", value); } }
        public bool UseAccentColor { set { Shader.Set("useAccentColor", value); } }
        public bool Orthographic { set { Shader.Set("orthographic", value); } }

        public Matrix4[] AnimationTransforms { set { Shader.Set("animationTransforms[0]", value); } }

        private const string ShaderName = "Passthrough";

        public ModelShader()
        {
            Shader.Set("currentTexture", 0);
            Shader.Set("useAccentColor", false);
        }

        public override string Name
        {
            get { return "ModelShader"; }
        }

        public override IEnumerable<ShaderType> GetShaderTypes()
        {
            yield return ShaderType.VertexShader;
            yield return ShaderType.FragmentShader;
        }
    }
}
