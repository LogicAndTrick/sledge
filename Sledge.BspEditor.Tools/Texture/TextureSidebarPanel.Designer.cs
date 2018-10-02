namespace Sledge.BspEditor.Tools.Texture
{
    partial class TextureSidebarPanel
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
            this.BrowseButton = new System.Windows.Forms.Button();
            this.ReplaceButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.SelectionPictureBox = new System.Windows.Forms.PictureBox();
            this.SizeLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(102, 137);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(95, 22);
            this.BrowseButton.TabIndex = 11;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClicked);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceButton.Location = new System.Drawing.Point(102, 161);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(95, 22);
            this.ReplaceButton.TabIndex = 12;
            this.ReplaceButton.Text = "Replace...";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButtonClicked);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ApplyButton.Location = new System.Drawing.Point(3, 161);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(95, 22);
            this.ApplyButton.TabIndex = 11;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButtonClicked);
            // 
            // SelectionPictureBox
            // 
            this.SelectionPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectionPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectionPictureBox.Location = new System.Drawing.Point(3, 3);
            this.SelectionPictureBox.Name = "SelectionPictureBox";
            this.SelectionPictureBox.Size = new System.Drawing.Size(194, 115);
            this.SelectionPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SelectionPictureBox.TabIndex = 10;
            this.SelectionPictureBox.TabStop = false;
            // 
            // SizeLabel
            // 
            this.SizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SizeLabel.AutoSize = true;
            this.SizeLabel.Location = new System.Drawing.Point(3, 142);
            this.SizeLabel.Name = "SizeLabel";
            this.SizeLabel.Size = new System.Drawing.Size(27, 13);
            this.SizeLabel.TabIndex = 13;
            this.SizeLabel.Text = "Size";
            // 
            // NameLabel
            // 
            this.NameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(3, 122);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(35, 13);
            this.NameLabel.TabIndex = 13;
            this.NameLabel.Text = "Name";
            // 
            // TextureSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.SizeLabel);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.ReplaceButton);
            this.Controls.Add(this.SelectionPictureBox);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(0, 250);
            this.MinimumSize = new System.Drawing.Size(200, 165);
            this.Name = "TextureSidebarPanel";
            this.Size = new System.Drawing.Size(200, 186);
            ((System.ComponentModel.ISupportInitialize)(this.SelectionPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.PictureBox SelectionPictureBox;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.Label NameLabel;
    }
}
