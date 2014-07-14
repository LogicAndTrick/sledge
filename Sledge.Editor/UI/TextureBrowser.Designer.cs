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
            this.SortDescendingCheckbox = new System.Windows.Forms.CheckBox();
            this.SortOrderCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SizeCombo = new System.Windows.Forms.ComboBox();
            this.TextureSizeLabel = new System.Windows.Forms.Label();
            this.TextureNameLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SelectButton = new System.Windows.Forms.Button();
            this.UsedTexturesOnlyBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FavouritesTree = new System.Windows.Forms.TreeView();
            this.LeftbarPanel = new System.Windows.Forms.Panel();
            this.DeleteFavouriteFolderButton = new System.Windows.Forms.Button();
            this.RemoveFavouriteItemButton = new System.Windows.Forms.Button();
            this.AddFavouriteFolderButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.TextureList = new Sledge.Editor.UI.TextureListPanel();
            this.panel1.SuspendLayout();
            this.LeftbarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PackageTree
            // 
            this.PackageTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.PackageTree.HideSelection = false;
            this.PackageTree.Location = new System.Drawing.Point(0, 0);
            this.PackageTree.Name = "PackageTree";
            this.PackageTree.Size = new System.Drawing.Size(226, 413);
            this.PackageTree.TabIndex = 1;
            this.PackageTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectedPackageChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SortDescendingCheckbox);
            this.panel1.Controls.Add(this.SortOrderCombo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.SizeCombo);
            this.panel1.Controls.Add(this.TextureSizeLabel);
            this.panel1.Controls.Add(this.TextureNameLabel);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.SelectButton);
            this.panel1.Controls.Add(this.UsedTexturesOnlyBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.FilterTextbox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 495);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(940, 70);
            this.panel1.TabIndex = 2;
            // 
            // SortDescendingCheckbox
            // 
            this.SortDescendingCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SortDescendingCheckbox.AutoSize = true;
            this.SortDescendingCheckbox.Location = new System.Drawing.Point(821, 34);
            this.SortDescendingCheckbox.Name = "SortDescendingCheckbox";
            this.SortDescendingCheckbox.Size = new System.Drawing.Size(103, 17);
            this.SortDescendingCheckbox.TabIndex = 10;
            this.SortDescendingCheckbox.Text = "Sort descending";
            this.SortDescendingCheckbox.UseVisualStyleBackColor = true;
            this.SortDescendingCheckbox.CheckedChanged += new System.EventHandler(this.SortDescendingCheckboxChanged);
            // 
            // SortOrderCombo
            // 
            this.SortOrderCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SortOrderCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SortOrderCombo.FormattingEnabled = true;
            this.SortOrderCombo.Location = new System.Drawing.Point(821, 9);
            this.SortOrderCombo.Name = "SortOrderCombo";
            this.SortOrderCombo.Size = new System.Drawing.Size(107, 21);
            this.SortOrderCombo.TabIndex = 9;
            this.SortOrderCombo.SelectedIndexChanged += new System.EventHandler(this.SortOrderComboIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(774, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Sort By";
            // 
            // SizeCombo
            // 
            this.SizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SizeCombo.FormattingEnabled = true;
            this.SizeCombo.Items.AddRange(new object[] {
            "1:1",
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
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(232, 32);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(57, 20);
            this.SelectButton.TabIndex = 5;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButtonClicked);
            // 
            // UsedTexturesOnlyBox
            // 
            this.UsedTexturesOnlyBox.AutoSize = true;
            this.UsedTexturesOnlyBox.Location = new System.Drawing.Point(232, 9);
            this.UsedTexturesOnlyBox.Name = "UsedTexturesOnlyBox";
            this.UsedTexturesOnlyBox.Size = new System.Drawing.Size(113, 17);
            this.UsedTexturesOnlyBox.TabIndex = 4;
            this.UsedTexturesOnlyBox.Text = "Used textures only";
            this.UsedTexturesOnlyBox.UseVisualStyleBackColor = true;
            this.UsedTexturesOnlyBox.CheckedChanged += new System.EventHandler(this.UsedTexturesOnlyChanged);
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
            this.FilterTextbox.HideSelection = false;
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
            // FavouritesTree
            // 
            this.FavouritesTree.AllowDrop = true;
            this.FavouritesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FavouritesTree.HideSelection = false;
            this.FavouritesTree.Location = new System.Drawing.Point(0, 432);
            this.FavouritesTree.Name = "FavouritesTree";
            this.FavouritesTree.Size = new System.Drawing.Size(226, 13);
            this.FavouritesTree.TabIndex = 1;
            this.FavouritesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectedFavouriteChanged);
            this.FavouritesTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.FavouritesTreeDragDrop);
            this.FavouritesTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.FavouritesTreeDragEnter);
            this.FavouritesTree.DragOver += new System.Windows.Forms.DragEventHandler(this.FavouritesTreeDragOver);
            this.FavouritesTree.DragLeave += new System.EventHandler(this.FavouritesTreeDragLeave);
            // 
            // LeftbarPanel
            // 
            this.LeftbarPanel.Controls.Add(this.DeleteFavouriteFolderButton);
            this.LeftbarPanel.Controls.Add(this.RemoveFavouriteItemButton);
            this.LeftbarPanel.Controls.Add(this.AddFavouriteFolderButton);
            this.LeftbarPanel.Controls.Add(this.label4);
            this.LeftbarPanel.Controls.Add(this.FavouritesTree);
            this.LeftbarPanel.Controls.Add(this.PackageTree);
            this.LeftbarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftbarPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftbarPanel.Name = "LeftbarPanel";
            this.LeftbarPanel.Size = new System.Drawing.Size(226, 495);
            this.LeftbarPanel.TabIndex = 3;
            // 
            // DeleteFavouriteFolderButton
            // 
            this.DeleteFavouriteFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeleteFavouriteFolderButton.Location = new System.Drawing.Point(104, 446);
            this.DeleteFavouriteFolderButton.Name = "DeleteFavouriteFolderButton";
            this.DeleteFavouriteFolderButton.Size = new System.Drawing.Size(86, 23);
            this.DeleteFavouriteFolderButton.TabIndex = 3;
            this.DeleteFavouriteFolderButton.Text = "Delete Folder";
            this.DeleteFavouriteFolderButton.UseVisualStyleBackColor = true;
            this.DeleteFavouriteFolderButton.Click += new System.EventHandler(this.DeleteFavouriteFolderButtonClicked);
            // 
            // RemoveFavouriteItemButton
            // 
            this.RemoveFavouriteItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveFavouriteItemButton.Location = new System.Drawing.Point(3, 469);
            this.RemoveFavouriteItemButton.Name = "RemoveFavouriteItemButton";
            this.RemoveFavouriteItemButton.Size = new System.Drawing.Size(187, 23);
            this.RemoveFavouriteItemButton.TabIndex = 3;
            this.RemoveFavouriteItemButton.Text = "Remove Selection From Folder";
            this.RemoveFavouriteItemButton.UseVisualStyleBackColor = true;
            this.RemoveFavouriteItemButton.Click += new System.EventHandler(this.RemoveFavouriteItemButtonClicked);
            // 
            // AddFavouriteFolderButton
            // 
            this.AddFavouriteFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddFavouriteFolderButton.Location = new System.Drawing.Point(3, 446);
            this.AddFavouriteFolderButton.Name = "AddFavouriteFolderButton";
            this.AddFavouriteFolderButton.Size = new System.Drawing.Size(100, 23);
            this.AddFavouriteFolderButton.TabIndex = 3;
            this.AddFavouriteFolderButton.Text = "Add Folder";
            this.AddFavouriteFolderButton.UseVisualStyleBackColor = true;
            this.AddFavouriteFolderButton.Click += new System.EventHandler(this.AddFavouriteFolderButtonClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 416);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Favourite Textures (drag and drop)";
            // 
            // TextureList
            // 
            this.TextureList.AllowMultipleSelection = true;
            this.TextureList.AllowSelection = true;
            this.TextureList.AutoScroll = true;
            this.TextureList.BackColor = System.Drawing.Color.Black;
            this.TextureList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextureList.EnableDrag = true;
            this.TextureList.ImageSize = 128;
            this.TextureList.Location = new System.Drawing.Point(226, 0);
            this.TextureList.Name = "TextureList";
            this.TextureList.Size = new System.Drawing.Size(714, 495);
            this.TextureList.SortDescending = false;
            this.TextureList.SortOrder = Sledge.Editor.UI.TextureListPanel.TextureSortOrder.Name;
            this.TextureList.TabIndex = 0;
            // 
            // TextureBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 565);
            this.Controls.Add(this.TextureList);
            this.Controls.Add(this.LeftbarPanel);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "TextureBrowser";
            this.Text = "Texture Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextureBrowserKeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextureBrowserKeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.LeftbarPanel.ResumeLayout(false);
            this.LeftbarPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView PackageTree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label TextureSizeLabel;
        private System.Windows.Forms.Label TextureNameLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.CheckBox UsedTexturesOnlyBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterTextbox;
        private System.Windows.Forms.Label label1;
        private TextureListPanel TextureList;
        private System.Windows.Forms.ComboBox SizeCombo;
        private System.Windows.Forms.ComboBox SortOrderCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox SortDescendingCheckbox;
        private System.Windows.Forms.TreeView FavouritesTree;
        private System.Windows.Forms.Panel LeftbarPanel;
        private System.Windows.Forms.Button DeleteFavouriteFolderButton;
        private System.Windows.Forms.Button AddFavouriteFolderButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button RemoveFavouriteItemButton;
    }
}