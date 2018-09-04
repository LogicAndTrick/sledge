namespace Sledge.Editor.Update
{
    partial class UpdaterForm
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
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.CancelButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.ReleaseDetails = new System.Windows.Forms.TextBox();
            this.ReleaseNotesLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(12, 9);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(66, 13);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Status Label";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(12, 246);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(293, 10);
            this.ProgressBar.Step = 1;
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 1;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(311, 233);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButtonClicked);
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(311, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Download";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.DownloadButtonClicked);
            // 
            // ReleaseDetails
            // 
            this.ReleaseDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReleaseDetails.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ReleaseDetails.Location = new System.Drawing.Point(15, 41);
            this.ReleaseDetails.Multiline = true;
            this.ReleaseDetails.Name = "ReleaseDetails";
            this.ReleaseDetails.ReadOnly = true;
            this.ReleaseDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReleaseDetails.Size = new System.Drawing.Size(371, 170);
            this.ReleaseDetails.TabIndex = 3;
            // 
            // ReleaseNotesLink
            // 
            this.ReleaseNotesLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ReleaseNotesLink.AutoSize = true;
            this.ReleaseNotesLink.Location = new System.Drawing.Point(12, 218);
            this.ReleaseNotesLink.Name = "ReleaseNotesLink";
            this.ReleaseNotesLink.Size = new System.Drawing.Size(252, 13);
            this.ReleaseNotesLink.TabIndex = 7;
            this.ReleaseNotesLink.TabStop = true;
            this.ReleaseNotesLink.Text = "Click here to see release notes for previous releases";
            this.ReleaseNotesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReleaseNotesLinkClicked);
            // 
            // UpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 266);
            this.Controls.Add(this.ReleaseNotesLink);
            this.Controls.Add(this.ReleaseDetails);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.StatusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdaterForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sledge Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdaterFormFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TextBox ReleaseDetails;
        private System.Windows.Forms.LinkLabel ReleaseNotesLink;
    }
}

