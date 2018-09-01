namespace Sledge.Shell.Settings.Editors
{
    partial class FileAssociationsEditor
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
            this.CheckboxPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // CheckboxPanel
            // 
            this.CheckboxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckboxPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.CheckboxPanel.Location = new System.Drawing.Point(3, 3);
            this.CheckboxPanel.Name = "CheckboxPanel";
            this.CheckboxPanel.Size = new System.Drawing.Size(505, 321);
            this.CheckboxPanel.TabIndex = 0;
            // 
            // FileAssociationsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CheckboxPanel);
            this.Name = "FileAssociationsEditor";
            this.Size = new System.Drawing.Size(511, 324);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel CheckboxPanel;
    }
}
