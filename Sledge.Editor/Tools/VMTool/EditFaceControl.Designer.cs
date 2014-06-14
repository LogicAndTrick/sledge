namespace Sledge.Editor.Tools.VMTool
{
    partial class EditFaceControl
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
            this.BevelButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.BevelValue = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PokeFaceButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PokeFaceCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.BevelValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PokeFaceCount)).BeginInit();
            this.SuspendLayout();
            // 
            // BevelButton
            // 
            this.BevelButton.Location = new System.Drawing.Point(147, 47);
            this.BevelButton.Name = "BevelButton";
            this.BevelButton.Size = new System.Drawing.Size(50, 23);
            this.BevelButton.TabIndex = 9;
            this.BevelButton.Text = "Bevel";
            this.BevelButton.UseVisualStyleBackColor = true;
            this.BevelButton.Click += new System.EventHandler(this.BevelButtonClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "With selected faces:";
            // 
            // BevelValue
            // 
            this.BevelValue.Location = new System.Drawing.Point(55, 50);
            this.BevelValue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.BevelValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BevelValue.Name = "BevelValue";
            this.BevelValue.Size = new System.Drawing.Size(58, 20);
            this.BevelValue.TabIndex = 11;
            this.BevelValue.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(115, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "units";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Bevel by";
            // 
            // PokeFaceButton
            // 
            this.PokeFaceButton.Location = new System.Drawing.Point(147, 21);
            this.PokeFaceButton.Name = "PokeFaceButton";
            this.PokeFaceButton.Size = new System.Drawing.Size(50, 23);
            this.PokeFaceButton.TabIndex = 9;
            this.PokeFaceButton.Text = "Poke";
            this.PokeFaceButton.UseVisualStyleBackColor = true;
            this.PokeFaceButton.Click += new System.EventHandler(this.PokeFaceButtonClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Poke by";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "units";
            // 
            // PokeFaceCount
            // 
            this.PokeFaceCount.Location = new System.Drawing.Point(55, 24);
            this.PokeFaceCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PokeFaceCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.PokeFaceCount.Name = "PokeFaceCount";
            this.PokeFaceCount.Size = new System.Drawing.Size(58, 20);
            this.PokeFaceCount.TabIndex = 11;
            this.PokeFaceCount.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // EditFaceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PokeFaceButton);
            this.Controls.Add(this.BevelButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PokeFaceCount);
            this.Controls.Add(this.BevelValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Name = "EditFaceControl";
            this.Size = new System.Drawing.Size(200, 78);
            ((System.ComponentModel.ISupportInitialize)(this.BevelValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PokeFaceCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BevelButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown BevelValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button PokeFaceButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown PokeFaceCount;
    }
}
