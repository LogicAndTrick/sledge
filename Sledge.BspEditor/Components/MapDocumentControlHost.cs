using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sledge.BspEditor.Controls;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Components
{
    /// <summary>
    /// The map document controls are shared between all map documents.
    /// This class is the main control host. It manages the table split panel
    /// as well as the child controls inside. Table and control configurations
    /// are saved.
    /// </summary>
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(ISettingsContainer))]
    public class MapDocumentControlHost : UserControl, ISettingsContainer, IInitialiseHook
    {
        [ImportMany] private IEnumerable<Lazy<IMapDocumentControlFactory>> _controlFactories;

        public async Task OnInitialise()
        {
            // Just here to make sure this gets initialised
        }

        public static MapDocumentControlHost Instance { get; private set; }
        private TableSplitControl Table { get; }
        private List<CellReference> MapDocumentControls { get; }

        private class CellReference
        {
            public IMapDocumentControl Control { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }

            public CellReference(IMapDocumentControl control, int column, int row)
            {
                Control = control;
                Column = column;
                Row = row;
            }
        }

        public MapDocumentControlHost()
        {
            Instance = this;
            Table = new TableSplitControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(Table);

            MapDocumentControls = new List<CellReference>();
        }

        public void SetControl(IMapDocumentControl control, int column, int row)
        {
            var controlAt = Table.GetControlFromPosition(column, row);
            if (controlAt != null) Table.Controls.Remove(controlAt);
            MapDocumentControls.RemoveAll(x => x.Row == row && x.Column == column);

            MapDocumentControls.Add(new CellReference(control, column, row));
            Table.Controls.Add(control.Control, column, row);
        }

        public IEnumerable<IMapDocumentControl> GetControls()
        {
            return MapDocumentControls.Select(x => x.Control);
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
            foreach (var hc in controls)
            {
                var ctrl = MakeControl(hc.Type, hc.Serialised);
                if (ctrl != null) SetControl(ctrl, hc.Column, hc.Row);
            }
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

        private IMapDocumentControl MakeControl(string type, string serialised)
        {
            var ctrl = _controlFactories.FirstOrDefault(x => x.Value.Type == type)?.Value.Create();
            ctrl?.SetSerialisedSettings(serialised);
            return ctrl;
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
    }
}
