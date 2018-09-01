namespace Sledge.Shell.Settings.Editors
{
    partial class HotkeysEditor
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
            this.HotkeyResetButton = new System.Windows.Forms.Button();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.HotkeyActionList = new System.Windows.Forms.ComboBox();
            this.HotkeyAddButton = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.HotkeyRemoveButton = new System.Windows.Forms.Button();
            this.HotkeyCombination = new System.Windows.Forms.TextBox();
            this.HotkeyList = new System.Windows.Forms.ListView();
            this.chAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ckKeyCombo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilterLabel = new System.Windows.Forms.Label();
            this.FilterBox = new System.Windows.Forms.TextBox();
            this.groupBox22.SuspendLayout();
            this.SuspendLayout();
            // 
            // HotkeyResetButton
            // 
            this.HotkeyResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HotkeyResetButton.Location = new System.Drawing.Point(523, 217);
            this.HotkeyResetButton.Name = "HotkeyResetButton";
            this.HotkeyResetButton.Size = new System.Drawing.Size(118, 23);
            this.HotkeyResetButton.TabIndex = 10;
            this.HotkeyResetButton.Text = "Reset to Defaults";
            this.HotkeyResetButton.UseVisualStyleBackColor = true;
            this.HotkeyResetButton.Click += new System.EventHandler(this.HotkeyResetButtonClicked);
            // 
            // groupBox22
            // 
            this.groupBox22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox22.Controls.Add(this.HotkeyActionList);
            this.groupBox22.Controls.Add(this.HotkeyAddButton);
            this.groupBox22.Controls.Add(this.label25);
            this.groupBox22.Controls.Add(this.label23);
            this.groupBox22.Controls.Add(this.HotkeyRemoveButton);
            this.groupBox22.Controls.Add(this.HotkeyCombination);
            this.groupBox22.Location = new System.Drawing.Point(3, 201);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(506, 50);
            this.groupBox22.TabIndex = 9;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Assign Hotkey";
            // 
            // HotkeyActionList
            // 
            this.HotkeyActionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HotkeyActionList.FormattingEnabled = true;
            this.HotkeyActionList.Location = new System.Drawing.Point(49, 18);
            this.HotkeyActionList.Name = "HotkeyActionList";
            this.HotkeyActionList.Size = new System.Drawing.Size(150, 21);
            this.HotkeyActionList.TabIndex = 3;
            // 
            // HotkeyAddButton
            // 
            this.HotkeyAddButton.Location = new System.Drawing.Point(358, 16);
            this.HotkeyAddButton.Name = "HotkeyAddButton";
            this.HotkeyAddButton.Size = new System.Drawing.Size(69, 23);
            this.HotkeyAddButton.TabIndex = 3;
            this.HotkeyAddButton.Text = "Set";
            this.HotkeyAddButton.UseVisualStyleBackColor = true;
            this.HotkeyAddButton.Click += new System.EventHandler(this.HotkeySetButtonClicked);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 22);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(37, 13);
            this.label25.TabIndex = 2;
            this.label25.Text = "Action";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(205, 22);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 13);
            this.label23.TabIndex = 2;
            this.label23.Text = "Hotkey";
            // 
            // HotkeyRemoveButton
            // 
            this.HotkeyRemoveButton.Location = new System.Drawing.Point(431, 16);
            this.HotkeyRemoveButton.Name = "HotkeyRemoveButton";
            this.HotkeyRemoveButton.Size = new System.Drawing.Size(69, 23);
            this.HotkeyRemoveButton.TabIndex = 8;
            this.HotkeyRemoveButton.Text = "Unset";
            this.HotkeyRemoveButton.UseVisualStyleBackColor = true;
            this.HotkeyRemoveButton.Click += new System.EventHandler(this.HotkeyUnsetButtonClicked);
            // 
            // HotkeyCombination
            // 
            this.HotkeyCombination.Location = new System.Drawing.Point(252, 19);
            this.HotkeyCombination.Name = "HotkeyCombination";
            this.HotkeyCombination.Size = new System.Drawing.Size(100, 20);
            this.HotkeyCombination.TabIndex = 1;
            this.HotkeyCombination.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyCombinationKeyDown);
            // 
            // HotkeyList
            // 
            this.HotkeyList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HotkeyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAction,
            this.chDescription,
            this.ckKeyCombo});
            this.HotkeyList.FullRowSelect = true;
            this.HotkeyList.HideSelection = false;
            this.HotkeyList.Location = new System.Drawing.Point(3, 29);
            this.HotkeyList.MultiSelect = false;
            this.HotkeyList.Name = "HotkeyList";
            this.HotkeyList.Size = new System.Drawing.Size(638, 166);
            this.HotkeyList.TabIndex = 6;
            this.HotkeyList.UseCompatibleStateImageBehavior = false;
            this.HotkeyList.View = System.Windows.Forms.View.Details;
            this.HotkeyList.SelectedIndexChanged += new System.EventHandler(this.HotkeyListSelectionChanged);
            this.HotkeyList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyListKeyDown);
            // 
            // chAction
            // 
            this.chAction.Text = "Action";
            // 
            // chDescription
            // 
            this.chDescription.Text = "Description";
            // 
            // ckKeyCombo
            // 
            this.ckKeyCombo.Text = "Hotkey";
            // 
            // FilterLabel
            // 
            this.FilterLabel.AutoSize = true;
            this.FilterLabel.Location = new System.Drawing.Point(5, 6);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(29, 13);
            this.FilterLabel.TabIndex = 12;
            this.FilterLabel.Text = "Filter";
            // 
            // FilterBox
            // 
            this.FilterBox.Location = new System.Drawing.Point(52, 3);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(150, 20);
            this.FilterBox.TabIndex = 11;
            this.FilterBox.TextChanged += new System.EventHandler(this.UpdateFilter);
            // 
            // HotkeysEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.FilterLabel);
            this.Controls.Add(this.FilterBox);
            this.Controls.Add(this.HotkeyResetButton);
            this.Controls.Add(this.groupBox22);
            this.Controls.Add(this.HotkeyList);
            this.Name = "HotkeysEditor";
            this.Size = new System.Drawing.Size(644, 253);
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button HotkeyResetButton;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.ComboBox HotkeyActionList;
        private System.Windows.Forms.Button HotkeyAddButton;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox HotkeyCombination;
        private System.Windows.Forms.Button HotkeyRemoveButton;
        private System.Windows.Forms.ListView HotkeyList;
        private System.Windows.Forms.ColumnHeader chAction;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader ckKeyCombo;
        private System.Windows.Forms.Label FilterLabel;
        private System.Windows.Forms.TextBox FilterBox;
    }
}
