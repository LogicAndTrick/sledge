using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Controls;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces;
using MouseEventHandler = Sledge.Gui.Events.MouseEventHandler;
using Size = Sledge.Gui.Interfaces.Size;

namespace Sledge.Gui.WinForms.Controls
{
    public abstract class WinFormsControl : ITextControl
    {
        private Control _control;
        private Size _preferredSize;

        public Control Control
        {
            get { return _control; }
            private set
            {
                if (_control != null) _control.Resize -= ControlResized;
                _control = value;
                if (_control != null)
                {
                    _control.Resize += ControlResized;
                    _control.AutoSize = false;
                }
            }
        }

        public IControl Implementation
        {
            get { return this; }
        }

        public string Text
        {
            get { return _control.Text; }
            set { _control.Text = value; }
        }

        private bool _bold;
        private bool _italic;

        public int FontSize
        {
            get { return (int) _control.Font.GetHeight(); }
        }

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; } // todo
        }

        public bool Italic
        {
            get { return _italic; }
            set { _italic = value; } // todo
        }

        public bool Enabled
        {
            get { return _control.Enabled; }
            set { _control.Enabled = value; }
        }

        public bool Focused
        {
            get { return _control.Focused; }
        }

        #region Events
        public event MouseEventHandler MouseDown
        {
            add { _control.MouseDown += ConvertDelegate(value, true); }
            remove { _control.MouseDown -= ConvertDelegate(value, false); }
        }

        public event MouseEventHandler MouseUp
        {
            add { _control.MouseUp += ConvertDelegate(value, true); }
            remove { _control.MouseUp -= ConvertDelegate(value, false); }
        }

        public event MouseEventHandler MouseWheel
        {
            add { _control.MouseWheel += ConvertDelegate(value, true); }
            remove { _control.MouseWheel -= ConvertDelegate(value, false); }
        }

        public event MouseEventHandler MouseMove
        {
            add { _control.MouseMove += ConvertDelegate(value, true); }
            remove { _control.MouseMove -= ConvertDelegate(value, false); }
        }

        public event MouseEventHandler MouseClick
        {
            add { _control.MouseClick += ConvertDelegate(value, true); }
            remove { _control.MouseClick -= ConvertDelegate(value, false); }
        }

        public event EventHandler MouseDoubleClick
        {
            add { _control.DoubleClick += value; }
            remove { _control.DoubleClick -= value; }
        }

        public event EventHandler MouseEnter
        {
            add { _control.MouseEnter += value; }
            remove { _control.MouseEnter -= value; }
        }

        public event EventHandler MouseLeave
        {
            add { _control.MouseLeave += value; }
            remove { _control.MouseLeave -= value; }
        }

        public event EventHandler Click
        {
            add { _control.Click += value; }
            remove { _control.Click -= value; }
        }

        public event EventHandler TextChanged
        {
            add { _control.TextChanged += value; }
            remove { _control.TextChanged -= value; }
        }

        private readonly Dictionary<Delegate, Delegate> _delegateCache = new Dictionary<Delegate, Delegate>();

        protected System.Windows.Forms.MouseEventHandler ConvertDelegate(MouseEventHandler value, bool adding)
        {
            if (!_delegateCache.ContainsKey(value)) _delegateCache.Add(value, value.ToMouseEventHandler(this));
            var val = (System.Windows.Forms.MouseEventHandler) _delegateCache[value];
            if (!adding) _delegateCache.Remove(value);
            return val;
        }
        
        #endregion

        protected abstract Size DefaultPreferredSize { get; }

        public virtual Size PreferredSize
        {
            get
            {
                var ps = _preferredSize;
                var dps = DefaultPreferredSize;
                return new Size(ps.Width == 0 ? dps.Width : ps.Width, ps.Height == 0 ? dps.Height : ps.Height);
            }
            set
            {
                _preferredSize = value;
                OnPreferredSizeChanged();
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