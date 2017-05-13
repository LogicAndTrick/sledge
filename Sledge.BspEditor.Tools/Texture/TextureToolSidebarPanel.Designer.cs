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
            this.label2 = new System.Windows.Forms.Label();
            this.RandomShiftMax = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.RandomShiftXButton = new System.Windows.Forms.Button();
            this.RandomShiftYButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TileFitX = new System.Windows.Forms.NumericUpDown();
            this.TileFitButton = new System.Windows.Forms.Button();
            this.TileFitY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMax)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min:";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max:";
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.RandomShiftMin);
            this.groupBox1.Controls.Add(this.RandomShiftYButton);
            this.groupBox1.Controls.Add(this.RandomShiftMax);
            this.groupBox1.Controls.Add(this.RandomShiftXButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 73);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Randomise shift values";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.TileFitX);
            this.groupBox2.Controls.Add(this.TileFitButton);
            this.groupBox2.Controls.Add(this.TileFitY);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(8, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(205, 99);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fit to multiple tiles";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Times to tile on face:";
            // 
            // TileFitX
            // 
            this.TileFitX.Location = new System.Drawing.Point(41, 41);
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
            this.TileFitButton.Location = new System.Drawing.Point(98, 50);
            this.TileFitButton.Name = "TileFitButton";
            this.TileFitButton.Size = new System.Drawing.Size(34, 23);
            this.TileFitButton.TabIndex = 3;
            this.TileFitButton.Text = "Fit";
            this.TileFitButton.UseVisualStyleBackColor = true;
            this.TileFitButton.Click += new System.EventHandler(this.TileFitButtonClicked);
            // 
            // TileFitY
            // 
            this.TileFitY.Location = new System.Drawing.Point(41, 67);
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
            this.label1.Location = new System.Drawing.Point(21, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 69);
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
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "TextureToolSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(220, 196);
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomShiftMax)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TileFitY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown RandomShiftMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown RandomShiftMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button RandomShiftXButton;
        private System.Windows.Forms.Button RandomShiftYButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown TileFitX;
        private System.Windows.Forms.Button TileFitButton;
        private System.Windows.Forms.NumericUpDown TileFitY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;


    }
}
