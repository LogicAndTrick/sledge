namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    partial class NumericControl
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
            this.Numeric = new System.Windows.Forms.NumericUpDown();
            this.Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric)).BeginInit();
            this.SuspendLayout();
            // 
            // Numeric
            // 
            this.Numeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Numeric.Location = new System.Drawing.Point(73, 3);
            this.Numeric.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.Numeric.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.Numeric.Name = "Numeric";
            this.Numeric.Size = new System.Drawing.Size(47, 20);
            this.Numeric.TabIndex = 3;
            this.Numeric.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.Numeric.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(1, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(57, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "Label Text";
            // 
            // NumericControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Numeric);
            this.Controls.Add(this.Label);
            this.Name = "NumericControl";
            this.Size = new System.Drawing.Size(124, 26);
            ((System.ComponentModel.ISupportInitialize)(this.Numeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown Numeric;
        private System.Windows.Forms.Label Label;
    }
}
