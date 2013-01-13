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

smooth in vec4 thefColor;
smooth in float outfThing;

out vec4 outputColor;
void main()
{
	outputColor = thefColor * (1.0-outfThing) + vec4(1,0,0,1) * outfThing;
}
";

        #region Verts
        private float[] _vertexData = new[]
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
        private int _program;
        private int _offsetUniform;
        private int _perspectiveUniform;
        private int _cameraUniform;
        private int _modelViewUniform;

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

        private int _buffer;
        private int _vao;

        private VertexArray _array;

        protected override void OnLoad(EventArgs e)
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new MapFormatProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());


            // Setup program
            _program = MakeProgram();
            _offsetUniform = GL.GetUniformLocation(_program, "offset");
            _perspectiveUniform = GL.GetUniformLocation(_program, "perspectiveMatrix");
            _cameraUniform = GL.GetUniformLocation(_program, "cameraMatrix");
            _modelViewUniform = GL.GetUniformLocation(_program, "modelViewMatrix");

            var ratio = _viewport.Width / (float)_viewport.Height;
            if (ratio <= 0) ratio = 1;
            var matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), ratio, 0.1f, 10000f);

            var cam = _viewport.Camera;
            var cameraMatrix = Matrix4.LookAt(
                new Vector3((float)cam.Location.X, (float)cam.Location.Y, (float)cam.Location.Z),
                new Vector3((float)cam.LookAt.X, (float)cam.LookAt.Y, (float)cam.LookAt.Z), 
                Vector3.UnitZ);

            //cameraMatrix = Matrix4.CreateTranslation(0.5f, 0f, -1f);

            var mv = Matrix4.Scale(100);

            GL.UseProgram(_program);
            GL.UniformMatrix4(_perspectiveUniform, false, ref matrix);
            GL.UniformMatrix4(_cameraUniform, false, ref cameraMatrix);
            GL.UniformMatrix4(_modelViewUniform, false, ref mv);
            GL.UseProgram(0);



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


            _viewport.Listeners.Add(new UpdateCameraListener(_program, _cameraUniform, _viewport));

            ms = new MemoryStream(1024);
            bw = new BinaryWriter(ms);
            for (int i = 0; i < _vertexData.Length; i++)
            {
                var f = _vertexData[i];
                bw.Write(f);
                if (i%4==3) bw.Write((float) rand.NextDouble());
            }
            var data = ms.ToArray();
            bw.Dispose();
            ms.Dispose();

            // Setup VBO
            GL.GenBuffers(1, out _buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(byte) * data.Length), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            GL.GenVertexArrays(1, out _vao);
            GL.BindVertexArray(_vao);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            _viewport.Run();

            //_viewport.RenderContext.Add(new Rawr(_program, _offsetUniform, (sizeof(byte) * data.Length) / 2, _buffer));

            _viewport.RenderContext.Add(new ArrayRenderable(_array, _program, _offsetUniform));

            base.OnLoad(e);
        }

        private int MakeProgram()
        {
            var shaders = new List<int>
                              {
                                  CreateShader(ShaderType.VertexShader, VertexShader),
                                  CreateShader(ShaderType.GeometryShader, GeometryShader),
                                  CreateShader(ShaderType.FragmentShader, FragmentShader)
                              };
            return CreateProgram(shaders);
        }

        int CreateProgram(List<int> shaders)
        {
            var program = GL.CreateProgram();
            shaders.ForEach(x => GL.AttachShader(program, x));
            GL.LinkProgram(program);
            return program;
    
            // Program error logging
            //GLint status;
            //glGetProgramiv (program, GL_LINK_STATUS, &status);
            //if (status == GL_FALSE)
            //{
            //    GLint infoLogLength;
            //    glGetProgramiv(program, GL_INFO_LOG_LENGTH, &infoLogLength);
            //
            //    GLchar *strInfoLog = new GLchar[infoLogLength + 1];
            //    glGetProgramInfoLog(program, infoLogLength, NULL, strInfoLog);
            //    fprintf(stderr, "Linker failure: %s\n", strInfoLog);
            //    delete[] strInfoLog;
            //}
            //
            //for(size_t iLoop = 0; iLoop < shaderList.size(); iLoop++)
            //    glDetachShader(program, shaderList[iLoop]);
            //
        }

        int CreateShader(ShaderType shaderType, string shaderCode)
        {
            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, shaderCode);
            GL.CompileShader(shader);
    
            // Shader error logging
            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                var err = GL.GetShaderInfoLog(shader);
                Console.WriteLine(err);
            }

            return shader;
        }
    }

    class UpdateCameraListener : IViewportEventListener
    {
        private int _program;
        private int _cameraUniform;

        public UpdateCameraListener(int program, int cameraUniform, Viewport3D vp)
        {
            _program = program;
            _cameraUniform = cameraUniform;
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

                GL.UseProgram(_program);
                GL.UniformMatrix4(_cameraUniform, false, ref cameraMatrix);
                GL.UseProgram(0);
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
        private int _program;
        private int _offset;
        public VertexArray Array { get; private set; }

        public ArrayRenderable(VertexArray array, int program, int offset)
        {
            Array = array;
            _program = program;
            _offset = offset;
        }

        public void Render(object sender)
        {
            GL.UseProgram(_program);
            GL.Uniform3(_offset, 0.5f, 0.5f, 0);
            Array.DrawElements();
            GL.UseProgram(0);
        }
    }

    class Rawr : IRenderable
    {
        private int _program;
        private int _offset;
        private int _colourOffset;
        private int _buffer;

        public Rawr(int program, int offset, int colourOffset, int buffer)
        {
            _program = program;
            _offset = offset;
            _colourOffset = colourOffset;
            _buffer = buffer;
        }

        public void Render(object sender)
        {
            GL.UseProgram(_program);
            GL.Uniform3(_offset, 0.5f, 0.5f, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 20, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 20, _colourOffset);

            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 20, 16);

            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.UseProgram(0);
        }
    }
}
