using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Settings;
using Sledge.Settings.Models;

namespace Sledge.Editor
{
    public partial class GameSelectionForm : Form
    {
        private class IDStringWrapper
        {
            public int ID { get; set; }
            public string String { get; set; }
            public override string ToString()
            {
                return String;
            }
        }

        public int SelectedGameID { get; set; }

        public GameSelectionForm()
        {
            InitializeComponent();
            SelectedGameID = -1;
        }

        private void GameSelectionFormLoad(object sender, EventArgs e)
        {
            GameTable.RowStyles.Clear();
            foreach (var g in SettingsManager.Games.GroupBy(x => x.Engine).OrderBy(x => (int)x.Key))
            {
                GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
                GameTable.Controls.Add(new Label{Text = g.Key.ToString(), Font = new Font(Font, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft});
                GameTable.Controls.Add(new Label {Text = ""});
                foreach (var game in g)
                {
                    GameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                    GameTable.Controls.Add(new Label{Text = game.Name,Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft});
                    var btn = new Button {Text = ">>", Width = 40};
                    var btnGame = game;
                    btn.Click += (s, ev) => SelectGame(btnGame);
                    GameTable.Controls.Add(btn);
                }
            }

            if (!SettingsManager.Games.Any())
            {
                if (MessageBox.Show(
                    "You must set up a game configuration before you can create a map. Would you like to set one up now?",
                    "No Game Configuration Found",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Mediator.Publish(EditorMediator.OpenSettings);
                }
                Close();
            }
        }

        private void SelectGame(Game game)
        {
            SelectedGameID = game.ID;
            Close();
        }
    }
}
