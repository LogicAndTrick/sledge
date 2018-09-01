namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    partial class VisgroupEditForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.VisgroupPanel = new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel();
            this.GroupName = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ColorLabel = new System.Windows.Forms.Label();
            this.ColourPanel = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // VisgroupPanel
            // 
            this.VisgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisgroupPanel.Location = new System.Drawing.Point(12, 12);
            this.VisgroupPanel.Name = "VisgroupPanel";
            this.VisgroupPanel.ShowCheckboxes = false;
            this.VisgroupPanel.Size = new System.Drawing.Size(233, 323);
            this.VisgroupPanel.TabIndex = 0;
            this.VisgroupPanel.VisgroupSelected += new Sledge.BspEditor.Editing.Components.Visgroup.VisgroupPanel.VisgroupSelectedEventHandler(this.SelectionChanged);
            // 
            // GroupName
            // 
            this.GroupName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupName.Enabled = false;
            this.GroupName.Location = new System.Drawing.Point(293, 15);
            this.GroupName.Name = "GroupName";
            this.GroupName.Size = new System.Drawing.Size(127, 20);
            this.GroupName.TabIndex = 1;
            this.GroupName.TextChanged += new System.EventHandler(this.GroupNameChanged);
            // 
            // NameLabel
            // 
            this.NameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(252, 18);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(35, 13);
            this.NameLabel.TabIndex = 2;
            this.NameLabel.Text = "Name";
            // 
            // ColorLabel
            // 
            this.ColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ColorLabel.AutoSize = true;
            this.ColorLabel.Location = new System.Drawing.Point(252, 47);
            this.ColorLabel.Name = "ColorLabel";
            this.ColorLabel.Size = new System.Drawing.Size(119, 13);
            this.ColorLabel.TabIndex = 3;
            this.ColorLabel.Text = "Colour (click to change)";
            // 
            // ColourPanel
            // 
            this.ColourPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ColourPanel.Enabled = false;
            this.ColourPanel.Location = new System.Drawing.Point(377, 42);
            this.ColourPanel.Name = "ColourPanel";
            this.ColourPanel.Size = new System.Drawing.Size(43, 22);
            this.ColourPanel.TabIndex = 4;
            this.ColourPanel.Click += new System.EventHandler(this.ColourClicked);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(345, 312);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 5;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Location = new System.Drawing.Point(251, 109);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(95, 23);
            this.AddButton.TabIndex = 5;
            this.AddButton.Text = "Add Group";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddGroup);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.Enabled = false;
            this.RemoveButton.Location = new System.Drawing.Point(251, 138);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(95, 23);
            this.RemoveButton.TabIndex = 5;
            this.RemoveButton.Text = "Remove Group";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveGroup);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(264, 312);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // VisgroupEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 347);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ColourPanel);
            this.Controls.Add(this.ColorLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.GroupName);
            this.Controls.Add(this.VisgroupPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisgroupEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Visgroup Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VisgroupPanel VisgroupPanel;
        private System.Windows.Forms.TextBox GroupName;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label ColorLabel;
        private System.Windows.Forms.Panel ColourPanel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button OkButton;
    }
}