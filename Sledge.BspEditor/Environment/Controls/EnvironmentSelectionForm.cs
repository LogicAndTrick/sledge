using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.BspEditor.Environment.Controls
{
    public partial class EnvironmentSelectionForm : Form
    {
        private readonly List<SerialisedEnvironment> _environments;
        public SerialisedEnvironment SelectedEnvironment { get; private set; }

        public EnvironmentSelectionForm(List<SerialisedEnvironment> environments)
        {
            _environments = environments;
            InitializeComponent();
            SelectedEnvironment = null;
        }

        private void GameSelectionFormLoad(object sender, EventArgs e)
        {
            GameTable.RowStyles.Clear();
            foreach (var g in _environments.GroupBy(x => x.Type).OrderBy(x => x.Key))
            {
                GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                GameTable.Controls.Add(new Label{Text = g.Key, Font = new Font(Font, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft});
                GameTable.Controls.Add(new Label {Text = ""});
                foreach (var game in g)
                {
                    GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                    GameTable.Controls.Add(new Label{Text = game.Name,Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft});
                    var btn = new Button {Text = @">>", Width = 40};
                    var btnGame = game;
                    btn.Click += (s, ev) => SelectGame(btnGame);
                    GameTable.Controls.Add(btn);
                }
            }

            if (!_environments.Any())
            {
                SelectedEnvironment = null;
                Close();
            }
        }

        private void SelectGame(SerialisedEnvironment environment)
        {
            DialogResult = DialogResult.OK;
            SelectedEnvironment = environment;
            Close();
        }
    }
}
