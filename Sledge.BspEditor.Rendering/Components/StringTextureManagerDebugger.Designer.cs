namespace Sledge.BspEditor.Rendering.Components
{
    partial class StringTextureManagerDebugger
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
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.TextureList = new System.Windows.Forms.ListView();
            this.StringCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DimensionCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LocationCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FontCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UpdateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.Location = new System.Drawing.Point(13, 13);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(663, 538);
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            // 
            // TextureList
            // 
            this.TextureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextureList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.StringCol,
            this.FontCol,
            this.LocationCol,
            this.DimensionCol});
            this.TextureList.FullRowSelect = true;
            this.TextureList.HideSelection = false;
            this.TextureList.Location = new System.Drawing.Point(682, 12);
            this.TextureList.MultiSelect = false;
            this.TextureList.Name = "TextureList";
            this.TextureList.Size = new System.Drawing.Size(292, 510);
            this.TextureList.TabIndex = 1;
            this.TextureList.UseCompatibleStateImageBehavior = false;
            this.TextureList.View = System.Windows.Forms.View.Details;
            this.TextureList.SelectedIndexChanged += new System.EventHandler(this.TextureList_SelectedIndexChanged);
            // 
            // StringCol
            // 
            this.StringCol.Text = "String";
            this.StringCol.Width = 75;
            // 
            // DimensionCol
            // 
            this.DimensionCol.Text = "Dimensions";
            this.DimensionCol.Width = 90;
            // 
            // LocationCol
            // 
            this.LocationCol.Text = "Location";
            this.LocationCol.Width = 108;
            // 
            // FontCol
            // 
            this.FontCol.Text = "Font";
            // 
            // UpdateButton
            // 
            this.UpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateButton.Location = new System.Drawing.Point(899, 528);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 23);
            this.UpdateButton.TabIndex = 2;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // StringTextureManagerDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 563);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.TextureList);
            this.Controls.Add(this.ImageBox);
            this.Name = "StringTextureManagerDebugger";
            this.Text = "StringTextureManagerDebugger";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.ListView TextureList;
        private System.Windows.Forms.ColumnHeader StringCol;
        private System.Windows.Forms.ColumnHeader LocationCol;
        private System.Windows.Forms.ColumnHeader DimensionCol;
        private System.Windows.Forms.ColumnHeader FontCol;
        private System.Windows.Forms.Button UpdateButton;
    }
}