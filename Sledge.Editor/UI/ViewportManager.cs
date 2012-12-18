using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Editor.Rendering;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.UI;
using System;

namespace Sledge.Editor.UI
{
    public static class ViewportManager
    {
        public static TableLayoutPanel MainWindowGrid { get; set; }
        public static List<ViewportBase> Viewports { get; set; }

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

            MainWindowGrid.Controls.Clear();
            MainWindowGrid.ColumnCount = 2;
            MainWindowGrid.RowCount = 2;

            MainWindowGrid.Controls.Add(tl, 0, 0);
            MainWindowGrid.Controls.Add(tr, 1, 0);
            MainWindowGrid.Controls.Add(bl, 0, 1);
            MainWindowGrid.Controls.Add(br, 1, 1);

            RunAll();
        }

        public static void RunAll()
        {
            Viewports.ForEach(v => v.Run());
        }

        public static void ClearContexts()
        {
            Viewports.ForEach(v => v.RenderContext.Clear());
        }

        public static void AddGrids()
        {
            Viewports.OfType<Viewport2D>().ToList()
                .ForEach(v => v.RenderContext.Add(new GridRenderable("2DGridRenderable_" + v.GetHashCode())));
        }

        public static void AddContext3D(IRenderable r)
        {
            Viewports.OfType<Viewport3D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void AddContext2D(IRenderable r)
        {
            Viewports.OfType<Viewport2D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void AddContext2D(IRenderable r, Viewport2D.ViewDirection dir)
        {
            Viewports.OfType<Viewport2D>().Where(v => v != null && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static Viewport3D Create3D()
        {
            var viewport = new Viewport3D
            {
                Dock = DockStyle.Fill,
                Camera =
                {
                    Location = new Vector3d(0, 0, 0),
                    LookAt = new Vector3d(0, 1, 0)
                }
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            viewport.Listeners.Add(new Camera3DViewportListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        public static Viewport2D Create2D(Viewport2D.ViewDirection direction)
        {
            var viewport = new Viewport2D(direction)
            {
                Dock = DockStyle.Fill
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL2D();
            viewport.Listeners.Add(new Camera2DViewportListener(viewport));
            viewport.Listeners.Add(new Grid2DEventListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }
    }
}
