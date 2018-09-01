namespace Sledge.Shell.Settings.Editors
{
    partial class SliderEditor
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
            this.Slider = new System.Windows.Forms.TrackBar();
            this.Label = new System.Windows.Forms.Label();
            this.NumericBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Slider
            // 
            this.Slider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Slider.Location = new System.Drawing.Point(44, 3);
            this.Slider.Name = "Slider";
            this.Slider.Size = new System.Drawing.Size(241, 45);
            this.Slider.TabIndex = 3;
            this.Slider.ValueChanged += new System.EventHandler(this.SliderChanged);
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
            // NumericBox
            // 
            this.NumericBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericBox.DecimalPlaces = 2;
            this.NumericBox.Location = new System.Drawing.Point(291, 6);
            this.NumericBox.Name = "NumericBox";
            this.NumericBox.Size = new System.Drawing.Size(56, 20);
            this.NumericBox.TabIndex = 4;
            this.NumericBox.ValueChanged += new System.EventHandler(this.NumberChanged);
            // 
            // SliderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NumericBox);
            this.Controls.Add(this.Slider);
            this.Controls.Add(this.Label);
            this.Name = "SliderEditor";
            this.Size = new System.Drawing.Size(350, 51);
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar Slider;
        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.NumericUpDown NumericBox;
    }
}
