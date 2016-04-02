using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;

namespace Sledge.Editor.UI.Sidebar
{
    public partial class HistorySiderbarPanel : UserControl, IMediatorListener
    {
        public HistorySiderbarPanel()
        {
            InitializeComponent();

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentDeactivated, this);
            Mediator.Subscribe(EditorMediator.HistoryChanged, this);
        }

        public void Notify(string message, object data)
        {
            Rebuild();
        }

        private void Rebuild()
        {
            TreeNode lastNode = null;
            HistoryView.BeginUpdate();

            HistoryView.Nodes.Clear();
            if (DocumentManager.CurrentDocument != null)
            {
                var nodes = HistoryView.Nodes;
                var stacks = DocumentManager.CurrentDocument.History.GetHistoryStacks().Reverse().ToList();
                for (var i = 0; i < stacks.Count; i++)
                {
                    var stack = stacks[i];
                    if (i > 0)
                    {
                        var n = nodes.Add(stack.Name, stack.Name);
                        n.Expand();
                        nodes = n.Nodes;
                        lastNode = n;
                    }
                    foreach (var item in stack.GetHistoryItems())
                    {
                        lastNode = nodes.Add(item.Name, item.Name);
                    }
                }
            }

            HistoryView.EndUpdate();
            if (lastNode != null) lastNode.EnsureVisible();
        }
    }
}
