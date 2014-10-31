using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gtk;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkToolbarItem : ToolButton, IToolbarItem
    {
        public string TextKey { get; set; }

        public string Text
        {
            get { return Label; }
            set { Label = value; }
        }

        public System.Drawing.Image Icon
        {
            set
            {
                if (IconWidget != null)
                {
                    IconWidget.Dispose();
                    IconWidget = null;
                }
                if (value != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        value.Save(ms, ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        IconWidget = new global::Gtk.Image(ms);
                        IconWidget.Show();
                    }
                }
            }
        }

        public new  event EventHandler Clicked;

        public GtkToolbarItem(string textKey, string text) : base(null, text)
        {
            text = text ?? UIManager.Manager.StringProvider.Fetch(textKey);
            TextKey = textKey;
            Text = text;
        }

        protected override void OnClicked()
        {
            if (Clicked != null) Clicked(this, EventArgs.Empty);
            base.OnClicked();
        }

        public override void Dispose()
        {
            if (IconWidget != null)
            {
                IconWidget.Dispose();
                IconWidget = null;
            }
            base.Dispose();
        }
    }
}