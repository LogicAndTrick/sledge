namespace Sledge.BspEditor.Editing.Components
{
    partial class SelectionDetailsDialog
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
            this.SelectionTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // SelectionTree
            // 
            this.SelectionTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionTree.Location = new System.Drawing.Point(0, 0);
            this.SelectionTree.Name = "SelectionTree";
            this.SelectionTree.Size = new System.Drawing.Size(526, 429);
            this.SelectionTree.TabIndex = 0;
            // 
            // SelectionDetailsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 429);
            this.Controls.Add(this.SelectionTree);
            this.Name = "SelectionDetailsDialog";
            this.Text = "SelectionDetailsDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView SelectionTree;
    }
}