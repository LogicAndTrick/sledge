using System;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class TextControlBase<T> : ControlBase<T>, ITextControl where T : ITextControl
    {
        public string TextKey
        {
            get { return Control.TextKey; }
            set { Control.TextKey = value; }
        }

        public string Text
        {
            get { return Control.Text; }
            set { Control.Text = value; }
        }

        public int FontSize
        {
            get { return Control.FontSize; }
        }

        public bool Bold
        {
            get { return Control.Bold; }
            set { Control.Bold = value; }
        }

        public bool Italic
        {
            get { return Control.Italic; }
            set { Control.Italic = value; }
        }

        public event EventHandler TextChanged
        {
            add { Control.TextChanged += value; }
            remove { Control.TextChanged -= value; }
        }
    }
}