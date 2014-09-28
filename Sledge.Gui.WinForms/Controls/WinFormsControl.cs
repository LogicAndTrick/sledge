using System;
using System.Windows.Forms;
using Sledge.Gui.Controls;

namespace Sledge.Gui.WinForms.Controls
{
    public abstract class WinFormsControl : IControl
    {
        private Control _control;

        public Control Control
        {
            get { return _control; }
            private set
            {
                if (_control != null) _control.Resize -= ControlResized;
                _control = value;
                if (_control != null) _control.Resize += ControlResized;
            }
        }

        private void ControlResized(object sender, EventArgs e)
        {
            OnActualSizeChanged();
        }

        public object BindingSource { get; set; }

        public Size ActualSize
        {
            get { return new Size(Control.Width, Control.Height); }
        }

        public abstract Size PreferredSize { get; }

        public event EventHandler ActualSizeChanged;
        public event EventHandler PreferredSizeChanged;

        protected virtual void OnActualSizeChanged()
        {
            if (ActualSizeChanged != null)
            {
                ActualSizeChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPreferredSizeChanged()
        {
            if (PreferredSizeChanged != null)
            {
                PreferredSizeChanged(this, EventArgs.Empty);
            }
        }

        protected WinFormsControl(Control control)
        {
            Control = control;
        }
    }
}