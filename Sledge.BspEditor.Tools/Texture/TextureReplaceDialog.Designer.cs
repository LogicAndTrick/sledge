namespace Sledge.BspEditor.Tools.Texture
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
            this.FindGroup = new System.Windows.Forms.GroupBox();
            this.FindTextbox = new System.Windows.Forms.TextBox();
            this.FindInfo = new System.Windows.Forms.Label();
            this.FindImage = new System.Windows.Forms.PictureBox();
            this.FindBrowse = new System.Windows.Forms.Button();
            this.ReplaceGroup = new System.Windows.Forms.GroupBox();
            this.ReplaceTextbox = new System.Windows.Forms.TextBox();
            this.ReplaceInfo = new System.Windows.Forms.Label();
            this.ReplaceImage = new System.Windows.Forms.PictureBox();
            this.ReplaceBrowse = new System.Windows.Forms.Button();
            this.ReplaceInGroup = new System.Windows.Forms.GroupBox();
            this.ReplaceEverything = new System.Windows.Forms.RadioButton();
            this.ReplaceVisible = new System.Windows.Forms.RadioButton();
            this.ReplaceSelection = new System.Windows.Forms.RadioButton();
            this.ActionGroup = new System.Windows.Forms.GroupBox();
            this.ActionSelect = new System.Windows.Forms.RadioButton();
            this.ActionSubstitute = new System.Windows.Forms.RadioButton();
            this.ActionPartial = new System.Windows.Forms.RadioButton();
            this.ActionExact = new System.Windows.Forms.RadioButton();
            this.RescaleTextures = new System.Windows.Forms.CheckBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.FindGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindImage)).BeginInit();
            this.ReplaceGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceImage)).BeginInit();
            this.ReplaceInGroup.SuspendLayout();
            this.ActionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // FindGroup
            // 
            this.FindGroup.Controls.Add(this.FindTextbox);
            this.FindGroup.Controls.Add(this.FindInfo);
            this.FindGroup.Controls.Add(this.FindImage);
            this.FindGroup.Controls.Add(this.FindBrowse);
            this.FindGroup.Location = new System.Drawing.Point(12, 12);
            this.FindGroup.Name = "FindGroup";
            this.FindGroup.Size = new System.Drawing.Size(193, 152);
            this.FindGroup.TabIndex = 0;
            this.FindGroup.TabStop = false;
            this.FindGroup.Text = "Find:";
            // 
            // FindTextbox
            // 
            this.FindTextbox.Location = new System.Drawing.Point(6, 20);
            this.FindTextbox.Name = "FindTextbox";
            this.FindTextbox.Size = new System.Drawing.Size(181, 20);
            this.FindTextbox.TabIndex = 0;
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
            // ReplaceGroup
            // 
            this.ReplaceGroup.Controls.Add(this.ReplaceTextbox);
            this.ReplaceGroup.Controls.Add(this.ReplaceInfo);
            this.ReplaceGroup.Controls.Add(this.ReplaceImage);
            this.ReplaceGroup.Controls.Add(this.ReplaceBrowse);
            this.ReplaceGroup.Location = new System.Drawing.Point(211, 12);
            this.ReplaceGroup.Name = "ReplaceGroup";
            this.ReplaceGroup.Size = new System.Drawing.Size(193, 152);
            this.ReplaceGroup.TabIndex = 0;
            this.ReplaceGroup.TabStop = false;
            this.ReplaceGroup.Text = "Replace:";
            // 
            // ReplaceTextbox
            // 
            this.ReplaceTextbox.Location = new System.Drawing.Point(6, 20);
            this.ReplaceTextbox.Name = "ReplaceTextbox";
            this.ReplaceTextbox.Size = new System.Drawing.Size(181, 20);
            this.ReplaceTextbox.TabIndex = 2;
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
            this.ReplaceBrowse.TabIndex = 3;
            this.ReplaceBrowse.Text = "Browse...";
            this.ReplaceBrowse.UseVisualStyleBackColor = true;
            // 
            // ReplaceInGroup
            // 
            this.ReplaceInGroup.Controls.Add(this.ReplaceEverything);
            this.ReplaceInGroup.Controls.Add(this.ReplaceVisible);
            this.ReplaceInGroup.Controls.Add(this.ReplaceSelection);
            this.ReplaceInGroup.Location = new System.Drawing.Point(12, 170);
            this.ReplaceInGroup.Name = "ReplaceInGroup";
            this.ReplaceInGroup.Size = new System.Drawing.Size(193, 87);
            this.ReplaceInGroup.TabIndex = 1;
            this.ReplaceInGroup.TabStop = false;
            this.ReplaceInGroup.Text = "Replace In:";
            // 
            // ReplaceEverything
            // 
            this.ReplaceEverything.AutoSize = true;
            this.ReplaceEverything.Location = new System.Drawing.Point(9, 65);
            this.ReplaceEverything.Name = "ReplaceEverything";
            this.ReplaceEverything.Size = new System.Drawing.Size(75, 17);
            this.ReplaceEverything.TabIndex = 7;
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
            this.ReplaceVisible.TabIndex = 6;
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
            this.ReplaceSelection.TabIndex = 5;
            this.ReplaceSelection.TabStop = true;
            this.ReplaceSelection.Text = "Selection";
            this.ReplaceSelection.UseVisualStyleBackColor = true;
            // 
            // ActionGroup
            // 
            this.ActionGroup.Controls.Add(this.ActionSelect);
            this.ActionGroup.Controls.Add(this.ActionSubstitute);
            this.ActionGroup.Controls.Add(this.ActionPartial);
            this.ActionGroup.Controls.Add(this.ActionExact);
            this.ActionGroup.Location = new System.Drawing.Point(211, 170);
            this.ActionGroup.Name = "ActionGroup";
            this.ActionGroup.Size = new System.Drawing.Size(193, 111);
            this.ActionGroup.TabIndex = 1;
            this.ActionGroup.TabStop = false;
            this.ActionGroup.Text = "Action:";
            // 
            // ActionSelect
            // 
            this.ActionSelect.AutoSize = true;
            this.ActionSelect.Location = new System.Drawing.Point(9, 88);
            this.ActionSelect.Name = "ActionSelect";
            this.ActionSelect.Size = new System.Drawing.Size(168, 17);
            this.ActionSelect.TabIndex = 12;
            this.ActionSelect.TabStop = true;
            this.ActionSelect.Text = "Select matches (don\'t replace)";
            this.ActionSelect.UseVisualStyleBackColor = true;
            // 
            // ActionSubstitute
            // 
            this.ActionSubstitute.AutoSize = true;
            this.ActionSubstitute.Location = new System.Drawing.Point(9, 65);
            this.ActionSubstitute.Name = "ActionSubstitute";
            this.ActionSubstitute.Size = new System.Drawing.Size(146, 17);
            this.ActionSubstitute.TabIndex = 11;
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
            this.ActionPartial.TabIndex = 10;
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
            this.ActionExact.TabIndex = 9;
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
            this.RescaleTextures.TabIndex = 8;
            this.RescaleTextures.Text = "Rescale texture coordinates";
            this.RescaleTextures.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(329, 287);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 14;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelClicked);
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(248, 287);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 13;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OkClicked);
            // 
            // TextureReplaceDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton;
            this.ClientSize = new System.Drawing.Size(418, 322);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.RescaleTextures);
            this.Controls.Add(this.ActionGroup);
            this.Controls.Add(this.ReplaceInGroup);
            this.Controls.Add(this.ReplaceGroup);
            this.Controls.Add(this.FindGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureReplaceDialog";
            this.ShowInTaskbar = false;
            this.Text = "Replace Textures";
            this.FindGroup.ResumeLayout(false);
            this.FindGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindImage)).EndInit();
            this.ReplaceGroup.ResumeLayout(false);
            this.ReplaceGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceImage)).EndInit();
            this.ReplaceInGroup.ResumeLayout(false);
            this.ReplaceInGroup.PerformLayout();
            this.ActionGroup.ResumeLayout(false);
            this.ActionGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox FindGroup;
        private System.Windows.Forms.Button FindBrowse;
        private System.Windows.Forms.PictureBox FindImage;
        private System.Windows.Forms.Label FindInfo;
        private System.Windows.Forms.GroupBox ReplaceGroup;
        private System.Windows.Forms.Label ReplaceInfo;
        private System.Windows.Forms.PictureBox ReplaceImage;
        private System.Windows.Forms.Button ReplaceBrowse;
        private System.Windows.Forms.GroupBox ReplaceInGroup;
        private System.Windows.Forms.RadioButton ReplaceEverything;
        private System.Windows.Forms.RadioButton ReplaceVisible;
        private System.Windows.Forms.RadioButton ReplaceSelection;
        private System.Windows.Forms.GroupBox ActionGroup;
        private System.Windows.Forms.RadioButton ActionSubstitute;
        private System.Windows.Forms.RadioButton ActionPartial;
        private System.Windows.Forms.RadioButton ActionExact;
        private System.Windows.Forms.CheckBox RescaleTextures;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox FindTextbox;
        private System.Windows.Forms.TextBox ReplaceTextbox;
        private System.Windows.Forms.RadioButton ActionSelect;
    }
}