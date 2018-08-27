using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Controls;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Shell;

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
        [ImportMany] private IEnumerable<Lazy<IMapDocumentControlFactory>> _controlFactories;
        [Import("Shell")] private Form _shell;
        
        public static MapDocumentControlHost Instance { get; private set; }

        private TableSplitControl Table { get; set; }
        private List<CellReference> MapDocumentControls { get; }

        // Be careful to ensure this is created on the UI thread

        public MapDocumentControlHost()
        {
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

            Oy.Subscribe("BspEditor:SplitView:Autosize", () => this.InvokeLater(() => Table.ResetViews()));
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

        public void StoreValues(ISettingsStore store)
        {
            var config = Table.Configuration ?? TableSplitConfiguration.Default();
            store.Set("TableConfiguration", config);

            var controls = new List<HostedControl>();

            foreach (var mdc in MapDocumentControls)
            {
                controls.Add(new HostedControl { Row = mdc.Row, Column = mdc.Column, Type = mdc.Control.Type, Serialised = mdc.Control.GetSerialisedSettings()});
            }
            store.Set("Controls", controls);
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
