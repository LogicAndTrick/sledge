namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    partial class VertexScaleControl
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
            this.button6 = new System.Windows.Forms.Button();
            this.DistanceValue = new System.Windows.Forms.NumericUpDown();
            this.ScaleDistanceLabel = new System.Windows.Forms.Label();
            this.ResetDistanceButton = new System.Windows.Forms.Button();
            this.ResetOriginButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceValue)).BeginInit();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button6.Location = new System.Drawing.Point(148, -168);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 23);
            this.button6.TabIndex = 12;
            this.button6.Text = "Reset";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // DistanceValue
            // 
            this.DistanceValue.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.DistanceValue.Location = new System.Drawing.Point(94, 2);
            this.DistanceValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.DistanceValue.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.DistanceValue.Name = "DistanceValue";
            this.DistanceValue.Size = new System.Drawing.Size(48, 20);
            this.DistanceValue.TabIndex = 10;
            this.DistanceValue.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.DistanceValue.ValueChanged += new System.EventHandler(this.DistanceValueChanged);
            // 
            // ScaleDistanceLabel
            // 
            this.ScaleDistanceLabel.AutoSize = true;
            this.ScaleDistanceLabel.Location = new System.Drawing.Point(-1, 4);
            this.ScaleDistanceLabel.Name = "ScaleDistanceLabel";
            this.ScaleDistanceLabel.Size = new System.Drawing.Size(94, 13);
            this.ScaleDistanceLabel.TabIndex = 9;
            this.ScaleDistanceLabel.Text = "Scale distance (%)";
            // 
            // ResetDistanceButton
            // 
            this.ResetDistanceButton.Location = new System.Drawing.Point(145, 0);
            this.ResetDistanceButton.Name = "ResetDistanceButton";
            this.ResetDistanceButton.Size = new System.Drawing.Size(50, 23);
            this.ResetDistanceButton.TabIndex = 14;
            this.ResetDistanceButton.Text = "Reset";
            this.ResetDistanceButton.UseVisualStyleBackColor = true;
            this.ResetDistanceButton.Click += new System.EventHandler(this.ResetDistanceClicked);
            // 
            // ResetOriginButton
            // 
            this.ResetOriginButton.Location = new System.Drawing.Point(55, 28);
            this.ResetOriginButton.Name = "ResetOriginButton";
            this.ResetOriginButton.Size = new System.Drawing.Size(75, 23);
            this.ResetOriginButton.TabIndex = 15;
            this.ResetOriginButton.Text = "Reset Origin";
            this.ResetOriginButton.UseVisualStyleBackColor = true;
            this.ResetOriginButton.Click += new System.EventHandler(this.ResetOriginClicked);
            // 
            // VertexScaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ResetOriginButton);
            this.Controls.Add(this.ResetDistanceButton);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.DistanceValue);
            this.Controls.Add(this.ScaleDistanceLabel);
            this.Name = "VertexScaleControl";
            this.Size = new System.Drawing.Size(198, 61);
            ((System.ComponentModel.ISupportInitialize)(this.DistanceValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.NumericUpDown DistanceValue;
        private System.Windows.Forms.Label ScaleDistanceLabel;
        private System.Windows.Forms.Button ResetDistanceButton;
        private System.Windows.Forms.Button ResetOriginButton;
    }
}
