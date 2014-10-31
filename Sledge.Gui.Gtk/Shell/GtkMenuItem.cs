using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Gtk;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkMenuItem : ImageMenuItem, IMenuItem
    {
        public string TextKey { get; set; }

        public string Text
        {
            get { return _textLabel.Text; }
            set { _textLabel.TextWithMnemonic = value; }
        }

        public System.Drawing.Image Icon
        {
            set
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }
                if (value != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        value.Save(ms, ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        Image = new global::Gtk.Image(ms);
                    }
                }
            }
        }

        public IList<IMenuItem> SubItems { get; private set; }
        public event EventHandler Clicked;

        private Menu _subMenu;
        private readonly AccelLabel _textLabel;

        public GtkMenuItem(string textKey, string text)
        {
            text = text ?? UIManager.Manager.StringProvider.Fetch(textKey);
            TextKey = textKey;
            var list = new ObservableCollection<IMenuItem>();
            list.CollectionChanged += CollectionChanged;
            SubItems = list;

            _textLabel = new AccelLabel("") {TextWithMnemonic = text};
            _textLabel.SetAlignment(0.0f, 0.5f);
            Add(_textLabel);
            _textLabel.AccelWidget = this;
            ShowAll();
        }

        protected override void OnActivated()
        {
            if (Clicked != null) Clicked(this, EventArgs.Empty);
            base.OnActivated();
        }

        public override void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
            base.Dispose();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && _subMenu != null)
            {
                foreach (var rem in e.OldItems.OfType<Widget>())
                {
                    _subMenu.Remove(rem);
                }
            }
            if (e.NewItems != null)
            {
                if (_subMenu == null)
                {
                    _subMenu = new Menu();
                    Submenu = _subMenu;
                    _subMenu.Show();
                }
                foreach (var add in e.NewItems.OfType<Widget>())
                {
                    _subMenu.Append(add);
                }
            }
            if (_subMenu != null && _subMenu.Children.Length == 0)
            {
                _subMenu.Detach();
                _subMenu.Dispose();
                _subMenu = null;
                Submenu = null;
            }
        }

        public IMenuItem AddSubMenuItem(string key, string text = null)
        {
            var item = new GtkMenuItem(key, text);
            SubItems.Add(item);
            item.Show();
            return item;
        }
    }
}