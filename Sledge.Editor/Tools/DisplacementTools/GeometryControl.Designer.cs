namespace Sledge.Editor.Tools.DisplacementTools
{
    partial class GeometryControl
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ResetAllPointsButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SmoothPointsRadio = new System.Windows.Forms.RadioButton();
            this.AbsoluteDistanceRadio = new System.Windows.Forms.RadioButton();
            this.RelativeDistanceRadio = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SoftEdgeBrushModeCombo = new System.Windows.Forms.ComboBox();
            this.PointBrushSizeSlider = new System.Windows.Forms.TrackBar();
            this.SpatialRadiusSlider = new System.Windows.Forms.TrackBar();
            this.SoftEdgeCheckbox = new System.Windows.Forms.CheckBox();
            this.PointBrushSizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.SpatialBrushRadiusUpDown = new System.Windows.Forms.NumericUpDown();
            this.PointBrushRadio = new System.Windows.Forms.RadioButton();
            this.SpatialBrushRadio = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.AutoSewCheckbox = new System.Windows.Forms.CheckBox();
            this.DistanceSlider = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.DistanceUpDown = new System.Windows.Forms.NumericUpDown();
            this.AxisCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointBrushSizeSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpatialRadiusSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointBrushSizeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpatialBrushRadiusUpDown)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ResetAllPointsButton);
            this.groupBox2.Location = new System.Drawing.Point(208, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(115, 89);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Actions";
            // 
            // ResetAllPointsButton
            // 
            this.ResetAllPointsButton.Location = new System.Drawing.Point(6, 19);
            this.ResetAllPointsButton.Name = "ResetAllPointsButton";
            this.ResetAllPointsButton.Size = new System.Drawing.Size(100, 23);
            this.ResetAllPointsButton.TabIndex = 0;
            this.ResetAllPointsButton.Text = "Reset All Points";
            this.ResetAllPointsButton.UseVisualStyleBackColor = true;
            this.ResetAllPointsButton.Click += new System.EventHandler(this.ResetAllPointsButtonClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SmoothPointsRadio);
            this.groupBox1.Controls.Add(this.AbsoluteDistanceRadio);
            this.groupBox1.Controls.Add(this.RelativeDistanceRadio);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 89);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Effect";
            // 
            // SmoothPointsRadio
            // 
            this.SmoothPointsRadio.AutoSize = true;
            this.SmoothPointsRadio.Location = new System.Drawing.Point(6, 65);
            this.SmoothPointsRadio.Name = "SmoothPointsRadio";
            this.SmoothPointsRadio.Size = new System.Drawing.Size(92, 17);
            this.SmoothPointsRadio.TabIndex = 2;
            this.SmoothPointsRadio.Text = "Smooth points";
            this.SmoothPointsRadio.UseVisualStyleBackColor = true;
            // 
            // AbsoluteDistanceRadio
            // 
            this.AbsoluteDistanceRadio.AutoSize = true;
            this.AbsoluteDistanceRadio.Location = new System.Drawing.Point(6, 42);
            this.AbsoluteDistanceRadio.Name = "AbsoluteDistanceRadio";
            this.AbsoluteDistanceRadio.Size = new System.Drawing.Size(109, 17);
            this.AbsoluteDistanceRadio.TabIndex = 1;
            this.AbsoluteDistanceRadio.Text = "Absolute distance";
            this.AbsoluteDistanceRadio.UseVisualStyleBackColor = true;
            // 
            // RelativeDistanceRadio
            // 
            this.RelativeDistanceRadio.AutoSize = true;
            this.RelativeDistanceRadio.Checked = true;
            this.RelativeDistanceRadio.Location = new System.Drawing.Point(6, 19);
            this.RelativeDistanceRadio.Name = "RelativeDistanceRadio";
            this.RelativeDistanceRadio.Size = new System.Drawing.Size(107, 17);
            this.RelativeDistanceRadio.TabIndex = 0;
            this.RelativeDistanceRadio.TabStop = true;
            this.RelativeDistanceRadio.Text = "Relative distance";
            this.RelativeDistanceRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SoftEdgeBrushModeCombo);
            this.groupBox3.Controls.Add(this.PointBrushSizeSlider);
            this.groupBox3.Controls.Add(this.SpatialRadiusSlider);
            this.groupBox3.Controls.Add(this.SoftEdgeCheckbox);
            this.groupBox3.Controls.Add(this.PointBrushSizeUpDown);
            this.groupBox3.Controls.Add(this.SpatialBrushRadiusUpDown);
            this.groupBox3.Controls.Add(this.PointBrushRadio);
            this.groupBox3.Controls.Add(this.SpatialBrushRadio);
            this.groupBox3.Location = new System.Drawing.Point(3, 98);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 107);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Brush";
            // 
            // SoftEdgeBrushModeCombo
            // 
            this.SoftEdgeBrushModeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SoftEdgeBrushModeCombo.FormattingEnabled = true;
            this.SoftEdgeBrushModeCombo.Location = new System.Drawing.Point(152, 77);
            this.SoftEdgeBrushModeCombo.Name = "SoftEdgeBrushModeCombo";
            this.SoftEdgeBrushModeCombo.Size = new System.Drawing.Size(133, 21);
            this.SoftEdgeBrushModeCombo.TabIndex = 6;
            // 
            // PointBrushSizeSlider
            // 
            this.PointBrushSizeSlider.AutoSize = false;
            this.PointBrushSizeSlider.Location = new System.Drawing.Point(114, 43);
            this.PointBrushSizeSlider.Maximum = 16;
            this.PointBrushSizeSlider.Minimum = 1;
            this.PointBrushSizeSlider.Name = "PointBrushSizeSlider";
            this.PointBrushSizeSlider.Size = new System.Drawing.Size(130, 26);
            this.PointBrushSizeSlider.TabIndex = 5;
            this.PointBrushSizeSlider.Value = 3;
            this.PointBrushSizeSlider.Scroll += new System.EventHandler(this.PointBrushSizeSliderScroll);
            // 
            // SpatialRadiusSlider
            // 
            this.SpatialRadiusSlider.AutoSize = false;
            this.SpatialRadiusSlider.LargeChange = 500;
            this.SpatialRadiusSlider.Location = new System.Drawing.Point(114, 12);
            this.SpatialRadiusSlider.Maximum = 100000;
            this.SpatialRadiusSlider.Minimum = 1;
            this.SpatialRadiusSlider.Name = "SpatialRadiusSlider";
            this.SpatialRadiusSlider.Size = new System.Drawing.Size(130, 28);
            this.SpatialRadiusSlider.SmallChange = 100;
            this.SpatialRadiusSlider.TabIndex = 5;
            this.SpatialRadiusSlider.TickFrequency = 5000;
            this.SpatialRadiusSlider.Value = 50000;
            this.SpatialRadiusSlider.Scroll += new System.EventHandler(this.SpatialRadiusSliderScroll);
            // 
            // SoftEdgeCheckbox
            // 
            this.SoftEdgeCheckbox.AutoSize = true;
            this.SoftEdgeCheckbox.Checked = true;
            this.SoftEdgeCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SoftEdgeCheckbox.Location = new System.Drawing.Point(6, 79);
            this.SoftEdgeCheckbox.Name = "SoftEdgeCheckbox";
            this.SoftEdgeCheckbox.Size = new System.Drawing.Size(140, 17);
            this.SoftEdgeCheckbox.TabIndex = 2;
            this.SoftEdgeCheckbox.Text = "Soft edge brush - Mode:";
            this.SoftEdgeCheckbox.UseVisualStyleBackColor = true;
            // 
            // PointBrushSizeUpDown
            // 
            this.PointBrushSizeUpDown.Location = new System.Drawing.Point(250, 42);
            this.PointBrushSizeUpDown.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.PointBrushSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PointBrushSizeUpDown.Name = "PointBrushSizeUpDown";
            this.PointBrushSizeUpDown.Size = new System.Drawing.Size(64, 20);
            this.PointBrushSizeUpDown.TabIndex = 1;
            this.PointBrushSizeUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.PointBrushSizeUpDown.ValueChanged += new System.EventHandler(this.PointBrushSizeUpDownValueChanged);
            // 
            // SpatialBrushRadiusUpDown
            // 
            this.SpatialBrushRadiusUpDown.DecimalPlaces = 2;
            this.SpatialBrushRadiusUpDown.Location = new System.Drawing.Point(250, 19);
            this.SpatialBrushRadiusUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.SpatialBrushRadiusUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.SpatialBrushRadiusUpDown.Name = "SpatialBrushRadiusUpDown";
            this.SpatialBrushRadiusUpDown.Size = new System.Drawing.Size(64, 20);
            this.SpatialBrushRadiusUpDown.TabIndex = 1;
            this.SpatialBrushRadiusUpDown.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.SpatialBrushRadiusUpDown.ValueChanged += new System.EventHandler(this.SpatialBrushRadiusUpDownValueChanged);
            // 
            // PointBrushRadio
            // 
            this.PointBrushRadio.AutoSize = true;
            this.PointBrushRadio.Location = new System.Drawing.Point(6, 42);
            this.PointBrushRadio.Name = "PointBrushRadio";
            this.PointBrushRadio.Size = new System.Drawing.Size(81, 17);
            this.PointBrushRadio.TabIndex = 0;
            this.PointBrushRadio.Text = "Point - Size:";
            this.PointBrushRadio.UseVisualStyleBackColor = true;
            // 
            // SpatialBrushRadio
            // 
            this.SpatialBrushRadio.AutoSize = true;
            this.SpatialBrushRadio.Checked = true;
            this.SpatialBrushRadio.Location = new System.Drawing.Point(6, 19);
            this.SpatialBrushRadio.Name = "SpatialBrushRadio";
            this.SpatialBrushRadio.Size = new System.Drawing.Size(102, 17);
            this.SpatialBrushRadio.TabIndex = 0;
            this.SpatialBrushRadio.TabStop = true;
            this.SpatialBrushRadio.Text = "Spatial - Radius:";
            this.SpatialBrushRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.AutoSewCheckbox);
            this.groupBox4.Controls.Add(this.DistanceSlider);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.DistanceUpDown);
            this.groupBox4.Controls.Add(this.AxisCombo);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(3, 211);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(320, 84);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Edit";
            // 
            // AutoSewCheckbox
            // 
            this.AutoSewCheckbox.AutoSize = true;
            this.AutoSewCheckbox.Location = new System.Drawing.Point(198, 21);
            this.AutoSewCheckbox.Name = "AutoSewCheckbox";
            this.AutoSewCheckbox.Size = new System.Drawing.Size(112, 17);
            this.AutoSewCheckbox.TabIndex = 5;
            this.AutoSewCheckbox.Text = "Automatically Sew";
            this.AutoSewCheckbox.UseVisualStyleBackColor = true;
            // 
            // DistanceSlider
            // 
            this.DistanceSlider.AutoSize = false;
            this.DistanceSlider.LargeChange = 500;
            this.DistanceSlider.Location = new System.Drawing.Point(61, 46);
            this.DistanceSlider.Maximum = 10000;
            this.DistanceSlider.Minimum = 1;
            this.DistanceSlider.Name = "DistanceSlider";
            this.DistanceSlider.Size = new System.Drawing.Size(193, 29);
            this.DistanceSlider.SmallChange = 100;
            this.DistanceSlider.TabIndex = 4;
            this.DistanceSlider.TickFrequency = 500;
            this.DistanceSlider.Value = 500;
            this.DistanceSlider.Scroll += new System.EventHandler(this.DistanceSliderScroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Distance";
            // 
            // DistanceUpDown
            // 
            this.DistanceUpDown.DecimalPlaces = 2;
            this.DistanceUpDown.Location = new System.Drawing.Point(260, 48);
            this.DistanceUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.DistanceUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DistanceUpDown.Name = "DistanceUpDown";
            this.DistanceUpDown.Size = new System.Drawing.Size(54, 20);
            this.DistanceUpDown.TabIndex = 2;
            this.DistanceUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.DistanceUpDown.ValueChanged += new System.EventHandler(this.DistanceUpDownValueChanged);
            // 
            // AxisCombo
            // 
            this.AxisCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AxisCombo.FormattingEnabled = true;
            this.AxisCombo.Location = new System.Drawing.Point(42, 19);
            this.AxisCombo.Name = "AxisCombo";
            this.AxisCombo.Size = new System.Drawing.Size(133, 21);
            this.AxisCombo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Axis";
            // 
            // GeometryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "GeometryControl";
            this.Size = new System.Drawing.Size(330, 302);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointBrushSizeSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpatialRadiusSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointBrushSizeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpatialBrushRadiusUpDown)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DistanceUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ResetAllPointsButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton SmoothPointsRadio;
        private System.Windows.Forms.RadioButton AbsoluteDistanceRadio;
        private System.Windows.Forms.RadioButton RelativeDistanceRadio;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown PointBrushSizeUpDown;
        private System.Windows.Forms.NumericUpDown SpatialBrushRadiusUpDown;
        private System.Windows.Forms.RadioButton PointBrushRadio;
        private System.Windows.Forms.RadioButton SpatialBrushRadio;
        private System.Windows.Forms.CheckBox SoftEdgeCheckbox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox AxisCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown DistanceUpDown;
        private System.Windows.Forms.TrackBar DistanceSlider;
        private System.Windows.Forms.CheckBox AutoSewCheckbox;
        private System.Windows.Forms.TrackBar PointBrushSizeSlider;
        private System.Windows.Forms.TrackBar SpatialRadiusSlider;
        private System.Windows.Forms.ComboBox SoftEdgeBrushModeCombo;
    }
}
