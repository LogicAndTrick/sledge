namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    partial class VertexErrorsSidebarPanel
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
            this.ErrorList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ErrorList
            // 
            this.ErrorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorList.FormattingEnabled = true;
            this.ErrorList.Location = new System.Drawing.Point(3, 3);
            this.ErrorList.Name = "ErrorList";
            this.ErrorList.Size = new System.Drawing.Size(214, 121);
            this.ErrorList.TabIndex = 7;
            this.ErrorList.SelectedIndexChanged += new System.EventHandler(this.ErrorListSelectionChanged);
            // 
            // VertexErrorsSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ErrorList);
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Name = "VertexErrorsSidebarPanel";
            this.Size = new System.Drawing.Size(220, 128);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ErrorList;

    }
}
