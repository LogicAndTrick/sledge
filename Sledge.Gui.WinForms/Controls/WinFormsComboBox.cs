using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Binding = Sledge.Gui.Bindings.Binding;
using Rectangle = System.Drawing.Rectangle;
using Size = Sledge.Gui.Interfaces.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsComboBox : WinFormsControl, IComboBox
    {
        private readonly ComboBox _combo;
        private readonly ItemList<ComboBoxItem> _items;
        private int _numImages;
        private int _numBorders;

        public WinFormsComboBox() : base(new ComboBox())
        {
            _combo = (ComboBox) Control;
            _items = new ItemList<ComboBoxItem>();
            _items.CollectionChanged += CollectionChanged;
            _numImages = 0;
            MaxHeight = 64;

            _combo.DrawItem += OwnerDrawItem;
            _combo.MeasureItem += OwnerMeasureItem;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _combo.BeginUpdate();
            var befImages = _numImages > 0;
            var befBorder = _numBorders > 0;
            if (e.OldItems != null)
            {
                foreach (ComboBoxItem rem in e.OldItems)
                {
                    _combo.Items.Remove(rem);
                    if (rem.HasImage) _numImages--;
                    if (rem.DrawBorder) _numBorders--;
                }
            }

            if (e.NewItems != null)
            {
                var idx = e.NewStartingIndex;
                foreach (ComboBoxItem add in e.NewItems)
                {
                    _combo.Items.Insert(idx, add);
                    idx++;
                    if (add.HasImage) _numImages++;
                    if (add.DrawBorder) _numBorders++;
                }
            }
            var aftImages = _numImages > 0;
            var aftBorder = _numBorders > 0;
            if (befImages != aftImages || befBorder != aftBorder)
            {
                _combo.DrawMode = _numImages > 0 ? DrawMode.OwnerDrawVariable : (_numBorders > 0 ? DrawMode.OwnerDrawVariable : DrawMode.Normal);
            }
            _combo.EndUpdate();
        }
        
        protected override void ApplyBinding(Binding binding)
        {
            switch (binding.TargetProperty)
            {
                case "SelectedItem":
                    ApplyManualEventBinding(binding, GetInheritedBindingSource(), "SelectedIndexChanged");
                    return;
            }
            base.ApplyBinding(binding);
        }

        protected override Size DefaultPreferredSize
        {
            get { return new Size(150, FontSize * 2); }
        }

        public ComboBoxItem SelectedItem
        {
            get { return (ComboBoxItem) _combo.SelectedItem; }
            set { _combo.SelectedItem = value; }
        }

        public int SelectedIndex
        {
            get { return _combo.SelectedIndex; }
            set { _combo.SelectedIndex = value; }
        }

        public int MaxHeight { get; set; }

        public ItemList<ComboBoxItem> Items
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

        public event EventHandler DropDownOpened
        {
            add { _combo.DropDown += value; }
            remove { _combo.DropDown -= value; }
        }

        public event EventHandler DropDownClosed
        {
            add { _combo.DropDownClosed += value; }
            remove { _combo.DropDownClosed -= value; }
        }

        private void OwnerMeasureItem(object sender, MeasureItemEventArgs e)
        {
            var item = (ComboBoxItem) _combo.Items[e.Index];
            int minHeight = _combo.ItemHeight, height = 0;
            if (item.HasImage)
            {
                var iw = item.ImageWidth;
                height = item.ImageHeight;

                if (iw > MaxHeight && iw >= height) height = (int) Math.Floor(MaxHeight * (height / (float) iw));
                else if (height > MaxHeight) height = MaxHeight;
                height += 9;
            }
            e.ItemHeight = Math.Max(minHeight, height);
        }

        private void OwnerDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var item = (ComboBoxItem) _combo.Items[e.Index];

            e.DrawBackground();
            OwnerDrawItem(e.Graphics, item, e.Bounds, e.ForeColor, e.Font);
            e.DrawFocusRectangle();
        }

        private void OwnerDrawItem(Graphics g, ComboBoxItem item, Rectangle bounds, Color textColour, Font font)
        {
            var imageSize = bounds.Height - 9;

            using (var brush = new SolidBrush(textColour))
            {
                var xAdd = item.HasImage ? 3 + imageSize : 0;
                g.DrawString(item.DisplayText ?? item.ToString(), font, brush, bounds.X + 3 + xAdd, bounds.Y + 1);
            }

            if (item.HasImage)
            {
                var iw = item.ImageWidth;
                var ih = item.ImageHeight;
                if (iw > imageSize && iw >= ih)
                {
                    ih = (int) Math.Floor(imageSize * (ih / (float) iw));
                    iw = imageSize;
                }
                else if (ih > imageSize)
                {
                    iw = (int) Math.Floor(imageSize * (iw / (float) ih));
                    ih = imageSize;
                }

                var img = item.Image;
                g.DrawImage(img, bounds.X + 3, bounds.Y + 3, iw, ih);
                if (item.DisposeImage) img.Dispose();
            }

            if (item.DrawBorder)
            {
                using (var pen = new Pen(textColour))
                {
                    var liney = bounds.Y + bounds.Height - 1;
                    g.DrawLine(pen, bounds.X, liney, bounds.X + bounds.Width, liney);
                }
            }
        }
    }
}