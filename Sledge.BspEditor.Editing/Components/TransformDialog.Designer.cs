namespace Sledge.BspEditor.Editing.Components
{
    partial class TransformDialog
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
            this.lblRotate = new System.Windows.Forms.RadioButton();
            this.lblTranslate = new System.Windows.Forms.RadioButton();
            this.lblScale = new System.Windows.Forms.RadioButton();
            this.ValueY = new System.Windows.Forms.NumericUpDown();
            this.ValueZ = new System.Windows.Forms.NumericUpDown();
            this.ValueX = new System.Windows.Forms.NumericUpDown();
            this.SourceValueZButton = new System.Windows.Forms.Button();
            this.ZeroValueZButton = new System.Windows.Forms.Button();
            this.SourceValueYButton = new System.Windows.Forms.Button();
            this.ZeroValueYButton = new System.Windows.Forms.Button();
            this.SourceValueXButton = new System.Windows.Forms.Button();
            this.ZeroValueXButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ValueY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueX)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRotate
            // 
            this.lblRotate.AutoSize = true;
            this.lblRotate.Checked = true;
            this.lblRotate.Location = new System.Drawing.Point(12, 12);
            this.lblRotate.Name = "lblRotate";
            this.lblRotate.Size = new System.Drawing.Size(57, 17);
            this.lblRotate.TabIndex = 0;
            this.lblRotate.TabStop = true;
            this.lblRotate.Text = "Rotate";
            this.lblRotate.UseVisualStyleBackColor = true;
            this.lblRotate.Click += new System.EventHandler(this.TypeChanged);
            // 
            // lblTranslate
            // 
            this.lblTranslate.AutoSize = true;
            this.lblTranslate.Location = new System.Drawing.Point(12, 38);
            this.lblTranslate.Name = "lblTranslate";
            this.lblTranslate.Size = new System.Drawing.Size(69, 17);
            this.lblTranslate.TabIndex = 0;
            this.lblTranslate.Text = "Translate";
            this.lblTranslate.UseVisualStyleBackColor = true;
            this.lblTranslate.Click += new System.EventHandler(this.TypeChanged);
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(12, 64);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(52, 17);
            this.lblScale.TabIndex = 0;
            this.lblScale.Text = "Scale";
            this.lblScale.UseVisualStyleBackColor = true;
            this.lblScale.Click += new System.EventHandler(this.TypeChanged);
            // 
            // ValueY
            // 
            this.ValueY.DecimalPlaces = 2;
            this.ValueY.Location = new System.Drawing.Point(113, 37);
            this.ValueY.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.ValueY.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.ValueY.Name = "ValueY";
            this.ValueY.Size = new System.Drawing.Size(66, 20);
            this.ValueY.TabIndex = 24;
            // 
            // ValueZ
            // 
            this.ValueZ.DecimalPlaces = 2;
            this.ValueZ.Location = new System.Drawing.Point(113, 63);
            this.ValueZ.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.ValueZ.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.ValueZ.Name = "ValueZ";
            this.ValueZ.Size = new System.Drawing.Size(66, 20);
            this.ValueZ.TabIndex = 25;
            // 
            // ValueX
            // 
            this.ValueX.DecimalPlaces = 2;
            this.ValueX.Location = new System.Drawing.Point(113, 11);
            this.ValueX.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.ValueX.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.ValueX.Name = "ValueX";
            this.ValueX.Size = new System.Drawing.Size(66, 20);
            this.ValueX.TabIndex = 26;
            // 
            // SourceValueZButton
            // 
            this.SourceValueZButton.Location = new System.Drawing.Point(216, 63);
            this.SourceValueZButton.Name = "SourceValueZButton";
            this.SourceValueZButton.Size = new System.Drawing.Size(49, 20);
            this.SourceValueZButton.TabIndex = 18;
            this.SourceValueZButton.Text = "Source";
            this.SourceValueZButton.UseVisualStyleBackColor = true;
            // 
            // ZeroValueZButton
            // 
            this.ZeroValueZButton.Location = new System.Drawing.Point(185, 63);
            this.ZeroValueZButton.Name = "ZeroValueZButton";
            this.ZeroValueZButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroValueZButton.TabIndex = 19;
            this.ZeroValueZButton.Text = "0";
            this.ZeroValueZButton.UseVisualStyleBackColor = true;
            // 
            // SourceValueYButton
            // 
            this.SourceValueYButton.Location = new System.Drawing.Point(216, 37);
            this.SourceValueYButton.Name = "SourceValueYButton";
            this.SourceValueYButton.Size = new System.Drawing.Size(49, 20);
            this.SourceValueYButton.TabIndex = 20;
            this.SourceValueYButton.Text = "Source";
            this.SourceValueYButton.UseVisualStyleBackColor = true;
            // 
            // ZeroValueYButton
            // 
            this.ZeroValueYButton.Location = new System.Drawing.Point(185, 37);
            this.ZeroValueYButton.Name = "ZeroValueYButton";
            this.ZeroValueYButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroValueYButton.TabIndex = 21;
            this.ZeroValueYButton.Text = "0";
            this.ZeroValueYButton.UseVisualStyleBackColor = true;
            // 
            // SourceValueXButton
            // 
            this.SourceValueXButton.Location = new System.Drawing.Point(216, 11);
            this.SourceValueXButton.Name = "SourceValueXButton";
            this.SourceValueXButton.Size = new System.Drawing.Size(49, 20);
            this.SourceValueXButton.TabIndex = 22;
            this.SourceValueXButton.Text = "Source";
            this.SourceValueXButton.UseVisualStyleBackColor = true;
            // 
            // ZeroValueXButton
            // 
            this.ZeroValueXButton.Location = new System.Drawing.Point(185, 11);
            this.ZeroValueXButton.Name = "ZeroValueXButton";
            this.ZeroValueXButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroValueXButton.TabIndex = 23;
            this.ZeroValueXButton.Text = "0";
            this.ZeroValueXButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(96, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Z:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(96, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(96, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "X:";
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(135, 89);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 27;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(54, 89);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 28;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // TransformDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 120);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.ValueY);
            this.Controls.Add(this.ValueZ);
            this.Controls.Add(this.ValueX);
            this.Controls.Add(this.SourceValueZButton);
            this.Controls.Add(this.ZeroValueZButton);
            this.Controls.Add(this.SourceValueYButton);
            this.Controls.Add(this.ZeroValueYButton);
            this.Controls.Add(this.SourceValueXButton);
            this.Controls.Add(this.ZeroValueXButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.lblTranslate);
            this.Controls.Add(this.lblRotate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransformDialog";
            this.ShowInTaskbar = false;
            this.Text = "Transform";
            ((System.ComponentModel.ISupportInitialize)(this.ValueY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton lblRotate;
        private System.Windows.Forms.RadioButton lblTranslate;
        private System.Windows.Forms.RadioButton lblScale;
        private System.Windows.Forms.NumericUpDown ValueY;
        private System.Windows.Forms.NumericUpDown ValueZ;
        private System.Windows.Forms.NumericUpDown ValueX;
        private System.Windows.Forms.Button SourceValueZButton;
        private System.Windows.Forms.Button ZeroValueZButton;
        private System.Windows.Forms.Button SourceValueYButton;
        private System.Windows.Forms.Button ZeroValueYButton;
        private System.Windows.Forms.Button SourceValueXButton;
        private System.Windows.Forms.Button ZeroValueXButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OkButton;
    }
}