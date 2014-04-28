using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Shaders;

namespace Sledge.Editor.Rendering.Shaders
{
    public class MapObject2DShader : IDisposable
    {
        public bool SelectedOnly { set { Shader.Set("drawSelectedOnly", value); } }
        public bool UnselectedOnly { set { Shader.Set("drawUnselectedOnly", value); } }
        public Vector4 SelectedColour { set { Shader.Set("selectedColour", value); } }
        public Vector4 OverrideColour { set { Shader.Set("overrideColour", value); } }

        public Matrix4 Perspective { set { Shader.Set("perspectiveMatrix", value); } }
        public Matrix4 Camera { set { Shader.Set("cameraMatrix", value); } }
        public Matrix4 ModelView { set { Shader.Set("modelViewMatrix", value); } }

        public Matrix4 SelectionTransform { set { Shader.Set("selectionTransform", value); } }

        private ShaderProgram Shader { get; set; }

        private const string ShaderName = "MapObject2D";

        public MapObject2DShader()
        {
            Shader = new ShaderProgram(GetShader(ShaderType.VertexShader), GetShader(ShaderType.FragmentShader));

            Shader.Bind();

            Perspective = Camera = ModelView = SelectionTransform = Matrix4.Identity;
            SelectedOnly = UnselectedOnly = false;
            SelectedColour = new Vector4(1, 0, 0, 0.5f);
            OverrideColour = new Vector4(0, 0, 0, 0);

            Shader.Unbind();
        }

        public void Bind(Viewport2DRenderOptions options)
        {
            Shader.Bind();

            Perspective = options.Viewport;
            Camera = options.Camera;
            ModelView = options.ModelView;
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
