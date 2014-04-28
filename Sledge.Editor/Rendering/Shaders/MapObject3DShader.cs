using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Shaders;

namespace Sledge.Editor.Rendering.Shaders
{
    public class MapObject3DShader : IDisposable
    {
        private bool Show3DGrid { set { Shader.Set("showGrid", value); } }
        private float GridSpacing { set { Shader.Set("gridSpacing", value); } }

        private Matrix4 Perspective { set { Shader.Set("perspectiveMatrix", value); } }
        private Matrix4 Camera { set { Shader.Set("cameraMatrix", value); } }
        private Matrix4 ModelView { set { Shader.Set("modelViewMatrix", value); } }

        public bool IsTextured { set { Shader.Set("isTextured", value); } }
        private bool IsLit { set { Shader.Set("isLit", value); } }

        public Vector4 SelectionColourMultiplier { set { Shader.Set("selectionColourMultiplier", value); } }
        public Matrix4 Transformation { set { Shader.Set("transformation", value); } }

        public Matrix4 SelectionTransform
        {
            set
            {
                Shader.Set("selectionTransform", value);
                Shader.Set("inverseSelectionTransform", Matrix4.Invert(value));
            }
        }

        private ShaderProgram Shader { get; set; }

        private const string ShaderName = "MapObject3D";

        public MapObject3DShader()
        {
            Shader = new ShaderProgram(GetShader(ShaderType.VertexShader), GetShader(ShaderType.FragmentShader));

            Shader.Bind();

            Perspective = Camera = ModelView = SelectionTransform = Matrix4.Identity;
            IsTextured = true;
            IsLit = true;
            Show3DGrid = false;
            GridSpacing = 64;
            SelectionColourMultiplier = new Vector4(1, 0.5f, 0.5f, 1);
            Transformation = Matrix4.Identity;

            GL.ActiveTexture(TextureUnit.Texture0);
            Shader.Set("currentTexture", 0);

            Shader.Unbind();
        }

        public void Bind(Viewport3DRenderOptions options)
        {
            Shader.Bind();

            Perspective = options.Viewport;
            Camera = options.Camera;
            ModelView = options.ModelView;
            IsTextured = options.Textured;
            IsLit = options.Shaded;
            Show3DGrid = options.ShowGrid;
            GridSpacing = (float) options.GridSpacing;
        }

        public void Unbind()
        {
            Shader.Unbind();
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
            using (var s = ty.Assembly.GetManifestResourceStream(ty, ShaderName + "." + ext))
            {
                if (s == null) throw new Exception("Shader file not found.");
                using (var r = new StreamReader(s))
                {
                    return new Shader(type, r.ReadToEnd());
                }
            }
        }

        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}
