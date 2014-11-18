using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.WinForms.Controls;

namespace Sledge.Gui.WinForms.Containers
{
    [ControlImplementation("WinForms")]
    public class WinFormsVerticalScrollContainer : WinFormsContainer, IVerticalScrollContainer
    {
        private readonly Panel _contentPanel;
        private readonly VScrollBar _scrollBar;

        public WinFormsVerticalScrollContainer() : base(new Panel())
        {
            _contentPanel = new Panel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 0),
                Size = new Size(Control.Width, Control.Height),
                Margin = new Padding(0, 0, 0, 10),
            };
            _scrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                LargeChange = 100,
                SmallChange = 30
            };
            _scrollBar.ValueChanged += Scrolled;

            Control.Controls.Add(_scrollBar);
            Control.Controls.Add(_contentPanel);
        }

        private void Scrolled(object sender, EventArgs e)
        {
            _contentPanel.Location = new Point(0, -_scrollBar.Value);
        }

        protected override void OnPreferredSizeChanged()
        {
            if (!Paused) CalculateLayout();
            // Intentionally stop propagation
            // base.OnPreferredSizeChanged();
        }

        protected override void CalculateLayout()
        {
            if (NumChildren != 1) return;

            Control.SuspendLayout();

            var child = Children[0];
            child.Control.Dock = DockStyle.Fill;

            var height = child.PreferredSize.Height;
            if (height > ActualSize.Height)
            {
                _contentPanel.Size = new Size(ActualSize.Width - SystemInformation.VerticalScrollBarWidth, height);
                _scrollBar.Width = SystemInformation.VerticalScrollBarWidth;
                _scrollBar.Maximum = (height - ActualSize.Height) + _scrollBar.LargeChange - 1;
                if (_contentPanel.Location.Y + height < ActualSize.Height) _contentPanel.Location = new Point(0, ActualSize.Height - height);
            }
            else
            {
                _contentPanel.Location = new Point(0, 0);
                _scrollBar.Width = _scrollBar.Value = 0;
                _contentPanel.Size = new Size(ActualSize.Width, height);
            }

            child.Control.Height = height;

            Control.ResumeLayout();
        }

        protected override void AppendChild(int index, WinFormsControl child)
        {
            _contentPanel.Controls.Add(child.Control);
        }

        protected override void RemoveChild(WinFormsControl child)
        {
            _contentPanel.Controls.Remove(child.Control);
        }

        public void Set(IControl child)
        {
            Insert(0, child);
        }
    }
}