namespace Sledge.BspEditor.Editing.Components
{
    partial class CheckForProblemsDialog
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
            this.ProblemsList = new System.Windows.Forms.ListBox();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.btnGoToError = new System.Windows.Forms.Button();
            this.btnFix = new System.Windows.Forms.Button();
            this.btnFixAllOfType = new System.Windows.Forms.Button();
            this.btnFixAll = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.lnkExtraDetails = new System.Windows.Forms.LinkLabel();
            this.chkVisibleOnly = new System.Windows.Forms.CheckBox();
            this.chkSelectedOnly = new System.Windows.Forms.CheckBox();
            this.grpDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProblemsList
            // 
            this.ProblemsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProblemsList.FormattingEnabled = true;
            this.ProblemsList.Location = new System.Drawing.Point(12, 12);
            this.ProblemsList.Name = "ProblemsList";
            this.ProblemsList.Size = new System.Drawing.Size(388, 147);
            this.ProblemsList.TabIndex = 0;
            this.ProblemsList.SelectedIndexChanged += new System.EventHandler(this.UpdateSelectedProblem);
            this.ProblemsList.DoubleClick += new System.EventHandler(this.GoToError);
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(6, 19);
            this.DescriptionTextBox.Multiline = true;
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.Size = new System.Drawing.Size(219, 94);
            this.DescriptionTextBox.TabIndex = 1;
            // 
            // btnGoToError
            // 
            this.btnGoToError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoToError.Location = new System.Drawing.Point(231, 19);
            this.btnGoToError.Name = "btnGoToError";
            this.btnGoToError.Size = new System.Drawing.Size(151, 23);
            this.btnGoToError.TabIndex = 2;
            this.btnGoToError.Text = "Go to error";
            this.btnGoToError.UseVisualStyleBackColor = true;
            this.btnGoToError.Click += new System.EventHandler(this.GoToError);
            // 
            // btnFix
            // 
            this.btnFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFix.Location = new System.Drawing.Point(231, 48);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(151, 23);
            this.btnFix.TabIndex = 2;
            this.btnFix.Text = "Fix error";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.FixError);
            // 
            // btnFixAllOfType
            // 
            this.btnFixAllOfType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFixAllOfType.Location = new System.Drawing.Point(231, 77);
            this.btnFixAllOfType.Name = "btnFixAllOfType";
            this.btnFixAllOfType.Size = new System.Drawing.Size(151, 23);
            this.btnFixAllOfType.TabIndex = 2;
            this.btnFixAllOfType.Text = "Fix all of type";
            this.btnFixAllOfType.UseVisualStyleBackColor = true;
            this.btnFixAllOfType.Click += new System.EventHandler(this.FixAllOfType);
            // 
            // btnFixAll
            // 
            this.btnFixAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFixAll.Location = new System.Drawing.Point(231, 106);
            this.btnFixAll.Name = "btnFixAll";
            this.btnFixAll.Size = new System.Drawing.Size(151, 23);
            this.btnFixAll.TabIndex = 2;
            this.btnFixAll.Text = "Fix all problems";
            this.btnFixAll.UseVisualStyleBackColor = true;
            this.btnFixAll.Click += new System.EventHandler(this.FixAll);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(296, 313);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.CloseWindow);
            // 
            // grpDetails
            // 
            this.grpDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetails.Controls.Add(this.lnkExtraDetails);
            this.grpDetails.Controls.Add(this.DescriptionTextBox);
            this.grpDetails.Controls.Add(this.btnGoToError);
            this.grpDetails.Controls.Add(this.btnFixAll);
            this.grpDetails.Controls.Add(this.btnFix);
            this.grpDetails.Controls.Add(this.btnFixAllOfType);
            this.grpDetails.Location = new System.Drawing.Point(12, 167);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(388, 140);
            this.grpDetails.TabIndex = 3;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "Details";
            // 
            // lnkExtraDetails
            // 
            this.lnkExtraDetails.AutoSize = true;
            this.lnkExtraDetails.Location = new System.Drawing.Point(6, 116);
            this.lnkExtraDetails.Name = "lnkExtraDetails";
            this.lnkExtraDetails.Size = new System.Drawing.Size(171, 13);
            this.lnkExtraDetails.TabIndex = 3;
            this.lnkExtraDetails.TabStop = true;
            this.lnkExtraDetails.Text = "Click here for additional information";
            this.lnkExtraDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenUrl);
            // 
            // chkVisibleOnly
            // 
            this.chkVisibleOnly.AutoSize = true;
            this.chkVisibleOnly.Checked = true;
            this.chkVisibleOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVisibleOnly.Location = new System.Drawing.Point(18, 317);
            this.chkVisibleOnly.Name = "chkVisibleOnly";
            this.chkVisibleOnly.Size = new System.Drawing.Size(115, 17);
            this.chkVisibleOnly.TabIndex = 4;
            this.chkVisibleOnly.Text = "Visible objects only";
            this.chkVisibleOnly.UseVisualStyleBackColor = true;
            this.chkVisibleOnly.CheckedChanged += new System.EventHandler(this.VisibleOnlyCheckboxChanged);
            // 
            // chkSelectedOnly
            // 
            this.chkSelectedOnly.AutoSize = true;
            this.chkSelectedOnly.Location = new System.Drawing.Point(152, 317);
            this.chkSelectedOnly.Name = "chkSelectedOnly";
            this.chkSelectedOnly.Size = new System.Drawing.Size(127, 17);
            this.chkSelectedOnly.TabIndex = 4;
            this.chkSelectedOnly.Text = "Selected objects only";
            this.chkSelectedOnly.UseVisualStyleBackColor = true;
            this.chkSelectedOnly.CheckedChanged += new System.EventHandler(this.VisibleOnlyCheckboxChanged);
            // 
            // CheckForProblemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 345);
            this.Controls.Add(this.chkSelectedOnly);
            this.Controls.Add(this.chkVisibleOnly);
            this.Controls.Add(this.grpDetails);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ProblemsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckForProblemsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Problems in Map";
            this.grpDetails.ResumeLayout(false);
            this.grpDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ProblemsList;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.Button btnGoToError;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnFixAllOfType;
        private System.Windows.Forms.Button btnFixAll;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.CheckBox chkVisibleOnly;
        private System.Windows.Forms.CheckBox chkSelectedOnly;
        private System.Windows.Forms.LinkLabel lnkExtraDetails;
    }
}