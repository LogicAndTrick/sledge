using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Editing.Components.Compile.Specification;
using Sledge.Common.Extensions;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public partial class BuildParametersPanel : UserControl
    {
        private CompileTool _tool;
        private DataTable _data;

        public CompileTool Tool
        {
            get => _tool;
            set => SetTool(value);
        }

        public string Arguments
        {
            get => GetArguments();
            set => SetArguments(value);
        }

        public BuildParametersPanel()
        {
            InitializeComponent();

            _data = new DataTable();
            _data.Columns.Add("Flag", typeof(string));
            _data.Columns.Add("Selected", typeof(bool));
            _data.Columns.Add("Name", typeof(string));
            _data.Columns.Add("Value", typeof(object));
            
            _data.RowChanged += (s, e) => UpdatePreview();
            
            var flagColumn = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DataPropertyName = "Flag",
                ReadOnly = true
            };
            dataTable.Columns.Add(flagColumn);
            
            var checkboxColumn = new DataGridViewCheckBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Width = 20,
                DataPropertyName = "Selected"
            };
            dataTable.Columns.Add(checkboxColumn);

            var nameColumn = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
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
            
            dataTable.DataSource = _data;
        }

        private void SetTool(CompileTool tool)
        {
            _tool = tool;

            _data.Rows.Clear();
            foreach (var parameter in tool.Parameters)
            {
                _data.Rows.Add(parameter.Flag, false, parameter.Name, parameter.Value);
            }

            dataTable.Update();
            UpdatePreview();
        }

        private string GetArguments()
        {
            var list = new List<string>();

            foreach (DataRow row in ((DataTable) dataTable.DataSource).Rows)
            {
                if (Convert.ToBoolean(row[1]))
                {
                    var key = Convert.ToString(row[0]);
                    var val = Convert.ToString(row[3]).Trim();
                    list.Add(key);
                    if (val.Length > 0) list.Add(val);
                }
            }

            return String.Join(" ", list);
        }

        private void SetArguments(string arguments)
        {
            var kvs = arguments.SplitWithQuotes().ToList();

            foreach (DataRow row in ((DataTable) dataTable.DataSource).Rows)
            {
                var key = Convert.ToString(row[0]);
                var idx = kvs.IndexOf(key);

                if (idx >= 0)
                {
                    var val = "";
                    if (idx < kvs.Count - 1)
                    {
                        var next = kvs[idx + 1];
                        if (next[0] != '-') val = next;
                    }

                    row[1] = true;
                    row[3] = val;
                }
                else
                {
                    row[1] = false;
                }
            }
            dataTable.Update();
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            txtPreviewText.Text = GetArguments();
        }

        private void StateChanged(object sender, EventArgs e)
        {
            if (!(dataTable.CurrentCell.Value is bool)) return;

            dataTable.CommitEdit(DataGridViewDataErrorContexts.Commit);
            _data.AcceptChanges();
        }
    }
}
