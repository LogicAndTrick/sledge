using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Providers.Texture;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes.Lights;

namespace Sledge.Sandbox
{
    public class TestTexture : ITexture
    {
        private TextureItem _item;

        public TestTexture(TextureItem item)
        {
            _item = item;
        }

        public void Dispose()
        {
            
        }

        public TextureFlags Flags { get { return _item.Flags; } }
        public string Name { get { return _item.Name; } }
        public int Width { get { return _item.Width; } }
        public int Height { get { return _item.Height; } }
        public void Bind()
        {
            throw new NotImplementedException();
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }
    }
    public class MainForm2 : Form
    {
        public MainForm2()
        {
            var wp = new WadProvider();
            var packages = wp.CreatePackages(new[] { @"F:\Steam\SteamApps\common\Half-Life\valve" }, new string[0], new string[0], new [] { "halflife" }).ToList();
            var textures = packages.SelectMany(x => x.Items.Values).Skip(500).Take(100).ToList();

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

            foreach (var ti in textures)
            {
                renderer.Textures.Create(ti.Name);
            }

            var animat = Material.Animated(7, Enumerable.Range(1, 7).Select(x => "+" + x + "~c2a4_cmp2").ToArray());
            //renderer.Materials.Add(animat);
            
            var r = new Random();
            var b = new BlockBrush();
            Parallel.For(0, 10000, i =>
            {
                var coord = new Coordinate(r.Next(-50, 50), r.Next(-50, 50), r.Next(-50, 50));
                var brushes = b.Create(new IDGenerator(), new Box(coord, coord + Coordinate.One), new TestTexture(textures[i % textures.Count]), 2).ToList();

                var r2 = new Random();
                lock (scene)
                {
                    var material = i % 2 == 0 ? Material.Texture(textures[i % textures.Count].Name) : Material.Flat(Color.FromArgb(r2.Next(128, 255), r2.Next(128, 255), r2.Next(128, 255)));
                    renderer.Materials.Add(material);
                    foreach (var s in brushes.OfType<Solid>().SelectMany(x => x.Faces))
                    {
                        s.FitTextureToPointCloud(new Cloud(s.Vertices.Select(v => v.Location)), 1, 1);
                        var face = new Rendering.Scenes.Renderables.Face(material, s.Vertices.Select(x => new Sledge.Rendering.Scenes.Renderables.Vertex(x.Location, x.TextureU, x.TextureV)).ToList());
                        scene.Add(face);
                    }
                }
            });

            scene.EndUpdate();

            // Add scene to renderer / add renderer to scene

            var c = this;

            Task.Factory.StartNew(() =>
            {
                using (var ss = wp.GetStreamSource(512, 512, packages))
                {
                    foreach (var ti in textures)
                    {
                        Thread.Sleep(100);
                        var ti1 = ti;
                        c.Invoke((Action) (() =>
                        {
                            var bmp = ss.GetImage(ti1);
                            renderer.Textures.Create(ti1.Name, bmp, ti1.Width, ti1.Height, ti1.Flags);
                            bmp.Dispose();
                        }));
                    }
                }
            });
        }
    }
}