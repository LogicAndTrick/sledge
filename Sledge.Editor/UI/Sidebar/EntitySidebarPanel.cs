using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.Editor.Documents;
using Sledge.Settings;

namespace Sledge.Editor.UI.Sidebar
{
    public partial class EntitySidebarPanel : UserControl, IMediatorListener
    {
        public EntitySidebarPanel()
        {
            InitializeComponent();

            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
        }

        private void DocumentActivated(Document doc)
        {
            var selEnt = EntityTypeList.SelectedItem as GameDataObject;
            var sel = selEnt == null ? null : selEnt.Name;
            var def = doc.Game.DefaultPointEntity;
            GameDataObject reselect = null, redef = null;
            EntityTypeList.Items.Clear();
            foreach (var gdo in doc.GameData.Classes.Where(x => x.ClassType == ClassType.Point).OrderBy(x => x.Name.ToLowerInvariant()))
            {
                EntityTypeList.Items.Add(gdo);
                if (String.Equals(sel, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) reselect = gdo;
                if (String.Equals(def, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) redef = gdo;
            }
            if (reselect == null && redef == null) redef = doc.GameData.Classes
                .Where(x => x.ClassType == ClassType.Point)
                .OrderBy(x => x.Name.StartsWith("info_player_start") ? 0 : 1)
                .FirstOrDefault();
            EntityTypeList.SelectedItem = reselect ?? redef;
            SelectedEntityChanged(null, null);
        }

        private void DocumentAllClosed()
        {
            EntityTypeList.Items.Clear();
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        private void SelectedEntityChanged(object sender, EventArgs e)
        {
            Mediator.Publish(EditorMediator.EntitySelected, EntityTypeList.SelectedItem as GameDataObject);
        }
    }
}
