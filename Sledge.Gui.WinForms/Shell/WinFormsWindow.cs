using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.WinForms.Containers;
using Sledge.Gui.WinForms.Controls;
using HandledEventArgs = Sledge.Gui.Events.HandledEventArgs;
using Size = System.Drawing.Size;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsWindow : WinFormsControl, IWindow
    {
        internal Form Form { get; private set; }

        public new bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                _autoSize = value;
                OnPreferredSizeChanged();
            }
        }

        protected WinFormsCellContainerWrapper ContainerWrapper;
        private bool _autoSize;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public ICell Container
        {
            get { return ContainerWrapper; }
        }

        protected override Structures.Size DefaultPreferredSize
        {
            get { return new Structures.Size(200, 200); }
        }

        public new Structures.Size PreferredSize
        {
            get { return ContainerWrapper.PreferredSize; }
            set { ContainerWrapper.PreferredSize = value; }
        }

        public WinFormsWindow() : base(new Form())
        {
            Form = (Form) Control;
            Form.Size = new Size(800, 600);
            CreateWrapper();
            Form.Resize += OnResize;
            Form.Closing += OnClosing;
        }

        protected virtual void CreateWrapper()
        {
            var panel = new Panel {Dock = DockStyle.Fill};
            Form.Controls.Add(panel);
            ContainerWrapper = new WinFormsCellContainerWrapper(panel);
            ContainerWrapper.PreferredSizeChanged += ContainerPreferredSizeChanged;
        }

        private void ContainerPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            OnActualSizeChanged();
        }

        protected override void OnPreferredSizeChanged()
        {
            if (_autoSize)
            {
                var ps = PreferredSize;
                Form.ClientSize = new Size(ps.Width, ps.Height);
            }
            base.OnPreferredSizeChanged();
        }

        public void Open()
        {
            Form.Show();
        }

        public void Close()
        {
            Form.Close();
        }

        public event EventHandler WindowLoaded
        {
            add { Form.Shown += value; }
            remove { Form.Shown -= value; }
        }

        public event EventHandler<HandledEventArgs> WindowClosing;

        public event EventHandler WindowClosed
        {
            add { Form.Closed += value; }
            remove { Form.Closed -= value; }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (WindowClosing != null)
            {
                var hea = new HandledEventArgs();
                WindowClosing(this, hea);
                if (hea.Handled) e.Cancel = true;
            }
        }
    }
}