using System;
using System.ComponentModel;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;

namespace Sledge.Shell.Forms
{
    public partial class Shell : Form
    {
        public Shell()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Bootstrapping.Startup().ContinueWith(Bootstrapping.Initialise);
            Closing += DoClosing;

            var btn = new Button();
            btn.Text = "Click Me";
            btn.Click += (sender, args) =>
            {
                Oy.Publish("Command:Run", new CommandMessage("TestCommand"));
            };
            DocumentContainer.Controls.Add(btn);
        }

        private async void DoClosing(object sender, CancelEventArgs e)
        {
            if (!await Bootstrapping.ShuttingDown())
            {
                e.Cancel = true;
                return;
            }
            Closing -= DoClosing;
            Enabled = false;
            e.Cancel = true;
            await Bootstrapping.Shutdown();
            Close();
        }
    }
}
