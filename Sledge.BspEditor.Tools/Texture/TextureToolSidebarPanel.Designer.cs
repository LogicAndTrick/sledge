namespace Sledge.BspEditor.Tools.Texture
{
    partial class TextureToolSidebarPanel
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
            this.RandomShiftMin = new System.Windows.Forms.NumericUpDown();
            this.MinLabel = new System.Windows.Forms.Label();
            this.RandomShiftMax = new System.Windows.Forms.NumericUpDown();
            this.MaxLabel = new System.Windows.Forms.Label();
            this.RandomShiftXButton = new System.Windows.Forms.Button();
            this.RandomShiftYButton = new System.Windows.Forms.Button();
            this.RandomiseShiftValuesGroup = new System.Windows.Forms.GroupBox();
            this.FitGroup = new System.Windows.Forms.GroupBox();
            this.TimesToTileLabel = new System.Windows.Forms.Label();
            this.TileFitX = new System.Windows.Forms.NumericUpDown();
            this.TileFitButton = new System.Windows.Forms.Button();
            this.TileFitY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMax)).BeginInit();
            this.RandomiseShiftValuesGroup.SuspendLayout();
            this.FitGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitY)).BeginInit();
            this.SuspendLayout();
            // 
            // RandomShiftMin
            // 
            this.RandomShiftMin.Location = new System.Drawing.Point(41, 19);
            this.RandomShiftMin.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.RandomShiftMin.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.RandomShiftMin.Name = "RandomShiftMin";
            this.RandomShiftMin.Size = new System.Drawing.Size(46, 20);
            this.RandomShiftMin.TabIndex = 1;
            // 
            // MinLabel
            // 
            this.MinLabel.AutoSize = true;
            this.MinLabel.Location = new System.Drawing.Point(8, 23);
            this.MinLabel.Name = "MinLabel";
            this.MinLabel.Size = new System.Drawing.Size(27, 13);
            this.MinLabel.TabIndex = 2;
            this.MinLabel.Text = "Min:";
            // 
            // RandomShiftMax
            // 
            this.RandomShiftMax.Location = new System.Drawing.Point(41, 45);
            this.RandomShiftMax.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.RandomShiftMax.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.RandomShiftMax.Name = "RandomShiftMax";
            this.RandomShiftMax.Size = new System.Drawing.Size(46, 20);
            this.RandomShiftMax.TabIndex = 1;
            this.RandomShiftMax.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // MaxLabel
            // 
            this.MaxLabel.AutoSize = true;
            this.MaxLabel.Location = new System.Drawing.Point(8, 49);
            this.MaxLabel.Name = "MaxLabel";
            this.MaxLabel.Size = new System.Drawing.Size(30, 13);
            this.MaxLabel.TabIndex = 2;
            this.MaxLabel.Text = "Max:";
            // 
            // RandomShiftXButton
            // 
            this.RandomShiftXButton.Location = new System.Drawing.Point(93, 16);
            this.RandomShiftXButton.Name = "RandomShiftXButton";
            this.RandomShiftXButton.Size = new System.Drawing.Size(86, 23);
            this.RandomShiftXButton.TabIndex = 3;
            this.RandomShiftXButton.Text = "Randomise X";
            this.RandomShiftXButton.UseVisualStyleBackColor = true;
            this.RandomShiftXButton.Click += new System.EventHandler(this.RandomShiftXButtonClicked);
            // 
            // RandomShiftYButton
            // 
            this.RandomShiftYButton.Location = new System.Drawing.Point(93, 42);
            this.RandomShiftYButton.Name = "RandomShiftYButton";
            this.RandomShiftYButton.Size = new System.Drawing.Size(86, 23);
            this.RandomShiftYButton.TabIndex = 3;
            this.RandomShiftYButton.Text = "Randomise Y";
            this.RandomShiftYButton.UseVisualStyleBackColor = true;
            this.RandomShiftYButton.Click += new System.EventHandler(this.RandomShiftYButtonClicked);
            // 
            // RandomiseShiftValuesGroup
            // 
            this.RandomiseShiftValuesGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RandomiseShiftValuesGroup.Controls.Add(this.RandomShiftMin);
            this.RandomiseShiftValuesGroup.Controls.Add(this.RandomShiftYButton);
            this.RandomiseShiftValuesGroup.Controls.Add(this.RandomShiftMax);
            this.RandomiseShiftValuesGroup.Controls.Add(this.RandomShiftXButton);
            this.RandomiseShiftValuesGroup.Controls.Add(this.MinLabel);
            this.RandomiseShiftValuesGroup.Controls.Add(this.MaxLabel);
            this.RandomiseShiftValuesGroup.Location = new System.Drawing.Point(8, 8);
            this.RandomiseShiftValuesGroup.Name = "RandomiseShiftValuesGroup";
            this.RandomiseShiftValuesGroup.Size = new System.Drawing.Size(205, 73);
            this.RandomiseShiftValuesGroup.TabIndex = 4;
            this.RandomiseShiftValuesGroup.TabStop = false;
            this.RandomiseShiftValuesGroup.Text = "Randomise shift values";
            // 
            // FitGroup
            // 
            this.FitGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FitGroup.Controls.Add(this.TimesToTileLabel);
            this.FitGroup.Controls.Add(this.TileFitX);
            this.FitGroup.Controls.Add(this.TileFitButton);
            this.FitGroup.Controls.Add(this.TileFitY);
            this.FitGroup.Controls.Add(this.label1);
            this.FitGroup.Controls.Add(this.label4);
            this.FitGroup.Location = new System.Drawing.Point(8, 87);
            this.FitGroup.Name = "FitGroup";
            this.FitGroup.Size = new System.Drawing.Size(205, 108);
            this.FitGroup.TabIndex = 5;
            this.FitGroup.TabStop = false;
            this.FitGroup.Text = "Fit to multiple tiles";
            // 
            // TimesToTileLabel
            // 
            this.TimesToTileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TimesToTileLabel.Location = new System.Drawing.Point(8, 21);
            this.TimesToTileLabel.Name = "TimesToTileLabel";
            this.TimesToTileLabel.Size = new System.Drawing.Size(191, 27);
            this.TimesToTileLabel.TabIndex = 4;
            this.TimesToTileLabel.Text = "Times to tile on face:";
            // 
            // TileFitX
            // 
            this.TileFitX.Location = new System.Drawing.Point(41, 51);
            this.TileFitX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TileFitX.Name = "TileFitX";
            this.TileFitX.Size = new System.Drawing.Size(46, 20);
            this.TileFitX.TabIndex = 1;
            this.TileFitX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TileFitButton
            // 
            this.TileFitButton.Location = new System.Drawing.Point(98, 60);
            this.TileFitButton.Name = "TileFitButton";
            this.TileFitButton.Size = new System.Drawing.Size(71, 23);
            this.TileFitButton.TabIndex = 3;
            this.TileFitButton.Text = "Fit";
            this.TileFitButton.UseVisualStyleBackColor = true;
            this.TileFitButton.Click += new System.EventHandler(this.TileFitButtonClicked);
            // 
            // TileFitY
            // 
            this.TileFitY.Location = new System.Drawing.Point(41, 77);
            this.TileFitY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TileFitY.Name = "TileFitY";
            this.TileFitY.Size = new System.Drawing.Size(46, 20);
            this.TileFitY.TabIndex = 1;
            this.TileFitY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Y:";
            // 
            // TextureToolSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.FitGroup);
            this.Controls.Add(this.RandomiseShiftValuesGroup);
            this.Name = "TextureToolSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(220, 203);
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMax)).EndInit();
            this.RandomiseShiftValuesGroup.ResumeLayout(false);
            this.RandomiseShiftValuesGroup.PerformLayout();
            this.FitGroup.ResumeLayout(false);
            this.FitGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown RandomShiftMin;
        private System.Windows.Forms.Label MinLabel;
        private System.Windows.Forms.NumericUpDown RandomShiftMax;
        private System.Windows.Forms.Label MaxLabel;
        private System.Windows.Forms.Button RandomShiftXButton;
        private System.Windows.Forms.Button RandomShiftYButton;
        private System.Windows.Forms.GroupBox RandomiseShiftValuesGroup;
        private System.Windows.Forms.GroupBox FitGroup;
        private System.Windows.Forms.NumericUpDown TileFitX;
        private System.Windows.Forms.Button TileFitButton;
        private System.Windows.Forms.NumericUpDown TileFitY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TimesToTileLabel;


    }
}
