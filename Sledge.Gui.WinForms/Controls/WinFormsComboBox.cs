using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Size = Sledge.Gui.Interfaces.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsComboBox : WinFormsControl, IComboBox
    {
        private readonly ComboBox _combo;
        private readonly IList<object> _items;

        public WinFormsComboBox() : base(new ComboBox())
        {
            _combo = (ComboBox) Control;
            var items = new ObservableCollection<object>();
            items.CollectionChanged += CollectionChanged;
            _items = items;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var rem in e.OldItems)
                {
                    _combo.Items.Remove(rem);
                }
            }

            if (e.NewItems != null)
            {
                var idx = e.NewStartingIndex;
                foreach (var add in e.NewItems)
                {
                    _combo.Items.Insert(idx, add);
                    idx++;
                }
            }
        }

        protected override Size DefaultPreferredSize
        {
            get { return new Size(100, FontSize * 2); }
        }

        public object SelectedItem
        {
            get { return _combo.SelectedItem; }
            set { _combo.SelectedItem = value; }
        }

        public int SelectedIndex
        {
            get { return _combo.SelectedIndex; }
            set { _combo.SelectedIndex = value; }
        }

        public IList<object> Items
        {
            get { return _items; }
        }

        public event EventHandler SelectedItemChanged
        {
            add { _combo.SelectedValueChanged += value; }
            remove { _combo.SelectedValueChanged -= value; }
        }

        public event EventHandler SelectedIndexChanged
        {
            add { _combo.SelectedIndexChanged += value; }
            remove { _combo.SelectedIndexChanged -= value; }
        }
    }
}