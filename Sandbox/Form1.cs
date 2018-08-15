using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Buffer = Sledge.Rendering.Renderables.Buffer;

namespace Sandbox
{
    public partial class Form1 : Form, IUpdateable
    {
        private List<Buffer> _buffers;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var viewport = Engine.Instance.CreateViewport();

            viewport.Control.Dock = DockStyle.Fill;
            Controls.Add(viewport.Control);

            if (viewport.Camera is PerspectiveCamera pc)
            {
                pc.Location = new Vector3(-10, -20, 15);
                pc.Direction = Vector3.Zero - pc.Location;
            }
            
            var _engine = new EngineInterface();

            _buffers = Enumerable.Range(0, 10000)
                .Select(x => _engine.CreateBuffer())
                .ToList();

            Update(0);

            Engine.Instance.Scene.Add(this);
            Engine.Instance.Scene.Add(new FpsMonitor());

            foreach (var b in _buffers)
            {
                Engine.Instance.Scene.Add(new SimpleRenderable(b, new [] { PipelineType.WireframeGeneric }, 0, b.IndexCount));
            }
        }

        private long _lastFrame = -10000;
        public void Update(long frame)
        {
            //if (frame - _lastFrame <= 1000) return;

            _lastFrame = frame;

            var points = new[]
            {
                // X axis - red
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitX + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitX * 10, Colour = Vector4.UnitX + Vector4.UnitW },

                // Y axis - green
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitY + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitY * 10, Colour = Vector4.UnitY + Vector4.UnitW },

                // Z axis - blue
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitZ + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitZ * 10, Colour = Vector4.UnitZ + Vector4.UnitW },
            };

            var indices = new uint[] { 0, 1, 2, 3, 4, 5 };
            var r = new Random();
            var randR = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
            var randG = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
            var randB = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);

            var offs = new Vector3(0.01f, 0, 0);
            foreach (var b in _buffers)
            {
                points[0].Position += offs;
                points[0].Colour = randR;
                points[1].Position += offs;
                points[1].Colour = randR;

                points[2].Position += offs;
                points[2].Colour = randG;
                points[3].Position += offs;
                points[3].Colour = randG;

                points[4].Position += offs;
                points[4].Colour = randB;
                points[5].Position += offs;
                points[5].Colour = randB;

                //offs.X += 0.01f;

                b.Update(points, indices);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var vp = Controls[0];
            Controls.Remove(vp);
        }
    }
}
