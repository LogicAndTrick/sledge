namespace Sledge.Editor.UI.Sidebar
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
            this.label2 = new System.Windows.Forms.Label();
            this.EntityTypeList = new System.Windows.Forms.ComboBox();
            this.MoveToWorldButton = new System.Windows.Forms.Button();
            this.MoveToEntityButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Entity Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
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
            // 
            // MoveToWorldButton
            // 
            this.MoveToWorldButton.Location = new System.Drawing.Point(3, 47);
            this.MoveToWorldButton.Name = "MoveToWorldButton";
            this.MoveToWorldButton.Size = new System.Drawing.Size(86, 23);
            this.MoveToWorldButton.TabIndex = 6;
            this.MoveToWorldButton.Text = "Move to World";
            this.MoveToWorldButton.UseVisualStyleBackColor = true;
            this.MoveToWorldButton.Click += new System.EventHandler(this.MoveToWorldButtonClicked);
            // 
            // MoveToEntityButton
            // 
            this.MoveToEntityButton.Location = new System.Drawing.Point(95, 47);
            this.MoveToEntityButton.Name = "MoveToEntityButton";
            this.MoveToEntityButton.Size = new System.Drawing.Size(84, 23);
            this.MoveToEntityButton.TabIndex = 7;
            this.MoveToEntityButton.Text = "Tie to Entity";
            this.MoveToEntityButton.UseVisualStyleBackColor = true;
            this.MoveToEntityButton.Click += new System.EventHandler(this.TieToEntityButtonClicked);
            // 
            // EntitySidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EntityTypeList);
            this.Controls.Add(this.MoveToWorldButton);
            this.Controls.Add(this.MoveToEntityButton);
            this.MinimumSize = new System.Drawing.Size(200, 80);
            this.Name = "EntitySidebarPanel";
            this.Size = new System.Drawing.Size(200, 80);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox EntityTypeList;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
    }
}
