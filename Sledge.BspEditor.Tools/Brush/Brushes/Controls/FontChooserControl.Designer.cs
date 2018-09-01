namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    partial class FontChooserControl
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
            this.Label = new System.Windows.Forms.Label();
            this.FontPicker = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(1, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(28, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "Font";
            // 
            // FontPicker
            // 
            this.FontPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FontPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontPicker.FormattingEnabled = true;
            this.FontPicker.Location = new System.Drawing.Point(35, 3);
            this.FontPicker.Name = "FontPicker";
            this.FontPicker.Size = new System.Drawing.Size(118, 21);
            this.FontPicker.TabIndex = 4;
            this.FontPicker.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // FontChooserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FontPicker);
            this.Controls.Add(this.Label);
            this.Name = "FontChooserControl";
            this.Size = new System.Drawing.Size(156, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.ComboBox FontPicker;
    }
}
