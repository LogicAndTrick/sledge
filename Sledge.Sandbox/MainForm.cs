using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.Common.Easings;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Editor.Rendering;
using Sledge.Editor.Rendering.Arrays;
using Sledge.Editor.UI;
using Sledge.Graphics;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Sandbox
{
    public partial class MainForm : Form
    {
        private Viewport3D _viewport;

        public MainForm()
        {
            InitializeComponent();
            _viewport = new Viewport3D(Viewport3D.ViewType.Flat, new RenderContext())
            {
                Dock = DockStyle.Fill,
                Camera =
                {
                    Location = new Vector3(-1, -10, -10),
                    LookAt = new Vector3(0, 0, 0),
                    FOV = 90,
                    ClipDistance = 1000
                }
            };
            _viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            GL.ClearColor(Color.Black);
            Controls.Add(_viewport);

            _viewport.RenderContext.Add(new BlahRenderable());
            _viewport.RenderContext.Add(new WidgetLinesRenderable());
            _viewport.Run();
        }
    }

    public struct ObjectVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Color4 Colour;
        public int IsSelected;
    }

    public class BlahRenderable : IRenderable
    {
        private MapObject3DShader _shader;
        private MapObjectArray _array;
        private decimal _progress;
        private decimal _direction;
        private Easing _easing;

        public BlahRenderable()
        {
            _shader = new MapObject3DShader();
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.InOut);
            _progress = 0;
            _direction = 0.02m;

            var b = new BlockBrush();
            var brushes = b.Create(new IDGenerator(), new Box(-Coordinate.One * 3, Coordinate.One * 2), null, 2).ToList();
            var back = b.Create(new IDGenerator(), new Box(new Coordinate(-20, 10, -20), new Coordinate(20, 21, 20)), null, 2);
            _array = new MapObjectArray(brushes.Union(back));
        }

        public void Render(object sender)
        {
            var vp = (Viewport3D) sender;
            _shader.Bind(new Viewport3DRenderOptions
            {
                Camera = vp.GetCameraMatrix(),
                GridSpacing = 64,
                ModelView = vp.GetModelViewMatrix(),
                Shaded = true,
                ShowGrid = false,
                Textured = false,
                Viewport = vp.GetViewportMatrix(),
                Wireframe = false
            });
            _shader.IsTextured = false;
            
            _progress += _direction;
            if (_progress >= 1 || _progress <= -1) _direction = -_direction;
            var xpos = (float) _easing.Evaluate(_progress);
            _shader.SetLight(0, new Vector3(xpos * 10 - 5, -4, 0), new Vector3(0.8f,0.8f,0), new Vector3(1,1,1));

            _array.RenderUntextured(vp.Context, Coordinate.Zero);
            _shader.Unbind();
        }
    }
}
