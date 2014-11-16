using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Mediator;
using Sledge.EditorNew.Logging;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;
using Sledge.Settings;
using Rectangle = System.Drawing.Rectangle;

namespace Sledge.EditorNew.UI.Viewports
{
    public static class ViewportManager
    {
        private static IResizableTable MainWindowGrid { get; set; }
        public static List<IMapViewport> Viewports { get; private set; }
        public static List<ViewportWindow> Windows { get; private set; }

        static ViewportManager()
        {
            Viewports = new List<IMapViewport>();
            Windows = new List<ViewportWindow>();
        }

        private static IMapViewport CreateViewport(string setting, ViewType preferred3D)
        {
            return CreateViewport(setting, true, preferred3D, ViewDirection.Top);
        }

        private static IMapViewport CreateViewport(string setting, ViewDirection preferred2D)
        {
            return CreateViewport(setting, false, ViewType.Textured, preferred2D);
        }

        private static IMapViewport CreateViewport(string setting, bool prefer3D, ViewType preferred3D, ViewDirection preferred2D)
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
                            ViewType vt;
                            if (Enum.TryParse(spl[1], true, out vt)) preferred3D = vt;
                            break;
                        case "viewport2d":
                            prefer3D = false;
                            ViewDirection vd;
                            if (Enum.TryParse(spl[1], true, out vd)) preferred2D = vd;
                            break;
                    }
                }
            }

            if (prefer3D) return Create3D(preferred3D);
            else return Create2D(preferred2D);
        }

        public static void Init(IResizableTable tlp)
        {
            MainWindowGrid = tlp;
            LoadViewports(MainWindowGrid, MainWindowGrid.Configuration);
        }

        //public static void CreateNewWindow()
        //{
        //    CreateViewportWindow(new ResizableTableConfiguration
        //    {
        //        Size = Rectangle.Empty,
        //        WindowID = Windows.Count + 1,
        //        Configuration = TableSplitConfiguration.Default()
        //    });
        //}

        //private static void CreateViewportWindow(ResizableTableConfiguration config)
        //{
        //    var win = new ViewportWindow(config.Configuration);
        //    win.Text += " - Window " + config.WindowID;
        //    LoadViewports(win.IResizableTable, config);
        //    Windows.Add(win);
        //    win.Closed += (s, e) =>
        //    {
        //        Windows.Remove(win);
        //        win.Dispose();
        //    };
        //    win.Show(Editor.Instance);
        //    if (!config.Size.IsEmpty)
        //    {
        //        win.Location = config.Size.Location;
        //        win.Size = config.Size.Size;
        //        win.WindowState = config.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
        //    }
        //}

        private static void LoadViewports(IResizableTable resizableTable, ResizableTableConfiguration config)
        {
            var defaultViewports = new[]
            {
                Tuple.Create(true, ViewType.Textured, ViewDirection.Top),
                Tuple.Create(false, ViewType.Textured, ViewDirection.Top),
                Tuple.Create(false, ViewType.Textured, ViewDirection.Front),
                Tuple.Create(false, ViewType.Textured, ViewDirection.Side)
            };

            var viewports = new List<string>();

            for (int i = 0; i < config.Rectangles.Count; i++)
            {
                var viewport = viewports.Count > i ? viewports[i] : "";
                var def = defaultViewports[(i % defaultViewports.Length)];
                var vp = CreateViewport(viewport, def.Item1, def.Item2, def.Item3);
                Viewports.Add(vp);
                SubscribeExceptions(vp);
                var rec = config.Rectangles[i];
                resizableTable.Insert(rec.X, rec.Y, vp, rec.Width, rec.Height);
                Mediator.Publish(EditorMediator.ViewportCreated, vp);
                vp.Run();
            }
        }

        //private static ResizableTableConfiguration GetDefaultWindowConfiguration()
        //{
        //    return new ResizableTableConfiguration
        //    {
        //        WindowID = 0,
        //        Size = Rectangle.Empty,
        //        Configuration = TableSplitConfiguration.Default(),
        //        Maximised = true
        //    };
        //}

        //private static string SerialiseViewport(Control vp)
        //{
        //    var vp2 = vp as Viewport2D;
        //    var vp3 = vp as Viewport3D;
        //    if (vp2 != null) return "Viewport2D." + vp2.Direction;
        //    if (vp3 != null) return "Viewport3D." + vp3.Type;
        //    return "";
        //}

        //public static void SaveLayout()
        //{
        //    SettingsManager.SetAdditionalData("ViewportManagerWindowConfiguration", GetWindowConfigurations());
        //}

        //public static List<ResizableTableConfiguration> GetWindowConfigurations()
        //{
        //    var list = new List<ResizableTableConfiguration>();
        //    list.Add(new ResizableTableConfiguration
        //    {
        //        WindowID = 0,
        //        Size = new Rectangle(Editor.Instance.Location, Editor.Instance.Size),
        //        Configuration = MainWindowGrid.Configuration,
        //        Viewports = GetViewportsForIResizableTable(MainWindowGrid).Select(SerialiseViewport).ToList(),
        //        Maximised = Editor.Instance.WindowState != FormWindowState.Normal
        //    });
        //    for (var i = 0; i < Windows.Count; i++)
        //    {
        //        list.Add(new ResizableTableConfiguration
        //        {
        //            WindowID = i + 1,
        //            Size = new Rectangle(Windows[i].Location, Windows[i].Size),
        //            Configuration = Windows[i].IResizableTable.Configuration,
        //            Viewports = GetViewportsForIResizableTable(Windows[i].IResizableTable).Select(SerialiseViewport).ToList(),
        //            Maximised = Windows[i].WindowState != FormWindowState.Normal
        //        });
        //    }
        //    return list;
        //}

        //private static void SetConfiguration(IResizableTable control, ResizableTableConfiguration configuration)
        //{
        //    if (control == null || configuration == null) return;

        //    control.Configuration = configuration.Configuration;
        //    var viewports = GetViewportsForIResizableTable(control);

        //    control.Controls.Clear();
        //    foreach (var vp in viewports)
        //    {
        //        vp.Dispose();
        //        Viewports.Remove(vp);
        //    }

        //    LoadViewports(control, configuration);
        //}

        //public static void SetWindowConfigurations(List<ResizableTableConfiguration> configurations)
        //{
        //    SetConfiguration(MainWindowGrid, configurations.FirstOrDefault(x => x.WindowID == 0));
        //    for (var i = 0; i < Windows.Count; i++)
        //    {
        //        var win = Windows[i];
        //        SetConfiguration(win.IResizableTable, configurations.FirstOrDefault(x => x.WindowID == i + 1));
        //    }
        //}

        private static IEnumerable<IMapViewport> GetViewportsForIResizableTable(IResizableTable control)
        {
            return Viewports.Where(x => GetParentSplitControl(x) == control);
        }

        //public static PointF GetSplitterPosition()
        //{
        //    var w = MainWindowGrid.GetColumnWidths();
        //    var h = MainWindowGrid.GetRowHeights();
        //    return new PointF(MainWindowGrid.ColumnStyles[0].Width, MainWindowGrid.RowStyles[0].Height);
        //}

        //public static void SetSplitterPosition(PointF pos)
        //{
        //    if (pos.X <= 0 || pos.Y <= 0) return;
        //    MainWindowGrid.ColumnStyles[0].Width = pos.X;
        //    MainWindowGrid.ColumnStyles[1].Width = 100 - pos.X;
        //    MainWindowGrid.RowStyles[0].Height = pos.Y;
        //    MainWindowGrid.RowStyles[1].Height = 100 - pos.Y;
        //}

        private static void SubscribeExceptions(IMapViewport vp)
        {
            vp.ListenerException += (sender, ex) => Logger.ShowException(ex, "Viewport Listener Exception");
            vp.RenderException += (sender, ex) => Logger.ShowException(ex, "Viewport Render Exception");
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
            Viewports.OfType<IViewport3D>().Where(x => x.Is3D).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext3D(IRenderable r)
        {
            Viewports.OfType<IViewport3D>().Where(x => x.Is3D).ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r)
        {
            Viewports.OfType<IViewport2D>().Where(x => x.Is2D).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r)
        {
            Viewports.OfType<IViewport2D>().Where(x => x.Is2D).ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r, ViewDirection dir)
        {
            Viewports.OfType<IViewport2D>().Where(v => v.Is2D && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r, ViewDirection dir)
        {
            Viewports.OfType<IViewport2D>().Where(v => v.Is2D && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void RefreshClearColour()
        {
            foreach (var vp in Viewports)
            {
                vp.MakeCurrent();
                GL.ClearColor(vp.Is3D ? View.ViewportBackground : Grid.Background);
            }
        }

        private static IViewport3D Create3D(ViewType type)
        {
            var viewport = new MapViewport(type)
            {
                Camera =
                {
                    Location = new Vector3(0, 0, 0),
                    LookAt = new Vector3(0, 1, 0),
                    FOV = View.CameraFOV,
                    ClipDistance = View.BackClippingPane
                }
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            GL.ClearColor(View.ViewportBackground);
            //viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new CameraViewportListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static IViewport2D Create2D(ViewDirection direction)
        {
            var viewport = new MapViewport(direction);
            viewport.MakeCurrent();
            GraphicsHelper.InitGL2D();
            GL.ClearColor(Grid.Background);
            //viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new CameraViewportListener(viewport));
            //viewport.Listeners.Add(new Grid2DEventListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static IResizableTable GetParentSplitControl(IMapViewport viewport)
        {
            var par = viewport.Parent;
            while (par != null && !(par is IResizableTable))
            {
                par = par.Parent;
            }
            return par as IResizableTable;
        }

        //public static Viewport2D Make2D(IMapViewport viewport, Viewport2D.ViewDirection direction)
        //{
        //    var parent = GetParentSplitControl(viewport);
        //    if (parent == null) return null;

        //    Viewports.Remove(viewport);
            
        //    var newViewport = Create2D(direction);
        //    SubscribeExceptions(newViewport);

        //    parent.ReplaceControl(viewport, newViewport);

        //    Viewports.Add(newViewport);

        //    viewport.Dispose();
        //    newViewport.Run();

        //    return newViewport;
        //}

        //public static Viewport3D Make3D(IMapViewport viewport, Viewport3D.ViewType type)
        //{
        //    var parent = GetParentSplitControl(viewport);
        //    if (parent == null) return null;

        //    Viewports.Remove(viewport);

        //    var newViewport = Create3D(type);
        //    SubscribeExceptions(newViewport);

        //    parent.ReplaceControl(viewport, newViewport);

        //    Viewports.Add(newViewport);

        //    viewport.Dispose();
        //    newViewport.Run();

        //    return newViewport;
        //}

        //public static Image CreateScreenshot(IMapViewport viewport, int width, int height)
        //{
        //    var shot = new ScreenshotViewportListener(viewport);
        //    var parent = GetParentSplitControl(viewport);
        //    if (parent == null) return null;

        //    var pos = parent.GetPositionFromControl(viewport);
        //    var form = new Form();
        //    //form.FormBorderStyle = FormBorderStyle.None;
        //    form.TopMost = true;
        //    form.Width = width;
        //    form.Height = height;
        //    form.WindowState = FormWindowState.Maximized;
        //    var panel = new Panel {Width = width, Height = height};
        //    form.Controls.Add(panel);
        //    panel.Controls.Add(viewport);

        //    viewport.Dock = DockStyle.Top | DockStyle.Left;
        //    viewport.Width = width;
        //    viewport.Height = height;

        //    //form.ShowDialog();
        //    viewport.Listeners.Add(shot);
        //    viewport.UpdateNextFrameImmediately();
        //    viewport.Listeners.Remove(shot);

        //    viewport.Dock = DockStyle.Fill;
        //    parent.Controls.Add(viewport, pos.Column, pos.Row);
        //    return shot.Screenshot;
        //}
    }
}
