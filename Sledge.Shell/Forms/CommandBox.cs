using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Gimme;
using Sledge.Shell.Commands;

namespace Sledge.Shell.Forms
{
    public partial class CommandBox : Form
    {
        public CommandBox()
        {
            InitializeComponent();
            DoubleBuffered = true;
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
        }

        private void SearchBoxTextChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private static readonly Color ButtonBackColour = Color.FromArgb(255, 188, 188, 255);
        private static readonly Color ButtonOverColour = Color.FromArgb(255, 154, 154, 255);

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
                        Anchor = AnchorStyles.Left | AnchorStyles.Right
                    });
                    return;
                }
                var result = await Gimme.Fetch<IActivator>("shell://commandbox", SearchBox.Text.Split(' ').ToList())
                    .ToList()
                    .ToTask();

                foreach (var x in result)
                {
                    var btn = new Button
                    {
                        BackColor = ButtonBackColour,
                        Text = x.Name,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        FlatStyle = FlatStyle.Flat,
                        FlatAppearance =
                        {
                            BorderColor = ButtonOverColour,
                            BorderSize = 1,
                            MouseOverBackColor = ButtonOverColour
                        }
                    };
                    btn.Click += async (s, e) =>
                    {
                        Hide();
                        await x.Activate();
                        Close();
                    };
                    SearchResults.Controls.Add(btn);
                }
            }
            finally
            {
                SearchResults.ResumeLayout();
            }
        }
    }
}
