namespace Sledge.Editor.UI.Sidebar
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
            this.GroupComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.SelectionPictureBox = new System.Windows.Forms.PictureBox();
            this.SizeLabel = new System.Windows.Forms.Label();
            this.TextureComboBox = new Sledge.Editor.UI.TextureComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(131, 101);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(66, 22);
            this.BrowseButton.TabIndex = 11;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClicked);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceButton.Location = new System.Drawing.Point(131, 129);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(66, 22);
            this.ReplaceButton.TabIndex = 12;
            this.ReplaceButton.Text = "Replace...";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButtonClicked);
            // 
            // GroupComboBox
            // 
            this.GroupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupComboBox.FormattingEnabled = true;
            this.GroupComboBox.Location = new System.Drawing.Point(60, 3);
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.Size = new System.Drawing.Size(137, 21);
            this.GroupComboBox.TabIndex = 8;
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Texture:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Group:";
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyButton.Location = new System.Drawing.Point(131, 73);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(66, 22);
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
            this.SelectionPictureBox.Location = new System.Drawing.Point(3, 57);
            this.SelectionPictureBox.Name = "SelectionPictureBox";
            this.SelectionPictureBox.Size = new System.Drawing.Size(122, 103);
            this.SelectionPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SelectionPictureBox.TabIndex = 10;
            this.SelectionPictureBox.TabStop = false;
            // 
            // SizeLabel
            // 
            this.SizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeLabel.AutoSize = true;
            this.SizeLabel.Location = new System.Drawing.Point(131, 57);
            this.SizeLabel.Name = "SizeLabel";
            this.SizeLabel.Size = new System.Drawing.Size(27, 13);
            this.SizeLabel.TabIndex = 13;
            this.SizeLabel.Text = "Size";
            // 
            // TextureComboBox
            // 
            this.TextureComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextureComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.TextureComboBox.DropDownHeight = 600;
            this.TextureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextureComboBox.FormattingEnabled = true;
            this.TextureComboBox.IntegralHeight = false;
            this.TextureComboBox.ItemHeight = 15;
            this.TextureComboBox.Location = new System.Drawing.Point(60, 30);
            this.TextureComboBox.Name = "TextureComboBox";
            this.TextureComboBox.Size = new System.Drawing.Size(137, 21);
            this.TextureComboBox.TabIndex = 9;
            this.TextureComboBox.SelectionChangeCommitted += new System.EventHandler(this.TextureSelectionChanged);
            // 
            // TextureSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SizeLabel);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.ReplaceButton);
            this.Controls.Add(this.SelectionPictureBox);
            this.Controls.Add(this.TextureComboBox);
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(0, 250);
            this.MinimumSize = new System.Drawing.Size(200, 165);
            this.Name = "TextureSidebarPanel";
            this.Size = new System.Drawing.Size(200, 165);
            ((System.ComponentModel.ISupportInitialize)(this.SelectionPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.PictureBox SelectionPictureBox;
        private TextureComboBox TextureComboBox;
        private System.Windows.Forms.ComboBox GroupComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Label SizeLabel;
    }
}
