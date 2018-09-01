namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    partial class BooleanControl
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
            this.Checkbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Checkbox
            // 
            this.Checkbox.AutoSize = true;
            this.Checkbox.Location = new System.Drawing.Point(3, 3);
            this.Checkbox.Name = "Checkbox";
            this.Checkbox.Size = new System.Drawing.Size(73, 17);
            this.Checkbox.TabIndex = 0;
            this.Checkbox.Text = "LabelText";
            this.Checkbox.UseVisualStyleBackColor = true;
            this.Checkbox.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // NumericControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Checkbox);
            this.Name = "NumericControl";
            this.Size = new System.Drawing.Size(124, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Checkbox;

    }
}
