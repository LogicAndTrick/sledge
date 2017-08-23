using System.Data;
using System.Windows.Forms;
using Sledge.BspEditor.Editing.Components.Compile.Specification;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public partial class BuildParametersPanel : UserControl
    {
        public BuildParametersPanel()
        {
            InitializeComponent();
        }

        public void SetTool(CompileTool tool)
        {
            var dt = new DataTable();
            dt.Columns.Add("Selected", typeof(bool));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(object));
            foreach (var parameter in tool.Parameters)
            {
                dt.Rows.Add(false, parameter.Name, parameter.Value);
            }


            var checkboxColumn = new DataGridViewCheckBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Width = 20,
                DataPropertyName = "Selected"
            };
            dataTable.Columns.Add(checkboxColumn);

            var nameColumn = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Width = 5,
                DataPropertyName = "Name",
                ReadOnly = true
            };
            dataTable.Columns.Add(nameColumn);

            var valueColumn = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = "Value"
            };
            dataTable.Columns.Add(valueColumn);
            
            dataTable.DataSource = dt;
        }
    }
}
