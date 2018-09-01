namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    partial class VertexEditFaceControl
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
            this.WithSelectedFacesLabel = new System.Windows.Forms.Label();
            this.BevelValue = new System.Windows.Forms.NumericUpDown();
            this.UnitsLabel2 = new System.Windows.Forms.Label();
            this.BevelByLabel = new System.Windows.Forms.Label();
            this.PokeFaceButton = new System.Windows.Forms.Button();
            this.PokeByLabel = new System.Windows.Forms.Label();
            this.UnitsLabel1 = new System.Windows.Forms.Label();
            this.PokeFaceCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.BevelValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PokeFaceCount)).BeginInit();
            this.SuspendLayout();
            // 
            // BevelButton
            // 
            this.BevelButton.Location = new System.Drawing.Point(144, 47);
            this.BevelButton.Name = "BevelButton";
            this.BevelButton.Size = new System.Drawing.Size(50, 23);
            this.BevelButton.TabIndex = 9;
            this.BevelButton.Text = "Bevel";
            this.BevelButton.UseVisualStyleBackColor = true;
            this.BevelButton.Click += new System.EventHandler(this.BevelButtonClicked);
            // 
            // WithSelectedFacesLabel
            // 
            this.WithSelectedFacesLabel.AutoSize = true;
            this.WithSelectedFacesLabel.Location = new System.Drawing.Point(2, 4);
            this.WithSelectedFacesLabel.Name = "WithSelectedFacesLabel";
            this.WithSelectedFacesLabel.Size = new System.Drawing.Size(104, 13);
            this.WithSelectedFacesLabel.TabIndex = 3;
            this.WithSelectedFacesLabel.Text = "With selected faces:";
            // 
            // BevelValue
            // 
            this.BevelValue.Location = new System.Drawing.Point(52, 50);
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
            // UnitsLabel2
            // 
            this.UnitsLabel2.AutoSize = true;
            this.UnitsLabel2.Location = new System.Drawing.Point(112, 52);
            this.UnitsLabel2.Name = "UnitsLabel2";
            this.UnitsLabel2.Size = new System.Drawing.Size(29, 13);
            this.UnitsLabel2.TabIndex = 6;
            this.UnitsLabel2.Text = "units";
            // 
            // BevelByLabel
            // 
            this.BevelByLabel.AutoSize = true;
            this.BevelByLabel.Location = new System.Drawing.Point(3, 52);
            this.BevelByLabel.Name = "BevelByLabel";
            this.BevelByLabel.Size = new System.Drawing.Size(48, 13);
            this.BevelByLabel.TabIndex = 7;
            this.BevelByLabel.Text = "Bevel by";
            // 
            // PokeFaceButton
            // 
            this.PokeFaceButton.Location = new System.Drawing.Point(144, 21);
            this.PokeFaceButton.Name = "PokeFaceButton";
            this.PokeFaceButton.Size = new System.Drawing.Size(50, 23);
            this.PokeFaceButton.TabIndex = 9;
            this.PokeFaceButton.Text = "Poke";
            this.PokeFaceButton.UseVisualStyleBackColor = true;
            this.PokeFaceButton.Click += new System.EventHandler(this.PokeFaceButtonClicked);
            // 
            // PokeByLabel
            // 
            this.PokeByLabel.AutoSize = true;
            this.PokeByLabel.Location = new System.Drawing.Point(3, 26);
            this.PokeByLabel.Name = "PokeByLabel";
            this.PokeByLabel.Size = new System.Drawing.Size(46, 13);
            this.PokeByLabel.TabIndex = 7;
            this.PokeByLabel.Text = "Poke by";
            // 
            // UnitsLabel1
            // 
            this.UnitsLabel1.AutoSize = true;
            this.UnitsLabel1.Location = new System.Drawing.Point(112, 26);
            this.UnitsLabel1.Name = "UnitsLabel1";
            this.UnitsLabel1.Size = new System.Drawing.Size(29, 13);
            this.UnitsLabel1.TabIndex = 6;
            this.UnitsLabel1.Text = "units";
            // 
            // PokeFaceCount
            // 
            this.PokeFaceCount.Location = new System.Drawing.Point(52, 24);
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
            // VertexEditFaceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PokeFaceButton);
            this.Controls.Add(this.BevelButton);
            this.Controls.Add(this.WithSelectedFacesLabel);
            this.Controls.Add(this.PokeFaceCount);
            this.Controls.Add(this.BevelValue);
            this.Controls.Add(this.UnitsLabel1);
            this.Controls.Add(this.PokeByLabel);
            this.Controls.Add(this.UnitsLabel2);
            this.Controls.Add(this.BevelByLabel);
            this.Name = "VertexEditFaceControl";
            this.Size = new System.Drawing.Size(199, 78);
            ((System.ComponentModel.ISupportInitialize)(this.BevelValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PokeFaceCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BevelButton;
        private System.Windows.Forms.Label WithSelectedFacesLabel;
        private System.Windows.Forms.NumericUpDown BevelValue;
        private System.Windows.Forms.Label UnitsLabel2;
        private System.Windows.Forms.Label BevelByLabel;
        private System.Windows.Forms.Button PokeFaceButton;
        private System.Windows.Forms.Label PokeByLabel;
        private System.Windows.Forms.Label UnitsLabel1;
        private System.Windows.Forms.NumericUpDown PokeFaceCount;
    }
}
