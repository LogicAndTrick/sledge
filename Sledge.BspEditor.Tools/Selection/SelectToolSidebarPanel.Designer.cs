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
            this.label3 = new System.Windows.Forms.Label();
            this.TranslateModeCheckbox = new System.Windows.Forms.CheckBox();
            this.RotateModeCheckbox = new System.Windows.Forms.CheckBox();
            this.SkewModeCheckbox = new System.Windows.Forms.CheckBox();
            this.MoveToWorldButton = new System.Windows.Forms.Button();
            this.MoveToEntityButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Show3DWidgetsCheckbox
            // 
            this.Show3DWidgetsCheckbox.AutoSize = true;
            this.Show3DWidgetsCheckbox.Location = new System.Drawing.Point(7, 50);
            this.Show3DWidgetsCheckbox.Name = "Show3DWidgetsCheckbox";
            this.Show3DWidgetsCheckbox.Size = new System.Drawing.Size(112, 17);
            this.Show3DWidgetsCheckbox.TabIndex = 6;
            this.Show3DWidgetsCheckbox.Text = "Show 3D Widgets";
            this.Show3DWidgetsCheckbox.UseVisualStyleBackColor = true;
            this.Show3DWidgetsCheckbox.CheckedChanged += new System.EventHandler(this.Show3DWidgetsChecked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Manipulation Mode:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TranslateModeCheckbox
            // 
            this.TranslateModeCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.TranslateModeCheckbox.AutoSize = true;
            this.TranslateModeCheckbox.Location = new System.Drawing.Point(5, 21);
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
            this.RotateModeCheckbox.Location = new System.Drawing.Point(72, 21);
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
            this.SkewModeCheckbox.Location = new System.Drawing.Point(127, 21);
            this.SkewModeCheckbox.Name = "SkewModeCheckbox";
            this.SkewModeCheckbox.Size = new System.Drawing.Size(44, 23);
            this.SkewModeCheckbox.TabIndex = 7;
            this.SkewModeCheckbox.Text = "Skew";
            this.SkewModeCheckbox.UseVisualStyleBackColor = true;
            this.SkewModeCheckbox.CheckedChanged += new System.EventHandler(this.SkewModeChecked);
            // 
            // MoveToWorldButton
            // 
            this.MoveToWorldButton.Location = new System.Drawing.Point(5, 91);
            this.MoveToWorldButton.Name = "MoveToWorldButton";
            this.MoveToWorldButton.Size = new System.Drawing.Size(86, 23);
            this.MoveToWorldButton.TabIndex = 8;
            this.MoveToWorldButton.Text = "Move to World";
            this.MoveToWorldButton.UseVisualStyleBackColor = true;
            this.MoveToWorldButton.Click += new System.EventHandler(this.MoveToWorldButtonClicked);
            // 
            // MoveToEntityButton
            // 
            this.MoveToEntityButton.Location = new System.Drawing.Point(97, 91);
            this.MoveToEntityButton.Name = "MoveToEntityButton";
            this.MoveToEntityButton.Size = new System.Drawing.Size(84, 23);
            this.MoveToEntityButton.TabIndex = 9;
            this.MoveToEntityButton.Text = "Tie to Entity";
            this.MoveToEntityButton.UseVisualStyleBackColor = true;
            this.MoveToEntityButton.Click += new System.EventHandler(this.TieToEntityButtonClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Actions:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SelectToolSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.MoveToWorldButton);
            this.Controls.Add(this.MoveToEntityButton);
            this.Controls.Add(this.SkewModeCheckbox);
            this.Controls.Add(this.RotateModeCheckbox);
            this.Controls.Add(this.TranslateModeCheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Show3DWidgetsCheckbox);
            this.Name = "SelectToolSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(232, 122);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Show3DWidgetsCheckbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox TranslateModeCheckbox;
        private System.Windows.Forms.CheckBox RotateModeCheckbox;
        private System.Windows.Forms.CheckBox SkewModeCheckbox;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
        private System.Windows.Forms.Label label1;

    }
}
