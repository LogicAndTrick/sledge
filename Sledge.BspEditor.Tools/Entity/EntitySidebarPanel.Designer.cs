namespace Sledge.BspEditor.Tools.Entity
{
    partial class EntitySidebarPanel
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
            this.EntityTypeLabel = new System.Windows.Forms.Label();
            this.EntityTypeList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // EntityTypeLabel
            // 
            this.EntityTypeLabel.Location = new System.Drawing.Point(3, 0);
            this.EntityTypeLabel.Name = "EntityTypeLabel";
            this.EntityTypeLabel.Size = new System.Drawing.Size(108, 17);
            this.EntityTypeLabel.TabIndex = 9;
            this.EntityTypeLabel.Text = "Entity Type:";
            this.EntityTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EntityTypeList
            // 
            this.EntityTypeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EntityTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EntityTypeList.FormattingEnabled = true;
            this.EntityTypeList.Location = new System.Drawing.Point(3, 20);
            this.EntityTypeList.Name = "EntityTypeList";
            this.EntityTypeList.Size = new System.Drawing.Size(194, 21);
            this.EntityTypeList.TabIndex = 5;
            this.EntityTypeList.SelectedIndexChanged += new System.EventHandler(this.EntityTypeList_SelectedIndexChanged);
            // 
            // EntitySidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EntityTypeLabel);
            this.Controls.Add(this.EntityTypeList);
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Name = "EntitySidebarPanel";
            this.Size = new System.Drawing.Size(200, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label EntityTypeLabel;
        private System.Windows.Forms.ComboBox EntityTypeList;
    }
}
