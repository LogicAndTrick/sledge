namespace Sledge.Editor.UI
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
            this.GoToButton = new System.Windows.Forms.Button();
            this.FixButton = new System.Windows.Forms.Button();
            this.FixAllTypeButton = new System.Windows.Forms.Button();
            this.FixAllButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.VisibleOnlyCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
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
            this.DescriptionTextBox.Size = new System.Drawing.Size(272, 110);
            this.DescriptionTextBox.TabIndex = 1;
            // 
            // GoToButton
            // 
            this.GoToButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoToButton.Location = new System.Drawing.Point(284, 19);
            this.GoToButton.Name = "GoToButton";
            this.GoToButton.Size = new System.Drawing.Size(98, 23);
            this.GoToButton.TabIndex = 2;
            this.GoToButton.Text = "Go to error";
            this.GoToButton.UseVisualStyleBackColor = true;
            this.GoToButton.Click += new System.EventHandler(this.GoToError);
            // 
            // FixButton
            // 
            this.FixButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FixButton.Location = new System.Drawing.Point(284, 48);
            this.FixButton.Name = "FixButton";
            this.FixButton.Size = new System.Drawing.Size(98, 23);
            this.FixButton.TabIndex = 2;
            this.FixButton.Text = "Fix error";
            this.FixButton.UseVisualStyleBackColor = true;
            this.FixButton.Click += new System.EventHandler(this.FixError);
            // 
            // FixAllTypeButton
            // 
            this.FixAllTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FixAllTypeButton.Location = new System.Drawing.Point(284, 77);
            this.FixAllTypeButton.Name = "FixAllTypeButton";
            this.FixAllTypeButton.Size = new System.Drawing.Size(98, 23);
            this.FixAllTypeButton.TabIndex = 2;
            this.FixAllTypeButton.Text = "Fix all of type";
            this.FixAllTypeButton.UseVisualStyleBackColor = true;
            this.FixAllTypeButton.Click += new System.EventHandler(this.FixAllOfType);
            // 
            // FixAllButton
            // 
            this.FixAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FixAllButton.Location = new System.Drawing.Point(284, 106);
            this.FixAllButton.Name = "FixAllButton";
            this.FixAllButton.Size = new System.Drawing.Size(98, 23);
            this.FixAllButton.TabIndex = 2;
            this.FixAllButton.Text = "Fix all problems";
            this.FixAllButton.UseVisualStyleBackColor = true;
            this.FixAllButton.Click += new System.EventHandler(this.FixAll);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(296, 313);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(98, 23);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.DescriptionTextBox);
            this.groupBox1.Controls.Add(this.GoToButton);
            this.groupBox1.Controls.Add(this.FixAllButton);
            this.groupBox1.Controls.Add(this.FixButton);
            this.groupBox1.Controls.Add(this.FixAllTypeButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 140);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // VisibleOnlyCheckbox
            // 
            this.VisibleOnlyCheckbox.AutoSize = true;
            this.VisibleOnlyCheckbox.Checked = true;
            this.VisibleOnlyCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VisibleOnlyCheckbox.Location = new System.Drawing.Point(18, 317);
            this.VisibleOnlyCheckbox.Name = "VisibleOnlyCheckbox";
            this.VisibleOnlyCheckbox.Size = new System.Drawing.Size(115, 17);
            this.VisibleOnlyCheckbox.TabIndex = 4;
            this.VisibleOnlyCheckbox.Text = "Visible objects only";
            this.VisibleOnlyCheckbox.UseVisualStyleBackColor = true;
            this.VisibleOnlyCheckbox.CheckedChanged += new System.EventHandler(this.VisibleOnlyCheckboxChanged);
            // 
            // CheckForProblemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 345);
            this.Controls.Add(this.VisibleOnlyCheckbox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ProblemsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckForProblemsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Problems in Map";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ProblemsList;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.Button GoToButton;
        private System.Windows.Forms.Button FixButton;
        private System.Windows.Forms.Button FixAllTypeButton;
        private System.Windows.Forms.Button FixAllButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox VisibleOnlyCheckbox;
    }
}