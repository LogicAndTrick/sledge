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
using Sledge.Editor.UI;
using Sledge.Graphics;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Providers.Texture;
using Sledge.UI;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace Sledge.ShaderTest
{
    public partial class Form1 : Form
    {
        #region Single Pass Wireframe test
        /*
        public const string VertexShader = @"#version 330

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;

smooth out vec4 theColor;

uniform vec3 offset;
uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * modelViewMatrix * position;// + vec4(offset.x, offset.y, offset.z, 0.0);
	gl_Position = perspectiveMatrix * cameraPos;
	theColor = color;
}
";

        public const string GeometryShader = @"#version 330
#extension GL_EXT_gpu_shader4 : enable
 
layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

smooth in vec4 theColor[];

smooth out vec4 thefColor;

noperspective out vec3 dist;
void main()
{
    vec2 p0 = vec2(400, 400) * gl_in[0].gl_Position.xy / gl_in[0].gl_Position.w;
    vec2 p1 = vec2(400, 400) * gl_in[1].gl_Position.xy / gl_in[1].gl_Position.w;
    vec2 p2 = vec2(400, 400) * gl_in[2].gl_Position.xy / gl_in[2].gl_Position.w;
    
    vec2 v0 = p2 - p1;
    vec2 v1 = p2 - p0;
    vec2 v2 = p1 - p0;
    float area = abs(v1.x*v2.y - v1.y * v2.x);

thefColor = theColor[0];
    dist = vec3(area/length(v0),0,0);
    gl_Position = gl_in[0].gl_Position;
    EmitVertex();
    //
thefColor = theColor[1];
    dist = vec3(0,area/length(v1),0);
    gl_Position = gl_in[1].gl_Position;
    EmitVertex();
    
thefColor = theColor[2];
    dist = vec3(0,0,area/length(v2));
    gl_Position = gl_in[2].gl_Position;
    EmitVertex();
    
    EndPrimitive();

//for(int i = 0; i < gl_in.length(); i++) {
//    gl_Position = gl_in[i].gl_Position;
//    EmitVertex();
//  }
//  EndPrimitive();
}
";

        public const string FragmentShader = @"#version 330

smooth in vec4 thefColor;

out vec4 outputColor;

noperspective in vec3 dist;
const vec4 WIRE_COL = vec4(1,0,0, 1);
const vec4 FILL_COL = vec4(1,1,1, 1);

void main()
{
    float d = min(dist.x, min(dist.y, dist.z));
//if (d > 2) { discard; }
 	float I = sin(exp2( -2 * d * d ));
 	outputColor = I * WIRE_COL + (1.0 - I) * thefColor;
	//outputColor = theColor;

    //d = clamp(d - (1 - 1.0), 0.0, 2.0);
    //outputColor = vec4(WIRE_COL, exp2(-2.0 * d * d));
//float I = exp2( -2 * d * d );
//outputColor = I * WIRE_COL + (1.0 - I) * FILL_COL;
//outputColor = vec4(WIRE_COL.xyz, exp2(-2.0 * d * d));
}
";
         */
        #endregion

        public const string VertexShader = @"#version 330

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in float thing;

smooth out vec4 theColor;
smooth out float outThing;

uniform vec3 offset;
uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * modelViewMatrix * position;// + vec4(offset.x, offset.y, offset.z, 0.0);
	gl_Position = perspectiveMatrix * cameraPos;
	theColor = color;
    outThing = thing;
}
";

        public const string GeometryShader = @"#version 330
#extension GL_EXT_gpu_shader4 : enable
 
layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

smooth in vec4 theColor[];
smooth in float outThing[];
smooth out vec4 thefColor;
smooth out float outfThing;

void main()
{
    for(int i = 0; i < gl_in.length(); i++) {
        thefColor = theColor[i];
        outfThing = outThing[i];
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }
    EndPrimitive();
}
";

        public const string FragmentShader = @"#version 330

uniform bool isSelected;

smooth in vec4 thefColor;
smooth in float outfThing;

out vec4 outputColor;
void main()
{
	outputColor = thefColor * (1.0-outfThing) + vec4(1,0,0,1) * outfThing;
    if (isSelected) {
        outputColor = thefColor * 0.2 + vec4(0.8,0,0,1);
    } else {
        outputColor = thefColor;
    }
}
";

        #region Verts
        public static float[] _vertexData = new[]
                                          {
                                              0.25f, 0.25f, -1.25f, 1.0f,
                                              0.25f, -0.25f, -1.25f, 1.0f,
                                              -0.25f, 0.25f, -1.25f, 1.0f,

                                              0.25f, -0.25f, -1.25f, 1.0f,
                                              -0.25f, -0.25f, -1.25f, 1.0f,
                                              -0.25f, 0.25f, -1.25f, 1.0f,

                                              0.25f, 0.25f, -2.75f, 1.0f,
                                              -0.25f, 0.25f, -2.75f, 1.0f,
                                              0.25f, -0.25f, -2.75f, 1.0f,

                                              0.25f, -0.25f, -2.75f, 1.0f,
                                              -0.25f, 0.25f, -2.75f, 1.0f,
                                              -0.25f, -0.25f, -2.75f, 1.0f,

                                              -0.25f, 0.25f, -1.25f, 1.0f,
                                              -0.25f, -0.25f, -1.25f, 1.0f,
                                              -0.25f, -0.25f, -2.75f, 1.0f,

                                              -0.25f, 0.25f, -1.25f, 1.0f,
                                              -0.25f, -0.25f, -2.75f, 1.0f,
                                              -0.25f, 0.25f, -2.75f, 1.0f,

                                              0.25f, 0.25f, -1.25f, 1.0f,
                                              0.25f, -0.25f, -2.75f, 1.0f,
                                              0.25f, -0.25f, -1.25f, 1.0f,

                                              0.25f, 0.25f, -1.25f, 1.0f,
                                              0.25f, 0.25f, -2.75f, 1.0f,
                                              0.25f, -0.25f, -2.75f, 1.0f,

                                              0.25f, 0.25f, -2.75f, 1.0f,
                                              0.25f, 0.25f, -1.25f, 1.0f,
                                              -0.25f, 0.25f, -1.25f, 1.0f,

                                              0.25f, 0.25f, -2.75f, 1.0f,
                                              -0.25f, 0.25f, -1.25f, 1.0f,
                                              -0.25f, 0.25f, -2.75f, 1.0f,

                                              0.25f, -0.25f, -2.75f, 1.0f,
                                              -0.25f, -0.25f, -1.25f, 1.0f,
                                              0.25f, -0.25f, -1.25f, 1.0f,

                                              0.25f, -0.25f, -2.75f, 1.0f,
                                              -0.25f, -0.25f, -2.75f, 1.0f,
                                              -0.25f, -0.25f, -1.25f, 1.0f,




                                              0.0f, 0.0f, 1.0f, 1.0f,
                                              0.0f, 0.0f, 1.0f, 1.0f,
                                              0.0f, 0.0f, 1.0f, 1.0f,

                                              0.0f, 0.0f, 1.0f, 1.0f,
                                              0.0f, 0.0f, 1.0f, 1.0f,
                                              0.0f, 0.0f, 1.0f, 1.0f,

                                              0.8f, 0.8f, 0.8f, 1.0f,
                                              0.8f, 0.8f, 0.8f, 1.0f,
                                              0.8f, 0.8f, 0.8f, 1.0f,

                                              0.8f, 0.8f, 0.8f, 1.0f,
                                              0.8f, 0.8f, 0.8f, 1.0f,
                                              0.8f, 0.8f, 0.8f, 1.0f,

                                              0.0f, 1.0f, 0.0f, 1.0f,
                                              0.0f, 1.0f, 0.0f, 1.0f,
                                              0.0f, 1.0f, 0.0f, 1.0f,

                                              0.0f, 1.0f, 0.0f, 1.0f,
                                              0.0f, 1.0f, 0.0f, 1.0f,
                                              0.0f, 1.0f, 0.0f, 1.0f,

                                              0.5f, 0.5f, 0.0f, 1.0f,
                                              0.5f, 0.5f, 0.0f, 1.0f,
                                              0.5f, 0.5f, 0.0f, 1.0f,

                                              0.5f, 0.5f, 0.0f, 1.0f,
                                              0.5f, 0.5f, 0.0f, 1.0f,
                                              0.5f, 0.5f, 0.0f, 1.0f,

                                              1.0f, 0.0f, 0.0f, 1.0f,
                                              1.0f, 0.0f, 0.0f, 1.0f,
                                              1.0f, 0.0f, 0.0f, 1.0f,

                                              1.0f, 0.0f, 0.0f, 1.0f,
                                              1.0f, 0.0f, 0.0f, 1.0f,
                                              1.0f, 0.0f, 0.0f, 1.0f,

                                              0.0f, 1.0f, 1.0f, 1.0f,
                                              0.0f, 1.0f, 1.0f, 1.0f,
                                              0.0f, 1.0f, 1.0f, 1.0f,

                                              0.0f, 1.0f, 1.0f, 1.0f,
                                              0.0f, 1.0f, 1.0f, 1.0f,
                                              0.0f, 1.0f, 1.0f, 1.0f,

                                          };
        #endregion

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
        private VertexArray _array;

        protected override void OnLoad(EventArgs e)
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());

            _shaderProgram = new ShaderProgram(new Shader(ShaderType.VertexShader, VertexShader),
                                               new Shader(ShaderType.GeometryShader, GeometryShader),
                                               new Shader(ShaderType.FragmentShader, FragmentShader));

            var ratio = _viewport.Width / (float)_viewport.Height;
            if (ratio <= 0) ratio = 1;
            var matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), ratio, 0.1f, 10000f);

            var cam = _viewport.Camera;
            var cameraMatrix = Matrix4.LookAt(
                new Vector3((float)cam.Location.X, (float)cam.Location.Y, (float)cam.Location.Z),
                new Vector3((float)cam.LookAt.X, (float)cam.LookAt.Y, (float)cam.LookAt.Z), 
                Vector3.UnitZ);

            var mv = Matrix4.Scale(100);

            _shaderProgram.Bind();
            _shaderProgram.Set("perspectiveMatrix", matrix);
            _shaderProgram.Set("cameraMatrix", cameraMatrix);
            _shaderProgram.Set("modelViewMatrix", mv);
            _shaderProgram.Set("isSelected", true);
            _shaderProgram.Unbind();



            var spec = new ArraySpecification(ArrayIndex.Vector4("Vertices"),
                                              ArrayIndex.Vector4("Colours"),
                                              ArrayIndex.Float("Thing"));
            var colourOffset = _vertexData.Length / 2;

            var ms = new MemoryStream(1024);
            var bw = new BinaryWriter(ms);
            var rand = new Random();
            var count = (short)0;
            var indices = new List<short>();
            for (var i = 0; i < colourOffset; i+=4)
            {
                bw.Write(_vertexData[i + 0]);
                bw.Write(_vertexData[i + 1]);
                bw.Write(_vertexData[i + 2]);
                bw.Write(_vertexData[i + 3]);
                bw.Write(_vertexData[i + colourOffset + 0]);
                bw.Write(_vertexData[i + colourOffset + 1]);
                bw.Write(_vertexData[i + colourOffset + 2]);
                bw.Write(_vertexData[i + colourOffset + 3]);
                bw.Write((float)rand.NextDouble());
                indices.Add(count);
                count++;
            }
            var data2 = ms.ToArray();
            bw.Dispose();
            ms.Dispose();

            _array = new VertexArray(spec, BeginMode.Triangles, count, data2, indices.ToArray());


            _viewport.Listeners.Add(new UpdateCameraListener(_array, _shaderProgram, _viewport));


            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            _viewport.Run();

            //_viewport.RenderContext.Add(new Rawr(_program, _offsetUniform, (sizeof(byte) * data.Length) / 2, _buffer));

            _viewport.RenderContext.Add(new ArrayRenderable(_array, _shaderProgram));

            base.OnLoad(e);
        }
    }

    class UpdateCameraListener : IViewportEventListener
    {
        private VertexArray _array;
        private ShaderProgram _shaderProgram;

        public UpdateCameraListener(VertexArray array, ShaderProgram shaderProgram, Viewport3D vp)
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
            var _vertexData = Form1._vertexData;
            var colourOffset = _vertexData.Length / 2;
            var ms = new MemoryStream(1024);
            var bw = new BinaryWriter(ms);
            var rand = new Random();
            var count = (short)0;
            var indices = new List<short>();
            for (var i = 0; i < colourOffset; i += 4)
            {
                bw.Write(_vertexData[i + 0]);
                bw.Write(_vertexData[i + 1]);
                bw.Write(_vertexData[i + 2]);
                bw.Write(_vertexData[i + 3]);
                bw.Write(_vertexData[i + colourOffset + 0]);
                bw.Write(_vertexData[i + colourOffset + 1]);
                bw.Write(_vertexData[i + colourOffset + 2]);
                bw.Write(_vertexData[i + colourOffset + 3]);
                bw.Write((float)rand.NextDouble());
                indices.Add(count);
                count++;
            }
            var data2 = ms.ToArray();
            bw.Dispose();
            ms.Dispose();

            _array.Update(count, data2, indices.ToArray());
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
        public VertexArray Array { get; private set; }

        public ArrayRenderable(VertexArray array, ShaderProgram shaderProgram)
        {
            _shaderProgram = shaderProgram;
            Array = array;
        }

        public void Render(object sender)
        {
            _shaderProgram.Bind();
            Array.DrawElements();
            _shaderProgram.Unbind();
        }
    }
}
