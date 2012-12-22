namespace Sledge.Editor.Tools.DisplacementTools
{
    partial class SelectControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ApplyAttributesButton = new System.Windows.Forms.Button();
            this.NoRayCollisionValue = new System.Windows.Forms.CheckBox();
            this.NoHullCollisionValue = new System.Windows.Forms.CheckBox();
            this.NoPhysicsCollisionValue = new System.Windows.Forms.CheckBox();
            this.ScaleValue = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.ElevationValue = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.PowerValue = new System.Windows.Forms.NumericUpDown();
            this.labelPower = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PowerValue)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ApplyAttributesButton);
            this.groupBox1.Controls.Add(this.NoRayCollisionValue);
            this.groupBox1.Controls.Add(this.NoHullCollisionValue);
            this.groupBox1.Controls.Add(this.NoPhysicsCollisionValue);
            this.groupBox1.Controls.Add(this.ScaleValue);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ElevationValue);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.PowerValue);
            this.groupBox1.Controls.Add(this.labelPower);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 201);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attributes";
            // 
            // ApplyAttributesButton
            // 
            this.ApplyAttributesButton.Location = new System.Drawing.Point(36, 167);
            this.ApplyAttributesButton.Name = "ApplyAttributesButton";
            this.ApplyAttributesButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyAttributesButton.TabIndex = 24;
            this.ApplyAttributesButton.Text = "Apply";
            this.ApplyAttributesButton.UseVisualStyleBackColor = true;
            // 
            // NoRayCollisionValue
            // 
            this.NoRayCollisionValue.AutoSize = true;
            this.NoRayCollisionValue.Location = new System.Drawing.Point(9, 144);
            this.NoRayCollisionValue.Name = "NoRayCollisionValue";
            this.NoRayCollisionValue.Size = new System.Drawing.Size(103, 17);
            this.NoRayCollisionValue.TabIndex = 23;
            this.NoRayCollisionValue.Text = "No Ray Collision";
            this.NoRayCollisionValue.UseVisualStyleBackColor = true;
            // 
            // NoHullCollisionValue
            // 
            this.NoHullCollisionValue.AutoSize = true;
            this.NoHullCollisionValue.Location = new System.Drawing.Point(9, 121);
            this.NoHullCollisionValue.Name = "NoHullCollisionValue";
            this.NoHullCollisionValue.Size = new System.Drawing.Size(102, 17);
            this.NoHullCollisionValue.TabIndex = 22;
            this.NoHullCollisionValue.Text = "No Hull Collision";
            this.NoHullCollisionValue.UseVisualStyleBackColor = true;
            // 
            // NoPhysicsCollisionValue
            // 
            this.NoPhysicsCollisionValue.AutoSize = true;
            this.NoPhysicsCollisionValue.Location = new System.Drawing.Point(9, 98);
            this.NoPhysicsCollisionValue.Name = "NoPhysicsCollisionValue";
            this.NoPhysicsCollisionValue.Size = new System.Drawing.Size(120, 17);
            this.NoPhysicsCollisionValue.TabIndex = 21;
            this.NoPhysicsCollisionValue.Text = "No Physics Collision";
            this.NoPhysicsCollisionValue.UseVisualStyleBackColor = true;
            // 
            // ScaleValue
            // 
            this.ScaleValue.BackColor = System.Drawing.SystemColors.Window;
            this.ScaleValue.DecimalPlaces = 4;
            this.ScaleValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleValue.Location = new System.Drawing.Point(71, 72);
            this.ScaleValue.Name = "ScaleValue";
            this.ScaleValue.Size = new System.Drawing.Size(57, 20);
            this.ScaleValue.TabIndex = 20;
            this.ScaleValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 25);
            this.label2.TabIndex = 19;
            this.label2.Text = "Scale";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ElevationValue
            // 
            this.ElevationValue.BackColor = System.Drawing.SystemColors.Window;
            this.ElevationValue.DecimalPlaces = 2;
            this.ElevationValue.Location = new System.Drawing.Point(71, 46);
            this.ElevationValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ElevationValue.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.ElevationValue.Name = "ElevationValue";
            this.ElevationValue.Size = new System.Drawing.Size(57, 20);
            this.ElevationValue.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 25);
            this.label1.TabIndex = 19;
            this.label1.Text = "Elevation";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PowerValue
            // 
            this.PowerValue.BackColor = System.Drawing.SystemColors.Window;
            this.PowerValue.Location = new System.Drawing.Point(71, 20);
            this.PowerValue.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.PowerValue.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.PowerValue.Name = "PowerValue";
            this.PowerValue.Size = new System.Drawing.Size(57, 20);
            this.PowerValue.TabIndex = 20;
            this.PowerValue.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // labelPower
            // 
            this.labelPower.Location = new System.Drawing.Point(6, 16);
            this.labelPower.Name = "labelPower";
            this.labelPower.Size = new System.Drawing.Size(59, 25);
            this.labelPower.TabIndex = 19;
            this.labelPower.Text = "Power";
            this.labelPower.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SelectForm";
            this.Size = new System.Drawing.Size(469, 431);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PowerValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ApplyAttributesButton;
        private System.Windows.Forms.CheckBox NoRayCollisionValue;
        private System.Windows.Forms.CheckBox NoHullCollisionValue;
        private System.Windows.Forms.CheckBox NoPhysicsCollisionValue;
        private System.Windows.Forms.NumericUpDown ScaleValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ElevationValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown PowerValue;
        private System.Windows.Forms.Label labelPower;

    }
}
