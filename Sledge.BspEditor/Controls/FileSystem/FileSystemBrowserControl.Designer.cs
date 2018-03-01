namespace Sledge.BspEditor.Controls.FileSystem
{
    partial class FileSystemBrowserControl
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
            this.components = new System.ComponentModel.Container();
            this.FileList = new System.Windows.Forms.ListView();
            this.FileImages = new System.Windows.Forms.ImageList(this.components);
            this.LocationTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UpButton = new System.Windows.Forms.Button();
            this.SelectionTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.FilterLabelStart = new System.Windows.Forms.Label();
            this.FilterLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FileList
            // 
            this.FileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileList.Location = new System.Drawing.Point(3, 34);
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(467, 339);
            this.FileList.SmallImageList = this.FileImages;
            this.FileList.TabIndex = 0;
            this.FileList.UseCompatibleStateImageBehavior = false;
            this.FileList.View = System.Windows.Forms.View.List;
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.UpdateSelection);
            this.FileList.DoubleClick += new System.EventHandler(this.FileListDoubleClicked);
            // 
            // FileImages
            // 
            this.FileImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.FileImages.ImageSize = new System.Drawing.Size(16, 16);
            this.FileImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // LocationTextbox
            // 
            this.LocationTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocationTextbox.Enabled = false;
            this.LocationTextbox.Location = new System.Drawing.Point(57, 8);
            this.LocationTextbox.Name = "LocationTextbox";
            this.LocationTextbox.Size = new System.Drawing.Size(372, 20);
            this.LocationTextbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Location";
            // 
            // UpButton
            // 
            this.UpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UpButton.Location = new System.Drawing.Point(435, 8);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(35, 20);
            this.UpButton.TabIndex = 3;
            this.UpButton.Text = "Up";
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButtonClicked);
            // 
            // SelectionTextbox
            // 
            this.SelectionTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectionTextbox.Enabled = false;
            this.SelectionTextbox.Location = new System.Drawing.Point(57, 379);
            this.SelectionTextbox.Name = "SelectionTextbox";
            this.SelectionTextbox.Size = new System.Drawing.Size(413, 20);
            this.SelectionTextbox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 382);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Selected";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(395, 405);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OkButtonClicked);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(314, 405);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.CancelButtonClicked);
            // 
            // FilterLabelStart
            // 
            this.FilterLabelStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FilterLabelStart.AutoSize = true;
            this.FilterLabelStart.Location = new System.Drawing.Point(3, 410);
            this.FilterLabelStart.Name = "FilterLabelStart";
            this.FilterLabelStart.Size = new System.Drawing.Size(51, 13);
            this.FilterLabelStart.TabIndex = 5;
            this.FilterLabelStart.Text = "File Filter:";
            // 
            // FilterLabel
            // 
            this.FilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FilterLabel.AutoSize = true;
            this.FilterLabel.Location = new System.Drawing.Point(60, 410);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(37, 13);
            this.FilterLabel.TabIndex = 5;
            this.FilterLabel.Text = "(none)";
            // 
            // FileSystemBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FilterLabel);
            this.Controls.Add(this.FilterLabelStart);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.UpButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SelectionTextbox);
            this.Controls.Add(this.LocationTextbox);
            this.Controls.Add(this.FileList);
            this.Name = "FileSystemBrowserControl";
            this.Size = new System.Drawing.Size(473, 434);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView FileList;
        private System.Windows.Forms.TextBox LocationTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.ImageList FileImages;
        private System.Windows.Forms.TextBox SelectionTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label FilterLabelStart;
        private System.Windows.Forms.Label FilterLabel;
    }
}
