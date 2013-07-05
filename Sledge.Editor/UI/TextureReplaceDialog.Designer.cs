namespace Sledge.Editor.UI
{
    partial class TextureReplaceDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Find = new System.Windows.Forms.TextBox();
            this.FindInfo = new System.Windows.Forms.Label();
            this.FindImage = new System.Windows.Forms.PictureBox();
            this.FindBrowse = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Replace = new System.Windows.Forms.TextBox();
            this.ReplaceInfo = new System.Windows.Forms.Label();
            this.ReplaceImage = new System.Windows.Forms.PictureBox();
            this.ReplaceBrowse = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ReplaceEverything = new System.Windows.Forms.RadioButton();
            this.ReplaceVisible = new System.Windows.Forms.RadioButton();
            this.ReplaceSelection = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ActionSubstitute = new System.Windows.Forms.RadioButton();
            this.ActionPartial = new System.Windows.Forms.RadioButton();
            this.ActionExact = new System.Windows.Forms.RadioButton();
            this.RescaleTextures = new System.Windows.Forms.CheckBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.ActionSelect = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindImage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceImage)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Find);
            this.groupBox1.Controls.Add(this.FindInfo);
            this.groupBox1.Controls.Add(this.FindImage);
            this.groupBox1.Controls.Add(this.FindBrowse);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 152);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find:";
            // 
            // Find
            // 
            this.Find.Location = new System.Drawing.Point(6, 20);
            this.Find.Name = "Find";
            this.Find.Size = new System.Drawing.Size(181, 20);
            this.Find.TabIndex = 4;
            // 
            // FindInfo
            // 
            this.FindInfo.AutoSize = true;
            this.FindInfo.Location = new System.Drawing.Point(113, 76);
            this.FindInfo.Name = "FindInfo";
            this.FindInfo.Size = new System.Drawing.Size(63, 13);
            this.FindInfo.TabIndex = 3;
            this.FindInfo.Text = "Texture info";
            // 
            // FindImage
            // 
            this.FindImage.Location = new System.Drawing.Point(6, 46);
            this.FindImage.Name = "FindImage";
            this.FindImage.Size = new System.Drawing.Size(100, 100);
            this.FindImage.TabIndex = 2;
            this.FindImage.TabStop = false;
            // 
            // FindBrowse
            // 
            this.FindBrowse.Location = new System.Drawing.Point(112, 46);
            this.FindBrowse.Name = "FindBrowse";
            this.FindBrowse.Size = new System.Drawing.Size(75, 23);
            this.FindBrowse.TabIndex = 1;
            this.FindBrowse.Text = "Browse...";
            this.FindBrowse.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Replace);
            this.groupBox2.Controls.Add(this.ReplaceInfo);
            this.groupBox2.Controls.Add(this.ReplaceImage);
            this.groupBox2.Controls.Add(this.ReplaceBrowse);
            this.groupBox2.Location = new System.Drawing.Point(211, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(193, 152);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Replace:";
            // 
            // Replace
            // 
            this.Replace.Location = new System.Drawing.Point(6, 20);
            this.Replace.Name = "Replace";
            this.Replace.Size = new System.Drawing.Size(181, 20);
            this.Replace.TabIndex = 4;
            // 
            // ReplaceInfo
            // 
            this.ReplaceInfo.AutoSize = true;
            this.ReplaceInfo.Location = new System.Drawing.Point(113, 76);
            this.ReplaceInfo.Name = "ReplaceInfo";
            this.ReplaceInfo.Size = new System.Drawing.Size(63, 13);
            this.ReplaceInfo.TabIndex = 3;
            this.ReplaceInfo.Text = "Texture info";
            // 
            // ReplaceImage
            // 
            this.ReplaceImage.Location = new System.Drawing.Point(6, 46);
            this.ReplaceImage.Name = "ReplaceImage";
            this.ReplaceImage.Size = new System.Drawing.Size(100, 100);
            this.ReplaceImage.TabIndex = 2;
            this.ReplaceImage.TabStop = false;
            // 
            // ReplaceBrowse
            // 
            this.ReplaceBrowse.Location = new System.Drawing.Point(112, 46);
            this.ReplaceBrowse.Name = "ReplaceBrowse";
            this.ReplaceBrowse.Size = new System.Drawing.Size(75, 23);
            this.ReplaceBrowse.TabIndex = 1;
            this.ReplaceBrowse.Text = "Browse...";
            this.ReplaceBrowse.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ReplaceEverything);
            this.groupBox3.Controls.Add(this.ReplaceVisible);
            this.groupBox3.Controls.Add(this.ReplaceSelection);
            this.groupBox3.Location = new System.Drawing.Point(12, 170);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(193, 87);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Replace In:";
            // 
            // ReplaceEverything
            // 
            this.ReplaceEverything.AutoSize = true;
            this.ReplaceEverything.Location = new System.Drawing.Point(9, 65);
            this.ReplaceEverything.Name = "ReplaceEverything";
            this.ReplaceEverything.Size = new System.Drawing.Size(75, 17);
            this.ReplaceEverything.TabIndex = 0;
            this.ReplaceEverything.TabStop = true;
            this.ReplaceEverything.Text = "Everything";
            this.ReplaceEverything.UseVisualStyleBackColor = true;
            // 
            // ReplaceVisible
            // 
            this.ReplaceVisible.AutoSize = true;
            this.ReplaceVisible.Location = new System.Drawing.Point(9, 42);
            this.ReplaceVisible.Name = "ReplaceVisible";
            this.ReplaceVisible.Size = new System.Drawing.Size(105, 17);
            this.ReplaceVisible.TabIndex = 0;
            this.ReplaceVisible.TabStop = true;
            this.ReplaceVisible.Text = "All visible objects";
            this.ReplaceVisible.UseVisualStyleBackColor = true;
            // 
            // ReplaceSelection
            // 
            this.ReplaceSelection.AutoSize = true;
            this.ReplaceSelection.Location = new System.Drawing.Point(9, 19);
            this.ReplaceSelection.Name = "ReplaceSelection";
            this.ReplaceSelection.Size = new System.Drawing.Size(69, 17);
            this.ReplaceSelection.TabIndex = 0;
            this.ReplaceSelection.TabStop = true;
            this.ReplaceSelection.Text = "Selection";
            this.ReplaceSelection.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ActionSelect);
            this.groupBox4.Controls.Add(this.ActionSubstitute);
            this.groupBox4.Controls.Add(this.ActionPartial);
            this.groupBox4.Controls.Add(this.ActionExact);
            this.groupBox4.Location = new System.Drawing.Point(211, 170);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(193, 111);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Action:";
            // 
            // ActionSubstitute
            // 
            this.ActionSubstitute.AutoSize = true;
            this.ActionSubstitute.Location = new System.Drawing.Point(9, 65);
            this.ActionSubstitute.Name = "ActionSubstitute";
            this.ActionSubstitute.Size = new System.Drawing.Size(146, 17);
            this.ActionSubstitute.TabIndex = 0;
            this.ActionSubstitute.TabStop = true;
            this.ActionSubstitute.Text = "Substitute partial matches";
            this.ActionSubstitute.UseVisualStyleBackColor = true;
            // 
            // ActionPartial
            // 
            this.ActionPartial.AutoSize = true;
            this.ActionPartial.Location = new System.Drawing.Point(9, 42);
            this.ActionPartial.Name = "ActionPartial";
            this.ActionPartial.Size = new System.Drawing.Size(139, 17);
            this.ActionPartial.TabIndex = 0;
            this.ActionPartial.TabStop = true;
            this.ActionPartial.Text = "Replace partial matches";
            this.ActionPartial.UseVisualStyleBackColor = true;
            // 
            // ActionExact
            // 
            this.ActionExact.AutoSize = true;
            this.ActionExact.Location = new System.Drawing.Point(9, 19);
            this.ActionExact.Name = "ActionExact";
            this.ActionExact.Size = new System.Drawing.Size(137, 17);
            this.ActionExact.TabIndex = 0;
            this.ActionExact.TabStop = true;
            this.ActionExact.Text = "Replace exact matches";
            this.ActionExact.UseVisualStyleBackColor = true;
            // 
            // RescaleTextures
            // 
            this.RescaleTextures.AutoSize = true;
            this.RescaleTextures.Location = new System.Drawing.Point(21, 263);
            this.RescaleTextures.Name = "RescaleTextures";
            this.RescaleTextures.Size = new System.Drawing.Size(158, 17);
            this.RescaleTextures.TabIndex = 2;
            this.RescaleTextures.Text = "Rescale texture coordinates";
            this.RescaleTextures.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(329, 287);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(248, 287);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // ActionSelect
            // 
            this.ActionSelect.AutoSize = true;
            this.ActionSelect.Location = new System.Drawing.Point(9, 88);
            this.ActionSelect.Name = "ActionSelect";
            this.ActionSelect.Size = new System.Drawing.Size(168, 17);
            this.ActionSelect.TabIndex = 0;
            this.ActionSelect.TabStop = true;
            this.ActionSelect.Text = "Select matches (don\'t replace)";
            this.ActionSelect.UseVisualStyleBackColor = true;
            // 
            // TextureReplaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 322);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.RescaleTextures);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureReplaceDialog";
            this.ShowInTaskbar = false;
            this.Text = "Replace Textures";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindImage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceImage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button FindBrowse;
        private System.Windows.Forms.PictureBox FindImage;
        private System.Windows.Forms.Label FindInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label ReplaceInfo;
        private System.Windows.Forms.PictureBox ReplaceImage;
        private System.Windows.Forms.Button ReplaceBrowse;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton ReplaceEverything;
        private System.Windows.Forms.RadioButton ReplaceVisible;
        private System.Windows.Forms.RadioButton ReplaceSelection;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton ActionSubstitute;
        private System.Windows.Forms.RadioButton ActionPartial;
        private System.Windows.Forms.RadioButton ActionExact;
        private System.Windows.Forms.CheckBox RescaleTextures;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox Find;
        private System.Windows.Forms.TextBox Replace;
        private System.Windows.Forms.RadioButton ActionSelect;
    }
}