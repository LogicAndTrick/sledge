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
        }

        private void MoveToWorldButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToWorld);
        }

        private void TieToEntityButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.TieToEntity);
        }

        private void DocumentActivated(Document doc)
        {
            var selEnt = EntityTypeList.SelectedItem;
            var def = doc.Game.DefaultPointEntity;
            EntityTypeList.Items.Clear();
            foreach (var gdo in doc.GameData.Classes.Where(x => x.ClassType == ClassType.Point).OrderBy(x => x.Name.ToLowerInvariant()))
            {
                EntityTypeList.Items.Add(gdo);
                if (selEnt == null && gdo.Name == def) selEnt = gdo;
            }
            if (selEnt == null) selEnt = doc.GameData.Classes
                .Where(x => x.ClassType == ClassType.Point)
                .OrderBy(x => x.Name.StartsWith("info_player_start") ? 0 : 1)
                .FirstOrDefault();
            EntityTypeList.SelectedItem = selEnt;
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}
