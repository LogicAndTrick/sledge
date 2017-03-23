using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
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
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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

        private async void UpdateFilter()
        {
            SearchResults.Controls.Clear();

            if (String.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchResults.Controls.Add(new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.LightGray,
                    Font = new Font(Font, FontStyle.Italic),
                    Text = "Search results will appear here"
                });
                return;
            }
            var result = await Gimme.Fetch<IActivator>("shell://commandbox", SearchBox.Text.Split(' ').ToList()).ToList().ToTask();

            foreach (var x in result)
            {
                SearchResults.Controls.Add(new Button
                {
                    Text = x.Name
                });
            }
        }
    }
}
