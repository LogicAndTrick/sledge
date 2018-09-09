using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Controls;
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
    public class MapDocumentControlHost : UserControl, ISettingsContainer, IUIShutdownHook, IUIStartupHook
    {
        private readonly IEnumerable<Lazy<IMapDocumentControlFactory>> _controlFactories;
        private readonly Form _shell;
        private ContextMenuStrip _contextMenu;

        public static MapDocumentControlHost Instance { get; private set; }

        private TableSplitControl Table { get; set; }
        private List<CellReference> MapDocumentControls { get; }

        // Be careful to ensure this is created on the UI thread

        [ImportingConstructor]
        public MapDocumentControlHost(
            [ImportMany] IEnumerable<Lazy<IMapDocumentControlFactory>> controlFactories,
            [Import("Shell")] Form shell
        )
        {
            _controlFactories = controlFactories;
            _shell = shell;

            MapDocumentControls = new List<CellReference>();
        }

        public void OnUIStartup()
        {
            Instance = this;
            Table = new TableSplitControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(Table);
            CreateHandle();

            Application.AddMessageFilter(new LeftClickMessageFilter(this));

            Oy.Subscribe("BspEditor:SplitView:Autosize", () => this.InvokeLater(() => Table.ResetViews()));
        }

        private IMapDocumentControl _contextControl;
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

            var position = Table.GetPositionFromControl(_contextControl.Control);
            if (position.Row < 0 || position.Column < 0) return;

            var hc = new HostedControl
            {
                Row = position.Row, 
                Column = position.Column, 
                Type = mi.Type, 
                Serialised = mi.Style
            };

            _shell.InvokeSync(() =>
            {
                if (UpdateControl(hc)) return;
                var ctrl = MakeControl(hc.Type, hc.Serialised);
                if (ctrl != null) SetControl(ctrl, hc.Column, hc.Row);
            });
        }

        public void OnUIShutdown()
        {
            MapDocumentControls.ForEach(x => x.Dispose());
            MapDocumentControls.Clear();
        }

        // Settings container

        string ISettingsContainer.Name => "Sledge.BspEditor.Components.MapDocumentControlHost";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            var newConfig = store.Get("TableConfiguration", Table.Configuration);
            if (newConfig.IsValid()) Table.Configuration = newConfig;

            Table.RowSizes = store.Get("RowSizes", Table.RowSizes);
            Table.ColumnSizes = store.Get("ColumnSizes", Table.ColumnSizes);

            var controls = store.Get<List<HostedControl>>("Controls");
            if (controls == null || !controls.Any())
            {
                controls = HostedControl.Default;
            }

            // Ensure that controls are created on the UI thread
            _shell.InvokeSync(() =>
            {
                foreach (var hc in controls)
                {
                    if (UpdateControl(hc)) continue;
                    var ctrl = MakeControl(hc.Type, hc.Serialised);
                    if (ctrl != null) SetControl(ctrl, hc.Column, hc.Row);
                }
            });
        }

        public void StoreValues(ISettingsStore store)
        {
            var config = Table.Configuration ?? TableSplitConfiguration.Default();
            store.Set("TableConfiguration", config);
            store.Set("RowSizes", Table.RowSizes);
            store.Set("ColumnSizes", Table.ColumnSizes);

            var controls = new List<HostedControl>();

            foreach (var mdc in MapDocumentControls)
            {
                controls.Add(new HostedControl { Row = mdc.Row, Column = mdc.Column, Type = mdc.Control.Type, Serialised = mdc.Control.GetSerialisedSettings() });
            }
            store.Set("Controls", controls);
        }

        // Create and update controls

        private bool UpdateControl(HostedControl hc)
        {
            // Try and find the control in the same slot
            var controlAt = Table.GetControlFromPosition(hc.Column, hc.Row);
            if (controlAt == null) return false;

            // Find the corresponding map document control
            var mdc = MapDocumentControls.FirstOrDefault(x => x.Control.Control == controlAt);
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

        private void SetControl(IMapDocumentControl control, int column, int row)
        {
            var controlAt = Table.GetControlFromPosition(column, row);
            if (controlAt != null) Table.Controls.Remove(controlAt);

            foreach (var rem in MapDocumentControls.Where(x => x.Row == row && x.Column == column).ToList())
            {
                rem.Dispose();
                MapDocumentControls.Remove(rem);
            }

            MapDocumentControls.Add(new CellReference(control, column, row));
            Table.Controls.Add(control.Control, column, row);
        }
        
        public IEnumerable<IMapDocumentControl> GetControls()
        {
            return MapDocumentControls.Select(x => x.Control);
        }

        private bool InterceptRightClick()
        {
            var client = PointToClient(MousePosition);
            if (!ClientRectangle.Contains(client)) return false;

            foreach (var mdc in MapDocumentControls)
            {
                var control = mdc.Control;
                var mapped = control.Control.PointToClient(MousePosition);

                if (mapped.X >= 0 && mapped.X < 40 && mapped.Y >= 0 && mapped.Y < FontHeight + 2)
                {
                    ShowContextMenu(control, mapped);
                    return true;
                }
            }

            return false;
        }

        private void ShowContextMenu(IMapDocumentControl control, Point point)
        {
            CreateContextMenu();
            foreach (var cmi in _contextMenu.Items.OfType<ContextMenuItem>())
            {
                var f = _controlFactories.Select(x => x.Value).FirstOrDefault(x => x.Type == cmi.Type);
                cmi.Checked = f != null && f.IsStyle(control, cmi.Style);
            }

            _contextControl = control;
            _contextMenu.Show(control.Control, point);
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
            public string Type { get; set; }
            public string Style { get; set; }

            public ContextMenuItem(string text, string type, string style) : base(text)
            {
                Type = type;
                Style = style;
            }
        }

        private class HostedControl
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public string Type { get; set; }
            public string Serialised { get; set; }

            public static readonly List<HostedControl> Default = new List<HostedControl>
            {
                new HostedControl { Row = 0, Column = 0, Type = "MapViewport", Serialised = "PerspectiveCamera/", },
                new HostedControl { Row = 0, Column = 1, Type = "MapViewport", Serialised = "OrthographicCamera/Top" },
                new HostedControl { Row = 1, Column = 0, Type = "MapViewport", Serialised = "OrthographicCamera/Front" },
                new HostedControl { Row = 1, Column = 1, Type = "MapViewport", Serialised = "OrthographicCamera/Side" },
            };
        }

        private class CellReference : IDisposable
        {
            public IMapDocumentControl Control { get; }
            public int Column { get; }
            public int Row { get; }

            public CellReference(IMapDocumentControl control, int column, int row)
            {
                Control = control;
                Column = column;
                Row = row;
            }

            public void Dispose()
            {
                Control?.Dispose();
            }
        }
    }
}
