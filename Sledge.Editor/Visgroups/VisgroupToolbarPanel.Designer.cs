namespace Sledge.Editor.Visgroups
{
    partial class VisgroupToolbarPanel
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
            this.ShowAllButton = new System.Windows.Forms.Button();
            this.SelectButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.VisgroupPanel = new Sledge.Editor.Visgroups.VisgroupPanel();
            this.SuspendLayout();
            // 
            // ShowAllButton
            // 
            this.ShowAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowAllButton.Location = new System.Drawing.Point(83, 298);
            this.ShowAllButton.Name = "ShowAllButton";
            this.ShowAllButton.Size = new System.Drawing.Size(57, 22);
            this.ShowAllButton.TabIndex = 5;
            this.ShowAllButton.Tag = "ShowAll";
            this.ShowAllButton.Text = "Show All";
            this.ShowAllButton.UseVisualStyleBackColor = true;
            this.ShowAllButton.Click += new System.EventHandler(this.ShowAllButtonClicked);
            // 
            // SelectButton
            // 
            this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SelectButton.Location = new System.Drawing.Point(38, 298);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(46, 22);
            this.SelectButton.TabIndex = 6;
            this.SelectButton.Tag = "Select";
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButtonClicked);
            // 
            // EditButton
            // 
            this.EditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditButton.Location = new System.Drawing.Point(0, 298);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(39, 22);
            this.EditButton.TabIndex = 7;
            this.EditButton.Tag = "Edit";
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButtonClicked);
            // 
            // VisgroupPanel
            // 
            this.VisgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisgroupPanel.Location = new System.Drawing.Point(-1, -1);
            this.VisgroupPanel.Name = "VisgroupPanel";
            this.VisgroupPanel.ShowCheckboxes = true;
            this.VisgroupPanel.Size = new System.Drawing.Size(507, 299);
            this.VisgroupPanel.TabIndex = 8;
            this.VisgroupPanel.VisgroupToggled += new Sledge.Editor.Visgroups.VisgroupPanel.VisgroupToggledEventHandler(this.VisgroupToggled);
            this.VisgroupPanel.VisgroupSelected += new Sledge.Editor.Visgroups.VisgroupPanel.VisgroupSelectedEventHandler(this.VisgroupSelected);
            // 
            // VisgroupToolbarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VisgroupPanel);
            this.Controls.Add(this.ShowAllButton);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.EditButton);
            this.Name = "VisgroupToolbarPanel";
            this.Size = new System.Drawing.Size(505, 320);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ShowAllButton;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Button EditButton;
        private VisgroupPanel VisgroupPanel;
    }
}
