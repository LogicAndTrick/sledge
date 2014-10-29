using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Gtk;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkToolbar : Toolbar, IToolbar
    {
        public IList<IToolbarItem> Items { get; private set; }

        public GtkToolbar()
        {
            var list = new ObservableCollection<IToolbarItem>();
            list.CollectionChanged += CollectionChanged;
            Items = list;
            ToolbarStyle = ToolbarStyle.Icons;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems.OfType<ToolItem>())
                {
                    Remove(rem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var add in e.NewItems.OfType<ToolItem>())
                {
                    Insert(add, -1);
                }
            }
        }

        public IToolbarItem AddToolbarItem(string identifier, string text = null)
        {
            var item = new GtkToolbarItem(identifier, text);
            Items.Add(item);
            item.Show();
            return item;
        }
    }
}