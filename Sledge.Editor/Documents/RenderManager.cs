using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Rendering;
using Sledge.Graphics.Shaders;
using Sledge.UI;

namespace Sledge.Editor.Documents
{
    public class RenderManager
    {
        #region Shaders
        public const string VertexShader = @"#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec3 colour;
layout(location = 4) in float selected;

const vec3 light1direction = vec3(-1, -2, 3);
const vec3 light2direction = vec3(1, 2, 3);
const vec4 light1intensity = vec4(0.6, 0.6, 0.6, 1.0);
const vec4 light2intensity = vec4(0.3, 0.3, 0.3, 1.0);
const vec4 ambient = vec4(0.5, 0.5, 0.5, 1.0);

smooth out vec4 vertexLighting;
smooth out vec4 vertexColour;
smooth out vec2 texCoord;
smooth out float vertexSelected;

uniform bool isWireframe;
uniform bool in3d;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;
uniform mat4 selectionTransform;

void main()
{
    vec4 pos = vec4(position, 1);
    if (selected > 0.9 && (!isWireframe || !in3d)) pos = selectionTransform * pos;
    vec4 modelPos = modelViewMatrix * pos;
    
	vec4 cameraPos = cameraMatrix * modelPos;
	gl_Position = perspectiveMatrix * cameraPos;

    vec3 normalPos = normalize(normal);

    float incidence1 = dot(normalPos, light1direction);
    float incidence2 = dot(normalPos, light2direction);

    incidence1 = clamp(incidence1, 0, 1);
    incidence2 = clamp(incidence2, 0, 1);

	vertexColour = vec4(colour, 1);
    vertexLighting = (vec4(1,1,1,1) * light1intensity * incidence1) * 0.5
                   + (vec4(1,1,1,1) * light2intensity * incidence2) * 0.5
                   + (vec4(1,1,1,1) * ambient);
    texCoord = texture;
    vertexSelected = selected;
}
";

        public const string FragmentShader = @"#version 330

smooth in vec4 vertexColour;
smooth in vec4 vertexLighting;
smooth in vec2 texCoord;
smooth in float vertexSelected;

uniform bool isWireframe;
uniform bool isTextured;
uniform vec4 wireframeColour;
uniform bool in3d;
uniform sampler2D currentTexture;

out vec4 outputColor;
void main()
{
    if (in3d && vertexSelected <= 0.9) discard;
    if (isWireframe) {
        if (!in3d && vertexSelected > 0.9) outputColor = vec4(1, 0, 0, 1);
        else if (wireframeColour.w == 0) outputColor = vertexColour;
        else outputColor = wireframeColour;
    } else {
        if (isTextured) {
            outputColor = texture2D(currentTexture, texCoord) * vertexLighting;
        } else {
            outputColor = vertexColour * vertexLighting;
        }
        if (vertexSelected > 0.9) {
            outputColor = outputColor * vec4(1, 0, 0, 1);
        }
    }
}
";
        #endregion Shaders

        private Document _document;
        private ArrayManager _array;
        public ShaderProgram Shader { get; private set; }

        public RenderManager(Document document)
        {
            _document = document;
            _array = new ArrayManager(document.Map);
            Shader = new ShaderProgram(
                new Shader(ShaderType.VertexShader, VertexShader),
                new Shader(ShaderType.FragmentShader, FragmentShader));

            // Set up default values
            Shader.Bind();
            Shader.Set("perspectiveMatrix", Matrix4.Identity);
            Shader.Set("cameraMatrix", Matrix4.Identity);
            Shader.Set("modelViewMatrix", Matrix4.Identity);
            Shader.Set("selectionTransform", Matrix4.Identity);
            Shader.Set("isTextured", false);
            Shader.Set("isWireframe", false);
            Shader.Set("wireframeColour", new Vector4(0, 0, 0, 0));
            Shader.Set("in3d", false);
            Shader.Unbind();
        }

        public void Draw2D(object context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            Shader.Bind();
            Shader.Set("perspectiveMatrix", viewport);
            Shader.Set("cameraMatrix", camera);
            Shader.Set("modelViewMatrix", modelView);
            Shader.Set("isTextured", false);
            Shader.Set("isWireframe", true);
            Shader.Set("wireframeColour", new Vector4(0, 0, 0, 0));
            Shader.Set("in3d", false);
            _array.DrawWireframe(context, Shader);
            Shader.Unbind();
        }

        public void Draw3D(object context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView)
        {
            Shader.Bind();
            Shader.Set("perspectiveMatrix", viewport);
            Shader.Set("cameraMatrix", camera);
            Shader.Set("modelViewMatrix", modelView);
            Shader.Set("isTextured", true);
            Shader.Set("isWireframe", false);
            Shader.Set("in3d", false);
            GL.ActiveTexture(TextureUnit.Texture0);
            Shader.Set("currentTexture", 0);
            _array.DrawTextured(context, Shader);
            Shader.Set("in3d", true);
            Shader.Set("isWireframe", true);
            Shader.Set("wireframeColour", new Vector4(1, 1, 0, 1));
            _array.DrawWireframe(context, Shader);
            Shader.Unbind();
        }

        public void Update()
        {
            _array.Update(_document.Map);
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _array.UpdatePartial(faces);
        }

        public void Register(IEnumerable<ViewportBase> viewports)
        {
            foreach (var vp in viewports)
            {
                vp.RenderContext.Add(new RenderManagerRenderable(vp, this));
            }
        }
    }
}