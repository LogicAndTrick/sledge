using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.UI
{
    public static class ViewportManager
    {
        private static TableLayoutPanel MainWindowGrid { get; set; }
        public static List<ViewportBase> Viewports { get; private set; }

        static ViewportManager()
        {
            Viewports = new List<ViewportBase>();
        }

        public static void Init(TableLayoutPanel tlp)
        {
            MainWindowGrid = tlp;

            var tl = Create3D();
            var tr = Create2D(Viewport2D.ViewDirection.Top);
            var bl = Create2D(Viewport2D.ViewDirection.Front);
            var br = Create2D(Viewport2D.ViewDirection.Side);

            Viewports.Add(tl);
            Viewports.Add(tr);
            Viewports.Add(bl);
            Viewports.Add(br);

            Viewports.ForEach(SubscribeExceptions);

            MainWindowGrid.Controls.Clear();
            MainWindowGrid.ColumnCount = 2;
            MainWindowGrid.RowCount = 2;

            MainWindowGrid.Controls.Add(tl, 0, 0);
            MainWindowGrid.Controls.Add(tr, 1, 0);
            MainWindowGrid.Controls.Add(bl, 0, 1);
            MainWindowGrid.Controls.Add(br, 1, 1);

            RunAll();
        }

        private static void SubscribeExceptions(ViewportBase vp)
        {
            vp.ListenerException += (sender, ex) => Logging.Logger.ShowException(ex, "Viewport Listener Exception");
            vp.RenderException += (sender, ex) => Logging.Logger.ShowException(ex, "Viewport Render Exception");
        }

        private static void RunAll()
        {
            Viewports.ForEach(v => v.Run());
        }

        public static void ClearContexts()
        {
            Viewports.ForEach(v => v.RenderContext.Clear());
        }

        public static void AddContext3D(IRenderable r)
        {
            Viewports.OfType<Viewport3D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext3D(IRenderable r)
        {
            Viewports.OfType<Viewport3D>().ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r)
        {
            Viewports.OfType<Viewport2D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r)
        {
            Viewports.OfType<Viewport2D>().ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r, Viewport2D.ViewDirection dir)
        {
            Viewports.OfType<Viewport2D>().Where(v => v != null && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r, Viewport2D.ViewDirection dir)
        {
            Viewports.OfType<Viewport2D>().Where(v => v != null && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void RefreshClearColour()
        {
            foreach (var vp in Viewports)
            {
                vp.MakeCurrent();
                GL.ClearColor(Sledge.Settings.Grid.Background);
            }
        }

        private static Viewport3D Create3D()
        {
            var viewport = new Viewport3D
            {
                Dock = DockStyle.Fill,
                Camera =
                {
                    Location = new Vector3(0, 0, 0),
                    LookAt = new Vector3(0, 1, 0),
                    FOV = Sledge.Settings.View.CameraFOV
                }
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            GL.ClearColor(Sledge.Settings.Grid.Background);
            viewport.Listeners.Add(new Camera3DViewportListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static Viewport2D Create2D(Viewport2D.ViewDirection direction)
        {
            var viewport = new Viewport2D(direction)
            {
                Dock = DockStyle.Fill
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL2D();
            GL.ClearColor(Sledge.Settings.Grid.Background);
            viewport.Listeners.Add(new Camera2DViewportListener(viewport));
            viewport.Listeners.Add(new Grid2DEventListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }
    }
}
