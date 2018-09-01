namespace Sledge.Shell.Settings.Editors
{
    partial class LanguageSelectionEditor
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
            this.Combobox = new System.Windows.Forms.ComboBox();
            this.Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Combobox
            // 
            this.Combobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Combobox.Location = new System.Drawing.Point(86, 3);
            this.Combobox.Name = "Combobox";
            this.Combobox.Size = new System.Drawing.Size(261, 21);
            this.Combobox.TabIndex = 3;
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(3, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(35, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "label1";
            // 
            // EnumEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Combobox);
            this.Controls.Add(this.Label);
            this.Name = "EnumEditor";
            this.Size = new System.Drawing.Size(350, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox Combobox;
        private System.Windows.Forms.Label Label;
    }
}
