using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsMenu : MenuStrip, IMenu
    {
        public new IList<IMenuItem> Items { get; private set; }

        public WinFormsMenu()
        {
            var list = new ObservableCollection<IMenuItem>();
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

        public IMenuItem AddMenuItem(string identifier, string text)
        {
            var item = new WinFormsMenuItem(identifier, text);
            Items.Add(item);
            return item;
        }
    }
}