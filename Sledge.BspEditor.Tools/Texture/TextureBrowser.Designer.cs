namespace Sledge.BspEditor.Tools.Texture
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
            this.SortByLabel = new System.Windows.Forms.Label();
            this.SizeCombo = new System.Windows.Forms.ComboBox();
            this.TextureSizeLabel = new System.Windows.Forms.Label();
            this.TextureNameLabel = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.UsedTexturesOnlyBox = new System.Windows.Forms.CheckBox();
            this.SizeLabel = new System.Windows.Forms.Label();
            this.FilterTextbox = new System.Windows.Forms.TextBox();
            this.FilterLabel = new System.Windows.Forms.Label();
            this.FavouritesTree = new System.Windows.Forms.TreeView();
            this.LeftbarPanel = new System.Windows.Forms.Panel();
            this.DeleteFavouriteFolderButton = new System.Windows.Forms.Button();
            this.RemoveFavouriteItemButton = new System.Windows.Forms.Button();
            this.AddFavouriteFolderButton = new System.Windows.Forms.Button();
            this.FavouriteTexturesLabel = new System.Windows.Forms.Label();
            this.TextureListPanel = new System.Windows.Forms.Panel();
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
            this.panel1.Controls.Add(this.SortByLabel);
            this.panel1.Controls.Add(this.SizeCombo);
            this.panel1.Controls.Add(this.TextureSizeLabel);
            this.panel1.Controls.Add(this.TextureNameLabel);
            this.panel1.Controls.Add(this.SelectButton);
            this.panel1.Controls.Add(this.UsedTexturesOnlyBox);
            this.panel1.Controls.Add(this.SizeLabel);
            this.panel1.Controls.Add(this.FilterTextbox);
            this.panel1.Controls.Add(this.FilterLabel);
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
            // SortByLabel
            // 
            this.SortByLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SortByLabel.Location = new System.Drawing.Point(680, 9);
            this.SortByLabel.Name = "SortByLabel";
            this.SortByLabel.Size = new System.Drawing.Size(135, 21);
            this.SortByLabel.TabIndex = 8;
            this.SortByLabel.Text = "Sort By";
            this.SortByLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SizeCombo
            // 
            this.SizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SizeCombo.FormattingEnabled = true;
            this.SizeCombo.Items.AddRange(new object[] {
            "64",
            "128",
            "256",
            "512"});
            this.SizeCombo.Location = new System.Drawing.Point(85, 34);
            this.SizeCombo.Name = "SizeCombo";
            this.SizeCombo.Size = new System.Drawing.Size(179, 21);
            this.SizeCombo.TabIndex = 7;
            this.SizeCombo.SelectedIndexChanged += new System.EventHandler(this.SizeValueChanged);
            // 
            // TextureSizeLabel
            // 
            this.TextureSizeLabel.AutoSize = true;
            this.TextureSizeLabel.Location = new System.Drawing.Point(427, 38);
            this.TextureSizeLabel.Name = "TextureSizeLabel";
            this.TextureSizeLabel.Size = new System.Drawing.Size(27, 13);
            this.TextureSizeLabel.TabIndex = 6;
            this.TextureSizeLabel.Text = "Size";
            // 
            // TextureNameLabel
            // 
            this.TextureNameLabel.AutoSize = true;
            this.TextureNameLabel.Location = new System.Drawing.Point(427, 11);
            this.TextureNameLabel.Name = "TextureNameLabel";
            this.TextureNameLabel.Size = new System.Drawing.Size(35, 13);
            this.TextureNameLabel.TabIndex = 6;
            this.TextureNameLabel.Text = "Name";
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(270, 34);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(92, 20);
            this.SelectButton.TabIndex = 5;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButtonClicked);
            // 
            // UsedTexturesOnlyBox
            // 
            this.UsedTexturesOnlyBox.AutoSize = true;
            this.UsedTexturesOnlyBox.Location = new System.Drawing.Point(270, 11);
            this.UsedTexturesOnlyBox.Name = "UsedTexturesOnlyBox";
            this.UsedTexturesOnlyBox.Size = new System.Drawing.Size(113, 17);
            this.UsedTexturesOnlyBox.TabIndex = 4;
            this.UsedTexturesOnlyBox.Text = "Used textures only";
            this.UsedTexturesOnlyBox.UseVisualStyleBackColor = true;
            this.UsedTexturesOnlyBox.CheckedChanged += new System.EventHandler(this.UsedTexturesOnlyChanged);
            // 
            // SizeLabel
            // 
            this.SizeLabel.Location = new System.Drawing.Point(3, 34);
            this.SizeLabel.Name = "SizeLabel";
            this.SizeLabel.Size = new System.Drawing.Size(76, 21);
            this.SizeLabel.TabIndex = 2;
            this.SizeLabel.Text = "Size";
            this.SizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FilterTextbox
            // 
            this.FilterTextbox.HideSelection = false;
            this.FilterTextbox.Location = new System.Drawing.Point(85, 8);
            this.FilterTextbox.Name = "FilterTextbox";
            this.FilterTextbox.Size = new System.Drawing.Size(179, 20);
            this.FilterTextbox.TabIndex = 1;
            this.FilterTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FilterTextboxKeyUp);
            // 
            // FilterLabel
            // 
            this.FilterLabel.Location = new System.Drawing.Point(3, 8);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(76, 20);
            this.FilterLabel.TabIndex = 0;
            this.FilterLabel.Text = "Filter";
            this.FilterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.LeftbarPanel.Controls.Add(this.FavouriteTexturesLabel);
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
            this.DeleteFavouriteFolderButton.Enabled = false;
            this.DeleteFavouriteFolderButton.Location = new System.Drawing.Point(113, 446);
            this.DeleteFavouriteFolderButton.Name = "DeleteFavouriteFolderButton";
            this.DeleteFavouriteFolderButton.Size = new System.Drawing.Size(110, 23);
            this.DeleteFavouriteFolderButton.TabIndex = 3;
            this.DeleteFavouriteFolderButton.Text = "Delete Folder";
            this.DeleteFavouriteFolderButton.UseVisualStyleBackColor = true;
            this.DeleteFavouriteFolderButton.Click += new System.EventHandler(this.DeleteFavouriteFolderButtonClicked);
            // 
            // RemoveFavouriteItemButton
            // 
            this.RemoveFavouriteItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveFavouriteItemButton.Enabled = false;
            this.RemoveFavouriteItemButton.Location = new System.Drawing.Point(3, 469);
            this.RemoveFavouriteItemButton.Name = "RemoveFavouriteItemButton";
            this.RemoveFavouriteItemButton.Size = new System.Drawing.Size(220, 23);
            this.RemoveFavouriteItemButton.TabIndex = 3;
            this.RemoveFavouriteItemButton.Text = "Remove Selection From Folder";
            this.RemoveFavouriteItemButton.UseVisualStyleBackColor = true;
            this.RemoveFavouriteItemButton.Click += new System.EventHandler(this.RemoveFavouriteItemButtonClicked);
            // 
            // AddFavouriteFolderButton
            // 
            this.AddFavouriteFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddFavouriteFolderButton.Enabled = false;
            this.AddFavouriteFolderButton.Location = new System.Drawing.Point(3, 446);
            this.AddFavouriteFolderButton.Name = "AddFavouriteFolderButton";
            this.AddFavouriteFolderButton.Size = new System.Drawing.Size(110, 23);
            this.AddFavouriteFolderButton.TabIndex = 3;
            this.AddFavouriteFolderButton.Text = "Add Folder";
            this.AddFavouriteFolderButton.UseVisualStyleBackColor = true;
            this.AddFavouriteFolderButton.Click += new System.EventHandler(this.AddFavouriteFolderButtonClicked);
            // 
            // FavouriteTexturesLabel
            // 
            this.FavouriteTexturesLabel.AutoSize = true;
            this.FavouriteTexturesLabel.Location = new System.Drawing.Point(3, 416);
            this.FavouriteTexturesLabel.Name = "FavouriteTexturesLabel";
            this.FavouriteTexturesLabel.Size = new System.Drawing.Size(170, 13);
            this.FavouriteTexturesLabel.TabIndex = 2;
            this.FavouriteTexturesLabel.Text = "Favourite Textures (drag and drop)";
            // 
            // TextureListPanel
            // 
            this.TextureListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextureListPanel.Location = new System.Drawing.Point(226, 0);
            this.TextureListPanel.Name = "TextureListPanel";
            this.TextureListPanel.Size = new System.Drawing.Size(714, 495);
            this.TextureListPanel.TabIndex = 4;
            // 
            // TextureBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 565);
            this.Controls.Add(this.TextureListPanel);
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
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.CheckBox UsedTexturesOnlyBox;
        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.TextBox FilterTextbox;
        private System.Windows.Forms.Label FilterLabel;
        private System.Windows.Forms.ComboBox SizeCombo;
        private System.Windows.Forms.ComboBox SortOrderCombo;
        private System.Windows.Forms.Label SortByLabel;
        private System.Windows.Forms.CheckBox SortDescendingCheckbox;
        private System.Windows.Forms.TreeView FavouritesTree;
        private System.Windows.Forms.Panel LeftbarPanel;
        private System.Windows.Forms.Button DeleteFavouriteFolderButton;
        private System.Windows.Forms.Button AddFavouriteFolderButton;
        private System.Windows.Forms.Label FavouriteTexturesLabel;
        private System.Windows.Forms.Button RemoveFavouriteItemButton;
        private System.Windows.Forms.Panel TextureListPanel;
    }
}