namespace Sledge.BspEditor.Editing.Components
{
    partial class MapInformationDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SolidsLabel = new System.Windows.Forms.Label();
            this.FacesLabel = new System.Windows.Forms.Label();
            this.PointEntitiesLabel = new System.Windows.Forms.Label();
            this.SolidEntitiesLabel = new System.Windows.Forms.Label();
            this.UniqueTexturesLabel = new System.Windows.Forms.Label();
            this.TextureMemoryLabel = new System.Windows.Forms.Label();
            this.NumSolids = new System.Windows.Forms.Label();
            this.NumFaces = new System.Windows.Forms.Label();
            this.NumPointEntities = new System.Windows.Forms.Label();
            this.NumSolidEntities = new System.Windows.Forms.Label();
            this.NumUniqueTextures = new System.Windows.Forms.Label();
            this.TexturePackagesUsedLabel = new System.Windows.Forms.Label();
            this.TexturePackages = new System.Windows.Forms.ListBox();
            this.CloseDialogButton = new System.Windows.Forms.Button();
            this.TextureMemoryValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.tableLayoutPanel1.Controls.Add(this.SolidsLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FacesLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.PointEntitiesLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SolidEntitiesLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.UniqueTexturesLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.TextureMemoryLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.NumSolids, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.NumFaces, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.NumPointEntities, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.NumSolidEntities, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.NumUniqueTextures, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.TextureMemoryValue, 1, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 122);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // SolidsLabel
            // 
            this.SolidsLabel.AutoSize = true;
            this.SolidsLabel.Location = new System.Drawing.Point(3, 0);
            this.SolidsLabel.Name = "SolidsLabel";
            this.SolidsLabel.Size = new System.Drawing.Size(38, 13);
            this.SolidsLabel.TabIndex = 0;
            this.SolidsLabel.Text = "Solids:";
            // 
            // FacesLabel
            // 
            this.FacesLabel.AutoSize = true;
            this.FacesLabel.Location = new System.Drawing.Point(3, 20);
            this.FacesLabel.Name = "FacesLabel";
            this.FacesLabel.Size = new System.Drawing.Size(39, 13);
            this.FacesLabel.TabIndex = 0;
            this.FacesLabel.Text = "Faces:";
            // 
            // PointEntitiesLabel
            // 
            this.PointEntitiesLabel.AutoSize = true;
            this.PointEntitiesLabel.Location = new System.Drawing.Point(3, 40);
            this.PointEntitiesLabel.Name = "PointEntitiesLabel";
            this.PointEntitiesLabel.Size = new System.Drawing.Size(68, 13);
            this.PointEntitiesLabel.TabIndex = 0;
            this.PointEntitiesLabel.Text = "Point Entities";
            // 
            // SolidEntitiesLabel
            // 
            this.SolidEntitiesLabel.AutoSize = true;
            this.SolidEntitiesLabel.Location = new System.Drawing.Point(3, 60);
            this.SolidEntitiesLabel.Name = "SolidEntitiesLabel";
            this.SolidEntitiesLabel.Size = new System.Drawing.Size(67, 13);
            this.SolidEntitiesLabel.TabIndex = 0;
            this.SolidEntitiesLabel.Text = "Solid Entities";
            // 
            // UniqueTexturesLabel
            // 
            this.UniqueTexturesLabel.AutoSize = true;
            this.UniqueTexturesLabel.Location = new System.Drawing.Point(3, 80);
            this.UniqueTexturesLabel.Name = "UniqueTexturesLabel";
            this.UniqueTexturesLabel.Size = new System.Drawing.Size(88, 13);
            this.UniqueTexturesLabel.TabIndex = 0;
            this.UniqueTexturesLabel.Text = "Unique Textures:";
            // 
            // TextureMemoryLabel
            // 
            this.TextureMemoryLabel.AutoSize = true;
            this.TextureMemoryLabel.Location = new System.Drawing.Point(3, 100);
            this.TextureMemoryLabel.Name = "TextureMemoryLabel";
            this.TextureMemoryLabel.Size = new System.Drawing.Size(86, 13);
            this.TextureMemoryLabel.TabIndex = 0;
            this.TextureMemoryLabel.Text = "Texture Memory:";
            // 
            // NumSolids
            // 
            this.NumSolids.AutoSize = true;
            this.NumSolids.Location = new System.Drawing.Point(102, 0);
            this.NumSolids.Name = "NumSolids";
            this.NumSolids.Size = new System.Drawing.Size(37, 13);
            this.NumSolids.TabIndex = 0;
            this.NumSolids.Text = "12345";
            // 
            // NumFaces
            // 
            this.NumFaces.AutoSize = true;
            this.NumFaces.Location = new System.Drawing.Point(102, 20);
            this.NumFaces.Name = "NumFaces";
            this.NumFaces.Size = new System.Drawing.Size(37, 13);
            this.NumFaces.TabIndex = 0;
            this.NumFaces.Text = "12345";
            // 
            // NumPointEntities
            // 
            this.NumPointEntities.AutoSize = true;
            this.NumPointEntities.Location = new System.Drawing.Point(102, 40);
            this.NumPointEntities.Name = "NumPointEntities";
            this.NumPointEntities.Size = new System.Drawing.Size(37, 13);
            this.NumPointEntities.TabIndex = 0;
            this.NumPointEntities.Text = "12345";
            // 
            // NumSolidEntities
            // 
            this.NumSolidEntities.AutoSize = true;
            this.NumSolidEntities.Location = new System.Drawing.Point(102, 60);
            this.NumSolidEntities.Name = "NumSolidEntities";
            this.NumSolidEntities.Size = new System.Drawing.Size(37, 13);
            this.NumSolidEntities.TabIndex = 0;
            this.NumSolidEntities.Text = "12345";
            // 
            // NumUniqueTextures
            // 
            this.NumUniqueTextures.AutoSize = true;
            this.NumUniqueTextures.Location = new System.Drawing.Point(102, 80);
            this.NumUniqueTextures.Name = "NumUniqueTextures";
            this.NumUniqueTextures.Size = new System.Drawing.Size(37, 13);
            this.NumUniqueTextures.TabIndex = 0;
            this.NumUniqueTextures.Text = "12345";
            // 
            // TexturePackagesUsedLabel
            // 
            this.TexturePackagesUsedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TexturePackagesUsedLabel.AutoSize = true;
            this.TexturePackagesUsedLabel.Location = new System.Drawing.Point(12, 137);
            this.TexturePackagesUsedLabel.Name = "TexturePackagesUsedLabel";
            this.TexturePackagesUsedLabel.Size = new System.Drawing.Size(122, 13);
            this.TexturePackagesUsedLabel.TabIndex = 1;
            this.TexturePackagesUsedLabel.Text = "Texture packages used:";
            // 
            // TexturePackages
            // 
            this.TexturePackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TexturePackages.FormattingEnabled = true;
            this.TexturePackages.Location = new System.Drawing.Point(12, 153);
            this.TexturePackages.Name = "TexturePackages";
            this.TexturePackages.Size = new System.Drawing.Size(251, 108);
            this.TexturePackages.TabIndex = 2;
            // 
            // CloseDialogButton
            // 
            this.CloseDialogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseDialogButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseDialogButton.Location = new System.Drawing.Point(188, 267);
            this.CloseDialogButton.Name = "CloseDialogButton";
            this.CloseDialogButton.Size = new System.Drawing.Size(75, 23);
            this.CloseDialogButton.TabIndex = 3;
            this.CloseDialogButton.Text = "Close";
            this.CloseDialogButton.UseVisualStyleBackColor = true;
            this.CloseDialogButton.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // TextureMemoryValue
            // 
            this.TextureMemoryValue.AutoSize = true;
            this.TextureMemoryValue.Location = new System.Drawing.Point(102, 100);
            this.TextureMemoryValue.Name = "TextureMemoryValue";
            this.TextureMemoryValue.Size = new System.Drawing.Size(37, 13);
            this.TextureMemoryValue.TabIndex = 0;
            this.TextureMemoryValue.Text = "12345";
            // 
            // MapInformationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 300);
            this.Controls.Add(this.CloseDialogButton);
            this.Controls.Add(this.TexturePackages);
            this.Controls.Add(this.TexturePackagesUsedLabel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapInformationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map Information";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label SolidsLabel;
        private System.Windows.Forms.Label FacesLabel;
        private System.Windows.Forms.Label PointEntitiesLabel;
        private System.Windows.Forms.Label SolidEntitiesLabel;
        private System.Windows.Forms.Label UniqueTexturesLabel;
        private System.Windows.Forms.Label TextureMemoryLabel;
        private System.Windows.Forms.Label NumSolids;
        private System.Windows.Forms.Label NumFaces;
        private System.Windows.Forms.Label NumPointEntities;
        private System.Windows.Forms.Label NumSolidEntities;
        private System.Windows.Forms.Label NumUniqueTextures;
        private System.Windows.Forms.Label TexturePackagesUsedLabel;
        private System.Windows.Forms.ListBox TexturePackages;
        private System.Windows.Forms.Button CloseDialogButton;
        private System.Windows.Forms.Label TextureMemoryValue;
    }
}