namespace Sledge.Shell.Settings.Editors
{
    partial class TextEditor
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
            this.Textbox = new System.Windows.Forms.TextBox();
            this.Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Textbox
            // 
            this.Textbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Textbox.Location = new System.Drawing.Point(86, 3);
            this.Textbox.Name = "Textbox";
            this.Textbox.Size = new System.Drawing.Size(261, 20);
            this.Textbox.TabIndex = 3;
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(3, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(35, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "label1";
            // 
            // TextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Textbox);
            this.Controls.Add(this.Label);
            this.Name = "TextEditor";
            this.Size = new System.Drawing.Size(350, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Textbox;
        private System.Windows.Forms.Label Label;
    }
}
