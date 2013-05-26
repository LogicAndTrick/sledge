namespace Sledge.Editor.UI
{
    partial class TextureBrowser
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
            this.PackageTree = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SizeCombo = new System.Windows.Forms.ComboBox();
            this.TextureSizeLabel = new System.Windows.Forms.Label();
            this.TextureNameLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextureList = new Sledge.Editor.UI.TextureListPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PackageTree
            // 
            this.PackageTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.PackageTree.HideSelection = false;
            this.PackageTree.Location = new System.Drawing.Point(0, 0);
            this.PackageTree.Name = "PackageTree";
            this.PackageTree.Size = new System.Drawing.Size(120, 423);
            this.PackageTree.TabIndex = 1;
            this.PackageTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectedPackageChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SizeCombo);
            this.panel1.Controls.Add(this.TextureSizeLabel);
            this.panel1.Controls.Add(this.TextureNameLabel);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.FilterTextbox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 423);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(827, 70);
            this.panel1.TabIndex = 2;
            // 
            // SizeCombo
            // 
            this.SizeCombo.FormattingEnabled = true;
            this.SizeCombo.Items.AddRange(new object[] {
            "64",
            "128",
            "256",
            "512"});
            this.SizeCombo.Location = new System.Drawing.Point(47, 32);
            this.SizeCombo.Name = "SizeCombo";
            this.SizeCombo.Size = new System.Drawing.Size(179, 21);
            this.SizeCombo.TabIndex = 7;
            this.SizeCombo.SelectedIndexChanged += new System.EventHandler(this.SizeValueChanged);
            // 
            // TextureSizeLabel
            // 
            this.TextureSizeLabel.AutoSize = true;
            this.TextureSizeLabel.Location = new System.Drawing.Point(379, 36);
            this.TextureSizeLabel.Name = "TextureSizeLabel";
            this.TextureSizeLabel.Size = new System.Drawing.Size(27, 13);
            this.TextureSizeLabel.TabIndex = 6;
            this.TextureSizeLabel.Text = "Size";
            // 
            // TextureNameLabel
            // 
            this.TextureNameLabel.AutoSize = true;
            this.TextureNameLabel.Location = new System.Drawing.Point(379, 9);
            this.TextureNameLabel.Name = "TextureNameLabel";
            this.TextureNameLabel.Size = new System.Drawing.Size(35, 13);
            this.TextureNameLabel.TabIndex = 6;
            this.TextureNameLabel.Text = "Name";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(295, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 20);
            this.button2.TabIndex = 5;
            this.button2.Text = "Replace";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(232, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(57, 20);
            this.button1.TabIndex = 5;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(232, 9);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(113, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Used textures only";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Size";
            // 
            // FilterTextbox
            // 
            this.FilterTextbox.Location = new System.Drawing.Point(47, 6);
            this.FilterTextbox.Name = "FilterTextbox";
            this.FilterTextbox.Size = new System.Drawing.Size(179, 20);
            this.FilterTextbox.TabIndex = 1;
            this.FilterTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FilterTextboxKeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter";
            // 
            // TextureList
            // 
            this.TextureList.AllowMultipleSelection = true;
            this.TextureList.AllowSelection = true;
            this.TextureList.AutoScroll = true;
            this.TextureList.BackColor = System.Drawing.Color.Black;
            this.TextureList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextureList.ImageSize = 128;
            this.TextureList.Location = new System.Drawing.Point(120, 0);
            this.TextureList.Name = "TextureList";
            this.TextureList.Size = new System.Drawing.Size(707, 423);
            this.TextureList.TabIndex = 0;
            // 
            // TextureBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 493);
            this.Controls.Add(this.TextureList);
            this.Controls.Add(this.PackageTree);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "TextureBrowser";
            this.Text = "Texture Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextureBrowserKeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView PackageTree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label TextureSizeLabel;
        private System.Windows.Forms.Label TextureNameLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterTextbox;
        private System.Windows.Forms.Label label1;
        private TextureListPanel TextureList;
        private System.Windows.Forms.ComboBox SizeCombo;
    }
}