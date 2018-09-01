using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Shell.Commands;

namespace Sledge.Shell.Forms
{
    public partial class CommandBox : Form
    {
        private int _activeComponentIndex = 0;
        private List<IActivatorProvider> _providers;

        public CommandBox()
        {
            InitializeComponent();
            _providers = Common.Container.GetMany<IActivatorProvider>().ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateFilter();
        }

        protected override void OnShown(EventArgs e)
        {
            SearchBox.Focus();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            Close();
        }

        private void SearchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                var max = SearchResults.Controls.OfType<CommandItem>().Count() - 1;
                if (_activeComponentIndex < max) _activeComponentIndex++;
                UpdateActiveComponent();
            }
            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                if (_activeComponentIndex > 0) _activeComponentIndex--;
                UpdateActiveComponent();
            }
            if (e.KeyCode == Keys.Enter)
            {
                var list = SearchResults.Controls.OfType<CommandItem>().ToList();
                if (list.Count > _activeComponentIndex)
                {
                    var idx = list[_activeComponentIndex];
                    Activate(idx.Activator);
                }
            }
        }

        private void SearchBoxTextChanged(object sender, EventArgs e)
        {
            _activeComponentIndex = 0;
            UpdateFilter();
        }

        private static readonly Color ButtonBackColour = Color.FromArgb(255, 234, 240, 255);
        private static readonly Color ButtonOverColour = Color.FromArgb(255, 253, 244, 191);
        private static readonly Color ButtonOverBorderColour = Color.FromArgb(255, 229, 195, 101);

        private async void UpdateFilter()
        {
            try
            {
                SearchResults.SuspendLayout();

                SearchResults.Controls.Clear();

                if (String.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    SearchResults.Controls.Add(new Label
                    {
                        Text = "Search results will appear here",
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        TextAlign = ContentAlignment.MiddleCenter
                    });
                    return;
                }

                var keywords = SearchBox.Text.Trim();
                var result = _providers.SelectMany(p => p.SearchActivators(keywords)).Take(10).ToList();

                foreach (var x in result)
                {
                    var btn = new CommandItem(x, this);
                    SearchResults.Controls.Add(btn);
                }
            }
            finally
            {
                UpdateActiveComponent();
                SearchResults.ResumeLayout();
            }
        }

        private void SetActiveComponent(CommandItem item)
        {
            _activeComponentIndex = SearchResults.Controls.OfType<CommandItem>().ToList().IndexOf(item);
            UpdateActiveComponent();
        }

        private void UpdateActiveComponent()
        {
            var list = SearchResults.Controls.OfType<CommandItem>().ToList();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Active = i == _activeComponentIndex;
            }
        }

        private async Task Activate(IActivator activator)
        {
            Hide();
            await activator.Activate();
            Close();
        }

        private class CommandItem : UserControl
        {
            public IActivator Activator { get; }

            private readonly CommandBox _owner;
            private bool _active;
            private Label _label;

            public bool Active
            {
                get { return _active; }
                set
                {
                    _active = value;
                    UpdateActive();
                }
            }

            public CommandItem(IActivator activator, CommandBox owner)
            {
                Activator = activator;
                _owner = owner;
                _active = false;
                
                Anchor = AnchorStyles.Left | AnchorStyles.Right;
                Height = 25;
                BackColor = ButtonBackColour;
                Padding = new Padding(1);
                Margin = Padding.Empty;

                _label = new Label
                {
                    Text = activator.Name,
                    Dock = DockStyle.Fill,
                    BackColor = ButtonBackColour,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                Controls.Add(_label);

                _label.MouseEnter += (s, e) => { _owner.SetActiveComponent(this); };
                _label.Click += async (s, e) => { await _owner.Activate(Activator); };
            }

            private void UpdateActive()
            {
                _label.BackColor = _active ? ButtonOverColour : ButtonBackColour;
                BackColor = _active ? ButtonOverBorderColour : ButtonBackColour;
            }
        }
    }
}
