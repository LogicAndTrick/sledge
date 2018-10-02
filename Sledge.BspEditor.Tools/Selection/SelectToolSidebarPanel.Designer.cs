namespace Sledge.BspEditor.Tools.Selection
{
    partial class SelectToolSidebarPanel
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
            this.Show3DWidgetsCheckbox = new System.Windows.Forms.CheckBox();
            this.lblMode = new System.Windows.Forms.Label();
            this.TranslateModeCheckbox = new System.Windows.Forms.CheckBox();
            this.RotateModeCheckbox = new System.Windows.Forms.CheckBox();
            this.SkewModeCheckbox = new System.Windows.Forms.CheckBox();
            this.MoveToWorldButton = new System.Windows.Forms.Button();
            this.MoveToEntityButton = new System.Windows.Forms.Button();
            this.lblActions = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Show3DWidgetsCheckbox
            // 
            this.Show3DWidgetsCheckbox.AutoSize = true;
            this.Show3DWidgetsCheckbox.Location = new System.Drawing.Point(7, 54);
            this.Show3DWidgetsCheckbox.Name = "Show3DWidgetsCheckbox";
            this.Show3DWidgetsCheckbox.Size = new System.Drawing.Size(112, 17);
            this.Show3DWidgetsCheckbox.TabIndex = 6;
            this.Show3DWidgetsCheckbox.Text = "Show 3D Widgets";
            this.Show3DWidgetsCheckbox.UseVisualStyleBackColor = true;
            this.Show3DWidgetsCheckbox.CheckedChanged += new System.EventHandler(this.Show3DWidgetsChecked);
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(3, 5);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(100, 13);
            this.lblMode.TabIndex = 5;
            this.lblMode.Text = "Manipulation Mode:";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TranslateModeCheckbox
            // 
            this.TranslateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.TranslateModeCheckbox.AutoSize = true;
            this.TranslateModeCheckbox.Location = new System.Drawing.Point(3, 3);
            this.TranslateModeCheckbox.Name = "TranslateModeCheckbox";
            this.TranslateModeCheckbox.Size = new System.Drawing.Size(61, 23);
            this.TranslateModeCheckbox.TabIndex = 7;
            this.TranslateModeCheckbox.Text = "Translate";
            this.TranslateModeCheckbox.UseVisualStyleBackColor = true;
            this.TranslateModeCheckbox.CheckedChanged += new System.EventHandler(this.TranslateModeChecked);
            // 
            // RotateModeCheckbox
            // 
            this.RotateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.RotateModeCheckbox.AutoSize = true;
            this.RotateModeCheckbox.Location = new System.Drawing.Point(70, 3);
            this.RotateModeCheckbox.Name = "RotateModeCheckbox";
            this.RotateModeCheckbox.Size = new System.Drawing.Size(49, 23);
            this.RotateModeCheckbox.TabIndex = 7;
            this.RotateModeCheckbox.Text = "Rotate";
            this.RotateModeCheckbox.UseVisualStyleBackColor = true;
            this.RotateModeCheckbox.CheckedChanged += new System.EventHandler(this.RotateModeChecked);
            // 
            // SkewModeCheckbox
            // 
            this.SkewModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.SkewModeCheckbox.AutoSize = true;
            this.SkewModeCheckbox.Location = new System.Drawing.Point(125, 3);
            this.SkewModeCheckbox.Name = "SkewModeCheckbox";
            this.SkewModeCheckbox.Size = new System.Drawing.Size(44, 23);
            this.SkewModeCheckbox.TabIndex = 7;
            this.SkewModeCheckbox.Text = "Skew";
            this.SkewModeCheckbox.UseVisualStyleBackColor = true;
            this.SkewModeCheckbox.CheckedChanged += new System.EventHandler(this.SkewModeChecked);
            // 
            // MoveToWorldButton
            // 
            this.MoveToWorldButton.AutoSize = true;
            this.MoveToWorldButton.Location = new System.Drawing.Point(1, 1);
            this.MoveToWorldButton.Margin = new System.Windows.Forms.Padding(1);
            this.MoveToWorldButton.Name = "MoveToWorldButton";
            this.MoveToWorldButton.Size = new System.Drawing.Size(100, 23);
            this.MoveToWorldButton.TabIndex = 8;
            this.MoveToWorldButton.Text = "Move to World";
            this.MoveToWorldButton.UseVisualStyleBackColor = true;
            this.MoveToWorldButton.Click += new System.EventHandler(this.MoveToWorldButtonClicked);
            // 
            // MoveToEntityButton
            // 
            this.MoveToEntityButton.AutoSize = true;
            this.MoveToEntityButton.Location = new System.Drawing.Point(1, 26);
            this.MoveToEntityButton.Margin = new System.Windows.Forms.Padding(1);
            this.MoveToEntityButton.Name = "MoveToEntityButton";
            this.MoveToEntityButton.Size = new System.Drawing.Size(100, 23);
            this.MoveToEntityButton.TabIndex = 9;
            this.MoveToEntityButton.Text = "Tie to Entity";
            this.MoveToEntityButton.UseVisualStyleBackColor = true;
            this.MoveToEntityButton.Click += new System.EventHandler(this.TieToEntityButtonClicked);
            // 
            // lblActions
            // 
            this.lblActions.AutoSize = true;
            this.lblActions.Location = new System.Drawing.Point(3, 75);
            this.lblActions.Name = "lblActions";
            this.lblActions.Size = new System.Drawing.Size(45, 13);
            this.lblActions.TabIndex = 5;
            this.lblActions.Text = "Actions:";
            this.lblActions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.TranslateModeCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.RotateModeCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.SkewModeCheckbox);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(220, 30);
            this.flowLayoutPanel1.TabIndex = 10;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.MoveToWorldButton);
            this.flowLayoutPanel2.Controls.Add(this.MoveToEntityButton);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 88);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(220, 53);
            this.flowLayoutPanel2.TabIndex = 11;
            this.flowLayoutPanel2.WrapContents = false;
            // 
            // SelectToolSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.lblActions);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.Show3DWidgetsCheckbox);
            this.Name = "SelectToolSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(232, 149);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Show3DWidgetsCheckbox;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.CheckBox TranslateModeCheckbox;
        private System.Windows.Forms.CheckBox RotateModeCheckbox;
        private System.Windows.Forms.CheckBox SkewModeCheckbox;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
        private System.Windows.Forms.Label lblActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}
