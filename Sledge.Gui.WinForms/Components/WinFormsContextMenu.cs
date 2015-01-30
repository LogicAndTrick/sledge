using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Components;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.WinForms.Controls;
using Sledge.Gui.WinForms.Shell;

namespace Sledge.Gui.WinForms.Components
{
    [ControlImplementation("WinForms")]
    public class WinFormsContextMenu : ContextMenuStrip, IContextMenu
    {
        public new event EventHandler Closed;
        public new IList<IMenuItem> Items { get; private set; }

        public WinFormsContextMenu()
        {
            var list = new ObservableCollection<IMenuItem>();
            list.CollectionChanged += CollectionChanged;
            Items = list;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems.OfType<ToolStripMenuItem>())
                {
                    base.Items.Remove(rem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var add in e.NewItems.OfType<ToolStripItem>())
                {
                    base.Items.Add(add);
                }
            }
        }

        public IMenuItem AddMenuItem(string key, string text = null)
        {
            var item = new WinFormsMenuItem(key, text);
            Items.Add(item);
            return item;
        }
        
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            if (Closed != null) Closed(this, EventArgs.Empty);
            base.OnClosed(e);
        }

        public void Open(IControl control, int x, int y)
        {
            Show(((WinFormsControl) control.Implementation).Control, x, y);
        }
    }
}