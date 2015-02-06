using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes.Lights;

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
            //var camera = new OrthographicCamera() { Zoom = 16 };
            var viewport = engine.CreateViewport(camera);

            viewport.Control.Dock = DockStyle.Fill;
            Controls.Add(viewport.Control);

            // Create scene
            var scene = renderer.Scene;
            scene.StartUpdate();

            var light = new AmbientLight(Color.White, new Coordinate(1, 2, 3), 0.8f);
            scene.Add(light);
            
            var r = new Random();
            var b = new BlockBrush();
            Parallel.For(0, 10000, i =>
            {
                var coord = new Coordinate(r.Next(-50, 50), r.Next(-50, 50), r.Next(-50, 50));
                var brushes = b.Create(new IDGenerator(), new Box(coord, coord + Coordinate.One), null, 2).ToList();

                lock (scene)
                {
                    var material = Material.Flat(Common.Colour.GetRandomColour());
                    foreach (var s in brushes.OfType<Solid>().SelectMany(x => x.Faces))
                    {
                        var face = new Rendering.Scenes.Renderables.Face(material, s.Vertices.Select(x => new Sledge.Rendering.Vertex(x.Location, x.TextureU, x.TextureV)).ToList());
                        scene.Add(face);
                    }
                }
            });

            scene.EndUpdate();

            // Add scene to renderer / add renderer to scene


        }
    }
}