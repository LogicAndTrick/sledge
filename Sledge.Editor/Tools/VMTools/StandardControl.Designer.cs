namespace Sledge.Editor.Tools.VMTools
{
    partial class StandardControl
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
            this.MergeButton = new System.Windows.Forms.Button();
            this.AutoMerge = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SplitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MergeButton
            // 
            this.MergeButton.Location = new System.Drawing.Point(6, 55);
            this.MergeButton.Name = "MergeButton";
            this.MergeButton.Size = new System.Drawing.Size(211, 23);
            this.MergeButton.TabIndex = 8;
            this.MergeButton.Text = "Merge overlapping vertices in selection";
            this.MergeButton.UseVisualStyleBackColor = true;
            this.MergeButton.Click += new System.EventHandler(this.MergeButtonClicked);
            // 
            // AutoMerge
            // 
            this.AutoMerge.AutoSize = true;
            this.AutoMerge.Checked = true;
            this.AutoMerge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoMerge.Location = new System.Drawing.Point(6, 84);
            this.AutoMerge.Name = "AutoMerge";
            this.AutoMerge.Size = new System.Drawing.Size(160, 17);
            this.AutoMerge.TabIndex = 7;
            this.AutoMerge.Text = "Automatically merge vertices";
            this.AutoMerge.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 52);
            this.label1.TabIndex = 6;
            this.label1.Text = "Click a vertex to select all points under the cursor.\r\n - Hold control to select " +
    "multiple points.\r\n - Hold shift to only select the topmost point.\r\nDrag vertices" +
    " to move them around.\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 26);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select two (non-adjacent) points\r\non a face to enable splitting.\r\n";
            // 
            // SplitButton
            // 
            this.SplitButton.Location = new System.Drawing.Point(168, 115);
            this.SplitButton.Name = "SplitButton";
            this.SplitButton.Size = new System.Drawing.Size(76, 23);
            this.SplitButton.TabIndex = 8;
            this.SplitButton.Text = "Split face";
            this.SplitButton.UseVisualStyleBackColor = true;
            this.SplitButton.Click += new System.EventHandler(this.SplitButtonClicked);
            // 
            // StandardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SplitButton);
            this.Controls.Add(this.MergeButton);
            this.Controls.Add(this.AutoMerge);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "StandardControl";
            this.Size = new System.Drawing.Size(250, 156);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button MergeButton;
        private System.Windows.Forms.CheckBox AutoMerge;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SplitButton;
    }
}
