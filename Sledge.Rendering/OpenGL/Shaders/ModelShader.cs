using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Shaders
{
    public class ModelShader : ShaderBase
    {
        public Matrix4 SelectionTransform
        {
            set
            {
                Shader.Set("selectionTransform", value);
                // This shader is not currently used in the shader program outputs, so let's comment it out
                // todo : better system for this later?
                // Shader.Set("selectionTransformInverseTranspose", Matrix4.Transpose(value.Inverted()));
            }
        }

        public Matrix4 ModelMatrix { set { Shader.Set("modelMatrix", value); } }
        public Matrix4 ViewportMatrix { set { Shader.Set("viewportMatrix", value); } }
        public Matrix4 CameraMatrix { set { Shader.Set("cameraMatrix", value); } }
        public bool UseAccentColor { set { Shader.Set("useAccentColor", value); } }
        public bool Orthographic { set { Shader.Set("orthographic", value); } }

        public Matrix4[] AnimationTransforms { set { Shader.Set("animationTransforms[0]", value); } }

        private const string ShaderName = "ModelShader";

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
