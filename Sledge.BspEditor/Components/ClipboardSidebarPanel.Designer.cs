namespace Sledge.BspEditor.Components
{
    partial class ClipboardSidebarPanel
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
            this.ClipboardList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ClipboardList
            // 
            this.ClipboardList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClipboardList.FormattingEnabled = true;
            this.ClipboardList.Location = new System.Drawing.Point(3, 3);
            this.ClipboardList.Name = "ClipboardList";
            this.ClipboardList.Size = new System.Drawing.Size(220, 134);
            this.ClipboardList.TabIndex = 0;
            // 
            // ClipboardSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ClipboardList);
            this.Name = "ClipboardSidebarPanel";
            this.Size = new System.Drawing.Size(226, 140);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ClipboardList;
    }
}
