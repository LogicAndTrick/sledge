using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Rendering;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Settings;
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

        private static ViewportBase CreateViewport(string setting, Viewport3D.ViewType preferred3D)
        {
            return CreateViewport(setting, true, preferred3D, Viewport2D.ViewDirection.Top);
        }

        private static ViewportBase CreateViewport(string setting, Viewport2D.ViewDirection preferred2D)
        {
            return CreateViewport(setting, false, Viewport3D.ViewType.Textured, preferred2D);
        }

        private static ViewportBase CreateViewport(string setting, bool prefer3D, Viewport3D.ViewType preferred3D, Viewport2D.ViewDirection preferred2D)
        {
            if (setting != null)
            {
                var spl = setting.ToLowerInvariant().Split('.');
                if (spl.Length == 2)
                {
                    switch (spl[0])
                    {
                        case "viewport3d":
                            prefer3D = true;
                            Viewport3D.ViewType vt;
                            if (Enum.TryParse(spl[1], true, out vt)) preferred3D = vt;
                            break;
                        case "viewport2d":
                            prefer3D = false;
                            Viewport2D.ViewDirection vd;
                            if (Enum.TryParse(spl[1], true, out vd)) preferred2D = vd;
                            break;
                    }
                }
            }

            if (prefer3D) return Create3D(preferred3D);
            else return Create2D(preferred2D);
        }

        public static void Init(TableLayoutPanel tlp)
        {
            MainWindowGrid = tlp;

            var tl = CreateViewport(Layout.ViewportTopLeft, Viewport3D.ViewType.Textured);
            var tr = CreateViewport(Layout.ViewportTopRight, Viewport2D.ViewDirection.Top);
            var bl = CreateViewport(Layout.ViewportBottomLeft, Viewport2D.ViewDirection.Front);
            var br = CreateViewport(Layout.ViewportBottomRight, Viewport2D.ViewDirection.Side);

            var one = Create2D(Viewport2D.ViewDirection.Top);
            var two = Create2D(Viewport2D.ViewDirection.Front);
            var three = Create2D(Viewport2D.ViewDirection.Side);
            var four = Create3D(Viewport3D.ViewType.Textured);

            Viewports.AddRange(new ViewportBase[] {one,two,three,four});

            Viewports.Add(tl);
            Viewports.Add(tr);
            Viewports.Add(bl);
            Viewports.Add(br);

            Viewports.ForEach(SubscribeExceptions);

            MainWindowGrid.Controls.Clear();
            //MainWindowGrid.ColumnCount = 2;
            //MainWindowGrid.RowCount = 2;

            MainWindowGrid.Controls.Add(tl);//, 0, 0);
            MainWindowGrid.Controls.Add(tr);//, 1, 0);
            MainWindowGrid.Controls.Add(bl);//, 0, 1);
            MainWindowGrid.Controls.Add(br);//, 1, 1);

            MainWindowGrid.Controls.Add(one);
            MainWindowGrid.Controls.Add(two);
            //MainWindowGrid.Controls.Add(three);
            //MainWindowGrid.Controls.Add(four);

            RunAll();
        }

        private static string SerialiseViewport(Control vp)
        {
            var vp2 = vp as Viewport2D;
            var vp3 = vp as Viewport3D;
            if (vp2 != null) return "Viewport2D." + vp2.Direction;
            if (vp3 != null) return "Viewport3D." + vp3.Type;
            return "";
        }

        public static void SaveLayout()
        {
            Layout.ViewportTopLeft = SerialiseViewport(MainWindowGrid.GetControlFromPosition(0, 0));
            Layout.ViewportTopRight = SerialiseViewport(MainWindowGrid.GetControlFromPosition(1, 0));
            Layout.ViewportBottomLeft = SerialiseViewport(MainWindowGrid.GetControlFromPosition(0, 1));
            Layout.ViewportBottomRight = SerialiseViewport(MainWindowGrid.GetControlFromPosition(1, 1));
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
