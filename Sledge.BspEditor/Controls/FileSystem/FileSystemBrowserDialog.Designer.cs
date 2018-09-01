namespace Sledge.BspEditor.Controls.FileSystem
{
    partial class FileSystemBrowserDialog
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
            this.Browser = new FileSystemBrowserControl();
            this.SuspendLayout();
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.File = null;
            this.Browser.Filter = null;
            this.Browser.FilterText = "";
            this.Browser.Location = new System.Drawing.Point(0, 0);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(537, 369);
            this.Browser.TabIndex = 0;
            // 
            // FileSystemBrowserDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 369);
            this.Controls.Add(this.Browser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileSystemBrowserDialog";
            this.Text = "File System Browser";
            this.ResumeLayout(false);

        }

        #endregion

        private FileSystemBrowserControl Browser;
    }
}