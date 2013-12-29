using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Rendering;
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

            var tl = Create3D(Viewport3D.ViewType.Textured);
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

        public static PointF GetSplitterPosition()
        {
            var w = MainWindowGrid.GetColumnWidths();
            var h = MainWindowGrid.GetRowHeights();
            return new PointF(MainWindowGrid.ColumnStyles[0].Width, MainWindowGrid.RowStyles[0].Height);
        }

        public static void SetSplitterPosition(PointF pos)
        {
            if (pos.X <= 0 || pos.Y <= 0) return;
            MainWindowGrid.ColumnStyles[0].Width = pos.X;
            MainWindowGrid.ColumnStyles[1].Width = 100 - pos.X;
            MainWindowGrid.RowStyles[0].Height = pos.Y;
            MainWindowGrid.RowStyles[1].Height = 100 - pos.Y;
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

        public static void AddContextAll(IRenderable r)
        {
            Viewports.ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContextAll(IRenderable r)
        {
            Viewports.ForEach(v => v.RenderContext.Remove(r));
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
                GL.ClearColor(vp is Viewport3D ? Sledge.Settings.View.ViewportBackground : Sledge.Settings.Grid.Background);
            }
        }

        private static Viewport3D Create3D(Viewport3D.ViewType type)
        {
            var viewport = new Viewport3D(type)
            {
                Dock = DockStyle.Fill,
                Camera =
                {
                    Location = new Vector3(0, 0, 0),
                    LookAt = new Vector3(0, 1, 0),
                    FOV = Sledge.Settings.View.CameraFOV,
                    ClipDistance = Sledge.Settings.View.BackClippingPane
                },
                VSync = false
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            GL.ClearColor(Sledge.Settings.View.ViewportBackground);
            viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new Camera3DViewportListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static Viewport2D Create2D(Viewport2D.ViewDirection direction)
        {
            var viewport = new Viewport2D(direction)
            {
                Dock = DockStyle.Fill,
                VSync = false
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL2D();
            GL.ClearColor(Sledge.Settings.Grid.Background);
            viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new Camera2DViewportListener(viewport));
            viewport.Listeners.Add(new Grid2DEventListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        public static Viewport2D Make2D(ViewportBase viewport, Viewport2D.ViewDirection direction)
        {
            Viewports.Remove(viewport);
            var pos = MainWindowGrid.GetCellPosition(viewport);
            MainWindowGrid.Controls.Remove(viewport);
            viewport.Dispose();

            viewport = Create2D(direction);
            Viewports.Add(viewport);
            SubscribeExceptions(viewport);
            MainWindowGrid.Controls.Add(viewport, pos.Column, pos.Row);
            viewport.Run();
            return (Viewport2D) viewport;
        }

        public static Viewport3D Make3D(ViewportBase viewport, Viewport3D.ViewType type)
        {
            Viewports.Remove(viewport);
            var pos = MainWindowGrid.GetCellPosition(viewport);
            MainWindowGrid.Controls.Remove(viewport);
            viewport.Dispose();

            viewport = Create3D(type);
            Viewports.Add(viewport);
            SubscribeExceptions(viewport);
            MainWindowGrid.Controls.Add(viewport, pos.Column, pos.Row);
            viewport.Run();
            return (Viewport3D) viewport;
        }
    }
}
