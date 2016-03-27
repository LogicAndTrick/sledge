using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.FileSystem;
using Sledge.Providers.Model;
using Sledge.Providers.Texture;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures.Models;
using Sledge.Rendering.Materials;
using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Lights;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Line = Sledge.Rendering.Scenes.Renderables.Line;
using Model = Sledge.Rendering.DataStructures.Models.Model;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Sandbox
{
    public class MainForm2 : Form
    {
        public MainForm2()
        {
            var wp = new WadProvider();
            var packages = wp.CreatePackages(new[] { @"C:\Working\Wads", @"D:\Github\sledge\_Resources\WAD" }, new string[0], new string[0], new[] { "halflife" }).ToList();
            var textures = packages.SelectMany(x => x.Items.Values).ToList();

            // var mdl = new MdlProvider();
            // var model = mdl.LoadMDL(new NativeFile(@"D:\Github\sledge\_Resources\MDL\HL1_10\barney.mdl"), ModelLoadItems.AllStatic | ModelLoadItems.Animations);
            // model.PreprocessModel();

            ClientSize = new Size(600, 600);
            
            // Create engine
            var renderer = new OpenGLRenderer();
            var engine = new Engine(renderer);

            // Get render control/context
            var camera = new PerspectiveCamera { Position = new Vector3(70, 70, 70), LookAt = Vector3.Zero };
            //var camera = new OrthographicCamera(OrthographicCamera.OrthographicType.Side) { Zoom = 32 };
            var viewport = engine.CreateViewport(camera);

            camera.RenderOptions.RenderFaceWireframe = true;

            viewport.Control.Dock = DockStyle.Fill;
            Controls.Add(viewport.Control);

            // Create scene
            var scene = renderer.CreateScene();
            renderer.SetActiveScene(scene);
            //scene.StartUpdate();

            /**
            var scene2 = renderer.CreateScene();

            scene2.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene2.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene2.Add(new Sledge.Rendering.Scenes.Renderables.Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });

            Task.Factory.StartNew(() =>
                                  {
                                      Thread.Sleep(2000);
                                      renderer.SetActiveScene(scene2);
                                      Thread.Sleep(2000);
                                      renderer.SetActiveScene(scene);
                                  });
            //*/

            var light = new AmbientLight(Color.White, new Vector3(1, 2, 3), 0.8f);
            scene.Add(light);

            scene.Add(new Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene.Add(new Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            scene.Add(new Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });

            foreach (var ti in textures)
            {
                renderer.Textures.Create(ti.Name);
            }

            var animat = Material.Animated(7, Enumerable.Range(1, 7).Select(x => "+" + x + "~c2a4_cmp2").ToArray());
            renderer.Materials.Add(animat);
            foreach (var textureFrame in animat.TextureFrames)
            {
                renderer.Textures.Create(textureFrame);
            }

            var s1 = new Sprite(new Vector3(3, 3, 3), animat, 3, 3);
            //scene.Add(s1);

            //{
            //    var meshes = model.GetActiveMeshes().Select(x =>
            //    {
            //        var verts = x.Vertices.Select(v =>
            //        {
            //            var weight = v.BoneWeightings.ToDictionary(w => w.Bone.BoneIndex, w => w.Weight);
            //            return new MeshVertex(v.Location.ToVector3(), v.Normal.ToVector3(), v.TextureU, v.TextureV, weight);
            //        });
            //        var mat = Material.Texture("Model::Test::" + x.SkinRef);
            //        renderer.Materials.Add(mat);
            //        return new Mesh(mat, verts.ToList());
            //    });
            //    var transforms = model.GetTransforms().Select(x =>
            //    {
            //        return new Matrix4(
            //            x[0], x[1], x[2], x[3],
            //            x[4], x[5], x[6], x[7],
            //            x[8], x[9], x[10], x[11],
            //            x[12], x[13], x[14], x[15]
            //            );
            //    });

            //    foreach (var t in model.Textures)
            //    {
            //        renderer.Textures.Create("Model::Test::" + t.Index, t.Image, t.Width, t.Height, TextureFlags.None);
            //    }

            //    var anim = new Animation(15, new List<AnimationFrame> {new AnimationFrame(transforms.ToList())});
            //    var modelObj = new Model(meshes.ToList());
            //    modelObj.Animation = anim;
            //    renderer.Models.Add("Test", modelObj);
            //    var scModel = new Rendering.Scenes.Renderables.Model("Test", Vector3.Zero);
            //    scene.Add(scModel);
            //}

            // elements
            var le = new LineElement(PositionType.Screen, Color.Lime, new[]
            {
                new Position(new Vector3(0, 0, 0)) { Normalised = true },
                new Position(new Vector3(0.1f, 0.1f, 0)) {Normalised = true},
                new Position(new Vector3(50, 0, 0))
            }.ToList());
            scene.Add(le);

            var uimat = Material.Flat(Color.FromArgb(128, Color.DeepSkyBlue));
            renderer.Materials.Add(uimat);

            scene.Add(new FaceElement(PositionType.Screen, uimat, new List<PositionVertex>
            {
                new PositionVertex(new Position(new Vector3(25, 25, 0)), 0, 0),
                new PositionVertex(new Position(new Vector3(150, 25, 0)), 1, 0),
                new PositionVertex(new Position(new Vector3(150, 150, 0)), 1, 1),
                new PositionVertex(new Position(new Vector3(25, 150, 0)), 0, 1),
            }));
            scene.Add(new FaceElement(PositionType.Screen, uimat, new List<PositionVertex>
            {
                new PositionVertex(new Position(new Vector3(50, 50, 0)), 0, 0),
                new PositionVertex(new Position(new Vector3(100, 50, 0)), 1, 0),
                new PositionVertex(new Position(new Vector3(100, 100, 0)), 1, 1),
                new PositionVertex(new Position(new Vector3(50, 100, 0)), 0, 1),
            }));
            scene.Add(new FaceElement(PositionType.Screen, uimat, new List<PositionVertex>
            {
                new PositionVertex(new Position(new Vector3(75, 75, 0)), 0, 0),
                new PositionVertex(new Position(new Vector3(200, 75, 0)), 1, 0),
                new PositionVertex(new Position(new Vector3(200, 200, 0)), 1, 1),
                new PositionVertex(new Position(new Vector3(75, 200, 0)), 0, 1),
            }));

            Task.Factory.StartNew(() =>
            {
                return;
                for (var i = 0; ; i = (i + 1) % 4)
                {
                    Thread.Sleep(5);
                    var random = new Random();
                    lock (scene)
                    {
                        var objects = scene.Objects.ToList();
                        var index = random.Next(0, objects.Count);
                        var face = objects[index] as Face;
                        if (face != null)
                        {
                            if (false && i < 2)
                            {
                                var material = i % 2 == 0
                                    ? Material.Texture(textures[i % textures.Count].Name, false)
                                    : Material.Flat(Color.FromArgb(random.Next(128, 255), random.Next(128, 255), random.Next(128, 255)));
                                face.Material = material;
                            }
                            else
                            {
                                scene.Remove(face);
                                // face.Vertices = face.Vertices.Select(x => new Vertex(x.Position + Coordinate.One, x.TextureU, x.TextureV)).ToList();
                            }
                        }
                    }
                }
            });

            //scene.EndUpdate();

            // Add scene to renderer / add renderer to scene

            var c = this;

            Task.Factory.StartNew(() =>
            {
                using (var ss = wp.GetStreamSource(512, 512, packages))
                {
                    foreach (var ti in textures)
                    {
                        //Thread.Sleep(100);
                        var ti1 = ti;
                        try
                        {
                            c.Invoke((Action) (() =>
                                               {
                                                   var bmp = ss.GetImage(ti1);
                                                   renderer.Textures.Create(ti1.Name, bmp, ti1.Width, ti1.Height, ti1.Flags);
                                                   bmp.Dispose();
                                               }));
                        }
                        catch
                        {
                        }
                    }
                }
            });
        }
    }
}