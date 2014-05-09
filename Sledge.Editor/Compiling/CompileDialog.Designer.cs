namespace Sledge.Editor.Compiling
{
    partial class CompileDialog
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
            this.ProfileSelect = new System.Windows.Forms.ComboBox();
            this.label40 = new System.Windows.Forms.Label();
            this.ToolTabs = new System.Windows.Forms.TabControl();
            this.tabBuildAdvancedCSG = new System.Windows.Forms.TabPage();
            this.CsgParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tabBuildAdvancedBSP = new System.Windows.Forms.TabPage();
            this.BspParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tabBuildAdvancedVIS = new System.Windows.Forms.TabPage();
            this.VisParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tabBuildAdvancedRAD = new System.Windows.Forms.TabPage();
            this.RadParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tabBuildAdvancedShared = new System.Windows.Forms.TabPage();
            this.SharedParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tabBuildAdvancedPreview = new System.Windows.Forms.TabPage();
            this.ProfilePreview = new System.Windows.Forms.TextBox();
            this.CompileButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.NewProfileButton = new System.Windows.Forms.Button();
            this.RenameProfileButton = new System.Windows.Forms.Button();
            this.DeleteProfileButton = new System.Windows.Forms.Button();
            this.SaveProfileButton = new System.Windows.Forms.Button();
            this.SaveProfileAsButton = new System.Windows.Forms.Button();
            this.AdvancedPanel = new System.Windows.Forms.Panel();
            this.SimpleModeButton = new System.Windows.Forms.Button();
            this.SimplePanel = new System.Windows.Forms.Panel();
            this.AdvancedModeButton = new System.Windows.Forms.Button();
            this.PresetTable = new System.Windows.Forms.TableLayoutPanel();
            this.tabSteps = new System.Windows.Forms.TabPage();
            this.RunCsgCheckbox = new System.Windows.Forms.CheckBox();
            this.RunBspCheckbox = new System.Windows.Forms.CheckBox();
            this.RunVisCheckbox = new System.Windows.Forms.CheckBox();
            this.RunRadCheckbox = new System.Windows.Forms.CheckBox();
            this.ToolTabs.SuspendLayout();
            this.tabBuildAdvancedCSG.SuspendLayout();
            this.tabBuildAdvancedBSP.SuspendLayout();
            this.tabBuildAdvancedVIS.SuspendLayout();
            this.tabBuildAdvancedRAD.SuspendLayout();
            this.tabBuildAdvancedShared.SuspendLayout();
            this.tabBuildAdvancedPreview.SuspendLayout();
            this.AdvancedPanel.SuspendLayout();
            this.SimplePanel.SuspendLayout();
            this.tabSteps.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProfileSelect
            // 
            this.ProfileSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProfileSelect.FormattingEnabled = true;
            this.ProfileSelect.Location = new System.Drawing.Point(46, 5);
            this.ProfileSelect.Name = "ProfileSelect";
            this.ProfileSelect.Size = new System.Drawing.Size(121, 21);
            this.ProfileSelect.TabIndex = 24;
            this.ProfileSelect.SelectedIndexChanged += new System.EventHandler(this.ProfileSelected);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(3, 8);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(36, 13);
            this.label40.TabIndex = 23;
            this.label40.Text = "Profile";
            // 
            // ToolTabs
            // 
            this.ToolTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ToolTabs.Controls.Add(this.tabSteps);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedCSG);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedBSP);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedVIS);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedRAD);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedShared);
            this.ToolTabs.Controls.Add(this.tabBuildAdvancedPreview);
            this.ToolTabs.Location = new System.Drawing.Point(3, 32);
            this.ToolTabs.Name = "ToolTabs";
            this.ToolTabs.SelectedIndex = 0;
            this.ToolTabs.Size = new System.Drawing.Size(618, 283);
            this.ToolTabs.TabIndex = 22;
            // 
            // tabBuildAdvancedCSG
            // 
            this.tabBuildAdvancedCSG.Controls.Add(this.CsgParameters);
            this.tabBuildAdvancedCSG.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedCSG.Name = "tabBuildAdvancedCSG";
            this.tabBuildAdvancedCSG.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedCSG.TabIndex = 0;
            this.tabBuildAdvancedCSG.Text = "CSG";
            this.tabBuildAdvancedCSG.UseVisualStyleBackColor = true;
            // 
            // CsgParameters
            // 
            this.CsgParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CsgParameters.Location = new System.Drawing.Point(0, 0);
            this.CsgParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.CsgParameters.Name = "CsgParameters";
            this.CsgParameters.Size = new System.Drawing.Size(610, 257);
            this.CsgParameters.TabIndex = 0;
            this.CsgParameters.ValueChanged += new System.EventHandler(this.UpdatePreview);
            // 
            // tabBuildAdvancedBSP
            // 
            this.tabBuildAdvancedBSP.Controls.Add(this.BspParameters);
            this.tabBuildAdvancedBSP.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedBSP.Name = "tabBuildAdvancedBSP";
            this.tabBuildAdvancedBSP.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedBSP.TabIndex = 1;
            this.tabBuildAdvancedBSP.Text = "BSP";
            this.tabBuildAdvancedBSP.UseVisualStyleBackColor = true;
            // 
            // BspParameters
            // 
            this.BspParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BspParameters.Location = new System.Drawing.Point(0, 0);
            this.BspParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.BspParameters.Name = "BspParameters";
            this.BspParameters.Size = new System.Drawing.Size(610, 257);
            this.BspParameters.TabIndex = 1;
            this.BspParameters.ValueChanged += new System.EventHandler(this.UpdatePreview);
            // 
            // tabBuildAdvancedVIS
            // 
            this.tabBuildAdvancedVIS.Controls.Add(this.VisParameters);
            this.tabBuildAdvancedVIS.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedVIS.Name = "tabBuildAdvancedVIS";
            this.tabBuildAdvancedVIS.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedVIS.TabIndex = 2;
            this.tabBuildAdvancedVIS.Text = "VIS";
            this.tabBuildAdvancedVIS.UseVisualStyleBackColor = true;
            // 
            // VisParameters
            // 
            this.VisParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisParameters.Location = new System.Drawing.Point(0, 0);
            this.VisParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.VisParameters.Name = "VisParameters";
            this.VisParameters.Size = new System.Drawing.Size(610, 257);
            this.VisParameters.TabIndex = 1;
            this.VisParameters.ValueChanged += new System.EventHandler(this.UpdatePreview);
            // 
            // tabBuildAdvancedRAD
            // 
            this.tabBuildAdvancedRAD.Controls.Add(this.RadParameters);
            this.tabBuildAdvancedRAD.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedRAD.Name = "tabBuildAdvancedRAD";
            this.tabBuildAdvancedRAD.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedRAD.TabIndex = 3;
            this.tabBuildAdvancedRAD.Text = "RAD";
            this.tabBuildAdvancedRAD.UseVisualStyleBackColor = true;
            // 
            // RadParameters
            // 
            this.RadParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RadParameters.Location = new System.Drawing.Point(0, 0);
            this.RadParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.RadParameters.Name = "RadParameters";
            this.RadParameters.Size = new System.Drawing.Size(610, 257);
            this.RadParameters.TabIndex = 1;
            this.RadParameters.ValueChanged += new System.EventHandler(this.UpdatePreview);
            // 
            // tabBuildAdvancedShared
            // 
            this.tabBuildAdvancedShared.Controls.Add(this.SharedParameters);
            this.tabBuildAdvancedShared.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedShared.Name = "tabBuildAdvancedShared";
            this.tabBuildAdvancedShared.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedShared.TabIndex = 4;
            this.tabBuildAdvancedShared.Text = "Shared";
            this.tabBuildAdvancedShared.UseVisualStyleBackColor = true;
            // 
            // SharedParameters
            // 
            this.SharedParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SharedParameters.Location = new System.Drawing.Point(0, 0);
            this.SharedParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SharedParameters.Name = "SharedParameters";
            this.SharedParameters.Size = new System.Drawing.Size(610, 257);
            this.SharedParameters.TabIndex = 1;
            this.SharedParameters.ValueChanged += new System.EventHandler(this.UpdatePreview);
            // 
            // tabBuildAdvancedPreview
            // 
            this.tabBuildAdvancedPreview.Controls.Add(this.ProfilePreview);
            this.tabBuildAdvancedPreview.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedPreview.Name = "tabBuildAdvancedPreview";
            this.tabBuildAdvancedPreview.Size = new System.Drawing.Size(610, 257);
            this.tabBuildAdvancedPreview.TabIndex = 5;
            this.tabBuildAdvancedPreview.Text = "Preview";
            this.tabBuildAdvancedPreview.UseVisualStyleBackColor = true;
            // 
            // ProfilePreview
            // 
            this.ProfilePreview.BackColor = System.Drawing.SystemColors.Window;
            this.ProfilePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfilePreview.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProfilePreview.Location = new System.Drawing.Point(0, 0);
            this.ProfilePreview.Multiline = true;
            this.ProfilePreview.Name = "ProfilePreview";
            this.ProfilePreview.ReadOnly = true;
            this.ProfilePreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProfilePreview.Size = new System.Drawing.Size(610, 257);
            this.ProfilePreview.TabIndex = 3;
            // 
            // CompileButton
            // 
            this.CompileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CompileButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CompileButton.Location = new System.Drawing.Point(522, 321);
            this.CompileButton.Name = "CompileButton";
            this.CompileButton.Size = new System.Drawing.Size(99, 23);
            this.CompileButton.TabIndex = 25;
            this.CompileButton.Text = "Compile";
            this.CompileButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(417, 321);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(99, 23);
            this.CancelButton.TabIndex = 25;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // NewProfileButton
            // 
            this.NewProfileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NewProfileButton.Location = new System.Drawing.Point(540, 3);
            this.NewProfileButton.Name = "NewProfileButton";
            this.NewProfileButton.Size = new System.Drawing.Size(81, 23);
            this.NewProfileButton.TabIndex = 26;
            this.NewProfileButton.Text = "New Profile...";
            this.NewProfileButton.UseVisualStyleBackColor = true;
            this.NewProfileButton.Click += new System.EventHandler(this.NewProfileButtonClicked);
            // 
            // RenameProfileButton
            // 
            this.RenameProfileButton.Location = new System.Drawing.Point(173, 3);
            this.RenameProfileButton.Name = "RenameProfileButton";
            this.RenameProfileButton.Size = new System.Drawing.Size(56, 23);
            this.RenameProfileButton.TabIndex = 27;
            this.RenameProfileButton.Text = "Rename";
            this.RenameProfileButton.UseVisualStyleBackColor = true;
            this.RenameProfileButton.Click += new System.EventHandler(this.RenameProfileButtonClicked);
            // 
            // DeleteProfileButton
            // 
            this.DeleteProfileButton.Location = new System.Drawing.Point(235, 3);
            this.DeleteProfileButton.Name = "DeleteProfileButton";
            this.DeleteProfileButton.Size = new System.Drawing.Size(56, 23);
            this.DeleteProfileButton.TabIndex = 28;
            this.DeleteProfileButton.Text = "Delete";
            this.DeleteProfileButton.UseVisualStyleBackColor = true;
            this.DeleteProfileButton.Click += new System.EventHandler(this.DeleteProfileButtonClicked);
            // 
            // SaveProfileButton
            // 
            this.SaveProfileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveProfileButton.Location = new System.Drawing.Point(143, 321);
            this.SaveProfileButton.Name = "SaveProfileButton";
            this.SaveProfileButton.Size = new System.Drawing.Size(99, 23);
            this.SaveProfileButton.TabIndex = 25;
            this.SaveProfileButton.Text = "Save Profile";
            this.SaveProfileButton.UseVisualStyleBackColor = true;
            this.SaveProfileButton.Click += new System.EventHandler(this.SaveProfileButtonClicked);
            // 
            // SaveProfileAsButton
            // 
            this.SaveProfileAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveProfileAsButton.Location = new System.Drawing.Point(248, 321);
            this.SaveProfileAsButton.Name = "SaveProfileAsButton";
            this.SaveProfileAsButton.Size = new System.Drawing.Size(99, 23);
            this.SaveProfileAsButton.TabIndex = 25;
            this.SaveProfileAsButton.Text = "Save Profile As...";
            this.SaveProfileAsButton.UseVisualStyleBackColor = true;
            this.SaveProfileAsButton.Click += new System.EventHandler(this.SaveProfileAsButtonClicked);
            // 
            // AdvancedPanel
            // 
            this.AdvancedPanel.Controls.Add(this.SimpleModeButton);
            this.AdvancedPanel.Controls.Add(this.label40);
            this.AdvancedPanel.Controls.Add(this.NewProfileButton);
            this.AdvancedPanel.Controls.Add(this.ToolTabs);
            this.AdvancedPanel.Controls.Add(this.RenameProfileButton);
            this.AdvancedPanel.Controls.Add(this.ProfileSelect);
            this.AdvancedPanel.Controls.Add(this.DeleteProfileButton);
            this.AdvancedPanel.Controls.Add(this.CompileButton);
            this.AdvancedPanel.Controls.Add(this.CancelButton);
            this.AdvancedPanel.Controls.Add(this.SaveProfileButton);
            this.AdvancedPanel.Controls.Add(this.SaveProfileAsButton);
            this.AdvancedPanel.Location = new System.Drawing.Point(264, 12);
            this.AdvancedPanel.Name = "AdvancedPanel";
            this.AdvancedPanel.Size = new System.Drawing.Size(624, 347);
            this.AdvancedPanel.TabIndex = 29;
            // 
            // SimpleModeButton
            // 
            this.SimpleModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SimpleModeButton.Location = new System.Drawing.Point(3, 321);
            this.SimpleModeButton.Name = "SimpleModeButton";
            this.SimpleModeButton.Size = new System.Drawing.Size(107, 23);
            this.SimpleModeButton.TabIndex = 29;
            this.SimpleModeButton.Text = "Simple Mode";
            this.SimpleModeButton.UseVisualStyleBackColor = true;
            this.SimpleModeButton.Click += new System.EventHandler(this.SwitchToSimple);
            // 
            // SimplePanel
            // 
            this.SimplePanel.Controls.Add(this.AdvancedModeButton);
            this.SimplePanel.Controls.Add(this.PresetTable);
            this.SimplePanel.Location = new System.Drawing.Point(12, 12);
            this.SimplePanel.Name = "SimplePanel";
            this.SimplePanel.Size = new System.Drawing.Size(246, 347);
            this.SimplePanel.TabIndex = 30;
            // 
            // AdvancedModeButton
            // 
            this.AdvancedModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AdvancedModeButton.Location = new System.Drawing.Point(3, 321);
            this.AdvancedModeButton.Name = "AdvancedModeButton";
            this.AdvancedModeButton.Size = new System.Drawing.Size(107, 23);
            this.AdvancedModeButton.TabIndex = 25;
            this.AdvancedModeButton.Text = "Advanced Mode";
            this.AdvancedModeButton.UseVisualStyleBackColor = true;
            this.AdvancedModeButton.Click += new System.EventHandler(this.SwitchToAdvanced);
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
            // tabSteps
            // 
            this.tabSteps.Controls.Add(this.RunRadCheckbox);
            this.tabSteps.Controls.Add(this.RunVisCheckbox);
            this.tabSteps.Controls.Add(this.RunBspCheckbox);
            this.tabSteps.Controls.Add(this.RunCsgCheckbox);
            this.tabSteps.Location = new System.Drawing.Point(4, 22);
            this.tabSteps.Name = "tabSteps";
            this.tabSteps.Size = new System.Drawing.Size(610, 257);
            this.tabSteps.TabIndex = 6;
            this.tabSteps.Text = "Steps to run";
            this.tabSteps.UseVisualStyleBackColor = true;
            // 
            // RunCsgCheckbox
            // 
            this.RunCsgCheckbox.AutoSize = true;
            this.RunCsgCheckbox.Location = new System.Drawing.Point(7, 9);
            this.RunCsgCheckbox.Name = "RunCsgCheckbox";
            this.RunCsgCheckbox.Size = new System.Drawing.Size(71, 17);
            this.RunCsgCheckbox.TabIndex = 0;
            this.RunCsgCheckbox.Text = "Run CSG";
            this.RunCsgCheckbox.UseVisualStyleBackColor = true;
            // 
            // RunBspCheckbox
            // 
            this.RunBspCheckbox.AutoSize = true;
            this.RunBspCheckbox.Location = new System.Drawing.Point(7, 32);
            this.RunBspCheckbox.Name = "RunBspCheckbox";
            this.RunBspCheckbox.Size = new System.Drawing.Size(70, 17);
            this.RunBspCheckbox.TabIndex = 0;
            this.RunBspCheckbox.Text = "Run BSP";
            this.RunBspCheckbox.UseVisualStyleBackColor = true;
            // 
            // RunVisCheckbox
            // 
            this.RunVisCheckbox.AutoSize = true;
            this.RunVisCheckbox.Location = new System.Drawing.Point(7, 55);
            this.RunVisCheckbox.Name = "RunVisCheckbox";
            this.RunVisCheckbox.Size = new System.Drawing.Size(66, 17);
            this.RunVisCheckbox.TabIndex = 0;
            this.RunVisCheckbox.Text = "Run VIS";
            this.RunVisCheckbox.UseVisualStyleBackColor = true;
            // 
            // RunRadCheckbox
            // 
            this.RunRadCheckbox.AutoSize = true;
            this.RunRadCheckbox.Location = new System.Drawing.Point(7, 78);
            this.RunRadCheckbox.Name = "RunRadCheckbox";
            this.RunRadCheckbox.Size = new System.Drawing.Size(72, 17);
            this.RunRadCheckbox.TabIndex = 0;
            this.RunRadCheckbox.Text = "Run RAD";
            this.RunRadCheckbox.UseVisualStyleBackColor = true;
            // 
            // CompileDialog
            // 
            this.AcceptButton = this.CompileButton;
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
            this.tabBuildAdvancedCSG.ResumeLayout(false);
            this.tabBuildAdvancedBSP.ResumeLayout(false);
            this.tabBuildAdvancedVIS.ResumeLayout(false);
            this.tabBuildAdvancedRAD.ResumeLayout(false);
            this.tabBuildAdvancedShared.ResumeLayout(false);
            this.tabBuildAdvancedPreview.ResumeLayout(false);
            this.tabBuildAdvancedPreview.PerformLayout();
            this.AdvancedPanel.ResumeLayout(false);
            this.AdvancedPanel.PerformLayout();
            this.SimplePanel.ResumeLayout(false);
            this.tabSteps.ResumeLayout(false);
            this.tabSteps.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ProfileSelect;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TabControl ToolTabs;
        private System.Windows.Forms.TabPage tabBuildAdvancedCSG;
        private CompileParameterPanel CsgParameters;
        private System.Windows.Forms.TabPage tabBuildAdvancedBSP;
        private CompileParameterPanel BspParameters;
        private System.Windows.Forms.TabPage tabBuildAdvancedVIS;
        private CompileParameterPanel VisParameters;
        private System.Windows.Forms.TabPage tabBuildAdvancedRAD;
        private CompileParameterPanel RadParameters;
        private System.Windows.Forms.TabPage tabBuildAdvancedShared;
        private CompileParameterPanel SharedParameters;
        private System.Windows.Forms.TabPage tabBuildAdvancedPreview;
        private System.Windows.Forms.TextBox ProfilePreview;
        private System.Windows.Forms.Button CompileButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button NewProfileButton;
        private System.Windows.Forms.Button RenameProfileButton;
        private System.Windows.Forms.Button DeleteProfileButton;
        private System.Windows.Forms.Button SaveProfileButton;
        private System.Windows.Forms.Button SaveProfileAsButton;
        private System.Windows.Forms.Panel AdvancedPanel;
        private System.Windows.Forms.Panel SimplePanel;
        private System.Windows.Forms.TableLayoutPanel PresetTable;
        private System.Windows.Forms.Button AdvancedModeButton;
        private System.Windows.Forms.Button SimpleModeButton;
        private System.Windows.Forms.TabPage tabSteps;
        private System.Windows.Forms.CheckBox RunRadCheckbox;
        private System.Windows.Forms.CheckBox RunVisCheckbox;
        private System.Windows.Forms.CheckBox RunBspCheckbox;
        private System.Windows.Forms.CheckBox RunCsgCheckbox;
    }
}