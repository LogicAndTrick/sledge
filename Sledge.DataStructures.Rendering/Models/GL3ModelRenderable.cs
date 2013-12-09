using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Models;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering.Models
{
    public class GL3ModelRenderable : ImmediateModelRenderable
    {

        #region Shaders
        public const string VertexShader = @"#version 130

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;

smooth out vec4 worldPosition;
smooth out vec4 worldNormal;
smooth out vec2 texCoord;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
    vec4 pos = vec4(position, 1);
    vec4 modelPos = modelViewMatrix * pos;
	vec4 cameraPos = cameraMatrix * modelPos;
	gl_Position = perspectiveMatrix * cameraPos;

    vec4 npos = vec4(normal, 1);
    vec3 normalPos = normalize(npos.xyz);
    npos = vec4(normalPos, 1);

    worldPosition = pos;
    worldNormal = npos;
    texCoord = texture;
}
";

        public const string FragmentShader = @"#version 130

smooth in vec4 worldPosition;
smooth in vec4 worldNormal;
smooth in vec2 texCoord;

uniform sampler2D currentTexture;

out vec4 outputColor;

void main()
{
    outputColor = texture2D(currentTexture, texCoord);
    outputColor.w = 1;
}
";
        #endregion Shaders

        private readonly ModelVertexArray _array;
        private ShaderProgram _program;

        public GL3ModelRenderable(Model model) : base(model)
        {
            _array = new ModelVertexArray(model);
            _program = new ShaderProgram(
                new Shader(ShaderType.VertexShader, VertexShader),
                new Shader(ShaderType.FragmentShader, FragmentShader));
        }

        public override void Render(object sender)
        {
            //var vp = sender as ViewportBase;
            // if (vp == null) return;

            _program.Bind();
            /*_program.Set("modelViewMatrix", vp.GetModelViewMatrix());
            _program.Set("perspectiveMatrix", vp.GetViewportMatrix());
            _program.Set("cameraMatrix", vp.GetCameraMatrix());*/
            _program.Set("modelViewMatrix", Matrix4.Identity);
            _program.Set("perspectiveMatrix", Matrix4.Identity);
            _program.Set("cameraMatrix", Matrix4.Identity);

            if (_textures.Any()) _textures.First().Value.Bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            _program.Set("currentTexture", 0);

            _array.Render(sender);

            TextureHelper.Unbind();
            _program.Unbind();
        }
    }
}