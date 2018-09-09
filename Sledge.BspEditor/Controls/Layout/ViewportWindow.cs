using System.Windows.Forms;
using Sledge.BspEditor.Components;

namespace Sledge.BspEditor.Controls.Layout
{
    public partial class ViewportWindow : Form
    {
        public MapDocumentContainer MapDocumentContainer { get; }

        public ViewportWindow(MapDocumentControlWindowConfiguration config)
        {
            InitializeComponent();

            MapDocumentContainer = new MapDocumentContainer(config.WindowID)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(MapDocumentContainer);

            SetConfiguration(config);
        }

        public void SetConfiguration(MapDocumentControlWindowConfiguration config)
        {
            MapDocumentContainer.Table.Configuration = config.Configuration;
            MapDocumentContainer.Table.RowSizes = config.RowSizes;
            MapDocumentContainer.Table.ColumnSizes = config.ColumnSizes;

            Location = config.Size.Location;
            Size = config.Size.Size;
            WindowState = config.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
        }
    }
}
