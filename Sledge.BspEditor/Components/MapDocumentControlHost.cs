using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Controls;
using Sledge.BspEditor.Controls.Layout;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Shell;
using Message = System.Windows.Forms.Message;

namespace Sledge.BspEditor.Components
{
    /// <summary>
    /// The map document controls are shared between all map documents.
    /// This class is the main control host. It manages the table split panel
    /// as well as the child controls inside. Table and control configurations
    /// are saved.
    /// </summary>
    [Export(typeof(IUIShutdownHook))]
    [Export(typeof(IUIStartupHook))]
    [Export(typeof(ISettingsContainer))]
    [Export]
    public class MapDocumentControlHost : UserControl, ISettingsContainer, IUIShutdownHook, IUIStartupHook
    {
        private readonly IEnumerable<Lazy<IMapDocumentControlFactory>> _controlFactories;
        private readonly Form _shell;
        private ContextMenuStrip _contextMenu;

        public static MapDocumentControlHost Instance { get; private set; }

        private MapDocumentContainer MainWindow { get; set; }
        private List<ViewportWindow> Windows { get; set; }

        // Be careful to ensure this is created on the UI thread

        [ImportingConstructor]
        public MapDocumentControlHost(
            [ImportMany] IEnumerable<Lazy<IMapDocumentControlFactory>> controlFactories,
            [Import("Shell")] Form shell
        )
        {
            _controlFactories = controlFactories;
            _shell = shell;
        }

        public void OnUIStartup()
        {
            Instance = this;
            MainWindow = new MapDocumentContainer(0)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(MainWindow);
            Windows = new List<ViewportWindow>();
            CreateHandle();

            Application.AddMessageFilter(new LeftClickMessageFilter(this));
        }

        public void OnUIShutdown()
        {
            foreach (var container in GetContainers())
            {
                container.MapDocumentControls.ForEach(x => x.Dispose());
                container.MapDocumentControls.Clear();
            }
        }

        // Configs

        public IEnumerable<MapDocumentControlWindowConfiguration> GetConfigurations()
        {
            yield return new MapDocumentControlWindowConfiguration(null, MainWindow);
            foreach (var window in Windows)
            {
                yield return new MapDocumentControlWindowConfiguration(window, window.MapDocumentContainer);
            }
        }

        public void SetConfigurations(IEnumerable<MapDocumentControlWindowConfiguration> configurations)
        {
            _shell.InvokeSync(() =>
            {
                foreach (var config in configurations)
                {
                    if (config.WindowID == 0)
                    {
                        RemoveInvalidCells(MainWindow, config.Configuration);

                        MainWindow.Table.Configuration = config.Configuration;
                        MainWindow.Table.RowSizes = config.RowSizes;
                        MainWindow.Table.ColumnSizes = config.ColumnSizes;

                        PopulateEmptyCells(MainWindow);
                    }
                    else
                    {
                        var window = GetWindow(config.WindowID);
                        if (window == null) continue;

                        RemoveInvalidCells(window.MapDocumentContainer, config.Configuration);

                        window.SetConfiguration(config);

                        PopulateEmptyCells(window.MapDocumentContainer);
                    }
                }
            });
        }

        private void RemoveInvalidCells(MapDocumentContainer container, TableSplitConfiguration config)
        {
            var recs = config.Rectangles.ToList();
            foreach (var control in container.Table.Controls.OfType<Control>().ToList())
            {
                var pos = container.Table.GetPositionFromControl(control);
                var mdc = container.MapDocumentControls.FirstOrDefault(x => x.Control.Control == control);
                if (recs.Any(x => x.X == pos.Column && x.Y == pos.Row)) continue;

                container.Table.Controls.Remove(control);
                if (mdc == null) continue;

                container.MapDocumentControls.Remove(mdc);
                mdc.Control.Control.Dispose();
            }
        }

        private void PopulateEmptyCells(MapDocumentContainer container)
        {
            var loop = HostedControl.Default(container.WindowID);
            var index = 0;
            foreach (var rec in container. Table.Configuration.Rectangles)
            {
                var i = rec.Y;
                var j = rec.X;

                var c = container.MapDocumentControls.FirstOrDefault(x => x.Row == i && x.Column == j);
                if (c != null) continue;

                var hc = new HostedControl
                {
                    WindowID = container.WindowID,
                    Row = i,
                    Column = j,
                    Type = loop[index].Type,
                    Serialised = loop[index].Serialised
                };
                index = (index + 1) % loop.Count;

                var ctrl = MakeControl(hc.Type, hc.Serialised);
                if (ctrl != null) container.SetControl(ctrl, hc.Column, hc.Row);
            }
        }

        // Windows

        private ViewportWindow GetWindow(int windowId)
        {
            return Windows.FirstOrDefault(x => x.MapDocumentContainer.WindowID == windowId);
        }

        private ViewportWindow CreateWindow(MapDocumentControlWindowConfiguration config)
        {
            var window = new ViewportWindow(config);
            window.Closed += DestroyWindow;
            Windows.Add(window);
            window.Show(_shell);
            return window;
        }

        private void DestroyWindow(object sender, EventArgs e)
        {
            var window = (ViewportWindow)sender;
            Windows.Remove(window);
            window.Closed -= DestroyWindow;
            window.Dispose();
        }

        public void CreateNewWindow()
        {
            _shell.InvokeSync(() =>
            {
                var nextId = Enumerable.Range(1, Windows.Count + 1).Except(Windows.Select(x => x.MapDocumentContainer.WindowID)).First();
                var window = CreateWindow(MapDocumentControlWindowConfiguration.Default(nextId));
                var container = window.MapDocumentContainer;
                foreach (var hc in HostedControl.Default(nextId))
                {
                    if (UpdateControl(hc)) continue;
                    var ctrl = MakeControl(hc.Type, hc.Serialised);
                    if (ctrl != null) container.SetControl(ctrl, hc.Column, hc.Row);
                }
            });
        }

        // Containers

        private IEnumerable<MapDocumentContainer> GetContainers()
        {
            yield return MainWindow;
            foreach (var w in Windows) yield return w.MapDocumentContainer;
        }

        private MapDocumentContainer GetContainer(int windowId)
        {
            return GetContainers().FirstOrDefault(x => x.WindowID == windowId);
        }

        // Settings

        string ISettingsContainer.Name => "Sledge.BspEditor.Components.MapDocumentControlHost";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            var windowConfigs = store.Get<List<MapDocumentControlWindowConfiguration>>("WindowConfigurations")
                                ?? new List<MapDocumentControlWindowConfiguration> { new MapDocumentControlWindowConfiguration() };

            var containers = GetContainers().ToList();
            var windowIds = windowConfigs.Select(x => x.WindowID).Union(containers.Select(x => x.WindowID)).ToList();
            
            // Ensure that controls are created on the UI thread
            _shell.InvokeSync(() =>
            {
                foreach (var id in windowIds)
                {
                    var config = windowConfigs.FirstOrDefault(x => x.WindowID == id);
                    var container = containers.FirstOrDefault(x => x.WindowID == id);
                    if (config == null)
                    {
                        if (!(container?.WindowID > 0)) continue;

                        var window = GetWindow(container.WindowID);
                        if (window != null) DestroyWindow(window, EventArgs.Empty);
                    }
                    else if (container == null)
                    {
                        CreateWindow(config);
                    }
                    else if (config.WindowID > 0)
                    {
                        var window = GetWindow(container.WindowID);
                        window?.SetConfiguration(config);
                    }
                    else
                    {
                        if (config.Configuration.IsValid()) container.Table.Configuration = config.Configuration;
                        container.Table.RowSizes = config.RowSizes;
                        container.Table.ColumnSizes = config.ColumnSizes;
                    }
                }

                var controls = store.Get<List<HostedControl>>("Controls");
                if (controls == null || !controls.Any())
                {
                    controls = HostedControl.Default(0);
                }

                foreach (var hc in controls)
                {
                    if (UpdateControl(hc)) continue;
                    var ctrl = MakeControl(hc.Type, hc.Serialised);
                    var container = GetContainer(hc.WindowID);
                    if (container != null && ctrl != null) container.SetControl(ctrl, hc.Column, hc.Row);
                }
            });
        }

        public void StoreValues(ISettingsStore store)
        {
            var config = GetContainers().Select(x => new MapDocumentControlWindowConfiguration(GetWindow(x.WindowID), x)).ToList();
            store.Set("WindowConfigurations", config);

            var controls = new List<HostedControl>();

            foreach (var con in GetContainers())
            {
                foreach (var mdc in con.MapDocumentControls)
                {
                    controls.Add(new HostedControl
                    {
                        WindowID = con.WindowID,
                        Row = mdc.Row,
                        Column = mdc.Column,
                        Type = mdc.Control.Type,
                        Serialised = mdc.Control.GetSerialisedSettings()
                    });
                }
            }

            store.Set("Controls", controls);
        }

        // Create and update controls

        public IEnumerable<IMapDocumentControl> GetControls()
        {
            return GetContainers().SelectMany(x => x.MapDocumentControls).Select(x => x.Control);
        }

        private bool UpdateControl(HostedControl hc)
        {
            var container = GetContainer(hc.WindowID);
            if (container == null) return false;

            // Try and find the control in the same slot
            var controlAt = container.Table.GetControlFromPosition(hc.Column, hc.Row);
            if (controlAt == null) return false;

            // Find the corresponding map document control
            var mdc = container.MapDocumentControls.FirstOrDefault(x => x.Control.Control == controlAt);
            if (mdc == null) return false;

            // Ensure that the control is the type we want
            var factory = _controlFactories.FirstOrDefault(x => x.Value.Type == hc.Type)?.Value;
            if (factory == null || !factory.IsType(mdc.Control)) return false;

            // Update the control instead of replacing it
            mdc.Control.SetSerialisedSettings(hc.Serialised);
            return true;
        }

        private IMapDocumentControl MakeControl(string type, string serialised)
        {
            var ctrl = _controlFactories.FirstOrDefault(x => x.Value.Type == type)?.Value.Create();
            ctrl?.SetSerialisedSettings(serialised);
            return ctrl;
        }

        // Context menu

        private HostedControl _contextControl;

        private void CreateContextMenu()
        {
            if (_contextMenu != null) return;

            _contextMenu = new ContextMenuStrip();
            foreach (var cf in _controlFactories.Select(x => x.Value))
            {
                if (_contextMenu.Items.Count > 0) _contextMenu.Items.Add(new ToolStripSeparator());
                foreach (var kv in cf.GetStyles())
                {
                    _contextMenu.Items.Add(new ContextMenuItem(kv.Value, cf.Type, kv.Key));
                }
            }

            _contextMenu.Closed += (s, e) => _contextControl = null;
            _contextMenu.ItemClicked += SetContextControl;
        }

        private void SetContextControl(object sender, ToolStripItemClickedEventArgs e)
        {
            if (_contextControl == null || !(e.ClickedItem is ContextMenuItem mi)) return;

            var container = GetContainer(_contextControl.WindowID);
            if (container == null) return;

            var hc = new HostedControl
            {
                WindowID = _contextControl.WindowID,
                Row = _contextControl.Row,
                Column = _contextControl.Column,
                Type = mi.Type, 
                Serialised = mi.Style
            };

            _shell.InvokeSync(() =>
            {
                if (UpdateControl(hc)) return;
                var ctrl = MakeControl(hc.Type, hc.Serialised);
                if (ctrl != null) container.SetControl(ctrl, hc.Column, hc.Row);
            });
        }

        private bool InterceptRightClick()
        {
            var mousePosition = MousePosition;
            foreach (var container in GetContainers())
            {
                var client = container.PointToClient(mousePosition);
                if (!container.ClientRectangle.Contains(client)) continue;

                foreach (var control in container.Table.Controls.OfType<Control>())
                {
                    var mapped = control.PointToClient(mousePosition);

                    if (mapped.X >= 0 && mapped.X < 40 && mapped.Y >= 0 && mapped.Y < FontHeight + 2)
                    {
                        var pos = container.Table.GetPositionFromControl(control);
                        var hc = new HostedControl
                        {
                            WindowID = container.WindowID,
                            Row = pos.Row,
                            Column = pos.Column,
                        };
                        var mdc = GetControls().FirstOrDefault(x => x.Control == control);
                        ShowContextMenu(hc, mdc, mousePosition);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ShowContextMenu(HostedControl control, IMapDocumentControl mdc, Point screenPoint)
        {
            CreateContextMenu();
            
            foreach (var cmi in _contextMenu.Items.OfType<ContextMenuItem>())
            {
                var f = _controlFactories.Select(x => x.Value).FirstOrDefault(x => x.Type == cmi.Type);
                cmi.Checked = f != null && mdc != null && f.IsStyle(mdc, cmi.Style);
            }

            _contextControl = control;
            _contextMenu.Show(this, PointToClient(screenPoint));
        }

        private class LeftClickMessageFilter : IMessageFilter
        {
            private readonly MapDocumentControlHost _self;

            public LeftClickMessageFilter(MapDocumentControlHost self)
            {
                _self = self;
            }

            public bool PreFilterMessage(ref Message objMessage)
            {
                if (objMessage.Msg == 0x0204) // WM_RBUTTONDOWN
                {
                    if (_self.InterceptRightClick()) return true;
                }
                return false;
            }
        }

        private class ContextMenuItem : ToolStripMenuItem
        {
            public string Type { get; }
            public string Style { get; }

            public ContextMenuItem(string text, string type, string style) : base(text)
            {
                Type = type;
                Style = style;
            }
        }

        private class HostedControl
        {
            public int WindowID { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
            public string Type { get; set; }
            public string Serialised { get; set; }

            public static List<HostedControl> Default(int windowId)
            {
                return new List<HostedControl>
                {
                    new HostedControl { WindowID = windowId, Row = 0, Column = 0, Type = "MapViewport", Serialised = "PerspectiveCamera/", },
                    new HostedControl { WindowID = windowId, Row = 0, Column = 1, Type = "MapViewport", Serialised = "OrthographicCamera/Top" },
                    new HostedControl { WindowID = windowId, Row = 1, Column = 0, Type = "MapViewport", Serialised = "OrthographicCamera/Front" },
                    new HostedControl { WindowID = windowId, Row = 1, Column = 1, Type = "MapViewport", Serialised = "OrthographicCamera/Side" },
                };
            }
        }
    }
}
