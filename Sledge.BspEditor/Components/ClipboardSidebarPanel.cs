using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Components
{
    [Export(typeof(ISidebarComponent))]
    [SidebarComponent(OrderHint = "K")]
    public partial class ClipboardSidebarPanel : UserControl, ISidebarComponent
    {
        [Import] private Lazy<ClipboardManager> _clipboard;

        public string Title => "Clipboard";
        public object Control => this;

        public ClipboardSidebarPanel()
        {
            InitializeComponent();

            Oy.Subscribe<ClipboardManager>("BspEditor:ClipboardChanged", ClipboardChanged);
        }

        private async Task ClipboardChanged(ClipboardManager arg)
        {
            UpdateList();
        }

        private void UpdateList()
        {
            ClipboardList.BeginUpdate();
            ClipboardList.Items.Clear();

            foreach (var val in _clipboard.Value.GetClipboardRing().Reverse())
            {
                // todo !clipboard panel nice interface
                ClipboardList.Items.Add(val);
            }
            if (ClipboardList.Items.Count > 0) ClipboardList.SelectedIndex = 0;

            ClipboardList.EndUpdate();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }
    }
}
