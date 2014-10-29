using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsMenuItem : ToolStripMenuItem, IMenuItem
    {
        public string TextKey { get; set; }
        public Image Icon { set { Image = value; } }
        public IList<IMenuItem> SubItems { get; private set; }
        public event EventHandler Clicked
        {
            add { Click += value; }
            remove { Click -= value; }
        }

        public WinFormsMenuItem(string textKey, string text)
        {
            text = text ?? UIManager.Manager.StringProvider.Fetch(textKey);
            base.Text = text;
            TextKey = textKey;
            var list = new ObservableCollection<IMenuItem>();
            list.CollectionChanged += CollectionChanged;
            SubItems = list;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems.OfType<ToolStripItem>())
                {
                    DropDownItems.Remove(rem);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var add in e.NewItems.OfType<ToolStripItem>())
                {
                    DropDownItems.Add(add);
                }
            }
        }

        public IMenuItem AddSubMenuItem(string key, string text = null)
        {
            var item = new WinFormsMenuItem(key, text);
            SubItems.Add(item);
            return item;
        }
    }
}