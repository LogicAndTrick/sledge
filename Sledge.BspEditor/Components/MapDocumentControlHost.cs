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
using Sledge.Common.Hooks;
using Sledge.Common.Settings;

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

        // Settings container

        string ISettingsContainer.Name => "Sledge.BspEditor.Components.MapDocumentControlHost";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Columns", "Number of columns", typeof(int));
            yield return new SettingKey("Rows", "Number of rows", typeof(int));
            yield return new SettingKey("Rectangles", "Rectangle configuration", typeof(Rectangle[]));
            yield return new SettingKey("Controls", "Control configuration", typeof(HostedControl[]));
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            var config = Table.Configuration;
            var d = values.ToDictionary(x => x.Name, x => x.Value);
            int i;
            var cols = d.ContainsKey("Columns") && int.TryParse(d["Columns"], out i) ? i : config.Columns;
            var rows = d.ContainsKey("Rows") && int.TryParse(d["Rows"], out i) ? i : config.Rows;
            var rects = config.Rectangles;
            if (d.ContainsKey("Rectangles"))
            {
                try
                {
                    rects = JsonConvert.DeserializeObject<List<Rectangle>>(d["Rectangles"]);
                }
                catch
                {
                    // nah
                }
            }
            var newConfig = new TableSplitConfiguration
            {
                Columns = cols,
                Rows = rows,
                Rectangles = rects
            };
            if (newConfig.IsValid())
            {
                Table.Configuration = newConfig;
            }

            List<HostedControl> controls = null;
            if (d.ContainsKey("Controls"))
            {
                try
                {
                    controls = JsonConvert.DeserializeObject<List<HostedControl>>(d["Controls"]);
                }
                catch
                {
                    controls = null;
                }
            }
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

        public IEnumerable<SettingValue> GetValues()
        {
            var config = Table.Configuration ?? TableSplitConfiguration.Default();
            yield return new SettingValue("Columns", Convert.ToString(config.Columns, CultureInfo.InvariantCulture));
            yield return new SettingValue("Rows", Convert.ToString(config.Rows, CultureInfo.InvariantCulture));
            yield return new SettingValue("Rectangles", JsonConvert.SerializeObject(config.Rectangles, Formatting.None));

            var controls = new List<HostedControl>();
            foreach (var mdc in MapDocumentControls)
            {
                controls.Add(new HostedControl { Row = mdc.Row, Column = mdc.Column, Type = mdc.Control.Type, Serialised = mdc.Control.GetSerialisedSettings()});
            }
            yield return new SettingValue("Controls", JsonConvert.SerializeObject(controls));
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
