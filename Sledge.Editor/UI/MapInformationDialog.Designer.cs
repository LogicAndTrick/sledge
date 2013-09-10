namespace Sledge.Editor.UI
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.NumSolids = new System.Windows.Forms.Label();
            this.NumFaces = new System.Windows.Forms.Label();
            this.NumPointEntities = new System.Windows.Forms.Label();
            this.NumSolidEntities = new System.Windows.Forms.Label();
            this.NumUniqueTextures = new System.Windows.Forms.Label();
            this.TextureMemory = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TexturePackages = new System.Windows.Forms.ListBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.NumSolids, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.NumFaces, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.NumPointEntities, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.NumSolidEntities, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.NumUniqueTextures, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.TextureMemory, 1, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 120);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Solids:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Faces:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Point Entities";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Solid Entities";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Unique Textures:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Texture Memory:";
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
            // TextureMemory
            // 
            this.TextureMemory.AutoSize = true;
            this.TextureMemory.Location = new System.Drawing.Point(102, 100);
            this.TextureMemory.Name = "TextureMemory";
            this.TextureMemory.Size = new System.Drawing.Size(37, 13);
            this.TextureMemory.TabIndex = 0;
            this.TextureMemory.Text = "12345";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Texture packages used:";
            // 
            // TexturePackages
            // 
            this.TexturePackages.FormattingEnabled = true;
            this.TexturePackages.Location = new System.Drawing.Point(12, 151);
            this.TexturePackages.Name = "TexturePackages";
            this.TexturePackages.Size = new System.Drawing.Size(251, 108);
            this.TexturePackages.TabIndex = 2;
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(188, 265);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // MapInformationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 298);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.TexturePackages);
            this.Controls.Add(this.label7);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label NumSolids;
        private System.Windows.Forms.Label NumFaces;
        private System.Windows.Forms.Label NumPointEntities;
        private System.Windows.Forms.Label NumSolidEntities;
        private System.Windows.Forms.Label NumUniqueTextures;
        private System.Windows.Forms.Label TextureMemory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox TexturePackages;
        private System.Windows.Forms.Button CloseButton;
    }
}