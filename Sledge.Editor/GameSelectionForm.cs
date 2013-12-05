using System;
using System.Linq;
using System.Windows.Forms;
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
            foreach (Engine engine in Enum.GetValues(typeof(Engine)))
            {
                lstEngine.Items.Add(engine);
            }
        }

        private void LstEngineSelectedIndexChanged(object sender, EventArgs ea)
        {
            lstGame.Items.Clear();
            if (lstEngine.SelectedIndex < 0) return;
            var e = (Engine) lstEngine.SelectedItem;
            foreach (var game in SettingsManager.Games.Where(g => g.Engine == e))
            {
                lstGame.Items.Add(new IDStringWrapper {ID = game.ID, String = game.Name});
            }
        }

        private void LstGameMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstGame.SelectedIndex < 0) return;
            SelectedGameID = ((IDStringWrapper) lstGame.SelectedItem).ID;
            Close();
        }
    }
}
