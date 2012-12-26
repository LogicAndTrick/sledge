namespace Sledge.Editor.UI
{
    partial class CollapsingLabel
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
            this.TextLabel = new System.Windows.Forms.Label();
            this.ArrowImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            this.TextLabel.AutoSize = true;
            this.TextLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.TextLabel.Location = new System.Drawing.Point(0, 0);
            this.TextLabel.Name = "TextLabel";
            this.TextLabel.Size = new System.Drawing.Size(28, 13);
            this.TextLabel.TabIndex = 0;
            this.TextLabel.Text = "Text";
            // 
            // ArrowImage
            // 
            this.ArrowImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.ArrowImage.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.ArrowImage.FlatAppearance.BorderSize = 0;
            this.ArrowImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.ArrowImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.ArrowImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ArrowImage.Image = global::Sledge.Editor.Properties.Resources.Arrow_Up;
            this.ArrowImage.Location = new System.Drawing.Point(148, 0);
            this.ArrowImage.Name = "ArrowImage";
            this.ArrowImage.Size = new System.Drawing.Size(16, 16);
            this.ArrowImage.TabIndex = 2;
            this.ArrowImage.UseVisualStyleBackColor = true;
            this.ArrowImage.Click += new System.EventHandler(this.LabelClick);
            // 
            // CollapsingLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ArrowImage);
            this.Controls.Add(this.TextLabel);
            this.Name = "CollapsingLabel";
            this.Size = new System.Drawing.Size(164, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TextLabel;
        private System.Windows.Forms.Button ArrowImage;
    }
}
