using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Rendering;
using Sledge.Database;
using Sledge.Editor.Documents;
using Sledge.Editor.UI;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Providers.Texture;
using Sledge.UI;
using Camera = Sledge.Graphics.Camera;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace Sledge.ShaderTest
{
    public partial class Form1 : Form
    {

        public const string VertexShader = @"#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec3 colour;

const vec3 light1direction = vec3(-1, -2, 3);
const vec3 light2direction = vec3(1, 2, 3);
const vec4 light1intensity = vec4(0.6, 0.6, 0.6, 1.0);
const vec4 light2intensity = vec4(0.3, 0.3, 0.3, 1.0);
const vec4 ambient = vec4(0.5, 0.5, 0.5, 1.0);

smooth out vec4 vertexLighting;
smooth out vec4 vertexColour;
smooth out vec2 texCoord;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * modelViewMatrix * vec4(position, 1);
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
}
";

        public const string FragmentShader = @"#version 330

smooth in vec4 vertexColour;
smooth in vec4 vertexLighting;
smooth in vec2 texCoord;

uniform bool isTextured;
uniform sampler2D currentTexture;

out vec4 outputColor;
void main()
{
    if (isTextured) {
        outputColor = texture(currentTexture, texCoord) * vertexLighting;
    } else {
        outputColor = vertexColour * vertexLighting;
    }
}
";


        private Viewport3D _viewport;

        public Form1()
        {
            Size = new Size(800, 800);
            InitializeComponent();

            _viewport = new Viewport3D()
            {
                Dock = DockStyle.Fill,
                Camera = new Camera { Location = new Vector3d(-0.5f, -0.5f, 0), LookAt = new Vector3d(1, 0, 0)}
            };
            _viewport.Listeners.Add(new Camera3DViewportListener(_viewport));
            //_viewport.Listeners.Add(new ToolViewportListener(viewport));

            _viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();

            Controls.Add(_viewport);
        }

        private ShaderProgram _shaderProgram;
        private VertexArrayByte _array;

        protected override void OnLoad(EventArgs e)
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());
            try
            {
                _shaderProgram = new ShaderProgram(new Shader(ShaderType.VertexShader, VertexShader),
                                                   //new Shader(ShaderType.GeometryShader, GeometryShader),
                                                   new Shader(ShaderType.FragmentShader, FragmentShader));
            

            var ratio = _viewport.Width / (float)_viewport.Height;
            if (ratio <= 0) ratio = 1;
            var matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), ratio, 0.1f, 100000f);

            var cam = _viewport.Camera;
            var cameraMatrix = Matrix4.LookAt(
                new Vector3((float)cam.Location.X, (float)cam.Location.Y, (float)cam.Location.Z),
                new Vector3((float)cam.LookAt.X, (float)cam.LookAt.Y, (float)cam.LookAt.Z), 
                Vector3.UnitZ);

            var mv = Matrix4.Identity;

            _shaderProgram.Bind();
            _shaderProgram.Set("perspectiveMatrix", matrix);
            _shaderProgram.Set("cameraMatrix", cameraMatrix);
            _shaderProgram.Set("modelViewMatrix", mv);
            _shaderProgram.Set("isTextured", true);
            _shaderProgram.Unbind();

            var map = MapProvider.GetMapFromFile(@"D:\Github\sledge\_Resources\RMF\verc_18.rmf");
            Document = new Document(@"D:\Github\sledge\_Resources\RMF\verc_18.rmf", map, Context.DBContext.GetAllGames().First());

            var am = new ArrayManager(map);

            _viewport.Listeners.Add(new UpdateCameraListener(_array, _shaderProgram, _viewport));


            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
            //GL.FrontFace(FrontFaceDirection.Cw);

            //GL.Disable(EnableCap.CullFace);

            _viewport.Run();

            //_viewport.RenderContext.Add(new Rawr(_program, _offsetUniform, (sizeof(byte) * data.Length) / 2, _buffer));

            //_viewport.RenderContext.Add(new ArrayRenderable(_array, _shaderProgram));

            _viewport.RenderContext.Add(new ArrayManagerRenderable(am, map, _shaderProgram));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            base.OnLoad(e);
        }

        protected Document Document { get; set; }
    }

    class ArrayManagerRenderable : IRenderable
    {
        private ArrayManager _manager;
        private ShaderProgram _shaderProgram;
        private Map _map;

        public ArrayManagerRenderable(ArrayManager manager, Map map, ShaderProgram shaderProgram)
        {
            _map = map;
            _manager = manager;
            _shaderProgram = shaderProgram;
        }

        public void Render(object sender)
        {
            //_manager.Update(_map);

            TextureHelper.EnableTexturing();
            _shaderProgram.Bind();
            _manager.Draw(_shaderProgram);
            _shaderProgram.Unbind();
            TextureHelper.DisableTexturing();
        }
    }

    class UpdateCameraListener : IViewportEventListener
    {
        private VertexArrayByte _array;
        private ShaderProgram _shaderProgram;

        public UpdateCameraListener(VertexArrayByte array, ShaderProgram shaderProgram, Viewport3D vp)
        {
            _shaderProgram = shaderProgram;
            _array = array;
            Viewport = vp;
        }

        public ViewportBase Viewport { get; set; }
        public void KeyUp(KeyEventArgs e)
        {

        }

        public void KeyDown(KeyEventArgs e)
        {

        }

        public void KeyPress(KeyPressEventArgs e)
        {

        }

        public void MouseMove(MouseEventArgs e)
        {

        }

        public void MouseWheel(MouseEventArgs e)
        {

        }

        public void MouseUp(MouseEventArgs e)
        {

        }

        public void MouseDown(MouseEventArgs e)
        {
        }

        public void MouseEnter(EventArgs e)
        {

        }

        public void MouseLeave(EventArgs e)
        {

        }

        public void UpdateFrame()
        {
            if (Viewport is Viewport3D)
            {
                var cam = ((Viewport3D) Viewport).Camera;
                var cameraMatrix = Matrix4.LookAt(
                    new Vector3((float)cam.Location.X, (float)cam.Location.Y, (float)cam.Location.Z),
                    new Vector3((float)cam.LookAt.X, (float)cam.LookAt.Y, (float)cam.LookAt.Z),
                    Vector3.UnitZ);

                //cameraMatrix = Matrix4.CreateTranslation(0.5f, 0f, -1f);

                _shaderProgram.Bind();
                _shaderProgram.Set("cameraMatrix", cameraMatrix);
                _shaderProgram.Unbind();
            }
        }

        public void PreRender()
        {

        }

        public void Render3D()
        {

        }

        public void Render2D()
        {

        }
    }

    class ArrayRenderable : IRenderable
    {
        private ShaderProgram _shaderProgram;
        public VertexArrayByte Array { get; private set; }

        public ArrayRenderable(VertexArrayByte array, ShaderProgram shaderProgram)
        {
            _shaderProgram = shaderProgram;
            Array = array;
        }

        public void Render(object sender)
        {
            _shaderProgram.Bind();
            Array.Bind();
            Array.DrawElements();
            Array.Unbind();
            _shaderProgram.Unbind();
        }
    }
}
