using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    public sealed partial class VisgroupTab
    {
        private void InitializeComponent()
        {
            this.btnEditVisgroups = new System.Windows.Forms.Button();
            this.lblMemberOfGroup = new System.Windows.Forms.Label();
            this.visgroupPanel = new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel();
            this.SuspendLayout();
            // 
            // btnEditVisgroups
            // 
            this.btnEditVisgroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditVisgroups.Location = new System.Drawing.Point(486, 442);
            this.btnEditVisgroups.Name = "btnEditVisgroups";
            this.btnEditVisgroups.Size = new System.Drawing.Size(98, 23);
            this.btnEditVisgroups.TabIndex = 5;
            this.btnEditVisgroups.Text = "Edit Visgroups";
            this.btnEditVisgroups.UseVisualStyleBackColor = true;
            this.btnEditVisgroups.Click += new System.EventHandler(this.EditVisgroupsClicked);
            // 
            // lblMemberOfGroup
            // 
            this.lblMemberOfGroup.Location = new System.Drawing.Point(3, 0);
            this.lblMemberOfGroup.Name = "lblMemberOfGroup";
            this.lblMemberOfGroup.Size = new System.Drawing.Size(103, 20);
            this.lblMemberOfGroup.TabIndex = 4;
            this.lblMemberOfGroup.Text = "Member of group:";
            this.lblMemberOfGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // visgroupPanel
            // 
            this.visgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.visgroupPanel.Location = new System.Drawing.Point(3, 23);
            this.visgroupPanel.Name = "visgroupPanel";
            this.visgroupPanel.SelectedVisgroup = null;
            this.visgroupPanel.ShowCheckboxes = true;
            this.visgroupPanel.Size = new System.Drawing.Size(581, 413);
            this.visgroupPanel.TabIndex = 6;
            this.visgroupPanel.VisgroupToggled += new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel.VisgroupToggledEventHandler(this.VisgroupToggled);
            // 
            // VisgroupTab
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.visgroupPanel);
            this.Controls.Add(this.btnEditVisgroups);
            this.Controls.Add(this.lblMemberOfGroup);
            this.Name = "VisgroupTab";
            this.Size = new System.Drawing.Size(587, 468);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnEditVisgroups;
        private System.Windows.Forms.Label lblMemberOfGroup;
        private Visgroup.VisgroupPanel visgroupPanel;
    }
}
