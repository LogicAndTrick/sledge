using System;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Brushes;

namespace Sledge.Editor.UI.Sidebar
{
    public partial class BrushSidebarPanel : UserControl, IMediatorListener
    {
        public bool RoundCheckboxEnabled
        {
            get { return RoundCreatedVerticesCheckbox.Enabled; }
            set { RoundCreatedVerticesCheckbox.Enabled = value; }
        }

        public BrushSidebarPanel()
        {
            InitializeComponent();

            Mediator.Subscribe(EditorMediator.ResetSelectedBrushType, this);

            RoundCreatedVerticesCheckbox.Checked = BrushManager.RoundCreatedVertices;
        }

        public void ResetSelectedBrushType()
        {
            if (BrushTypeList.Items.Count > 0)
            {
                BrushTypeList.SelectedIndex = 0;
            }
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        private void RoundCreatedVerticesChanged(object sender, EventArgs e)
        {
            BrushManager.RoundCreatedVertices = RoundCreatedVerticesCheckbox.Checked;
        }
    }
}
