using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.Common;
using Sledge.Common.Translations;
using Sledge.Shell;
using Vg = Sledge.BspEditor.Primitives.MapData.Visgroup;

namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    public partial class VisgroupEditForm : Form, IManualTranslate
    {
        private readonly List<Vg> _visgroups;
        private readonly List<Vg> _deleted; 

        public VisgroupEditForm(MapDocument doc)
        {
            InitializeComponent();
            _visgroups = new List<Vg>(doc.Map.Data.Get<Vg>().Select(x => (Vg) x.Clone()));
            _deleted = new List<Vg>();
            UpdateVisgroups();
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                NameLabel.Text = strings.GetString(prefix, nameof(NameLabel));
                ColorLabel.Text = strings.GetString(prefix, nameof(ColorLabel));
                AddButton.Text = strings.GetString(prefix, nameof(AddButton));
                RemoveButton.Text = strings.GetString(prefix, nameof(RemoveButton));
                OkButton.Text = strings.GetString(prefix, nameof(OkButton));
                CancelButton.Text = strings.GetString(prefix, nameof(CancelButton));
            });
        }

        public void PopulateChangeLists(MapDocument doc, List<Vg> newVisgroups, List<Vg> changedVisgroups, List<Vg> deletedVisgroups)
        {
            foreach (var g in _visgroups)
            {
                var dg = doc.Map.Data.Get<Vg>().FirstOrDefault(x => x.ID == g.ID);
                if (dg == null) newVisgroups.Add(g);
                else if (dg.Name != g.Name || dg.Colour != g.Colour) changedVisgroups.Add(g);
            }
            deletedVisgroups.AddRange(_deleted.Where(x => doc.Map.Data.Get<Vg>().Any(y => y.ID == x.ID)));
        }

        private void UpdateVisgroups()
        {
            VisgroupPanel.Update(GetVisgroupItemList());
        }

        private IEnumerable<VisgroupItem> GetVisgroupItemList()
        {
            return _visgroups.Select(x => new VisgroupItem(x.Name)
            {
                Colour = x.Colour,
                Tag = x
            });
        }

        private void SelectionChanged(object sender, VisgroupItem visgroupItem)
        {
            ColourPanel.Enabled = RemoveButton.Enabled = GroupName.Enabled = visgroupItem != null;
            ColourPanel.BackColor = SystemColors.Control;
            if (visgroupItem != null)
            {
                var visgroup = (Vg) visgroupItem.Tag;
                GroupName.Text = visgroup.Name;
                ColourPanel.BackColor = visgroup.Colour;
                ColourPanel.ForeColor = visgroup.Colour.GetIdealForegroundColour();
            }
            else
            {
                GroupName.Text = "";
            }
        }

        private long GetNewID()
        {
            var ids = _visgroups.Select(x => x.ID).Union(_deleted.Select(x => x.ID)).ToList();
            return Math.Max(1, ids.Any() ? ids.Max() + 1 : 1);
        }

        private void AddGroup(object sender, EventArgs e)
        {
            var newGroup = new Vg
            {
                                   ID = GetNewID(),
                                   Colour = Colour.GetRandomLightColour(),
                                   Name = "New Group",
                                   Visible = true
                               };
            _visgroups.Add(newGroup);
            UpdateVisgroups();
            VisgroupPanel.SelectedVisgroup = new VisgroupItem("") { Tag = newGroup };
            GroupName.SelectAll();
            GroupName.Focus();
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            var visgroup = VisgroupPanel.SelectedVisgroup;
            if (visgroup == null) return;

            var vg = (Vg) visgroup.Tag;
            _visgroups.Remove(vg);
            _deleted.Add(vg);
            UpdateVisgroups();
        }

        private void GroupNameChanged(object sender, EventArgs e)
        {
            var visgroup = VisgroupPanel.SelectedVisgroup;
            if (visgroup == null) return;
            
            var vg = (Vg) visgroup.Tag;
            if (vg.Name == GroupName.Text) return;
            visgroup.Text = vg.Name = GroupName.Text;
            VisgroupPanel.UpdateVisgroupState(visgroup);
        }

        private void ColourClicked(object sender, EventArgs e)
        {
            var visgroup = VisgroupPanel.SelectedVisgroup;
            if (visgroup == null) return;
            
            var vg = (Vg) visgroup.Tag;
            using (var cp = new ColorDialog {Color = vg.Colour})
            {
                if (cp.ShowDialog() != DialogResult.OK) return;

                ColourPanel.BackColor = visgroup.Colour = vg.Colour = cp.Color;
                ColourPanel.ForeColor = vg.Colour.GetIdealForegroundColour();
                VisgroupPanel.UpdateVisgroupState(visgroup);
            }
        }

        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
