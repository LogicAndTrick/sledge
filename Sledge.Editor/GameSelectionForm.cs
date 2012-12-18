using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Database;

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
            foreach (var engine in Context.DBContext.Engines)
            {
                lstEngine.Items.Add(new IDStringWrapper { ID = engine.ID, String = engine.Name });
            }
        }

        private void LstEngineSelectedIndexChanged(object sender, EventArgs ea)
        {
            lstGame.Items.Clear();
            if (lstEngine.SelectedIndex < 0) return;
            foreach (var game in Context.DBContext.Engines.Single(e => e.ID == ((IDStringWrapper)lstEngine.SelectedItem).ID).Games)
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
