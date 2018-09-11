namespace Sledge.BspEditor.Editing.Components.Compile
{
    sealed partial class CompileDialog
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
            this.cmbProfile = new System.Windows.Forms.ComboBox();
            this.lblProfile = new System.Windows.Forms.Label();
            this.ToolTabs = new System.Windows.Forms.TabControl();
            this.tabSteps = new System.Windows.Forms.TabPage();
            this.pnlSteps = new System.Windows.Forms.FlowLayoutPanel();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.btnSaveProfileAs = new System.Windows.Forms.Button();
            this.AdvancedPanel = new System.Windows.Forms.Panel();
            this.btnSimpleMode = new System.Windows.Forms.Button();
            this.SimplePanel = new System.Windows.Forms.Panel();
            this.btnAdvancedMode = new System.Windows.Forms.Button();
            this.PresetTable = new System.Windows.Forms.TableLayoutPanel();
            this.ToolTabs.SuspendLayout();
            this.tabSteps.SuspendLayout();
            this.AdvancedPanel.SuspendLayout();
            this.SimplePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbProfile
            // 
            this.cmbProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfile.FormattingEnabled = true;
            this.cmbProfile.Location = new System.Drawing.Point(105, 5);
            this.cmbProfile.Name = "cmbProfile";
            this.cmbProfile.Size = new System.Drawing.Size(121, 21);
            this.cmbProfile.TabIndex = 24;
            this.cmbProfile.SelectedIndexChanged += new System.EventHandler(this.ProfileSelected);
            // 
            // lblProfile
            // 
            this.lblProfile.Location = new System.Drawing.Point(7, 5);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(92, 21);
            this.lblProfile.TabIndex = 23;
            this.lblProfile.Text = "Profile";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ToolTabs
            // 
            this.ToolTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ToolTabs.Controls.Add(this.tabSteps);
            this.ToolTabs.Location = new System.Drawing.Point(3, 32);
            this.ToolTabs.Name = "ToolTabs";
            this.ToolTabs.SelectedIndex = 0;
            this.ToolTabs.Size = new System.Drawing.Size(618, 283);
            this.ToolTabs.TabIndex = 22;
            // 
            // tabSteps
            // 
            this.tabSteps.Controls.Add(this.pnlSteps);
            this.tabSteps.Location = new System.Drawing.Point(4, 22);
            this.tabSteps.Name = "tabSteps";
            this.tabSteps.Size = new System.Drawing.Size(610, 257);
            this.tabSteps.TabIndex = 6;
            this.tabSteps.Text = "Steps to run";
            this.tabSteps.UseVisualStyleBackColor = true;
            // 
            // pnlSteps
            // 
            this.pnlSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSteps.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlSteps.Location = new System.Drawing.Point(0, 0);
            this.pnlSteps.Name = "pnlSteps";
            this.pnlSteps.Size = new System.Drawing.Size(610, 257);
            this.pnlSteps.TabIndex = 0;
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGo.Location = new System.Drawing.Point(522, 321);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(99, 23);
            this.btnGo.TabIndex = 25;
            this.btnGo.Text = "Compile";
            this.btnGo.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(417, 321);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 23);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(232, 5);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(78, 21);
            this.btnRename.TabIndex = 27;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.RenameProfileButtonClicked);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(316, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(78, 21);
            this.btnDelete.TabIndex = 28;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.DeleteProfileButtonClicked);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveProfile.Location = new System.Drawing.Point(143, 321);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(99, 23);
            this.btnSaveProfile.TabIndex = 25;
            this.btnSaveProfile.Text = "Save Profile";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.SaveProfileButtonClicked);
            // 
            // btnSaveProfileAs
            // 
            this.btnSaveProfileAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveProfileAs.Location = new System.Drawing.Point(248, 321);
            this.btnSaveProfileAs.Name = "btnSaveProfileAs";
            this.btnSaveProfileAs.Size = new System.Drawing.Size(99, 23);
            this.btnSaveProfileAs.TabIndex = 25;
            this.btnSaveProfileAs.Text = "Save Profile As...";
            this.btnSaveProfileAs.UseVisualStyleBackColor = true;
            this.btnSaveProfileAs.Click += new System.EventHandler(this.SaveProfileAsButtonClicked);
            // 
            // AdvancedPanel
            // 
            this.AdvancedPanel.Controls.Add(this.btnSimpleMode);
            this.AdvancedPanel.Controls.Add(this.lblProfile);
            this.AdvancedPanel.Controls.Add(this.ToolTabs);
            this.AdvancedPanel.Controls.Add(this.btnRename);
            this.AdvancedPanel.Controls.Add(this.cmbProfile);
            this.AdvancedPanel.Controls.Add(this.btnDelete);
            this.AdvancedPanel.Controls.Add(this.btnGo);
            this.AdvancedPanel.Controls.Add(this.btnCancel);
            this.AdvancedPanel.Controls.Add(this.btnSaveProfile);
            this.AdvancedPanel.Controls.Add(this.btnSaveProfileAs);
            this.AdvancedPanel.Location = new System.Drawing.Point(264, 12);
            this.AdvancedPanel.Name = "AdvancedPanel";
            this.AdvancedPanel.Size = new System.Drawing.Size(624, 347);
            this.AdvancedPanel.TabIndex = 29;
            // 
            // btnSimpleMode
            // 
            this.btnSimpleMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSimpleMode.Location = new System.Drawing.Point(3, 321);
            this.btnSimpleMode.Name = "btnSimpleMode";
            this.btnSimpleMode.Size = new System.Drawing.Size(107, 23);
            this.btnSimpleMode.TabIndex = 29;
            this.btnSimpleMode.Text = "Simple Mode";
            this.btnSimpleMode.UseVisualStyleBackColor = true;
            this.btnSimpleMode.Click += new System.EventHandler(this.SwitchToSimple);
            // 
            // SimplePanel
            // 
            this.SimplePanel.Controls.Add(this.btnAdvancedMode);
            this.SimplePanel.Controls.Add(this.PresetTable);
            this.SimplePanel.Location = new System.Drawing.Point(12, 12);
            this.SimplePanel.Name = "SimplePanel";
            this.SimplePanel.Size = new System.Drawing.Size(246, 347);
            this.SimplePanel.TabIndex = 30;
            // 
            // btnAdvancedMode
            // 
            this.btnAdvancedMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdvancedMode.Location = new System.Drawing.Point(3, 321);
            this.btnAdvancedMode.Name = "btnAdvancedMode";
            this.btnAdvancedMode.Size = new System.Drawing.Size(107, 23);
            this.btnAdvancedMode.TabIndex = 25;
            this.btnAdvancedMode.Text = "Advanced Mode";
            this.btnAdvancedMode.UseVisualStyleBackColor = true;
            this.btnAdvancedMode.Click += new System.EventHandler(this.SwitchToAdvanced);
            // 
            // PresetTable
            // 
            this.PresetTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetTable.AutoScroll = true;
            this.PresetTable.ColumnCount = 1;
            this.PresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.PresetTable.Location = new System.Drawing.Point(3, 3);
            this.PresetTable.Name = "PresetTable";
            this.PresetTable.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.PresetTable.RowCount = 1;
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 312F));
            this.PresetTable.Size = new System.Drawing.Size(240, 312);
            this.PresetTable.TabIndex = 24;
            // 
            // CompileDialog
            // 
            this.AcceptButton = this.btnGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 369);
            this.Controls.Add(this.SimplePanel);
            this.Controls.Add(this.AdvancedPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Compile Map";
            this.ToolTabs.ResumeLayout(false);
            this.tabSteps.ResumeLayout(false);
            this.AdvancedPanel.ResumeLayout(false);
            this.SimplePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.TabControl ToolTabs;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.Button btnSaveProfileAs;
        private System.Windows.Forms.Panel AdvancedPanel;
        private System.Windows.Forms.Panel SimplePanel;
        private System.Windows.Forms.TableLayoutPanel PresetTable;
        private System.Windows.Forms.Button btnAdvancedMode;
        private System.Windows.Forms.Button btnSimpleMode;
        private System.Windows.Forms.TabPage tabSteps;
        private System.Windows.Forms.FlowLayoutPanel pnlSteps;
    }
}