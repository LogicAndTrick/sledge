using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsToolbar : ToolStrip, IToolbar
    {
        public new IList<IToolbarItem> Items { get; private set; }

        public WinFormsToolbar()
        {
            var list = new ObservableCollection<IToolbarItem>();
            list.CollectionChanged += CollectionChanged;
            Items = list;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems.OfType<ToolStripItem>())
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

        public IToolbarItem AddToolbarItem(string identifier, string text = null)
        {
            var item = new WinFormsToolbarItem(identifier, text);
            Items.Add(item);
            return item;
        }
    }
}