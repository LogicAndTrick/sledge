namespace Sledge.Shell.Settings.Editors
{
    partial class NumericEditor
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
            this.Numericbox = new System.Windows.Forms.NumericUpDown();
            this.Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Numericbox)).BeginInit();
            this.SuspendLayout();
            // 
            // Numericbox
            // 
            this.Numericbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Numericbox.DecimalPlaces = 2;
            this.Numericbox.Location = new System.Drawing.Point(270, 3);
            this.Numericbox.Name = "Numericbox";
            this.Numericbox.Size = new System.Drawing.Size(77, 20);
            this.Numericbox.TabIndex = 3;
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
            // NumericEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Numericbox);
            this.Controls.Add(this.Label);
            this.Name = "NumericEditor";
            this.Size = new System.Drawing.Size(350, 26);
            ((System.ComponentModel.ISupportInitialize)(this.Numericbox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown Numericbox;
        private System.Windows.Forms.Label Label;
    }
}
