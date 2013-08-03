namespace Sledge.Editor.Tools.VMTools
{
    partial class ScaleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleControl));
            this.UseRelative = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.DistanceValue = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ResetDistanceButton = new System.Windows.Forms.Button();
            this.ResetOriginButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceValue)).BeginInit();
            this.SuspendLayout();
            // 
            // UseRelative
            // 
            this.UseRelative.AutoSize = true;
            this.UseRelative.Location = new System.Drawing.Point(9, 83);
            this.UseRelative.Name = "UseRelative";
            this.UseRelative.Size = new System.Drawing.Size(179, 17);
            this.UseRelative.TabIndex = 11;
            this.UseRelative.Text = "Use relative values (percentage)";
            this.UseRelative.UseVisualStyleBackColor = true;
            this.UseRelative.CheckedChanged += new System.EventHandler(this.UseRelativeChanged);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button6.Location = new System.Drawing.Point(150, -85);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 23);
            this.button6.TabIndex = 12;
            this.button6.Text = "Reset";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // DistanceValue
            // 
            this.DistanceValue.Location = new System.Drawing.Point(89, 57);
            this.DistanceValue.Name = "DistanceValue";
            this.DistanceValue.Size = new System.Drawing.Size(58, 20);
            this.DistanceValue.TabIndex = 10;
            this.DistanceValue.ValueChanged += new System.EventHandler(this.DistanceValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Scale distance";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 52);
            this.label1.TabIndex = 13;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // ResetDistanceButton
            // 
            this.ResetDistanceButton.Location = new System.Drawing.Point(153, 57);
            this.ResetDistanceButton.Name = "ResetDistanceButton";
            this.ResetDistanceButton.Size = new System.Drawing.Size(94, 20);
            this.ResetDistanceButton.TabIndex = 14;
            this.ResetDistanceButton.Text = "Reset Distance";
            this.ResetDistanceButton.UseVisualStyleBackColor = true;
            this.ResetDistanceButton.Click += new System.EventHandler(this.ResetDistanceClicked);
            // 
            // ResetOriginButton
            // 
            this.ResetOriginButton.Location = new System.Drawing.Point(101, 106);
            this.ResetOriginButton.Name = "ResetOriginButton";
            this.ResetOriginButton.Size = new System.Drawing.Size(75, 23);
            this.ResetOriginButton.TabIndex = 15;
            this.ResetOriginButton.Text = "Reset Origin";
            this.ResetOriginButton.UseVisualStyleBackColor = true;
            this.ResetOriginButton.Click += new System.EventHandler(this.ResetOriginClicked);
            // 
            // ScaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ResetOriginButton);
            this.Controls.Add(this.ResetDistanceButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UseRelative);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.DistanceValue);
            this.Controls.Add(this.label4);
            this.Name = "ScaleControl";
            this.Size = new System.Drawing.Size(276, 142);
            ((System.ComponentModel.ISupportInitialize)(this.DistanceValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox UseRelative;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.NumericUpDown DistanceValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ResetDistanceButton;
        private System.Windows.Forms.Button ResetOriginButton;
    }
}
