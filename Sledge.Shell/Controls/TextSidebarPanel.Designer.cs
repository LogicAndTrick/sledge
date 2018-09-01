namespace Sledge.Shell.Controls
{
    partial class TextSidebarPanel
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
            this.HelpTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // HelpTextBox
            // 
            this.HelpTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.HelpTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HelpTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.HelpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HelpTextBox.Location = new System.Drawing.Point(0, 0);
            this.HelpTextBox.Name = "HelpTextBox";
            this.HelpTextBox.ReadOnly = true;
            this.HelpTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.HelpTextBox.Size = new System.Drawing.Size(200, 80);
            this.HelpTextBox.TabIndex = 0;
            this.HelpTextBox.Text = "";
            // 
            // TextSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HelpTextBox);
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Name = "TextSidebarPanel";
            this.Size = new System.Drawing.Size(200, 80);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HelpTextBox;

    }
}
