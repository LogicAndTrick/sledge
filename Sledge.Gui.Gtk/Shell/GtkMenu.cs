using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Gtk;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkMenu : MenuBar, IMenu
    {
        public IList<IMenuItem> Items { get; private set; }

        public GtkMenu()
        {
            var list = new ObservableCollection<IMenuItem>();
            list.CollectionChanged += CollectionChanged;
            Items = list;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems.OfType<Widget>())
                {
                    Remove(rem);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var add in e.NewItems.OfType<Widget>())
                {
                    Append(add);
                }
            }
        }

        public IMenuItem AddMenuItem(string key, string text = null)
        {
            var item = new GtkMenuItem(key, text);
            Items.Add(item);
            item.Show();
            return item;
        }
    }
}