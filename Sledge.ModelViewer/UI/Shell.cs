using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Models;
using Sledge.EditorNew.UI.Viewports;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Interfaces.Dialogs;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Models;
using Sledge.Providers.Model;

namespace Sledge.ModelViewer.UI
{
    public class Shell : IMediatorListener
    {
        private static Shell _instance;
        private readonly IShell _shell;
        private TabStrip _tabs;
        private IViewport3D _viewport;

        public static void Bootstrap()
        {
            _instance = new Shell(UIManager.Manager.Shell);
        }

        private Shell(IShell shell)
        {
            ModelProvider.Register(new MdlProvider());
            _shell = shell;
            Build();

            //
        }

        private void Build()
        {
            var vbox = new VerticalBox();

            
            _tabs = new TabStrip();
            _tabs.Tabs.Add(new Tab{Text = "Model 1"});
            _tabs.TabCloseRequested += TabCloseRequested;
            _tabs.TabSelected += TabSelected;
            vbox.Add(_tabs);

            _viewport = new MapViewport(ViewType.Textured);
            _viewport.Camera.Advance(-100);
            _viewport.Update += (sender, frame) =>
            {
                _viewport.Camera.SetRotation(((frame.Milliseconds % 5000) / 5000m) * 2 * (decimal) Math.PI);
            };
            var mr = ModelProvider.CreateModelReference(new NativeFile(@"D:\Github\sledge\_Resources\MDL\HL1_10\barney.mdl"));

            _viewport.RenderContext.Add(new ModelDocument(mr.Model));
            vbox.Add(_viewport, true);
            _viewport.Run();

            _shell.Container.Set(vbox);

            _shell.Title = "Sledge Model Viewer";

            _shell.AddMenu();
            var file = _shell.Menu.AddMenuItem("File");
            var open = file.AddSubMenuItem("Open");
            open.Clicked += FileOpen;
            var exit = file.AddSubMenuItem("Exit");
            exit.Clicked += FileExit;
        }

        private void FileOpen(object sender, EventArgs e)
        {
            using (var d = UIManager.Manager.ConstructDialog<IFileOpenDialog>())
            {
                d.Filter = "Model files (*.mdl)|*.mdl";
                if (d.Prompt())
                {
                    // something?
                }
            }
        }

        private void FileExit(object sender, EventArgs e)
        {
            _shell.Close();
            _shell.Dispose();
        }

        private void TabSelected(object sender, ITab tab)
        {

        }

        private void TabCloseRequested(object sender, ITab tab)
        {

        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }

    public class ModelDocument : IRenderable
    {
        public Model Model { get; set; }
        private int _animation;
        private int _frame;

        public ModelDocument(Model model)
        {
            Model = model;
            _animation = 0;
            _frame = 0;
        }

        public void Render(object sender)
        {
            TextureHelper.EnableTexturing();

            var anim = _animation >= 0 && _animation < Model.Animations.Count ? Model.Animations[_animation] : null;
            if (anim != null) _frame = (_frame + 1) % anim.Frames.Count;

            var transforms = Model.GetTransforms(_animation, _frame);

            GL.Color4(1f, 1f, 1f, 1f);

            foreach (var group in Model.GetActiveMeshes().GroupBy(x => x.SkinRef))
            {
                var texture = Model.Textures[group.Key].TextureObject;
                if (texture != null) texture.Bind();
                foreach (var mesh in group)
                {
                    GL.Begin(PrimitiveType.Triangles);
                    foreach (var v in mesh.Vertices)
                    {
                        var transform = transforms[v.BoneWeightings.First().Bone.BoneIndex];
                        var c = v.Location * transform;
                        if (texture != null)
                        {
                            GL.TexCoord2(v.TextureU, v.TextureV);
                        }
                        GL.Vertex3(c.X, c.Y, c.Z);
                    }
                    GL.End();
                }
                if (texture != null) texture.Unbind();
            }
        }
    }
}
