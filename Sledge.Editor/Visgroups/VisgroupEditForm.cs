using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Visgroups
{
    public partial class VisgroupEditForm : Form
    {
        public bool NeedReload { get; set; }
        public Document Document { get; set; }

        public VisgroupEditForm(Document doc)
        {
            Document = doc;
            InitializeComponent();
            UpdateVisgroups();
            NeedReload = false;
        }

        private void UpdateVisgroups()
        {
            VisgroupPanel.Update(Document.Map.Visgroups);
        }

        private void SelectionChanged(object sender, int? visgroupId)
        {
            ColourPanel.Enabled = RemoveButton.Enabled = GroupName.Enabled = visgroupId.HasValue;
            ColourPanel.BackColor = SystemColors.Control;
            if (visgroupId.HasValue)
            {
                var visgroup = Document.Map.Visgroups.First(x => x.ID == visgroupId.Value);
                GroupName.Text = visgroup.Name;
                ColourPanel.BackColor = visgroup.Colour;
            } 
            else
            {
                GroupName.Text = "";
            }
        }

        private void AddGroup(object sender, EventArgs e)
        {
            var newGroup = new Visgroup
                               {
                                   ID = Document.Map.Visgroups.Any() ? Document.Map.Visgroups.Max(x => x.ID) + 1 : 1,
                                   Colour = Colour.GetRandomLightColour(),
                                   Name = "New Group",
                                   Visible = true
                               };
            Document.Map.Visgroups.Add(newGroup);
            UpdateVisgroups();
            VisgroupPanel.SetSelectedVisgroup(newGroup.ID);
            GroupName.SelectAll();
            GroupName.Focus();
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            Document.Map.Visgroups.RemoveAll(x => x.ID == id.Value);
            var collect = Document.Map.WorldSpawn.Find(x => x.IsInVisgroup(id.Value));
            if (collect.Any())
            {
                NeedReload = true;
                collect.ForEach(x =>
                                    {
                                        x.Visgroups.Remove(id.Value);
                                        x.IsVisgroupHidden = false;
                                    });
            }
            UpdateVisgroups();
        }

        private void GroupNameChanged(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            var vg = Document.Map.Visgroups.First(x => x.ID == id.Value);
            if (vg.Name == GroupName.Text) return;
            vg.Name = GroupName.Text;
            VisgroupPanel.UpdateVisgroupName(id.Value, GroupName.Text);
        }

        private void ColourClicked(object sender, EventArgs e)
        {
            var id = VisgroupPanel.GetSelectedVisgroup();
            if (!id.HasValue) return;
            var vg = Document.Map.Visgroups.First(x => x.ID == id.Value);
            using (var cp = new ColorDialog() {Color = vg.Colour})
            {
                if (cp.ShowDialog() == DialogResult.OK)
                {
                    vg.Colour = cp.Color;
                    VisgroupPanel.UpdateVisgroupColour(id.Value, cp.Color);
                }
            }
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
