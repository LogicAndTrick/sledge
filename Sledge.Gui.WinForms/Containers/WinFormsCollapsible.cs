using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;
using Sledge.Gui.WinForms.Controls;
using Sledge.Gui.WinForms.Shell;

namespace Sledge.Gui.WinForms.Containers
{
    [ControlImplementation("WinForms")]
    public class WinFormsCollapsible : WinFormsContainer, ICollapsible
    {
        private readonly WinFormsSidebarHeader _header;
        private readonly Panel _contentPanel;

        private bool _hideHeading;
        private bool _isCollapsed;

        public string Identifier { get; set; }
        public bool CanCollapse { get; set; }

        public bool HideHeading
        {
            get { return _hideHeading; }
            set
            {
                if (_hideHeading == value) return;
                _hideHeading = value;
                OnPreferredSizeChanged();
            }
        }

        public bool IsCollapsed
        {
            get { return _isCollapsed; }
            set
            {
                if (_isCollapsed == value) return;
                _isCollapsed = value;
                OnPreferredSizeChanged();
            }
        }

        public string Text
        {
            get { return _header.Text; }
            set { _header.Text = value; }
        }

        public override Size PreferredSize
        {
            get
            {
                var ps = base.PreferredSize;
                if (IsCollapsed) ps.Height = 0;
                ps.Height += _header.Height;
                return ps;
            }
        }

        public WinFormsCollapsible() : base(new Panel())
        {
            var panel = (Panel) Control;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = false
            };
            _header = new WinFormsSidebarHeader
            {
                Text = "This is a test", 
                Expanded = true,
                Dock = DockStyle.Top
            };
            _header.Click += HeaderClicked;

            Control.Controls.Add(_contentPanel);
            Control.Controls.Add(_header);
        }

        private void HeaderClicked(object sender, EventArgs e)
        {
            IsCollapsed = !IsCollapsed;
            _header.Expanded = !IsCollapsed;
        }

        protected override void CalculateLayout()
        {
            if (NumChildren != 1) return;

            Control.SuspendLayout();

            var child = Children[0];
            child.Control.Dock = DockStyle.Fill;
            _contentPanel.Height = IsCollapsed ? 0 : child.PreferredSize.Height;

            Control.ResumeLayout();
        }

        protected override void AppendChild(int index, WinFormsControl child)
        {
            _contentPanel.Controls.Add(child.Control);
        }

        public void Set(IControl child)
        {
            Insert(0, child);
        }
    }
}
