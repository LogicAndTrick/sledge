namespace Sledge.BspEditor.Editing.Components
{
    partial class PasteSpecialDialog
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
            this.lblCopies = new System.Windows.Forms.Label();
            this.NumCopies = new System.Windows.Forms.NumericUpDown();
            this.grpStartPoint = new System.Windows.Forms.GroupBox();
            this.StartSelection = new System.Windows.Forms.RadioButton();
            this.StartOriginal = new System.Windows.Forms.RadioButton();
            this.StartOrigin = new System.Windows.Forms.RadioButton();
            this.grpGrouping = new System.Windows.Forms.GroupBox();
            this.GroupAll = new System.Windows.Forms.RadioButton();
            this.GroupIndividual = new System.Windows.Forms.RadioButton();
            this.GroupNone = new System.Windows.Forms.RadioButton();
            this.grpOffset = new System.Windows.Forms.GroupBox();
            this.OffsetY = new System.Windows.Forms.NumericUpDown();
            this.OffsetZ = new System.Windows.Forms.NumericUpDown();
            this.OffsetX = new System.Windows.Forms.NumericUpDown();
            this.SourceOffsetZButton = new System.Windows.Forms.Button();
            this.ZeroOffsetZButton = new System.Windows.Forms.Button();
            this.SourceOffsetYButton = new System.Windows.Forms.Button();
            this.ZeroOffsetYButton = new System.Windows.Forms.Button();
            this.SourceOffsetXButton = new System.Windows.Forms.Button();
            this.ZeroOffsetXButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.UniqueEntityNames = new System.Windows.Forms.CheckBox();
            this.PrefixEntityNamesCheckbox = new System.Windows.Forms.CheckBox();
            this.EntityPrefix = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.grpRotation = new System.Windows.Forms.GroupBox();
            this.RotationY = new System.Windows.Forms.NumericUpDown();
            this.RotationZ = new System.Windows.Forms.NumericUpDown();
            this.RotationX = new System.Windows.Forms.NumericUpDown();
            this.ZeroRotationZButton = new System.Windows.Forms.Button();
            this.ZeroRotationYButton = new System.Windows.Forms.Button();
            this.ZeroRotationXButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumCopies)).BeginInit();
            this.grpStartPoint.SuspendLayout();
            this.grpGrouping.SuspendLayout();
            this.grpOffset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetX)).BeginInit();
            this.grpRotation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationX)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCopies
            // 
            this.lblCopies.AutoSize = true;
            this.lblCopies.Location = new System.Drawing.Point(12, 9);
            this.lblCopies.Name = "lblCopies";
            this.lblCopies.Size = new System.Drawing.Size(131, 13);
            this.lblCopies.TabIndex = 0;
            this.lblCopies.Text = "Number of copies to paste";
            // 
            // NumCopies
            // 
            this.NumCopies.Location = new System.Drawing.Point(149, 7);
            this.NumCopies.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.NumCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumCopies.Name = "NumCopies";
            this.NumCopies.Size = new System.Drawing.Size(57, 20);
            this.NumCopies.TabIndex = 1;
            this.NumCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // grpStartPoint
            // 
            this.grpStartPoint.Controls.Add(this.StartSelection);
            this.grpStartPoint.Controls.Add(this.StartOriginal);
            this.grpStartPoint.Controls.Add(this.StartOrigin);
            this.grpStartPoint.Location = new System.Drawing.Point(7, 33);
            this.grpStartPoint.Name = "grpStartPoint";
            this.grpStartPoint.Size = new System.Drawing.Size(210, 90);
            this.grpStartPoint.TabIndex = 2;
            this.grpStartPoint.TabStop = false;
            this.grpStartPoint.Text = "Start Point";
            // 
            // StartSelection
            // 
            this.StartSelection.AutoSize = true;
            this.StartSelection.Location = new System.Drawing.Point(11, 66);
            this.StartSelection.Name = "StartSelection";
            this.StartSelection.Size = new System.Drawing.Size(185, 17);
            this.StartSelection.TabIndex = 0;
            this.StartSelection.Text = "Start at center of current selection";
            this.StartSelection.UseVisualStyleBackColor = true;
            // 
            // StartOriginal
            // 
            this.StartOriginal.AutoSize = true;
            this.StartOriginal.Checked = true;
            this.StartOriginal.Location = new System.Drawing.Point(11, 43);
            this.StartOriginal.Name = "StartOriginal";
            this.StartOriginal.Size = new System.Drawing.Size(140, 17);
            this.StartOriginal.TabIndex = 0;
            this.StartOriginal.TabStop = true;
            this.StartOriginal.Text = "Start at center of original";
            this.StartOriginal.UseVisualStyleBackColor = true;
            // 
            // StartOrigin
            // 
            this.StartOrigin.AutoSize = true;
            this.StartOrigin.Location = new System.Drawing.Point(12, 20);
            this.StartOrigin.Name = "StartOrigin";
            this.StartOrigin.Size = new System.Drawing.Size(110, 17);
            this.StartOrigin.TabIndex = 0;
            this.StartOrigin.Text = "Start at map origin";
            this.StartOrigin.UseVisualStyleBackColor = true;
            // 
            // grpGrouping
            // 
            this.grpGrouping.Controls.Add(this.GroupAll);
            this.grpGrouping.Controls.Add(this.GroupIndividual);
            this.grpGrouping.Controls.Add(this.GroupNone);
            this.grpGrouping.Location = new System.Drawing.Point(223, 33);
            this.grpGrouping.Name = "grpGrouping";
            this.grpGrouping.Size = new System.Drawing.Size(208, 90);
            this.grpGrouping.TabIndex = 3;
            this.grpGrouping.TabStop = false;
            this.grpGrouping.Text = "Grouping";
            // 
            // GroupAll
            // 
            this.GroupAll.AutoSize = true;
            this.GroupAll.Location = new System.Drawing.Point(11, 66);
            this.GroupAll.Name = "GroupAll";
            this.GroupAll.Size = new System.Drawing.Size(101, 17);
            this.GroupAll.TabIndex = 0;
            this.GroupAll.Text = "Group all copies";
            this.GroupAll.UseVisualStyleBackColor = true;
            // 
            // GroupIndividual
            // 
            this.GroupIndividual.AutoSize = true;
            this.GroupIndividual.Checked = true;
            this.GroupIndividual.Location = new System.Drawing.Point(11, 43);
            this.GroupIndividual.Name = "GroupIndividual";
            this.GroupIndividual.Size = new System.Drawing.Size(135, 17);
            this.GroupIndividual.TabIndex = 0;
            this.GroupIndividual.TabStop = true;
            this.GroupIndividual.Text = "Group individual copies";
            this.GroupIndividual.UseVisualStyleBackColor = true;
            // 
            // GroupNone
            // 
            this.GroupNone.AutoSize = true;
            this.GroupNone.Location = new System.Drawing.Point(12, 20);
            this.GroupNone.Name = "GroupNone";
            this.GroupNone.Size = new System.Drawing.Size(83, 17);
            this.GroupNone.TabIndex = 0;
            this.GroupNone.Text = "No grouping";
            this.GroupNone.UseVisualStyleBackColor = true;
            // 
            // grpOffset
            // 
            this.grpOffset.Controls.Add(this.OffsetY);
            this.grpOffset.Controls.Add(this.OffsetZ);
            this.grpOffset.Controls.Add(this.OffsetX);
            this.grpOffset.Controls.Add(this.SourceOffsetZButton);
            this.grpOffset.Controls.Add(this.ZeroOffsetZButton);
            this.grpOffset.Controls.Add(this.SourceOffsetYButton);
            this.grpOffset.Controls.Add(this.ZeroOffsetYButton);
            this.grpOffset.Controls.Add(this.SourceOffsetXButton);
            this.grpOffset.Controls.Add(this.ZeroOffsetXButton);
            this.grpOffset.Controls.Add(this.label5);
            this.grpOffset.Controls.Add(this.label6);
            this.grpOffset.Controls.Add(this.label7);
            this.grpOffset.Location = new System.Drawing.Point(7, 129);
            this.grpOffset.Name = "grpOffset";
            this.grpOffset.Size = new System.Drawing.Size(210, 104);
            this.grpOffset.TabIndex = 4;
            this.grpOffset.TabStop = false;
            this.grpOffset.Text = "Offset (accumulative)";
            // 
            // OffsetY
            // 
            this.OffsetY.DecimalPlaces = 2;
            this.OffsetY.Location = new System.Drawing.Point(24, 46);
            this.OffsetY.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.OffsetY.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.OffsetY.Name = "OffsetY";
            this.OffsetY.Size = new System.Drawing.Size(66, 20);
            this.OffsetY.TabIndex = 14;
            // 
            // OffsetZ
            // 
            this.OffsetZ.DecimalPlaces = 2;
            this.OffsetZ.Location = new System.Drawing.Point(24, 72);
            this.OffsetZ.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.OffsetZ.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.OffsetZ.Name = "OffsetZ";
            this.OffsetZ.Size = new System.Drawing.Size(66, 20);
            this.OffsetZ.TabIndex = 14;
            // 
            // OffsetX
            // 
            this.OffsetX.DecimalPlaces = 2;
            this.OffsetX.Location = new System.Drawing.Point(24, 20);
            this.OffsetX.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this.OffsetX.Minimum = new decimal(new int[] {
            16384,
            0,
            0,
            -2147483648});
            this.OffsetX.Name = "OffsetX";
            this.OffsetX.Size = new System.Drawing.Size(66, 20);
            this.OffsetX.TabIndex = 14;
            // 
            // SourceOffsetZButton
            // 
            this.SourceOffsetZButton.Location = new System.Drawing.Point(127, 72);
            this.SourceOffsetZButton.Name = "SourceOffsetZButton";
            this.SourceOffsetZButton.Size = new System.Drawing.Size(49, 20);
            this.SourceOffsetZButton.TabIndex = 13;
            this.SourceOffsetZButton.Text = "Source";
            this.SourceOffsetZButton.UseVisualStyleBackColor = true;
            // 
            // ZeroOffsetZButton
            // 
            this.ZeroOffsetZButton.Location = new System.Drawing.Point(96, 72);
            this.ZeroOffsetZButton.Name = "ZeroOffsetZButton";
            this.ZeroOffsetZButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroOffsetZButton.TabIndex = 13;
            this.ZeroOffsetZButton.Text = "0";
            this.ZeroOffsetZButton.UseVisualStyleBackColor = true;
            // 
            // SourceOffsetYButton
            // 
            this.SourceOffsetYButton.Location = new System.Drawing.Point(127, 46);
            this.SourceOffsetYButton.Name = "SourceOffsetYButton";
            this.SourceOffsetYButton.Size = new System.Drawing.Size(49, 20);
            this.SourceOffsetYButton.TabIndex = 13;
            this.SourceOffsetYButton.Text = "Source";
            this.SourceOffsetYButton.UseVisualStyleBackColor = true;
            // 
            // ZeroOffsetYButton
            // 
            this.ZeroOffsetYButton.Location = new System.Drawing.Point(96, 46);
            this.ZeroOffsetYButton.Name = "ZeroOffsetYButton";
            this.ZeroOffsetYButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroOffsetYButton.TabIndex = 13;
            this.ZeroOffsetYButton.Text = "0";
            this.ZeroOffsetYButton.UseVisualStyleBackColor = true;
            // 
            // SourceOffsetXButton
            // 
            this.SourceOffsetXButton.Location = new System.Drawing.Point(127, 20);
            this.SourceOffsetXButton.Name = "SourceOffsetXButton";
            this.SourceOffsetXButton.Size = new System.Drawing.Size(49, 20);
            this.SourceOffsetXButton.TabIndex = 13;
            this.SourceOffsetXButton.Text = "Source";
            this.SourceOffsetXButton.UseVisualStyleBackColor = true;
            // 
            // ZeroOffsetXButton
            // 
            this.ZeroOffsetXButton.Location = new System.Drawing.Point(96, 20);
            this.ZeroOffsetXButton.Name = "ZeroOffsetXButton";
            this.ZeroOffsetXButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroOffsetXButton.TabIndex = 13;
            this.ZeroOffsetXButton.Text = "0";
            this.ZeroOffsetXButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Z:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "X:";
            // 
            // UniqueEntityNames
            // 
            this.UniqueEntityNames.AutoSize = true;
            this.UniqueEntityNames.Location = new System.Drawing.Point(12, 239);
            this.UniqueEntityNames.Name = "UniqueEntityNames";
            this.UniqueEntityNames.Size = new System.Drawing.Size(150, 17);
            this.UniqueEntityNames.TabIndex = 15;
            this.UniqueEntityNames.Text = "Make entity names unique";
            this.UniqueEntityNames.UseVisualStyleBackColor = true;
            // 
            // PrefixEntityNamesCheckbox
            // 
            this.PrefixEntityNamesCheckbox.AutoSize = true;
            this.PrefixEntityNamesCheckbox.Location = new System.Drawing.Point(12, 262);
            this.PrefixEntityNamesCheckbox.Name = "PrefixEntityNamesCheckbox";
            this.PrefixEntityNamesCheckbox.Size = new System.Drawing.Size(148, 17);
            this.PrefixEntityNamesCheckbox.TabIndex = 15;
            this.PrefixEntityNamesCheckbox.Text = "Prefix named entities with:";
            this.PrefixEntityNamesCheckbox.UseVisualStyleBackColor = true;
            // 
            // EntityPrefix
            // 
            this.EntityPrefix.Location = new System.Drawing.Point(166, 260);
            this.EntityPrefix.Name = "EntityPrefix";
            this.EntityPrefix.Size = new System.Drawing.Size(78, 20);
            this.EntityPrefix.TabIndex = 11;
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(270, 257);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 16;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButtonClicked);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(351, 257);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 16;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // grpRotation
            // 
            this.grpRotation.Controls.Add(this.RotationY);
            this.grpRotation.Controls.Add(this.RotationZ);
            this.grpRotation.Controls.Add(this.RotationX);
            this.grpRotation.Controls.Add(this.ZeroRotationZButton);
            this.grpRotation.Controls.Add(this.ZeroRotationYButton);
            this.grpRotation.Controls.Add(this.ZeroRotationXButton);
            this.grpRotation.Controls.Add(this.label8);
            this.grpRotation.Controls.Add(this.label9);
            this.grpRotation.Controls.Add(this.label10);
            this.grpRotation.Location = new System.Drawing.Point(223, 129);
            this.grpRotation.Name = "grpRotation";
            this.grpRotation.Size = new System.Drawing.Size(208, 104);
            this.grpRotation.TabIndex = 15;
            this.grpRotation.TabStop = false;
            this.grpRotation.Text = "Rotation (accumulative)";
            // 
            // RotationY
            // 
            this.RotationY.DecimalPlaces = 2;
            this.RotationY.Location = new System.Drawing.Point(24, 46);
            this.RotationY.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationY.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationY.Name = "RotationY";
            this.RotationY.Size = new System.Drawing.Size(66, 20);
            this.RotationY.TabIndex = 14;
            // 
            // RotationZ
            // 
            this.RotationZ.DecimalPlaces = 2;
            this.RotationZ.Location = new System.Drawing.Point(24, 72);
            this.RotationZ.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationZ.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationZ.Name = "RotationZ";
            this.RotationZ.Size = new System.Drawing.Size(66, 20);
            this.RotationZ.TabIndex = 14;
            // 
            // RotationX
            // 
            this.RotationX.DecimalPlaces = 2;
            this.RotationX.Location = new System.Drawing.Point(24, 20);
            this.RotationX.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationX.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationX.Name = "RotationX";
            this.RotationX.Size = new System.Drawing.Size(66, 20);
            this.RotationX.TabIndex = 14;
            // 
            // ZeroRotationZButton
            // 
            this.ZeroRotationZButton.Location = new System.Drawing.Point(96, 72);
            this.ZeroRotationZButton.Name = "ZeroRotationZButton";
            this.ZeroRotationZButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroRotationZButton.TabIndex = 13;
            this.ZeroRotationZButton.Text = "0";
            this.ZeroRotationZButton.UseVisualStyleBackColor = true;
            // 
            // ZeroRotationYButton
            // 
            this.ZeroRotationYButton.Location = new System.Drawing.Point(96, 46);
            this.ZeroRotationYButton.Name = "ZeroRotationYButton";
            this.ZeroRotationYButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroRotationYButton.TabIndex = 13;
            this.ZeroRotationYButton.Text = "0";
            this.ZeroRotationYButton.UseVisualStyleBackColor = true;
            // 
            // ZeroRotationXButton
            // 
            this.ZeroRotationXButton.Location = new System.Drawing.Point(96, 20);
            this.ZeroRotationXButton.Name = "ZeroRotationXButton";
            this.ZeroRotationXButton.Size = new System.Drawing.Size(25, 20);
            this.ZeroRotationXButton.TabIndex = 13;
            this.ZeroRotationXButton.Text = "0";
            this.ZeroRotationXButton.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Z:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Y:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "X:";
            // 
            // PasteSpecialDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 288);
            this.Controls.Add(this.grpRotation);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.PrefixEntityNamesCheckbox);
            this.Controls.Add(this.UniqueEntityNames);
            this.Controls.Add(this.grpOffset);
            this.Controls.Add(this.grpGrouping);
            this.Controls.Add(this.grpStartPoint);
            this.Controls.Add(this.NumCopies);
            this.Controls.Add(this.EntityPrefix);
            this.Controls.Add(this.lblCopies);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasteSpecialDialog";
            this.ShowInTaskbar = false;
            this.Text = "Paste Special";
            ((System.ComponentModel.ISupportInitialize)(this.NumCopies)).EndInit();
            this.grpStartPoint.ResumeLayout(false);
            this.grpStartPoint.PerformLayout();
            this.grpGrouping.ResumeLayout(false);
            this.grpGrouping.PerformLayout();
            this.grpOffset.ResumeLayout(false);
            this.grpOffset.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetX)).EndInit();
            this.grpRotation.ResumeLayout(false);
            this.grpRotation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCopies;
        private System.Windows.Forms.NumericUpDown NumCopies;
        private System.Windows.Forms.GroupBox grpStartPoint;
        private System.Windows.Forms.RadioButton StartSelection;
        private System.Windows.Forms.RadioButton StartOriginal;
        private System.Windows.Forms.RadioButton StartOrigin;
        private System.Windows.Forms.GroupBox grpGrouping;
        private System.Windows.Forms.RadioButton GroupAll;
        private System.Windows.Forms.RadioButton GroupIndividual;
        private System.Windows.Forms.RadioButton GroupNone;
        private System.Windows.Forms.GroupBox grpOffset;
        private System.Windows.Forms.Button SourceOffsetZButton;
        private System.Windows.Forms.Button ZeroOffsetZButton;
        private System.Windows.Forms.Button SourceOffsetYButton;
        private System.Windows.Forms.Button ZeroOffsetYButton;
        private System.Windows.Forms.Button SourceOffsetXButton;
        private System.Windows.Forms.Button ZeroOffsetXButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox UniqueEntityNames;
        private System.Windows.Forms.CheckBox PrefixEntityNamesCheckbox;
        private System.Windows.Forms.TextBox EntityPrefix;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.NumericUpDown OffsetY;
        private System.Windows.Forms.NumericUpDown OffsetZ;
        private System.Windows.Forms.NumericUpDown OffsetX;
        private System.Windows.Forms.GroupBox grpRotation;
        private System.Windows.Forms.NumericUpDown RotationY;
        private System.Windows.Forms.NumericUpDown RotationZ;
        private System.Windows.Forms.NumericUpDown RotationX;
        private System.Windows.Forms.Button ZeroRotationZButton;
        private System.Windows.Forms.Button ZeroRotationYButton;
        private System.Windows.Forms.Button ZeroRotationXButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}