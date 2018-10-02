namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    partial class VisgroupSidebarPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnShowAll = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.VisgroupPanel = new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowAll
            // 
            this.btnShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowAll.Location = new System.Drawing.Point(104, 188);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(95, 22);
            this.btnShowAll.TabIndex = 5;
            this.btnShowAll.Tag = "ShowAll";
            this.btnShowAll.Text = "Show All";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.ShowAllButtonClicked);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelect.Location = new System.Drawing.Point(3, 188);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(95, 22);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Tag = "Select";
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.SelectButtonClicked);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(3, 165);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(95, 22);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Tag = "Edit";
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.EditButtonClicked);
            // 
            // VisgroupPanel
            // 
            this.VisgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisgroupPanel.Location = new System.Drawing.Point(3, 3);
            this.VisgroupPanel.Name = "VisgroupPanel";
            this.VisgroupPanel.SelectedVisgroup = null;
            this.VisgroupPanel.ShowCheckboxes = true;
            this.VisgroupPanel.Size = new System.Drawing.Size(196, 160);
            this.VisgroupPanel.TabIndex = 8;
            this.VisgroupPanel.VisgroupToggled += new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel.VisgroupToggledEventHandler(this.VisgroupToggled);
            this.VisgroupPanel.VisgroupSelected += new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel.VisgroupSelectedEventHandler(this.VisgroupSelected);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNew.Location = new System.Drawing.Point(104, 165);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(95, 22);
            this.btnNew.TabIndex = 9;
            this.btnNew.Tag = "New";
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.NewButtonClicked);
            // 
            // VisgroupSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.VisgroupPanel);
            this.Controls.Add(this.btnShowAll);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnEdit);
            this.MaximumSize = new System.Drawing.Size(0, 250);
            this.MinimumSize = new System.Drawing.Size(200, 165);
            this.Name = "VisgroupSidebarPanel";
            this.Size = new System.Drawing.Size(200, 212);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnEdit;
        private Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel VisgroupPanel;
        private System.Windows.Forms.Button btnNew;
    }
}
