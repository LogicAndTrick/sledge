using System;
using System.Drawing;

namespace Sledge.Gui.Interfaces
{
    public class ComboBoxItem
    {
        public virtual bool HasImage { get { return Image != null; } }
        public virtual Image Image { get; set; }
        public virtual int ImageWidth { get { return Image != null ? Image.Width : 0; } }
        public virtual int ImageHeight { get { return Image != null ? Image.Height : 0; } }
        public bool DisposeImage { get { return false; } }

        public virtual bool DrawBorder { get; set; }

        public virtual string Text { get; set; }
        public virtual string DisplayText { get; set; }
        public virtual object Value { get; set; }

        public static implicit operator ComboBoxItem(string text)
        {
            return new ComboBoxItem { Text = text };
        }

        public override string ToString()
        {
            return Text ?? (Value != null ? Value.ToString() : String.Empty);
        }
    }
}