using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Rendering;
using Sledge.Rendering.OpenGL;

namespace Sledge.Sandbox
{
    public class MainForm2 : Form
    {
        public MainForm2()
        {
            ClientSize = new Size(600, 600);

            // Create engine
            var renderer = new OpenGLRenderer();
            var engine = new Engine(renderer);

            // Get render control/context
            var camera = new PerspectiveCamera { Position = new Coordinate(-10, -10, -10), LookAt = Coordinate.Zero };
            //var camera = new OrthographicCamera();
            var viewport = engine.CreateViewport(camera);

            viewport.Control.Dock = DockStyle.Fill;
            this.Controls.Add(viewport.Control);

            // Create scene
            var scene = engine.Scene;

            var light = new AmbientLight(Color.White, new Coordinate(1, 2, 3), 0.8f);
            scene.Add(light);

            var material = Material.Flat(Color.LightGreen);

            var b = new BlockBrush();
            var brushes = b.Create(new IDGenerator(), new Box(-Coordinate.One * 3, Coordinate.One * 2), null, 2).ToList();
            foreach (var s in brushes.OfType<Solid>().SelectMany(x => x.Faces))
            {
                var face = new Rendering.Face(material, s.Vertices.Select(x => x.Location).ToList());
                scene.Add(face);
            }

            // Add scene to renderer / add renderer to scene


        }
    }
}