using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sledge.Gui.Controls;
using Sledge.Gui.Shell;
using Sledge.Gui.WinForms.Controls;
using Size = System.Drawing.Size;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsWindow : Form, IWindow
    {
        public new bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                _autoSize = value;
                OnPreferredSizeChanged();
            }
        }

        public object BindingSource { get; set; }

        protected WinFormsCellContainerWrapper _containerWrapper;
        private bool _autoSize;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public new ICell Container
        {
            get { return _containerWrapper; }
        }

        public Gui.Controls.Size ActualSize
        {
            get { return new Gui.Controls.Size(Width, Height); }
        }

        public new Gui.Controls.Size PreferredSize
        {
            get { return _containerWrapper.PreferredSize; }
        }

        public WinFormsWindow()
        {
            Size = new Size(800, 600);
            CreateWrapper();
        }

        protected virtual void CreateWrapper()
        {
            var panel = new Panel {Dock = DockStyle.Fill};
            Controls.Add(panel);
            _containerWrapper = new WinFormsCellContainerWrapper(panel);
            _containerWrapper.PreferredSizeChanged += ContainerPreferredSizeChanged;
        }

        private void ContainerPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        protected override void OnResize(EventArgs e)
        {
            OnActualSizeChanged();
            base.OnResize(e);
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
            if (_autoSize)
            {
                var ps = PreferredSize;
                this.ClientSize = new Size(ps.Width, ps.Height);
            }
            if (PreferredSizeChanged != null)
            {
                PreferredSizeChanged(this, EventArgs.Empty);
            }
        }

        public void Open()
        {
            Show();
        }

        public event EventHandler WindowLoaded
        {
            add { Shown += value; }
            remove { Shown -= value; }
        }

        public event EventHandler<HandledEventArgs> WindowClosing;

        public event EventHandler WindowClosed
        {
            add { Closed += value; }
            remove { Closed -= value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (WindowClosing != null)
            {
                var hea = new HandledEventArgs();
                WindowClosing(this, hea);
                if (hea.Handled) e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}