using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkLabel : GtkTextControl, ILabel
    {
        public GtkLabel() : base(new Label(""))
        {
        }

        protected override string ControlText
        {
            get { return ((Label)Control).Text; }
            set { ((Label)Control).Text = value; }
        }
    }

    public abstract class GtkTextControl : GtkControl, ITextControl
    {
        protected GtkTextControl(Widget control) : base(control)
        {
            global::Gtk.Entry x;
        }

        private string _textKey;

        protected abstract string ControlText { get; set; }

        public string TextKey
        {
            get { return _textKey; }
            set
            {
                _textKey = value;
                ControlText = UIManager.Manager.StringProvider.Fetch(_textKey);
            }
        }

        public virtual string Text
        {
            get { return ControlText; }
            set
            {
                ControlText = value;
                _textKey = null;
            }
        }

        private bool _bold;
        private bool _italic;

        public virtual int FontSize
        {
            get { return Control.PangoContext.FontDescription.Size; }
        }

        public virtual bool Bold
        {
            get { return _bold; }
            set { _bold = value; } // todo
        }

        public virtual bool Italic
        {
            get { return _italic; }
            set { _italic = value; } // todo
        }
        public event EventHandler TextChanged;
    }
}
