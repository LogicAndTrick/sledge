using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Rendering;
using Sledge.Editor.UI.Layout;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Settings;

namespace Sledge.Editor.UI
{
    public static class ViewportManager
    {
        private static TableSplitControl MainWindowGrid { get; set; }
        public static List<MapViewport> Viewports { get; private set; }
        public static List<ViewportWindow> Windows { get; private set; }

        static ViewportManager()
        {
            Viewports = new List<MapViewport>();
            Windows = new List<ViewportWindow>();
        }

        public static void Init(TableSplitControl tlp)
        {
            MainWindowGrid = tlp;

            var configuration = SettingsManager.GetAdditionalData<List<ViewportWindowConfiguration>>("ViewportManagerWindowConfiguration")
                                ?? new List<ViewportWindowConfiguration>();
            var main = configuration.FirstOrDefault(x => x.WindowID == 0) ?? GetDefaultWindowConfiguration();
            MainWindowGrid.Configuration = main.Configuration;
            LoadViewports(MainWindowGrid, main);
            if (!main.Size.IsEmpty)
            {
                Editor.Instance.Location = main.Size.Location;
                Editor.Instance.Size = main.Size.Size;
                Editor.Instance.WindowState = main.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            foreach (var config in configuration.Where(x => x.WindowID > 0))
            {
                CreateViewportWindow(config);
            }
        }

        public static void CreateNewWindow()
        {
            CreateViewportWindow(new ViewportWindowConfiguration
            {
                Size = Rectangle.Empty,
                WindowID = Windows.Count + 1,
                Configuration = TableSplitConfiguration.Default()
            });
        }

        private static void CreateViewportWindow(ViewportWindowConfiguration config)
        {
            var win = new ViewportWindow(config.Configuration);
            win.Text += " - Window " + config.WindowID;
            LoadViewports(win.TableSplitControl, config);
            Windows.Add(win);
            win.Closed += (s, e) =>
            {
                Windows.Remove(win);
                win.Dispose();
            };
            win.Show(Editor.Instance);
            if (!config.Size.IsEmpty)
            {
                win.Location = config.Size.Location;
                win.Size = config.Size.Size;
                win.WindowState = config.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }
        }

        private static void LoadViewports(TableSplitControl tableSplitControl, ViewportWindowConfiguration config)
        {
            var defaultViewports = new[]
            {
                "PerspectiveCamera",
                "OrthographicCamera/Top",
                "OrthographicCamera/Front",
                "OrthographicCamera/Side"
            };

            var viewports = config.Viewports ?? new List<string>();
            viewports.Clear();

            for (var i = 0; i < config.Configuration.Rectangles.Count; i++)
            {
                var viewport = viewports.Count > i ? viewports[i] : "";
                var def = defaultViewports[(i % defaultViewports.Length)];

                var camera = Camera.Deserialise(viewport) ?? Camera.Deserialise(def) ?? new PerspectiveCamera();
                var view = SceneManager.Engine.CreateViewport(camera);

                // Crosshair cursor
                // Todo, this is pretty lazy
                if (Sledge.Settings.View.CrosshairCursorIn2DViews && view.Camera is OrthographicCamera)
                {
                    view.Control.Cursor = Cursors.Cross;
                }
                view.Control.CursorChanged += (sender, args) =>
                {
                    if (Sledge.Settings.View.CrosshairCursorIn2DViews && view.Control.Cursor == Cursors.Default  && view.Camera is OrthographicCamera)
                    {
                        view.Control.Cursor = Cursors.Cross;
                    }
                };
                
                var vp = CreateMapViewport(view);
                Viewports.Add(vp);
                SubscribeExceptions(vp);

                vp.Control.Dock = DockStyle.Fill;
                tableSplitControl.Controls.Add(vp.Control);

                Mediator.Publish(EditorMediator.ViewportCreated, vp);
            }
        }

        private static MapViewport CreateMapViewport(IViewport view)
        {
            var vp = new MapViewport(view);
            vp.Listeners.Add(new ToolViewportListener(vp));
            vp.Listeners.Add(new Camera3DViewportListener(vp));
            vp.Listeners.Add(new Camera2DViewportListener(vp));
            return vp;
        }

        private static ViewportWindowConfiguration GetDefaultWindowConfiguration()
        {
            return new ViewportWindowConfiguration
            {
                WindowID = 0,
                Size = Rectangle.Empty,
                Configuration = TableSplitConfiguration.Default(),
                Maximised = true
            };
        }

        private static string SerialiseViewport(MapViewport vp)
        {
            return Camera.Serialise(vp.Viewport.Camera);
        }

        public static void SaveLayout()
        {
            SettingsManager.SetAdditionalData("ViewportManagerWindowConfiguration", GetWindowConfigurations());
        }

        public static List<ViewportWindowConfiguration> GetWindowConfigurations()
        {
            var list = new List<ViewportWindowConfiguration>();
            list.Add(new ViewportWindowConfiguration
            {
                WindowID = 0,
                Size = new Rectangle(Editor.Instance.Location, Editor.Instance.Size),
                Configuration = MainWindowGrid.Configuration,
                Viewports = GetViewportsForTableSplitControl(MainWindowGrid).Select(SerialiseViewport).ToList(),
                Maximised = Editor.Instance.WindowState != FormWindowState.Normal
            });
            for (var i = 0; i < Windows.Count; i++)
            {
                list.Add(new ViewportWindowConfiguration
                {
                    WindowID = i + 1,
                    Size = new Rectangle(Windows[i].Location, Windows[i].Size),
                    Configuration = Windows[i].TableSplitControl.Configuration,
                    Viewports = GetViewportsForTableSplitControl(Windows[i].TableSplitControl).Select(SerialiseViewport).ToList(),
                    Maximised = Windows[i].WindowState != FormWindowState.Normal
                });
            }
            return list;
        }

        private static void SetConfiguration(TableSplitControl control, ViewportWindowConfiguration configuration)
        {
            if (control == null || configuration == null) return;

            control.Configuration = configuration.Configuration;
            var viewports = GetViewportsForTableSplitControl(control);

            control.Controls.Clear();
            foreach (var vp in viewports)
            {
                SceneManager.Engine.DestroyViewport(vp.Viewport);
                Viewports.Remove(vp);
            }

            LoadViewports(control, configuration);
        }

        public static void SetWindowConfigurations(List<ViewportWindowConfiguration> configurations)
        {
            SetConfiguration(MainWindowGrid, configurations.FirstOrDefault(x => x.WindowID == 0));
            for (var i = 0; i < Windows.Count; i++)
            {
                var win = Windows[i];
                SetConfiguration(win.TableSplitControl, configurations.FirstOrDefault(x => x.WindowID == i + 1));
            }
        }

        private static IEnumerable<MapViewport> GetViewportsForTableSplitControl(TableSplitControl control)
        {
            return Viewports.Where(x => GetParentSplitControl(x) == control);
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

        private static void SubscribeExceptions(MapViewport vp)
        {
            vp.ListenerException += (sender, ex) => Logging.Logger.ShowException(ex, "Viewport Listener Exception");
            vp.Viewport.RenderException += (sender, ex) => Logging.Logger.ShowException(ex, "Viewport Render Exception");
        }

        private static TableSplitControl GetParentSplitControl(MapViewport viewport)
        {
            var par = viewport.Control.Parent;
            while (par != null && !(par is TableSplitControl))
            {
                par = par.Parent;
            }
            return par as TableSplitControl;
        }

        public static MapViewport GetActiveViewport()
        {
            return Viewports.FirstOrDefault(x => x.Viewport.IsFocused);
        }
    }
}
