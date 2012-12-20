/*
 * Created by SharpDevelop.
 * User: Dan
 * Date: 10/09/2008
 * Time: 7:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sledge.Editor.Settings
{
	partial class SettingsForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("WON Goldsource");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Steam Goldsource");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Source");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Goldsource");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Source");
            this.tbcSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tab2DViews = new System.Windows.Forms.TabPage();
            this.ArrowKeysNudgeSelection = new System.Windows.Forms.CheckBox();
            this.NudgeUnits = new System.Windows.Forms.NumericUpDown();
            this.CrosshairCursorIn2DViews = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.RotationStyle_SnapNever = new System.Windows.Forms.RadioButton();
            this.RotationStyle_SnapOnShift = new System.Windows.Forms.RadioButton();
            this.RotationStyle_SnapOffShift = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.GridHighlight2Colour = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.GridBackgroundColour = new System.Windows.Forms.Panel();
            this.GridHighlight2UnitNum = new System.Windows.Forms.DomainUpDown();
            this.GridBoundaryColour = new System.Windows.Forms.Panel();
            this.GridZeroAxisColour = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this.GridHighlight1On = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.GridHighlight2On = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.GridHighlight1Colour = new System.Windows.Forms.Panel();
            this.GridColour = new System.Windows.Forms.Panel();
            this.GridHighlight1Distance = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SnapStyle_SnapOffAlt = new System.Windows.Forms.RadioButton();
            this.SnapStyle_SnapOnAlt = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.HideGridOn = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.HideGridLimit = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.HideGridFactor = new System.Windows.Forms.DomainUpDown();
            this.DefaultGridSize = new System.Windows.Forms.DomainUpDown();
            this.tab3DViews = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.o_3DInvertX = new System.Windows.Forms.CheckBox();
            this.o_3DInvertY = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.o_3DTimeToTopSpeed = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.o_3DForwardSpeed = new System.Windows.Forms.TrackBar();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.o_3DBackClippingPane = new System.Windows.Forms.TrackBar();
            this.label16 = new System.Windows.Forms.Label();
            this.tabGame = new System.Windows.Forms.TabPage();
            this.tabGameSubTabs = new System.Windows.Forms.TabControl();
            this.tabConfigDirectories = new System.Windows.Forms.TabPage();
            this.btnGameChangeName = new System.Windows.Forms.Button();
            this.grpConfigGame = new System.Windows.Forms.GroupBox();
            this.txtGameWONDir = new System.Windows.Forms.TextBox();
            this.lblGameWONDir = new System.Windows.Forms.Label();
            this.btnGameDirBrowse = new System.Windows.Forms.Button();
            this.lblGameSteamDir = new System.Windows.Forms.Label();
            this.cmbGameSteamDir = new System.Windows.Forms.ComboBox();
            this.lblGameMod = new System.Windows.Forms.Label();
            this.cmbGameMod = new System.Windows.Forms.ComboBox();
            this.lblGameName = new System.Windows.Forms.Label();
            this.cmbGameBuild = new System.Windows.Forms.ComboBox();
            this.grpConfigSaving = new System.Windows.Forms.GroupBox();
            this.lblGameMapSaveDir = new System.Windows.Forms.Label();
            this.chkGameEnableAutosave = new System.Windows.Forms.CheckBox();
            this.txtGameMapDir = new System.Windows.Forms.TextBox();
            this.btnGameMapDirBrowse = new System.Windows.Forms.Button();
            this.txtGameAutosaveDir = new System.Windows.Forms.TextBox();
            this.lblGameAutosaveDir = new System.Windows.Forms.Label();
            this.btnGameAutosaveDirBrowse = new System.Windows.Forms.Button();
            this.chkGameMapDiffAutosaveDir = new System.Windows.Forms.CheckBox();
            this.lblGameBuild = new System.Windows.Forms.Label();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.cmbGameEngine = new System.Windows.Forms.ComboBox();
            this.lblGameEngine = new System.Windows.Forms.Label();
            this.tabConfigEntities = new System.Windows.Forms.TabPage();
            this.lblGameFGD = new System.Windows.Forms.Label();
            this.btnGameAddFGD = new System.Windows.Forms.Button();
            this.lblConfigBrushEnt = new System.Windows.Forms.Label();
            this.btnGameRemoveFGD = new System.Windows.Forms.Button();
            this.lblConfigPointEnt = new System.Windows.Forms.Label();
            this.lstGameFGD = new System.Windows.Forms.ListBox();
            this.cmbGameBrushEnt = new System.Windows.Forms.ComboBox();
            this.cmbGamePointEnt = new System.Windows.Forms.ComboBox();
            this.tabConfigTextures = new System.Windows.Forms.TabPage();
            this.lblGameWAD = new System.Windows.Forms.Label();
            this.nudGameLightmapScale = new System.Windows.Forms.NumericUpDown();
            this.lblConfigLightmapScale = new System.Windows.Forms.Label();
            this.btnGameAddWAD = new System.Windows.Forms.Button();
            this.nudGameTextureScale = new System.Windows.Forms.NumericUpDown();
            this.lstGameWAD = new System.Windows.Forms.ListBox();
            this.lblConfigTextureScale = new System.Windows.Forms.Label();
            this.btnGameRemoveWAD = new System.Windows.Forms.Button();
            this.tree_games = new System.Windows.Forms.TreeView();
            this.btnGameRemove = new System.Windows.Forms.Button();
            this.btnGameAdd = new System.Windows.Forms.Button();
            this.tabBuild = new System.Windows.Forms.TabPage();
            this.tabBuildSubTabs = new System.Windows.Forms.TabControl();
            this.tabBuildGeneral = new System.Windows.Forms.TabPage();
            this.btnBuildChangeName = new System.Windows.Forms.Button();
            this.lblBuildName = new System.Windows.Forms.Label();
            this.txtBuildName = new System.Windows.Forms.TextBox();
            this.lblBuildEngine = new System.Windows.Forms.Label();
            this.cmbBuildEngine = new System.Windows.Forms.ComboBox();
            this.tabBuildExecutables = new System.Windows.Forms.TabPage();
            this.lstBuildPresets = new System.Windows.Forms.ListBox();
            this.lblBuildExeFolder = new System.Windows.Forms.Label();
            this.lblBuildBSP = new System.Windows.Forms.Label();
            this.txtBuildExeFolder = new System.Windows.Forms.TextBox();
            this.lblBuildCSG = new System.Windows.Forms.Label();
            this.cmbBuildRAD = new System.Windows.Forms.ComboBox();
            this.cmbBuildBSP = new System.Windows.Forms.ComboBox();
            this.lblBuildDetectedPresets = new System.Windows.Forms.Label();
            this.lblBuildVIS = new System.Windows.Forms.Label();
            this.cmbBuildVIS = new System.Windows.Forms.ComboBox();
            this.cmbBuildCSG = new System.Windows.Forms.ComboBox();
            this.lblBuildRAD = new System.Windows.Forms.Label();
            this.btnBuildExeFolderBrowse = new System.Windows.Forms.Button();
            this.tabBuildPostCompile = new System.Windows.Forms.TabPage();
            this.lblBuildCommandLine = new System.Windows.Forms.Label();
            this.chkBuildCopyBSP = new System.Windows.Forms.CheckBox();
            this.chkBuildAskBeforeRun = new System.Windows.Forms.CheckBox();
            this.radBuildRunGame = new System.Windows.Forms.RadioButton();
            this.txtBuildCommandLine = new System.Windows.Forms.TextBox();
            this.radBuildRunGameOnChange = new System.Windows.Forms.RadioButton();
            this.chkBuildShowLog = new System.Windows.Forms.CheckBox();
            this.radBuildDontRunGame = new System.Windows.Forms.RadioButton();
            this.tabBuildAdvanced = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedSubTabs = new System.Windows.Forms.TabControl();
            this.tabBuildAdvancedCSG = new System.Windows.Forms.TabPage();
            this.label20 = new System.Windows.Forms.Label();
            this.tabBuildAdvancedBSP = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedVIS = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedRAD = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedShared = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedPreview = new System.Windows.Forms.TabPage();
            this.txtBuildAdvancedPreview = new System.Windows.Forms.TextBox();
            this.lblBuildTEMPAdvancedConfig = new System.Windows.Forms.Label();
            this.btnBuildRemove = new System.Windows.Forms.Button();
            this.btnBuildAdd = new System.Windows.Forms.Button();
            this.tree_builds = new System.Windows.Forms.TreeView();
            this.tabSteam = new System.Windows.Forms.TabPage();
            this.o_SteamInstallDir = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.btnSteamInstallDirBrowse = new System.Windows.Forms.Button();
            this.o_SteamUsername = new System.Windows.Forms.ComboBox();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.chKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ckKeyCombo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTrigger = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.lblConfigSteamDir = new System.Windows.Forms.Label();
            this.btnConfigListSteamGames = new System.Windows.Forms.Button();
            this.btnConfigSteamDirBrowse = new System.Windows.Forms.Button();
            this.cmbConfigSteamUser = new System.Windows.Forms.ComboBox();
            this.lblConfigSteamUser = new System.Windows.Forms.Label();
            this.btnCancelSettings = new System.Windows.Forms.Button();
            this.btnApplyAndCloseSettings = new System.Windows.Forms.Button();
            this.btnApplySettings = new System.Windows.Forms.Button();
            this.tbcSettings.SuspendLayout();
            this.tab2DViews.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudgeUnits)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridHighlight1Distance)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HideGridLimit)).BeginInit();
            this.tab3DViews.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.o_3DTimeToTopSpeed)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.o_3DForwardSpeed)).BeginInit();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.o_3DBackClippingPane)).BeginInit();
            this.tabGame.SuspendLayout();
            this.tabGameSubTabs.SuspendLayout();
            this.tabConfigDirectories.SuspendLayout();
            this.grpConfigGame.SuspendLayout();
            this.grpConfigSaving.SuspendLayout();
            this.tabConfigEntities.SuspendLayout();
            this.tabConfigTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGameLightmapScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGameTextureScale)).BeginInit();
            this.tabBuild.SuspendLayout();
            this.tabBuildSubTabs.SuspendLayout();
            this.tabBuildGeneral.SuspendLayout();
            this.tabBuildExecutables.SuspendLayout();
            this.tabBuildPostCompile.SuspendLayout();
            this.tabBuildAdvanced.SuspendLayout();
            this.tabBuildAdvancedSubTabs.SuspendLayout();
            this.tabBuildAdvancedCSG.SuspendLayout();
            this.tabBuildAdvancedPreview.SuspendLayout();
            this.tabSteam.SuspendLayout();
            this.tabHotkeys.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbcSettings
            // 
            this.tbcSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcSettings.Controls.Add(this.tabGeneral);
            this.tbcSettings.Controls.Add(this.tab2DViews);
            this.tbcSettings.Controls.Add(this.tab3DViews);
            this.tbcSettings.Controls.Add(this.tabGame);
            this.tbcSettings.Controls.Add(this.tabBuild);
            this.tbcSettings.Controls.Add(this.tabSteam);
            this.tbcSettings.Controls.Add(this.tabHotkeys);
            this.tbcSettings.Location = new System.Drawing.Point(12, 12);
            this.tbcSettings.Name = "tbcSettings";
            this.tbcSettings.SelectedIndex = 0;
            this.tbcSettings.Size = new System.Drawing.Size(744, 537);
            this.tbcSettings.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(736, 511);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tab2DViews
            // 
            this.tab2DViews.Controls.Add(this.ArrowKeysNudgeSelection);
            this.tab2DViews.Controls.Add(this.NudgeUnits);
            this.tab2DViews.Controls.Add(this.CrosshairCursorIn2DViews);
            this.tab2DViews.Controls.Add(this.groupBox6);
            this.tab2DViews.Controls.Add(this.groupBox3);
            this.tab2DViews.Controls.Add(this.label2);
            this.tab2DViews.Controls.Add(this.groupBox4);
            this.tab2DViews.Controls.Add(this.groupBox5);
            this.tab2DViews.Location = new System.Drawing.Point(4, 22);
            this.tab2DViews.Name = "tab2DViews";
            this.tab2DViews.Padding = new System.Windows.Forms.Padding(3);
            this.tab2DViews.Size = new System.Drawing.Size(736, 511);
            this.tab2DViews.TabIndex = 1;
            this.tab2DViews.Text = "2D Views";
            this.tab2DViews.UseVisualStyleBackColor = true;
            // 
            // ArrowKeysNudgeSelection
            // 
            this.ArrowKeysNudgeSelection.Checked = true;
            this.ArrowKeysNudgeSelection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ArrowKeysNudgeSelection.Location = new System.Drawing.Point(6, 30);
            this.ArrowKeysNudgeSelection.Name = "ArrowKeysNudgeSelection";
            this.ArrowKeysNudgeSelection.Size = new System.Drawing.Size(126, 24);
            this.ArrowKeysNudgeSelection.TabIndex = 0;
            this.ArrowKeysNudgeSelection.Text = "Arrow keys nudge by";
            this.ArrowKeysNudgeSelection.UseVisualStyleBackColor = true;
            // 
            // NudgeUnits
            // 
            this.NudgeUnits.DecimalPlaces = 2;
            this.NudgeUnits.Location = new System.Drawing.Point(132, 31);
            this.NudgeUnits.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudgeUnits.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudgeUnits.Name = "NudgeUnits";
            this.NudgeUnits.Size = new System.Drawing.Size(51, 20);
            this.NudgeUnits.TabIndex = 2;
            this.NudgeUnits.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // CrosshairCursorIn2DViews
            // 
            this.CrosshairCursorIn2DViews.Location = new System.Drawing.Point(6, 6);
            this.CrosshairCursorIn2DViews.Name = "CrosshairCursorIn2DViews";
            this.CrosshairCursorIn2DViews.Size = new System.Drawing.Size(225, 24);
            this.CrosshairCursorIn2DViews.TabIndex = 0;
            this.CrosshairCursorIn2DViews.Tag = "";
            this.CrosshairCursorIn2DViews.Text = "Crosshair cursor in 2D views";
            this.CrosshairCursorIn2DViews.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.RotationStyle_SnapNever);
            this.groupBox6.Controls.Add(this.RotationStyle_SnapOnShift);
            this.groupBox6.Controls.Add(this.RotationStyle_SnapOffShift);
            this.groupBox6.Location = new System.Drawing.Point(6, 60);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(247, 114);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Rotation Style";
            // 
            // RotationStyle_SnapNever
            // 
            this.RotationStyle_SnapNever.Location = new System.Drawing.Point(14, 79);
            this.RotationStyle_SnapNever.Name = "RotationStyle_SnapNever";
            this.RotationStyle_SnapNever.Size = new System.Drawing.Size(137, 24);
            this.RotationStyle_SnapNever.TabIndex = 2;
            this.RotationStyle_SnapNever.Text = "No rotational snapping";
            this.RotationStyle_SnapNever.UseVisualStyleBackColor = true;
            // 
            // RotationStyle_SnapOnShift
            // 
            this.RotationStyle_SnapOnShift.Checked = true;
            this.RotationStyle_SnapOnShift.Location = new System.Drawing.Point(14, 19);
            this.RotationStyle_SnapOnShift.Name = "RotationStyle_SnapOnShift";
            this.RotationStyle_SnapOnShift.Size = new System.Drawing.Size(182, 24);
            this.RotationStyle_SnapOnShift.TabIndex = 2;
            this.RotationStyle_SnapOnShift.TabStop = true;
            this.RotationStyle_SnapOnShift.Text = "Press shift to snap to 15 degrees";
            this.RotationStyle_SnapOnShift.UseVisualStyleBackColor = true;
            // 
            // RotationStyle_SnapOffShift
            // 
            this.RotationStyle_SnapOffShift.Location = new System.Drawing.Point(14, 49);
            this.RotationStyle_SnapOffShift.Name = "RotationStyle_SnapOffShift";
            this.RotationStyle_SnapOffShift.Size = new System.Drawing.Size(230, 24);
            this.RotationStyle_SnapOffShift.TabIndex = 2;
            this.RotationStyle_SnapOffShift.Text = "Snap to 15 degrees unless shift is pressed";
            this.RotationStyle_SnapOffShift.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.GridHighlight2Colour);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.GridBackgroundColour);
            this.groupBox3.Controls.Add(this.GridHighlight2UnitNum);
            this.groupBox3.Controls.Add(this.GridBoundaryColour);
            this.groupBox3.Controls.Add(this.GridZeroAxisColour);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.GridHighlight1On);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.GridHighlight2On);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.GridHighlight1Colour);
            this.groupBox3.Controls.Add(this.GridColour);
            this.groupBox3.Controls.Add(this.GridHighlight1Distance);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(6, 271);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(409, 178);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Grid Colour Settings";
            // 
            // GridHighlight2Colour
            // 
            this.GridHighlight2Colour.BackColor = System.Drawing.Color.DarkRed;
            this.GridHighlight2Colour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridHighlight2Colour.Location = new System.Drawing.Point(94, 139);
            this.GridHighlight2Colour.Name = "GridHighlight2Colour";
            this.GridHighlight2Colour.Size = new System.Drawing.Size(51, 17);
            this.GridHighlight2Colour.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(14, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Background:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(14, 139);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 17);
            this.label9.TabIndex = 1;
            this.label9.Text = "Highlight 2:";
            // 
            // GridBackgroundColour
            // 
            this.GridBackgroundColour.BackColor = System.Drawing.Color.Black;
            this.GridBackgroundColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridBackgroundColour.Location = new System.Drawing.Point(94, 24);
            this.GridBackgroundColour.Name = "GridBackgroundColour";
            this.GridBackgroundColour.Size = new System.Drawing.Size(51, 17);
            this.GridBackgroundColour.TabIndex = 2;
            // 
            // GridHighlight2UnitNum
            // 
            this.GridHighlight2UnitNum.Items.Add("32768");
            this.GridHighlight2UnitNum.Items.Add("16384");
            this.GridHighlight2UnitNum.Items.Add("8192");
            this.GridHighlight2UnitNum.Items.Add("4096");
            this.GridHighlight2UnitNum.Items.Add("2048");
            this.GridHighlight2UnitNum.Items.Add("1024");
            this.GridHighlight2UnitNum.Items.Add("512");
            this.GridHighlight2UnitNum.Items.Add("256");
            this.GridHighlight2UnitNum.Items.Add("128");
            this.GridHighlight2UnitNum.Items.Add("64");
            this.GridHighlight2UnitNum.Items.Add("32");
            this.GridHighlight2UnitNum.Location = new System.Drawing.Point(249, 137);
            this.GridHighlight2UnitNum.Name = "GridHighlight2UnitNum";
            this.GridHighlight2UnitNum.Size = new System.Drawing.Size(50, 20);
            this.GridHighlight2UnitNum.TabIndex = 0;
            this.GridHighlight2UnitNum.Text = "1024";
            // 
            // GridBoundaryColour
            // 
            this.GridBoundaryColour.BackColor = System.Drawing.Color.Red;
            this.GridBoundaryColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridBoundaryColour.Location = new System.Drawing.Point(94, 93);
            this.GridBoundaryColour.Name = "GridBoundaryColour";
            this.GridBoundaryColour.Size = new System.Drawing.Size(51, 17);
            this.GridBoundaryColour.TabIndex = 2;
            // 
            // GridZeroAxisColour
            // 
            this.GridZeroAxisColour.BackColor = System.Drawing.Color.Aqua;
            this.GridZeroAxisColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridZeroAxisColour.Location = new System.Drawing.Point(94, 70);
            this.GridZeroAxisColour.Name = "GridZeroAxisColour";
            this.GridZeroAxisColour.Size = new System.Drawing.Size(51, 17);
            this.GridZeroAxisColour.TabIndex = 2;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(14, 93);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(74, 17);
            this.label19.TabIndex = 1;
            this.label19.Text = "Boundaries:";
            // 
            // GridHighlight1On
            // 
            this.GridHighlight1On.Checked = true;
            this.GridHighlight1On.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GridHighlight1On.Location = new System.Drawing.Point(149, 117);
            this.GridHighlight1On.Name = "GridHighlight1On";
            this.GridHighlight1On.Size = new System.Drawing.Size(98, 17);
            this.GridHighlight1On.TabIndex = 0;
            this.GridHighlight1On.Text = "Highlight every";
            this.GridHighlight1On.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(14, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 17);
            this.label10.TabIndex = 1;
            this.label10.Text = "Zero Axes:";
            // 
            // GridHighlight2On
            // 
            this.GridHighlight2On.Checked = true;
            this.GridHighlight2On.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GridHighlight2On.Location = new System.Drawing.Point(149, 139);
            this.GridHighlight2On.Name = "GridHighlight2On";
            this.GridHighlight2On.Size = new System.Drawing.Size(98, 17);
            this.GridHighlight2On.TabIndex = 0;
            this.GridHighlight2On.Text = "Highlight every";
            this.GridHighlight2On.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(305, 140);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(53, 17);
            this.label21.TabIndex = 3;
            this.label21.Text = "units";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(304, 118);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 17);
            this.label11.TabIndex = 3;
            this.label11.Text = "grid lines";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(14, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 17);
            this.label7.TabIndex = 1;
            this.label7.Text = "Grid:";
            // 
            // GridHighlight1Colour
            // 
            this.GridHighlight1Colour.BackColor = System.Drawing.Color.White;
            this.GridHighlight1Colour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridHighlight1Colour.Location = new System.Drawing.Point(94, 116);
            this.GridHighlight1Colour.Name = "GridHighlight1Colour";
            this.GridHighlight1Colour.Size = new System.Drawing.Size(51, 17);
            this.GridHighlight1Colour.TabIndex = 2;
            // 
            // GridColour
            // 
            this.GridColour.BackColor = System.Drawing.Color.Gainsboro;
            this.GridColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridColour.Location = new System.Drawing.Point(94, 47);
            this.GridColour.Name = "GridColour";
            this.GridColour.Size = new System.Drawing.Size(51, 17);
            this.GridColour.TabIndex = 2;
            // 
            // GridHighlight1Distance
            // 
            this.GridHighlight1Distance.Location = new System.Drawing.Point(249, 116);
            this.GridHighlight1Distance.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.GridHighlight1Distance.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.GridHighlight1Distance.Name = "GridHighlight1Distance";
            this.GridHighlight1Distance.Size = new System.Drawing.Size(50, 20);
            this.GridHighlight1Distance.TabIndex = 2;
            this.GridHighlight1Distance.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(14, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "Highlight 1:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(185, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "unit(s)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SnapStyle_SnapOffAlt);
            this.groupBox4.Controls.Add(this.SnapStyle_SnapOnAlt);
            this.groupBox4.Location = new System.Drawing.Point(259, 60);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(247, 114);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Snap to Grid";
            // 
            // SnapStyle_SnapOffAlt
            // 
            this.SnapStyle_SnapOffAlt.Checked = true;
            this.SnapStyle_SnapOffAlt.Location = new System.Drawing.Point(14, 19);
            this.SnapStyle_SnapOffAlt.Name = "SnapStyle_SnapOffAlt";
            this.SnapStyle_SnapOffAlt.Size = new System.Drawing.Size(182, 24);
            this.SnapStyle_SnapOffAlt.TabIndex = 2;
            this.SnapStyle_SnapOffAlt.TabStop = true;
            this.SnapStyle_SnapOffAlt.Text = "Hold alt to ignore snapping";
            this.SnapStyle_SnapOffAlt.UseVisualStyleBackColor = true;
            // 
            // SnapStyle_SnapOnAlt
            // 
            this.SnapStyle_SnapOnAlt.Location = new System.Drawing.Point(14, 49);
            this.SnapStyle_SnapOnAlt.Name = "SnapStyle_SnapOnAlt";
            this.SnapStyle_SnapOnAlt.Size = new System.Drawing.Size(213, 24);
            this.SnapStyle_SnapOnAlt.TabIndex = 2;
            this.SnapStyle_SnapOnAlt.Text = "Ignore snapping unless alt is pressed";
            this.SnapStyle_SnapOnAlt.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.HideGridOn);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.HideGridLimit);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.HideGridFactor);
            this.groupBox5.Controls.Add(this.DefaultGridSize);
            this.groupBox5.Location = new System.Drawing.Point(6, 180);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(409, 85);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Grid Settings";
            // 
            // HideGridOn
            // 
            this.HideGridOn.Checked = true;
            this.HideGridOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HideGridOn.Location = new System.Drawing.Point(17, 52);
            this.HideGridOn.Name = "HideGridOn";
            this.HideGridOn.Size = new System.Drawing.Size(127, 24);
            this.HideGridOn.TabIndex = 5;
            this.HideGridOn.Text = "Hide grid smaller than";
            this.HideGridOn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(14, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Default grid size:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HideGridLimit
            // 
            this.HideGridLimit.Location = new System.Drawing.Point(145, 54);
            this.HideGridLimit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.HideGridLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HideGridLimit.Name = "HideGridLimit";
            this.HideGridLimit.Size = new System.Drawing.Size(34, 20);
            this.HideGridLimit.TabIndex = 2;
            this.HideGridLimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(185, 54);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(110, 20);
            this.label13.TabIndex = 3;
            this.label13.Text = "pixel(s), by a factor of";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HideGridFactor
            // 
            this.HideGridFactor.Items.Add("64");
            this.HideGridFactor.Items.Add("32");
            this.HideGridFactor.Items.Add("16");
            this.HideGridFactor.Items.Add("8");
            this.HideGridFactor.Items.Add("4");
            this.HideGridFactor.Items.Add("2");
            this.HideGridFactor.Location = new System.Drawing.Point(295, 54);
            this.HideGridFactor.Name = "HideGridFactor";
            this.HideGridFactor.Size = new System.Drawing.Size(38, 20);
            this.HideGridFactor.TabIndex = 0;
            this.HideGridFactor.Text = "8";
            // 
            // DefaultGridSize
            // 
            this.DefaultGridSize.Items.Add("1024");
            this.DefaultGridSize.Items.Add("512");
            this.DefaultGridSize.Items.Add("256");
            this.DefaultGridSize.Items.Add("128");
            this.DefaultGridSize.Items.Add("64");
            this.DefaultGridSize.Items.Add("32");
            this.DefaultGridSize.Items.Add("16");
            this.DefaultGridSize.Items.Add("8");
            this.DefaultGridSize.Items.Add("4");
            this.DefaultGridSize.Items.Add("2");
            this.DefaultGridSize.Items.Add("1");
            this.DefaultGridSize.Location = new System.Drawing.Point(105, 25);
            this.DefaultGridSize.Name = "DefaultGridSize";
            this.DefaultGridSize.SelectedIndex = 4;
            this.DefaultGridSize.Size = new System.Drawing.Size(49, 20);
            this.DefaultGridSize.TabIndex = 0;
            this.DefaultGridSize.Text = "64";
            // 
            // tab3DViews
            // 
            this.tab3DViews.Controls.Add(this.groupBox11);
            this.tab3DViews.Controls.Add(this.groupBox12);
            this.tab3DViews.Controls.Add(this.groupBox13);
            this.tab3DViews.Controls.Add(this.groupBox14);
            this.tab3DViews.Location = new System.Drawing.Point(4, 22);
            this.tab3DViews.Name = "tab3DViews";
            this.tab3DViews.Padding = new System.Windows.Forms.Padding(3);
            this.tab3DViews.Size = new System.Drawing.Size(736, 511);
            this.tab3DViews.TabIndex = 4;
            this.tab3DViews.Text = "3D Views";
            this.tab3DViews.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.o_3DInvertX);
            this.groupBox11.Controls.Add(this.o_3DInvertY);
            this.groupBox11.Location = new System.Drawing.Point(438, 6);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(146, 98);
            this.groupBox11.TabIndex = 3;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Mouselook";
            // 
            // o_3DInvertX
            // 
            this.o_3DInvertX.Location = new System.Drawing.Point(27, 49);
            this.o_3DInvertX.Name = "o_3DInvertX";
            this.o_3DInvertX.Size = new System.Drawing.Size(104, 24);
            this.o_3DInvertX.TabIndex = 0;
            this.o_3DInvertX.Text = "Invert X Axis";
            this.o_3DInvertX.UseVisualStyleBackColor = true;
            // 
            // o_3DInvertY
            // 
            this.o_3DInvertY.Location = new System.Drawing.Point(27, 19);
            this.o_3DInvertY.Name = "o_3DInvertY";
            this.o_3DInvertY.Size = new System.Drawing.Size(104, 24);
            this.o_3DInvertY.TabIndex = 0;
            this.o_3DInvertY.Text = "Invert Y Axis";
            this.o_3DInvertY.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.o_3DTimeToTopSpeed);
            this.groupBox12.Controls.Add(this.label14);
            this.groupBox12.Location = new System.Drawing.Point(6, 214);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(426, 98);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Time to Top Speed";
            // 
            // o_3DTimeToTopSpeed
            // 
            this.o_3DTimeToTopSpeed.AutoSize = false;
            this.o_3DTimeToTopSpeed.BackColor = System.Drawing.SystemColors.Window;
            this.o_3DTimeToTopSpeed.Location = new System.Drawing.Point(6, 20);
            this.o_3DTimeToTopSpeed.Maximum = 50;
            this.o_3DTimeToTopSpeed.Name = "o_3DTimeToTopSpeed";
            this.o_3DTimeToTopSpeed.Size = new System.Drawing.Size(414, 42);
            this.o_3DTimeToTopSpeed.TabIndex = 0;
            this.o_3DTimeToTopSpeed.TickFrequency = 10000;
            this.o_3DTimeToTopSpeed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.o_3DTimeToTopSpeed.Value = 5;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(6, 65);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(414, 23);
            this.label14.TabIndex = 1;
            this.label14.Text = "0.5 sec";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.o_3DForwardSpeed);
            this.groupBox13.Controls.Add(this.label15);
            this.groupBox13.Location = new System.Drawing.Point(6, 110);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(426, 98);
            this.groupBox13.TabIndex = 2;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Forward Speed";
            // 
            // o_3DForwardSpeed
            // 
            this.o_3DForwardSpeed.AutoSize = false;
            this.o_3DForwardSpeed.BackColor = System.Drawing.SystemColors.Window;
            this.o_3DForwardSpeed.Location = new System.Drawing.Point(6, 20);
            this.o_3DForwardSpeed.Maximum = 5000;
            this.o_3DForwardSpeed.Minimum = 100;
            this.o_3DForwardSpeed.Name = "o_3DForwardSpeed";
            this.o_3DForwardSpeed.Size = new System.Drawing.Size(414, 42);
            this.o_3DForwardSpeed.TabIndex = 0;
            this.o_3DForwardSpeed.TickFrequency = 10000;
            this.o_3DForwardSpeed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.o_3DForwardSpeed.Value = 1000;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(6, 65);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(414, 23);
            this.label15.TabIndex = 1;
            this.label15.Text = "1000 units/sec";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.o_3DBackClippingPane);
            this.groupBox14.Controls.Add(this.label16);
            this.groupBox14.Location = new System.Drawing.Point(6, 6);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(426, 98);
            this.groupBox14.TabIndex = 2;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Back Clipping Pane";
            // 
            // o_3DBackClippingPane
            // 
            this.o_3DBackClippingPane.AutoSize = false;
            this.o_3DBackClippingPane.BackColor = System.Drawing.SystemColors.Window;
            this.o_3DBackClippingPane.Location = new System.Drawing.Point(6, 20);
            this.o_3DBackClippingPane.Maximum = 10000;
            this.o_3DBackClippingPane.Minimum = 2000;
            this.o_3DBackClippingPane.Name = "o_3DBackClippingPane";
            this.o_3DBackClippingPane.Size = new System.Drawing.Size(414, 42);
            this.o_3DBackClippingPane.TabIndex = 0;
            this.o_3DBackClippingPane.TickFrequency = 10000;
            this.o_3DBackClippingPane.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.o_3DBackClippingPane.Value = 5000;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(6, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(414, 23);
            this.label16.TabIndex = 1;
            this.label16.Text = "4000";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabGame
            // 
            this.tabGame.Controls.Add(this.tabGameSubTabs);
            this.tabGame.Controls.Add(this.tree_games);
            this.tabGame.Controls.Add(this.btnGameRemove);
            this.tabGame.Controls.Add(this.btnGameAdd);
            this.tabGame.Location = new System.Drawing.Point(4, 22);
            this.tabGame.Name = "tabGame";
            this.tabGame.Padding = new System.Windows.Forms.Padding(3);
            this.tabGame.Size = new System.Drawing.Size(736, 511);
            this.tabGame.TabIndex = 2;
            this.tabGame.Text = "Game Configurations";
            this.tabGame.UseVisualStyleBackColor = true;
            // 
            // tabGameSubTabs
            // 
            this.tabGameSubTabs.Controls.Add(this.tabConfigDirectories);
            this.tabGameSubTabs.Controls.Add(this.tabConfigEntities);
            this.tabGameSubTabs.Controls.Add(this.tabConfigTextures);
            this.tabGameSubTabs.ItemSize = new System.Drawing.Size(58, 18);
            this.tabGameSubTabs.Location = new System.Drawing.Point(245, 6);
            this.tabGameSubTabs.Name = "tabGameSubTabs";
            this.tabGameSubTabs.SelectedIndex = 0;
            this.tabGameSubTabs.Size = new System.Drawing.Size(477, 423);
            this.tabGameSubTabs.TabIndex = 23;
            this.tabGameSubTabs.Visible = false;
            // 
            // tabConfigDirectories
            // 
            this.tabConfigDirectories.Controls.Add(this.btnGameChangeName);
            this.tabConfigDirectories.Controls.Add(this.grpConfigGame);
            this.tabConfigDirectories.Controls.Add(this.lblGameName);
            this.tabConfigDirectories.Controls.Add(this.cmbGameBuild);
            this.tabConfigDirectories.Controls.Add(this.grpConfigSaving);
            this.tabConfigDirectories.Controls.Add(this.lblGameBuild);
            this.tabConfigDirectories.Controls.Add(this.txtGameName);
            this.tabConfigDirectories.Controls.Add(this.cmbGameEngine);
            this.tabConfigDirectories.Controls.Add(this.lblGameEngine);
            this.tabConfigDirectories.Location = new System.Drawing.Point(4, 22);
            this.tabConfigDirectories.Name = "tabConfigDirectories";
            this.tabConfigDirectories.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigDirectories.Size = new System.Drawing.Size(469, 397);
            this.tabConfigDirectories.TabIndex = 0;
            this.tabConfigDirectories.Text = "General";
            this.tabConfigDirectories.UseVisualStyleBackColor = true;
            // 
            // btnGameChangeName
            // 
            this.btnGameChangeName.Location = new System.Drawing.Point(220, 9);
            this.btnGameChangeName.Name = "btnGameChangeName";
            this.btnGameChangeName.Size = new System.Drawing.Size(93, 20);
            this.btnGameChangeName.TabIndex = 21;
            this.btnGameChangeName.Text = "Change Name";
            this.btnGameChangeName.UseVisualStyleBackColor = true;
            // 
            // grpConfigGame
            // 
            this.grpConfigGame.Controls.Add(this.txtGameWONDir);
            this.grpConfigGame.Controls.Add(this.lblGameWONDir);
            this.grpConfigGame.Controls.Add(this.btnGameDirBrowse);
            this.grpConfigGame.Controls.Add(this.lblGameSteamDir);
            this.grpConfigGame.Controls.Add(this.cmbGameSteamDir);
            this.grpConfigGame.Controls.Add(this.lblGameMod);
            this.grpConfigGame.Controls.Add(this.cmbGameMod);
            this.grpConfigGame.Location = new System.Drawing.Point(6, 89);
            this.grpConfigGame.Name = "grpConfigGame";
            this.grpConfigGame.Size = new System.Drawing.Size(445, 111);
            this.grpConfigGame.TabIndex = 19;
            this.grpConfigGame.TabStop = false;
            this.grpConfigGame.Text = "Game";
            // 
            // txtGameWONDir
            // 
            this.txtGameWONDir.Location = new System.Drawing.Point(68, 16);
            this.txtGameWONDir.Name = "txtGameWONDir";
            this.txtGameWONDir.Size = new System.Drawing.Size(269, 20);
            this.txtGameWONDir.TabIndex = 5;
            this.txtGameWONDir.Text = "(WON only) example: C:\\Sierra\\Half-Life";
            // 
            // lblGameWONDir
            // 
            this.lblGameWONDir.Location = new System.Drawing.Point(6, 16);
            this.lblGameWONDir.Name = "lblGameWONDir";
            this.lblGameWONDir.Size = new System.Drawing.Size(56, 20);
            this.lblGameWONDir.TabIndex = 6;
            this.lblGameWONDir.Text = "Game Dir";
            this.lblGameWONDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGameDirBrowse
            // 
            this.btnGameDirBrowse.Location = new System.Drawing.Point(343, 15);
            this.btnGameDirBrowse.Name = "btnGameDirBrowse";
            this.btnGameDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnGameDirBrowse.TabIndex = 8;
            this.btnGameDirBrowse.Text = "Browse...";
            this.btnGameDirBrowse.UseVisualStyleBackColor = true;
            // 
            // lblGameSteamDir
            // 
            this.lblGameSteamDir.Location = new System.Drawing.Point(16, 43);
            this.lblGameSteamDir.Name = "lblGameSteamDir";
            this.lblGameSteamDir.Size = new System.Drawing.Size(45, 20);
            this.lblGameSteamDir.TabIndex = 9;
            this.lblGameSteamDir.Text = "Game";
            this.lblGameSteamDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbGameSteamDir
            // 
            this.cmbGameSteamDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameSteamDir.FormattingEnabled = true;
            this.cmbGameSteamDir.Items.AddRange(new object[] {
            "(Steam only) Half-Life",
            "Counter-Strike"});
            this.cmbGameSteamDir.Location = new System.Drawing.Point(67, 42);
            this.cmbGameSteamDir.Name = "cmbGameSteamDir";
            this.cmbGameSteamDir.Size = new System.Drawing.Size(240, 21);
            this.cmbGameSteamDir.TabIndex = 10;
            // 
            // lblGameMod
            // 
            this.lblGameMod.Location = new System.Drawing.Point(16, 70);
            this.lblGameMod.Name = "lblGameMod";
            this.lblGameMod.Size = new System.Drawing.Size(45, 20);
            this.lblGameMod.TabIndex = 9;
            this.lblGameMod.Text = "Mod";
            this.lblGameMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbGameMod
            // 
            this.cmbGameMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameMod.FormattingEnabled = true;
            this.cmbGameMod.Items.AddRange(new object[] {
            "Valve"});
            this.cmbGameMod.Location = new System.Drawing.Point(67, 69);
            this.cmbGameMod.Name = "cmbGameMod";
            this.cmbGameMod.Size = new System.Drawing.Size(240, 21);
            this.cmbGameMod.TabIndex = 10;
            // 
            // lblGameName
            // 
            this.lblGameName.Location = new System.Drawing.Point(6, 9);
            this.lblGameName.Name = "lblGameName";
            this.lblGameName.Size = new System.Drawing.Size(69, 20);
            this.lblGameName.TabIndex = 6;
            this.lblGameName.Text = "Config Name";
            this.lblGameName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbGameBuild
            // 
            this.cmbGameBuild.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameBuild.FormattingEnabled = true;
            this.cmbGameBuild.Location = new System.Drawing.Point(81, 62);
            this.cmbGameBuild.Name = "cmbGameBuild";
            this.cmbGameBuild.Size = new System.Drawing.Size(121, 21);
            this.cmbGameBuild.TabIndex = 10;
            // 
            // grpConfigSaving
            // 
            this.grpConfigSaving.Controls.Add(this.lblGameMapSaveDir);
            this.grpConfigSaving.Controls.Add(this.chkGameEnableAutosave);
            this.grpConfigSaving.Controls.Add(this.txtGameMapDir);
            this.grpConfigSaving.Controls.Add(this.btnGameMapDirBrowse);
            this.grpConfigSaving.Controls.Add(this.txtGameAutosaveDir);
            this.grpConfigSaving.Controls.Add(this.lblGameAutosaveDir);
            this.grpConfigSaving.Controls.Add(this.btnGameAutosaveDirBrowse);
            this.grpConfigSaving.Controls.Add(this.chkGameMapDiffAutosaveDir);
            this.grpConfigSaving.Location = new System.Drawing.Point(6, 206);
            this.grpConfigSaving.Name = "grpConfigSaving";
            this.grpConfigSaving.Size = new System.Drawing.Size(445, 179);
            this.grpConfigSaving.TabIndex = 20;
            this.grpConfigSaving.TabStop = false;
            this.grpConfigSaving.Text = "Saving";
            // 
            // lblGameMapSaveDir
            // 
            this.lblGameMapSaveDir.Location = new System.Drawing.Point(9, 27);
            this.lblGameMapSaveDir.Name = "lblGameMapSaveDir";
            this.lblGameMapSaveDir.Size = new System.Drawing.Size(80, 20);
            this.lblGameMapSaveDir.TabIndex = 6;
            this.lblGameMapSaveDir.Text = "Map Save Dir";
            this.lblGameMapSaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkGameEnableAutosave
            // 
            this.chkGameEnableAutosave.Checked = true;
            this.chkGameEnableAutosave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGameEnableAutosave.Location = new System.Drawing.Point(95, 53);
            this.chkGameEnableAutosave.Name = "chkGameEnableAutosave";
            this.chkGameEnableAutosave.Size = new System.Drawing.Size(225, 24);
            this.chkGameEnableAutosave.TabIndex = 18;
            this.chkGameEnableAutosave.Text = "Enable autosave for this config";
            this.chkGameEnableAutosave.UseVisualStyleBackColor = true;
            // 
            // txtGameMapDir
            // 
            this.txtGameMapDir.Location = new System.Drawing.Point(95, 27);
            this.txtGameMapDir.Name = "txtGameMapDir";
            this.txtGameMapDir.Size = new System.Drawing.Size(225, 20);
            this.txtGameMapDir.TabIndex = 5;
            this.txtGameMapDir.Text = "Default folder to save VMF/RMF files";
            // 
            // btnGameMapDirBrowse
            // 
            this.btnGameMapDirBrowse.Location = new System.Drawing.Point(326, 25);
            this.btnGameMapDirBrowse.Name = "btnGameMapDirBrowse";
            this.btnGameMapDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnGameMapDirBrowse.TabIndex = 8;
            this.btnGameMapDirBrowse.Text = "Browse...";
            this.btnGameMapDirBrowse.UseVisualStyleBackColor = true;
            // 
            // txtGameAutosaveDir
            // 
            this.txtGameAutosaveDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtGameAutosaveDir.Location = new System.Drawing.Point(95, 109);
            this.txtGameAutosaveDir.Name = "txtGameAutosaveDir";
            this.txtGameAutosaveDir.Size = new System.Drawing.Size(225, 20);
            this.txtGameAutosaveDir.TabIndex = 11;
            this.txtGameAutosaveDir.Text = "Folder to put autosaves in";
            // 
            // lblGameAutosaveDir
            // 
            this.lblGameAutosaveDir.Location = new System.Drawing.Point(9, 109);
            this.lblGameAutosaveDir.Name = "lblGameAutosaveDir";
            this.lblGameAutosaveDir.Size = new System.Drawing.Size(80, 20);
            this.lblGameAutosaveDir.TabIndex = 12;
            this.lblGameAutosaveDir.Text = "Autosave Dir";
            this.lblGameAutosaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGameAutosaveDirBrowse
            // 
            this.btnGameAutosaveDirBrowse.Location = new System.Drawing.Point(326, 107);
            this.btnGameAutosaveDirBrowse.Name = "btnGameAutosaveDirBrowse";
            this.btnGameAutosaveDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnGameAutosaveDirBrowse.TabIndex = 13;
            this.btnGameAutosaveDirBrowse.Text = "Browse...";
            this.btnGameAutosaveDirBrowse.UseVisualStyleBackColor = true;
            // 
            // chkGameMapDiffAutosaveDir
            // 
            this.chkGameMapDiffAutosaveDir.Checked = true;
            this.chkGameMapDiffAutosaveDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGameMapDiffAutosaveDir.Location = new System.Drawing.Point(95, 83);
            this.chkGameMapDiffAutosaveDir.Name = "chkGameMapDiffAutosaveDir";
            this.chkGameMapDiffAutosaveDir.Size = new System.Drawing.Size(225, 20);
            this.chkGameMapDiffAutosaveDir.TabIndex = 14;
            this.chkGameMapDiffAutosaveDir.Text = "Use a different directory for autosaves";
            this.chkGameMapDiffAutosaveDir.UseVisualStyleBackColor = true;
            // 
            // lblGameBuild
            // 
            this.lblGameBuild.Location = new System.Drawing.Point(13, 61);
            this.lblGameBuild.Name = "lblGameBuild";
            this.lblGameBuild.Size = new System.Drawing.Size(62, 20);
            this.lblGameBuild.TabIndex = 9;
            this.lblGameBuild.Text = "Build Profile";
            this.lblGameBuild.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGameName
            // 
            this.txtGameName.Location = new System.Drawing.Point(81, 9);
            this.txtGameName.Name = "txtGameName";
            this.txtGameName.Size = new System.Drawing.Size(133, 20);
            this.txtGameName.TabIndex = 5;
            this.txtGameName.Text = "A_Config";
            // 
            // cmbGameEngine
            // 
            this.cmbGameEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameEngine.FormattingEnabled = true;
            this.cmbGameEngine.Items.AddRange(new object[] {
            "Goldsource (WON)",
            "Goldsource (Steam)",
            "Source"});
            this.cmbGameEngine.Location = new System.Drawing.Point(81, 35);
            this.cmbGameEngine.Name = "cmbGameEngine";
            this.cmbGameEngine.Size = new System.Drawing.Size(121, 21);
            this.cmbGameEngine.TabIndex = 7;
            // 
            // lblGameEngine
            // 
            this.lblGameEngine.Location = new System.Drawing.Point(30, 36);
            this.lblGameEngine.Name = "lblGameEngine";
            this.lblGameEngine.Size = new System.Drawing.Size(45, 20);
            this.lblGameEngine.TabIndex = 6;
            this.lblGameEngine.Text = "Engine";
            this.lblGameEngine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabConfigEntities
            // 
            this.tabConfigEntities.Controls.Add(this.lblGameFGD);
            this.tabConfigEntities.Controls.Add(this.btnGameAddFGD);
            this.tabConfigEntities.Controls.Add(this.lblConfigBrushEnt);
            this.tabConfigEntities.Controls.Add(this.btnGameRemoveFGD);
            this.tabConfigEntities.Controls.Add(this.lblConfigPointEnt);
            this.tabConfigEntities.Controls.Add(this.lstGameFGD);
            this.tabConfigEntities.Controls.Add(this.cmbGameBrushEnt);
            this.tabConfigEntities.Controls.Add(this.cmbGamePointEnt);
            this.tabConfigEntities.Location = new System.Drawing.Point(4, 22);
            this.tabConfigEntities.Name = "tabConfigEntities";
            this.tabConfigEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigEntities.Size = new System.Drawing.Size(469, 397);
            this.tabConfigEntities.TabIndex = 1;
            this.tabConfigEntities.Text = "Entities";
            this.tabConfigEntities.UseVisualStyleBackColor = true;
            // 
            // lblGameFGD
            // 
            this.lblGameFGD.Location = new System.Drawing.Point(15, 3);
            this.lblGameFGD.Name = "lblGameFGD";
            this.lblGameFGD.Size = new System.Drawing.Size(93, 20);
            this.lblGameFGD.TabIndex = 9;
            this.lblGameFGD.Text = "Game Data Files";
            this.lblGameFGD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnGameAddFGD
            // 
            this.btnGameAddFGD.Location = new System.Drawing.Point(389, 26);
            this.btnGameAddFGD.Name = "btnGameAddFGD";
            this.btnGameAddFGD.Size = new System.Drawing.Size(74, 23);
            this.btnGameAddFGD.TabIndex = 1;
            this.btnGameAddFGD.Text = "Add...";
            this.btnGameAddFGD.UseVisualStyleBackColor = true;
            // 
            // lblConfigBrushEnt
            // 
            this.lblConfigBrushEnt.Location = new System.Drawing.Point(66, 220);
            this.lblConfigBrushEnt.Name = "lblConfigBrushEnt";
            this.lblConfigBrushEnt.Size = new System.Drawing.Size(112, 20);
            this.lblConfigBrushEnt.TabIndex = 9;
            this.lblConfigBrushEnt.Text = "Default Brush Entity";
            this.lblConfigBrushEnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGameRemoveFGD
            // 
            this.btnGameRemoveFGD.Location = new System.Drawing.Point(389, 55);
            this.btnGameRemoveFGD.Name = "btnGameRemoveFGD";
            this.btnGameRemoveFGD.Size = new System.Drawing.Size(74, 23);
            this.btnGameRemoveFGD.TabIndex = 3;
            this.btnGameRemoveFGD.Text = "Remove";
            this.btnGameRemoveFGD.UseVisualStyleBackColor = true;
            // 
            // lblConfigPointEnt
            // 
            this.lblConfigPointEnt.Location = new System.Drawing.Point(66, 193);
            this.lblConfigPointEnt.Name = "lblConfigPointEnt";
            this.lblConfigPointEnt.Size = new System.Drawing.Size(112, 20);
            this.lblConfigPointEnt.TabIndex = 9;
            this.lblConfigPointEnt.Text = "Default Point Entity";
            this.lblConfigPointEnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lstGameFGD
            // 
            this.lstGameFGD.FormattingEnabled = true;
            this.lstGameFGD.Items.AddRange(new object[] {
            "half-life.fgd",
            "etc."});
            this.lstGameFGD.Location = new System.Drawing.Point(15, 26);
            this.lstGameFGD.Name = "lstGameFGD";
            this.lstGameFGD.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstGameFGD.Size = new System.Drawing.Size(368, 160);
            this.lstGameFGD.TabIndex = 16;
            // 
            // cmbGameBrushEnt
            // 
            this.cmbGameBrushEnt.DropDownHeight = 300;
            this.cmbGameBrushEnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameBrushEnt.FormattingEnabled = true;
            this.cmbGameBrushEnt.IntegralHeight = false;
            this.cmbGameBrushEnt.Items.AddRange(new object[] {
            "Valve"});
            this.cmbGameBrushEnt.Location = new System.Drawing.Point(184, 219);
            this.cmbGameBrushEnt.Name = "cmbGameBrushEnt";
            this.cmbGameBrushEnt.Size = new System.Drawing.Size(199, 21);
            this.cmbGameBrushEnt.TabIndex = 10;
            // 
            // cmbGamePointEnt
            // 
            this.cmbGamePointEnt.DropDownHeight = 300;
            this.cmbGamePointEnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGamePointEnt.FormattingEnabled = true;
            this.cmbGamePointEnt.IntegralHeight = false;
            this.cmbGamePointEnt.Items.AddRange(new object[] {
            "Valve"});
            this.cmbGamePointEnt.Location = new System.Drawing.Point(184, 192);
            this.cmbGamePointEnt.Name = "cmbGamePointEnt";
            this.cmbGamePointEnt.Size = new System.Drawing.Size(199, 21);
            this.cmbGamePointEnt.TabIndex = 10;
            // 
            // tabConfigTextures
            // 
            this.tabConfigTextures.Controls.Add(this.lblGameWAD);
            this.tabConfigTextures.Controls.Add(this.nudGameLightmapScale);
            this.tabConfigTextures.Controls.Add(this.lblConfigLightmapScale);
            this.tabConfigTextures.Controls.Add(this.btnGameAddWAD);
            this.tabConfigTextures.Controls.Add(this.nudGameTextureScale);
            this.tabConfigTextures.Controls.Add(this.lstGameWAD);
            this.tabConfigTextures.Controls.Add(this.lblConfigTextureScale);
            this.tabConfigTextures.Controls.Add(this.btnGameRemoveWAD);
            this.tabConfigTextures.Location = new System.Drawing.Point(4, 22);
            this.tabConfigTextures.Name = "tabConfigTextures";
            this.tabConfigTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigTextures.Size = new System.Drawing.Size(469, 397);
            this.tabConfigTextures.TabIndex = 2;
            this.tabConfigTextures.Text = "Textures";
            this.tabConfigTextures.UseVisualStyleBackColor = true;
            // 
            // lblGameWAD
            // 
            this.lblGameWAD.Location = new System.Drawing.Point(15, 3);
            this.lblGameWAD.Name = "lblGameWAD";
            this.lblGameWAD.Size = new System.Drawing.Size(80, 20);
            this.lblGameWAD.TabIndex = 9;
            this.lblGameWAD.Text = "WAD Textures";
            this.lblGameWAD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudGameLightmapScale
            // 
            this.nudGameLightmapScale.Location = new System.Drawing.Point(332, 218);
            this.nudGameLightmapScale.Name = "nudGameLightmapScale";
            this.nudGameLightmapScale.Size = new System.Drawing.Size(51, 20);
            this.nudGameLightmapScale.TabIndex = 17;
            this.nudGameLightmapScale.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // lblConfigLightmapScale
            // 
            this.lblConfigLightmapScale.Location = new System.Drawing.Point(209, 218);
            this.lblConfigLightmapScale.Name = "lblConfigLightmapScale";
            this.lblConfigLightmapScale.Size = new System.Drawing.Size(117, 20);
            this.lblConfigLightmapScale.TabIndex = 9;
            this.lblConfigLightmapScale.Text = "Default Lightmap Scale";
            this.lblConfigLightmapScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGameAddWAD
            // 
            this.btnGameAddWAD.Location = new System.Drawing.Point(389, 26);
            this.btnGameAddWAD.Name = "btnGameAddWAD";
            this.btnGameAddWAD.Size = new System.Drawing.Size(74, 23);
            this.btnGameAddWAD.TabIndex = 1;
            this.btnGameAddWAD.Text = "Add...";
            this.btnGameAddWAD.UseVisualStyleBackColor = true;
            // 
            // nudGameTextureScale
            // 
            this.nudGameTextureScale.DecimalPlaces = 2;
            this.nudGameTextureScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudGameTextureScale.Location = new System.Drawing.Point(332, 192);
            this.nudGameTextureScale.Name = "nudGameTextureScale";
            this.nudGameTextureScale.Size = new System.Drawing.Size(51, 20);
            this.nudGameTextureScale.TabIndex = 17;
            this.nudGameTextureScale.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            // 
            // lstGameWAD
            // 
            this.lstGameWAD.FormattingEnabled = true;
            this.lstGameWAD.Items.AddRange(new object[] {
            "(Goldsource only)",
            "halflife.wad",
            "etc."});
            this.lstGameWAD.Location = new System.Drawing.Point(15, 26);
            this.lstGameWAD.Name = "lstGameWAD";
            this.lstGameWAD.Size = new System.Drawing.Size(368, 160);
            this.lstGameWAD.TabIndex = 16;
            // 
            // lblConfigTextureScale
            // 
            this.lblConfigTextureScale.Location = new System.Drawing.Point(209, 192);
            this.lblConfigTextureScale.Name = "lblConfigTextureScale";
            this.lblConfigTextureScale.Size = new System.Drawing.Size(117, 20);
            this.lblConfigTextureScale.TabIndex = 9;
            this.lblConfigTextureScale.Text = "Default Texture Scale";
            this.lblConfigTextureScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGameRemoveWAD
            // 
            this.btnGameRemoveWAD.Location = new System.Drawing.Point(389, 55);
            this.btnGameRemoveWAD.Name = "btnGameRemoveWAD";
            this.btnGameRemoveWAD.Size = new System.Drawing.Size(74, 23);
            this.btnGameRemoveWAD.TabIndex = 3;
            this.btnGameRemoveWAD.Text = "Remove";
            this.btnGameRemoveWAD.UseVisualStyleBackColor = true;
            // 
            // tree_games
            // 
            this.tree_games.Location = new System.Drawing.Point(6, 6);
            this.tree_games.Name = "tree_games";
            treeNode11.Name = "nodeWHL";
            treeNode11.Text = "WON Goldsource";
            treeNode12.Name = "nodeSHL";
            treeNode12.Text = "Steam Goldsource";
            treeNode13.Name = "nodeSource";
            treeNode13.Text = "Source";
            this.tree_games.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12,
            treeNode13});
            this.tree_games.Size = new System.Drawing.Size(154, 448);
            this.tree_games.TabIndex = 0;
            // 
            // btnGameRemove
            // 
            this.btnGameRemove.Location = new System.Drawing.Point(166, 35);
            this.btnGameRemove.Name = "btnGameRemove";
            this.btnGameRemove.Size = new System.Drawing.Size(73, 23);
            this.btnGameRemove.TabIndex = 3;
            this.btnGameRemove.Text = "Remove";
            this.btnGameRemove.UseVisualStyleBackColor = true;
            // 
            // btnGameAdd
            // 
            this.btnGameAdd.Location = new System.Drawing.Point(166, 6);
            this.btnGameAdd.Name = "btnGameAdd";
            this.btnGameAdd.Size = new System.Drawing.Size(73, 23);
            this.btnGameAdd.TabIndex = 1;
            this.btnGameAdd.Text = "Add New";
            this.btnGameAdd.UseVisualStyleBackColor = true;
            // 
            // tabBuild
            // 
            this.tabBuild.Controls.Add(this.tabBuildSubTabs);
            this.tabBuild.Controls.Add(this.btnBuildRemove);
            this.tabBuild.Controls.Add(this.btnBuildAdd);
            this.tabBuild.Controls.Add(this.tree_builds);
            this.tabBuild.Location = new System.Drawing.Point(4, 22);
            this.tabBuild.Name = "tabBuild";
            this.tabBuild.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuild.Size = new System.Drawing.Size(736, 511);
            this.tabBuild.TabIndex = 3;
            this.tabBuild.Text = "Build Programs";
            this.tabBuild.UseVisualStyleBackColor = true;
            // 
            // tabBuildSubTabs
            // 
            this.tabBuildSubTabs.Controls.Add(this.tabBuildGeneral);
            this.tabBuildSubTabs.Controls.Add(this.tabBuildExecutables);
            this.tabBuildSubTabs.Controls.Add(this.tabBuildPostCompile);
            this.tabBuildSubTabs.Controls.Add(this.tabBuildAdvanced);
            this.tabBuildSubTabs.Location = new System.Drawing.Point(245, 6);
            this.tabBuildSubTabs.Name = "tabBuildSubTabs";
            this.tabBuildSubTabs.SelectedIndex = 0;
            this.tabBuildSubTabs.Size = new System.Drawing.Size(477, 499);
            this.tabBuildSubTabs.TabIndex = 29;
            this.tabBuildSubTabs.Visible = false;
            // 
            // tabBuildGeneral
            // 
            this.tabBuildGeneral.Controls.Add(this.btnBuildChangeName);
            this.tabBuildGeneral.Controls.Add(this.lblBuildName);
            this.tabBuildGeneral.Controls.Add(this.txtBuildName);
            this.tabBuildGeneral.Controls.Add(this.lblBuildEngine);
            this.tabBuildGeneral.Controls.Add(this.cmbBuildEngine);
            this.tabBuildGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabBuildGeneral.Name = "tabBuildGeneral";
            this.tabBuildGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildGeneral.Size = new System.Drawing.Size(469, 473);
            this.tabBuildGeneral.TabIndex = 0;
            this.tabBuildGeneral.Text = "General";
            this.tabBuildGeneral.UseVisualStyleBackColor = true;
            // 
            // btnBuildChangeName
            // 
            this.btnBuildChangeName.Location = new System.Drawing.Point(223, 6);
            this.btnBuildChangeName.Name = "btnBuildChangeName";
            this.btnBuildChangeName.Size = new System.Drawing.Size(90, 20);
            this.btnBuildChangeName.TabIndex = 20;
            this.btnBuildChangeName.Text = "Change Name";
            this.btnBuildChangeName.UseVisualStyleBackColor = true;
            // 
            // lblBuildName
            // 
            this.lblBuildName.Location = new System.Drawing.Point(9, 6);
            this.lblBuildName.Name = "lblBuildName";
            this.lblBuildName.Size = new System.Drawing.Size(69, 20);
            this.lblBuildName.TabIndex = 18;
            this.lblBuildName.Text = "Config Name";
            this.lblBuildName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBuildName
            // 
            this.txtBuildName.Location = new System.Drawing.Point(84, 6);
            this.txtBuildName.Name = "txtBuildName";
            this.txtBuildName.Size = new System.Drawing.Size(133, 20);
            this.txtBuildName.TabIndex = 14;
            this.txtBuildName.Text = "ZHLT";
            // 
            // lblBuildEngine
            // 
            this.lblBuildEngine.Location = new System.Drawing.Point(33, 33);
            this.lblBuildEngine.Name = "lblBuildEngine";
            this.lblBuildEngine.Size = new System.Drawing.Size(45, 20);
            this.lblBuildEngine.TabIndex = 16;
            this.lblBuildEngine.Text = "Engine";
            this.lblBuildEngine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbBuildEngine
            // 
            this.cmbBuildEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildEngine.FormattingEnabled = true;
            this.cmbBuildEngine.Items.AddRange(new object[] {
            "Goldsource",
            "Source"});
            this.cmbBuildEngine.Location = new System.Drawing.Point(84, 32);
            this.cmbBuildEngine.Name = "cmbBuildEngine";
            this.cmbBuildEngine.Size = new System.Drawing.Size(121, 21);
            this.cmbBuildEngine.TabIndex = 19;
            // 
            // tabBuildExecutables
            // 
            this.tabBuildExecutables.Controls.Add(this.lstBuildPresets);
            this.tabBuildExecutables.Controls.Add(this.lblBuildExeFolder);
            this.tabBuildExecutables.Controls.Add(this.lblBuildBSP);
            this.tabBuildExecutables.Controls.Add(this.txtBuildExeFolder);
            this.tabBuildExecutables.Controls.Add(this.lblBuildCSG);
            this.tabBuildExecutables.Controls.Add(this.cmbBuildRAD);
            this.tabBuildExecutables.Controls.Add(this.cmbBuildBSP);
            this.tabBuildExecutables.Controls.Add(this.lblBuildDetectedPresets);
            this.tabBuildExecutables.Controls.Add(this.lblBuildVIS);
            this.tabBuildExecutables.Controls.Add(this.cmbBuildVIS);
            this.tabBuildExecutables.Controls.Add(this.cmbBuildCSG);
            this.tabBuildExecutables.Controls.Add(this.lblBuildRAD);
            this.tabBuildExecutables.Controls.Add(this.btnBuildExeFolderBrowse);
            this.tabBuildExecutables.Location = new System.Drawing.Point(4, 22);
            this.tabBuildExecutables.Name = "tabBuildExecutables";
            this.tabBuildExecutables.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildExecutables.Size = new System.Drawing.Size(469, 473);
            this.tabBuildExecutables.TabIndex = 1;
            this.tabBuildExecutables.Text = "Build Programs";
            this.tabBuildExecutables.UseVisualStyleBackColor = true;
            // 
            // lstBuildPresets
            // 
            this.lstBuildPresets.FormattingEnabled = true;
            this.lstBuildPresets.Items.AddRange(new object[] {
            "ZHLT",
            "Quake Tools"});
            this.lstBuildPresets.Location = new System.Drawing.Point(6, 90);
            this.lstBuildPresets.Name = "lstBuildPresets";
            this.lstBuildPresets.Size = new System.Drawing.Size(167, 69);
            this.lstBuildPresets.TabIndex = 22;
            // 
            // lblBuildExeFolder
            // 
            this.lblBuildExeFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBuildExeFolder.Location = new System.Drawing.Point(6, 7);
            this.lblBuildExeFolder.Name = "lblBuildExeFolder";
            this.lblBuildExeFolder.Size = new System.Drawing.Size(176, 20);
            this.lblBuildExeFolder.TabIndex = 17;
            this.lblBuildExeFolder.Text = "Folder containing build executables:";
            this.lblBuildExeFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBuildBSP
            // 
            this.lblBuildBSP.Location = new System.Drawing.Point(179, 66);
            this.lblBuildBSP.Name = "lblBuildBSP";
            this.lblBuildBSP.Size = new System.Drawing.Size(34, 20);
            this.lblBuildBSP.TabIndex = 16;
            this.lblBuildBSP.Text = "BSP";
            this.lblBuildBSP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBuildExeFolder
            // 
            this.txtBuildExeFolder.Location = new System.Drawing.Point(6, 29);
            this.txtBuildExeFolder.Name = "txtBuildExeFolder";
            this.txtBuildExeFolder.Size = new System.Drawing.Size(323, 20);
            this.txtBuildExeFolder.TabIndex = 15;
            this.txtBuildExeFolder.Text = "example: C:\\hammer_alt";
            // 
            // lblBuildCSG
            // 
            this.lblBuildCSG.Location = new System.Drawing.Point(179, 93);
            this.lblBuildCSG.Name = "lblBuildCSG";
            this.lblBuildCSG.Size = new System.Drawing.Size(34, 20);
            this.lblBuildCSG.TabIndex = 16;
            this.lblBuildCSG.Text = "CSG";
            this.lblBuildCSG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbBuildRAD
            // 
            this.cmbBuildRAD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildRAD.FormattingEnabled = true;
            this.cmbBuildRAD.Location = new System.Drawing.Point(219, 146);
            this.cmbBuildRAD.Name = "cmbBuildRAD";
            this.cmbBuildRAD.Size = new System.Drawing.Size(234, 21);
            this.cmbBuildRAD.TabIndex = 19;
            // 
            // cmbBuildBSP
            // 
            this.cmbBuildBSP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildBSP.FormattingEnabled = true;
            this.cmbBuildBSP.Location = new System.Drawing.Point(219, 65);
            this.cmbBuildBSP.Name = "cmbBuildBSP";
            this.cmbBuildBSP.Size = new System.Drawing.Size(234, 21);
            this.cmbBuildBSP.TabIndex = 19;
            // 
            // lblBuildDetectedPresets
            // 
            this.lblBuildDetectedPresets.Location = new System.Drawing.Point(6, 66);
            this.lblBuildDetectedPresets.Name = "lblBuildDetectedPresets";
            this.lblBuildDetectedPresets.Size = new System.Drawing.Size(167, 20);
            this.lblBuildDetectedPresets.TabIndex = 17;
            this.lblBuildDetectedPresets.Text = "Detected presets:";
            this.lblBuildDetectedPresets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBuildVIS
            // 
            this.lblBuildVIS.Location = new System.Drawing.Point(179, 120);
            this.lblBuildVIS.Name = "lblBuildVIS";
            this.lblBuildVIS.Size = new System.Drawing.Size(34, 20);
            this.lblBuildVIS.TabIndex = 16;
            this.lblBuildVIS.Text = "VIS";
            this.lblBuildVIS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbBuildVIS
            // 
            this.cmbBuildVIS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildVIS.FormattingEnabled = true;
            this.cmbBuildVIS.Location = new System.Drawing.Point(219, 119);
            this.cmbBuildVIS.Name = "cmbBuildVIS";
            this.cmbBuildVIS.Size = new System.Drawing.Size(234, 21);
            this.cmbBuildVIS.TabIndex = 19;
            // 
            // cmbBuildCSG
            // 
            this.cmbBuildCSG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildCSG.FormattingEnabled = true;
            this.cmbBuildCSG.Location = new System.Drawing.Point(219, 92);
            this.cmbBuildCSG.Name = "cmbBuildCSG";
            this.cmbBuildCSG.Size = new System.Drawing.Size(234, 21);
            this.cmbBuildCSG.TabIndex = 19;
            // 
            // lblBuildRAD
            // 
            this.lblBuildRAD.Location = new System.Drawing.Point(179, 147);
            this.lblBuildRAD.Name = "lblBuildRAD";
            this.lblBuildRAD.Size = new System.Drawing.Size(34, 20);
            this.lblBuildRAD.TabIndex = 16;
            this.lblBuildRAD.Text = "RAD";
            this.lblBuildRAD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnBuildExeFolderBrowse
            // 
            this.btnBuildExeFolderBrowse.Location = new System.Drawing.Point(335, 27);
            this.btnBuildExeFolderBrowse.Name = "btnBuildExeFolderBrowse";
            this.btnBuildExeFolderBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnBuildExeFolderBrowse.TabIndex = 20;
            this.btnBuildExeFolderBrowse.Text = "Browse...";
            this.btnBuildExeFolderBrowse.UseVisualStyleBackColor = true;
            // 
            // tabBuildPostCompile
            // 
            this.tabBuildPostCompile.Controls.Add(this.lblBuildCommandLine);
            this.tabBuildPostCompile.Controls.Add(this.chkBuildCopyBSP);
            this.tabBuildPostCompile.Controls.Add(this.chkBuildAskBeforeRun);
            this.tabBuildPostCompile.Controls.Add(this.radBuildRunGame);
            this.tabBuildPostCompile.Controls.Add(this.txtBuildCommandLine);
            this.tabBuildPostCompile.Controls.Add(this.radBuildRunGameOnChange);
            this.tabBuildPostCompile.Controls.Add(this.chkBuildShowLog);
            this.tabBuildPostCompile.Controls.Add(this.radBuildDontRunGame);
            this.tabBuildPostCompile.Location = new System.Drawing.Point(4, 22);
            this.tabBuildPostCompile.Name = "tabBuildPostCompile";
            this.tabBuildPostCompile.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildPostCompile.Size = new System.Drawing.Size(469, 473);
            this.tabBuildPostCompile.TabIndex = 2;
            this.tabBuildPostCompile.Text = "After Compiling";
            this.tabBuildPostCompile.UseVisualStyleBackColor = true;
            // 
            // lblBuildCommandLine
            // 
            this.lblBuildCommandLine.Location = new System.Drawing.Point(7, 178);
            this.lblBuildCommandLine.Name = "lblBuildCommandLine";
            this.lblBuildCommandLine.Size = new System.Drawing.Size(107, 20);
            this.lblBuildCommandLine.TabIndex = 28;
            this.lblBuildCommandLine.Text = "Game command line";
            this.lblBuildCommandLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkBuildCopyBSP
            // 
            this.chkBuildCopyBSP.Checked = true;
            this.chkBuildCopyBSP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBuildCopyBSP.Location = new System.Drawing.Point(6, 7);
            this.chkBuildCopyBSP.Name = "chkBuildCopyBSP";
            this.chkBuildCopyBSP.Size = new System.Drawing.Size(256, 23);
            this.chkBuildCopyBSP.TabIndex = 30;
            this.chkBuildCopyBSP.Text = "Copy BSP into <mod>/maps folder on compile";
            this.chkBuildCopyBSP.UseVisualStyleBackColor = true;
            // 
            // chkBuildAskBeforeRun
            // 
            this.chkBuildAskBeforeRun.Checked = true;
            this.chkBuildAskBeforeRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBuildAskBeforeRun.Location = new System.Drawing.Point(7, 152);
            this.chkBuildAskBeforeRun.Name = "chkBuildAskBeforeRun";
            this.chkBuildAskBeforeRun.Size = new System.Drawing.Size(171, 23);
            this.chkBuildAskBeforeRun.TabIndex = 29;
            this.chkBuildAskBeforeRun.Text = "Ask before running game";
            this.chkBuildAskBeforeRun.UseVisualStyleBackColor = true;
            // 
            // radBuildRunGame
            // 
            this.radBuildRunGame.Location = new System.Drawing.Point(6, 65);
            this.radBuildRunGame.Name = "radBuildRunGame";
            this.radBuildRunGame.Size = new System.Drawing.Size(104, 23);
            this.radBuildRunGame.TabIndex = 26;
            this.radBuildRunGame.Text = "Run game";
            this.radBuildRunGame.UseVisualStyleBackColor = true;
            // 
            // txtBuildCommandLine
            // 
            this.txtBuildCommandLine.Location = new System.Drawing.Point(120, 178);
            this.txtBuildCommandLine.Name = "txtBuildCommandLine";
            this.txtBuildCommandLine.Size = new System.Drawing.Size(225, 20);
            this.txtBuildCommandLine.TabIndex = 27;
            this.txtBuildCommandLine.Text = "-dev -console";
            // 
            // radBuildRunGameOnChange
            // 
            this.radBuildRunGameOnChange.Checked = true;
            this.radBuildRunGameOnChange.Location = new System.Drawing.Point(6, 93);
            this.radBuildRunGameOnChange.Name = "radBuildRunGameOnChange";
            this.radBuildRunGameOnChange.Size = new System.Drawing.Size(192, 24);
            this.radBuildRunGameOnChange.TabIndex = 25;
            this.radBuildRunGameOnChange.TabStop = true;
            this.radBuildRunGameOnChange.Text = "Run game only if the BSP changed";
            this.radBuildRunGameOnChange.UseVisualStyleBackColor = true;
            // 
            // chkBuildShowLog
            // 
            this.chkBuildShowLog.Checked = true;
            this.chkBuildShowLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBuildShowLog.Location = new System.Drawing.Point(6, 36);
            this.chkBuildShowLog.Name = "chkBuildShowLog";
            this.chkBuildShowLog.Size = new System.Drawing.Size(256, 23);
            this.chkBuildShowLog.TabIndex = 31;
            this.chkBuildShowLog.Text = "Show compile log";
            this.chkBuildShowLog.UseVisualStyleBackColor = true;
            // 
            // radBuildDontRunGame
            // 
            this.radBuildDontRunGame.Location = new System.Drawing.Point(6, 123);
            this.radBuildDontRunGame.Name = "radBuildDontRunGame";
            this.radBuildDontRunGame.Size = new System.Drawing.Size(104, 23);
            this.radBuildDontRunGame.TabIndex = 24;
            this.radBuildDontRunGame.Text = "Do nothing";
            this.radBuildDontRunGame.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvanced
            // 
            this.tabBuildAdvanced.Controls.Add(this.tabBuildAdvancedSubTabs);
            this.tabBuildAdvanced.Controls.Add(this.lblBuildTEMPAdvancedConfig);
            this.tabBuildAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvanced.Name = "tabBuildAdvanced";
            this.tabBuildAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildAdvanced.Size = new System.Drawing.Size(469, 473);
            this.tabBuildAdvanced.TabIndex = 3;
            this.tabBuildAdvanced.Text = "Advanced";
            this.tabBuildAdvanced.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedSubTabs
            // 
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedCSG);
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedBSP);
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedVIS);
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedRAD);
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedShared);
            this.tabBuildAdvancedSubTabs.Controls.Add(this.tabBuildAdvancedPreview);
            this.tabBuildAdvancedSubTabs.Location = new System.Drawing.Point(6, 33);
            this.tabBuildAdvancedSubTabs.Name = "tabBuildAdvancedSubTabs";
            this.tabBuildAdvancedSubTabs.SelectedIndex = 0;
            this.tabBuildAdvancedSubTabs.Size = new System.Drawing.Size(457, 434);
            this.tabBuildAdvancedSubTabs.TabIndex = 19;
            // 
            // tabBuildAdvancedCSG
            // 
            this.tabBuildAdvancedCSG.Controls.Add(this.label20);
            this.tabBuildAdvancedCSG.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedCSG.Name = "tabBuildAdvancedCSG";
            this.tabBuildAdvancedCSG.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildAdvancedCSG.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedCSG.TabIndex = 0;
            this.tabBuildAdvancedCSG.Text = "CSG";
            this.tabBuildAdvancedCSG.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(93, 268);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(238, 36);
            this.label20.TabIndex = 2;
            this.label20.Text = "+WADINCLUDE";
            // 
            // tabBuildAdvancedBSP
            // 
            this.tabBuildAdvancedBSP.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedBSP.Name = "tabBuildAdvancedBSP";
            this.tabBuildAdvancedBSP.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedBSP.TabIndex = 1;
            // 
            // tabBuildAdvancedVIS
            // 
            this.tabBuildAdvancedVIS.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedVIS.Name = "tabBuildAdvancedVIS";
            this.tabBuildAdvancedVIS.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedVIS.TabIndex = 2;
            // 
            // tabBuildAdvancedRAD
            // 
            this.tabBuildAdvancedRAD.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedRAD.Name = "tabBuildAdvancedRAD";
            this.tabBuildAdvancedRAD.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedRAD.TabIndex = 3;
            // 
            // tabBuildAdvancedShared
            // 
            this.tabBuildAdvancedShared.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedShared.Name = "tabBuildAdvancedShared";
            this.tabBuildAdvancedShared.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedShared.TabIndex = 4;
            // 
            // tabBuildAdvancedPreview
            // 
            this.tabBuildAdvancedPreview.Controls.Add(this.txtBuildAdvancedPreview);
            this.tabBuildAdvancedPreview.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedPreview.Name = "tabBuildAdvancedPreview";
            this.tabBuildAdvancedPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildAdvancedPreview.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedPreview.TabIndex = 5;
            this.tabBuildAdvancedPreview.Text = "Preview";
            this.tabBuildAdvancedPreview.UseVisualStyleBackColor = true;
            // 
            // txtBuildAdvancedPreview
            // 
            this.txtBuildAdvancedPreview.BackColor = System.Drawing.SystemColors.Window;
            this.txtBuildAdvancedPreview.Location = new System.Drawing.Point(6, 6);
            this.txtBuildAdvancedPreview.Multiline = true;
            this.txtBuildAdvancedPreview.Name = "txtBuildAdvancedPreview";
            this.txtBuildAdvancedPreview.ReadOnly = true;
            this.txtBuildAdvancedPreview.Size = new System.Drawing.Size(437, 396);
            this.txtBuildAdvancedPreview.TabIndex = 3;
            // 
            // lblBuildTEMPAdvancedConfig
            // 
            this.lblBuildTEMPAdvancedConfig.Location = new System.Drawing.Point(6, 5);
            this.lblBuildTEMPAdvancedConfig.Name = "lblBuildTEMPAdvancedConfig";
            this.lblBuildTEMPAdvancedConfig.Size = new System.Drawing.Size(457, 33);
            this.lblBuildTEMPAdvancedConfig.TabIndex = 18;
            this.lblBuildTEMPAdvancedConfig.Text = "These will be set as default settings of the compile dialog, but you can change t" +
    "hen for each compile if you want.";
            // 
            // btnBuildRemove
            // 
            this.btnBuildRemove.Location = new System.Drawing.Point(166, 35);
            this.btnBuildRemove.Name = "btnBuildRemove";
            this.btnBuildRemove.Size = new System.Drawing.Size(73, 23);
            this.btnBuildRemove.TabIndex = 12;
            this.btnBuildRemove.Text = "Remove";
            this.btnBuildRemove.UseVisualStyleBackColor = true;
            // 
            // btnBuildAdd
            // 
            this.btnBuildAdd.Location = new System.Drawing.Point(166, 6);
            this.btnBuildAdd.Name = "btnBuildAdd";
            this.btnBuildAdd.Size = new System.Drawing.Size(73, 23);
            this.btnBuildAdd.TabIndex = 10;
            this.btnBuildAdd.Text = "Add New";
            this.btnBuildAdd.UseVisualStyleBackColor = true;
            // 
            // tree_builds
            // 
            this.tree_builds.Location = new System.Drawing.Point(6, 6);
            this.tree_builds.Name = "tree_builds";
            treeNode14.Name = "nodeHL";
            treeNode14.Text = "Goldsource";
            treeNode15.Name = "nodeSource";
            treeNode15.Text = "Source";
            this.tree_builds.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode15});
            this.tree_builds.Size = new System.Drawing.Size(154, 448);
            this.tree_builds.TabIndex = 9;
            // 
            // tabSteam
            // 
            this.tabSteam.Controls.Add(this.o_SteamInstallDir);
            this.tabSteam.Controls.Add(this.label17);
            this.tabSteam.Controls.Add(this.button1);
            this.tabSteam.Controls.Add(this.label18);
            this.tabSteam.Controls.Add(this.btnSteamInstallDirBrowse);
            this.tabSteam.Controls.Add(this.o_SteamUsername);
            this.tabSteam.Location = new System.Drawing.Point(4, 22);
            this.tabSteam.Name = "tabSteam";
            this.tabSteam.Padding = new System.Windows.Forms.Padding(3);
            this.tabSteam.Size = new System.Drawing.Size(736, 511);
            this.tabSteam.TabIndex = 5;
            this.tabSteam.Text = "Steam";
            this.tabSteam.UseVisualStyleBackColor = true;
            // 
            // o_SteamInstallDir
            // 
            this.o_SteamInstallDir.Location = new System.Drawing.Point(109, 20);
            this.o_SteamInstallDir.Name = "o_SteamInstallDir";
            this.o_SteamInstallDir.Size = new System.Drawing.Size(225, 20);
            this.o_SteamInstallDir.TabIndex = 5;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(6, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(97, 20);
            this.label17.TabIndex = 6;
            this.label17.Text = "Steam Directory";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(237, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 25);
            this.button1.TabIndex = 8;
            this.button1.Text = "List Available Games";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(6, 47);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(97, 20);
            this.label18.TabIndex = 6;
            this.label18.Text = "Steam Username";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSteamInstallDirBrowse
            // 
            this.btnSteamInstallDirBrowse.Location = new System.Drawing.Point(340, 18);
            this.btnSteamInstallDirBrowse.Name = "btnSteamInstallDirBrowse";
            this.btnSteamInstallDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnSteamInstallDirBrowse.TabIndex = 8;
            this.btnSteamInstallDirBrowse.Text = "Browse...";
            this.btnSteamInstallDirBrowse.UseVisualStyleBackColor = true;
            // 
            // o_SteamUsername
            // 
            this.o_SteamUsername.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.o_SteamUsername.FormattingEnabled = true;
            this.o_SteamUsername.Location = new System.Drawing.Point(109, 46);
            this.o_SteamUsername.Name = "o_SteamUsername";
            this.o_SteamUsername.Size = new System.Drawing.Size(121, 21);
            this.o_SteamUsername.TabIndex = 7;
            // 
            // tabHotkeys
            // 
            this.tabHotkeys.Controls.Add(this.listView1);
            this.tabHotkeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotkeys.Name = "tabHotkeys";
            this.tabHotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.tabHotkeys.Size = new System.Drawing.Size(736, 511);
            this.tabHotkeys.TabIndex = 6;
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chKey,
            this.ckKeyCombo,
            this.chTrigger});
            this.listView1.Location = new System.Drawing.Point(6, 6);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(724, 499);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // chKey
            // 
            this.chKey.Text = "Key";
            // 
            // ckKeyCombo
            // 
            this.ckKeyCombo.Text = "Key Combination";
            // 
            // chTrigger
            // 
            this.chTrigger.Text = "Action";
            // 
            // groupBox15
            // 
            this.groupBox15.Location = new System.Drawing.Point(0, 0);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(200, 100);
            this.groupBox15.TabIndex = 0;
            this.groupBox15.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.checkBox6);
            this.groupBox10.Controls.Add(this.checkBox5);
            this.groupBox10.Location = new System.Drawing.Point(6, 19);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(426, 83);
            this.groupBox10.TabIndex = 3;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Mouselook";
            // 
            // checkBox6
            // 
            this.checkBox6.Location = new System.Drawing.Point(27, 49);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(104, 24);
            this.checkBox6.TabIndex = 0;
            this.checkBox6.Text = "Invert X Axis";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.Location = new System.Drawing.Point(27, 19);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(104, 24);
            this.checkBox5.TabIndex = 0;
            this.checkBox5.Text = "Invert Y Axis";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.trackBar3);
            this.groupBox9.Controls.Add(this.label5);
            this.groupBox9.Location = new System.Drawing.Point(6, 319);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(426, 98);
            this.groupBox9.TabIndex = 2;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Time to Top Speed";
            // 
            // trackBar3
            // 
            this.trackBar3.AutoSize = false;
            this.trackBar3.BackColor = System.Drawing.SystemColors.Window;
            this.trackBar3.Location = new System.Drawing.Point(6, 20);
            this.trackBar3.Maximum = 50;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(414, 42);
            this.trackBar3.TabIndex = 0;
            this.trackBar3.TickFrequency = 10000;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar3.Value = 5;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(414, 23);
            this.label5.TabIndex = 1;
            this.label5.Text = "0.5 sec";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.trackBar2);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Location = new System.Drawing.Point(6, 215);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(426, 98);
            this.groupBox8.TabIndex = 2;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Forward Speed";
            // 
            // trackBar2
            // 
            this.trackBar2.AutoSize = false;
            this.trackBar2.BackColor = System.Drawing.SystemColors.Window;
            this.trackBar2.Location = new System.Drawing.Point(6, 20);
            this.trackBar2.Maximum = 5000;
            this.trackBar2.Minimum = 100;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(414, 42);
            this.trackBar2.TabIndex = 0;
            this.trackBar2.TickFrequency = 10000;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar2.Value = 1000;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(414, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "1000 units/sec";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.trackBar1);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Location = new System.Drawing.Point(6, 108);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(426, 98);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Back Clipping Pane";
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.BackColor = System.Drawing.SystemColors.Window;
            this.trackBar1.Location = new System.Drawing.Point(6, 20);
            this.trackBar1.Maximum = 10000;
            this.trackBar1.Minimum = 2000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(414, 42);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickFrequency = 10000;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 5000;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(414, 23);
            this.label4.TabIndex = 1;
            this.label4.Text = "4000";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConfigSteamDir
            // 
            this.lblConfigSteamDir.Location = new System.Drawing.Point(12, 9);
            this.lblConfigSteamDir.Name = "lblConfigSteamDir";
            this.lblConfigSteamDir.Size = new System.Drawing.Size(97, 20);
            this.lblConfigSteamDir.TabIndex = 6;
            this.lblConfigSteamDir.Text = "Steam Directory";
            this.lblConfigSteamDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnConfigListSteamGames
            // 
            this.btnConfigListSteamGames.Location = new System.Drawing.Point(243, 33);
            this.btnConfigListSteamGames.Name = "btnConfigListSteamGames";
            this.btnConfigListSteamGames.Size = new System.Drawing.Size(115, 25);
            this.btnConfigListSteamGames.TabIndex = 8;
            this.btnConfigListSteamGames.Text = "List Available Games";
            this.btnConfigListSteamGames.UseVisualStyleBackColor = true;
            // 
            // btnConfigSteamDirBrowse
            // 
            this.btnConfigSteamDirBrowse.Location = new System.Drawing.Point(346, 7);
            this.btnConfigSteamDirBrowse.Name = "btnConfigSteamDirBrowse";
            this.btnConfigSteamDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnConfigSteamDirBrowse.TabIndex = 8;
            this.btnConfigSteamDirBrowse.Text = "Browse...";
            this.btnConfigSteamDirBrowse.UseVisualStyleBackColor = true;
            // 
            // cmbConfigSteamUser
            // 
            this.cmbConfigSteamUser.FormattingEnabled = true;
            this.cmbConfigSteamUser.Location = new System.Drawing.Point(115, 35);
            this.cmbConfigSteamUser.Name = "cmbConfigSteamUser";
            this.cmbConfigSteamUser.Size = new System.Drawing.Size(121, 21);
            this.cmbConfigSteamUser.TabIndex = 7;
            this.cmbConfigSteamUser.Text = "Penguinboy77";
            // 
            // lblConfigSteamUser
            // 
            this.lblConfigSteamUser.Location = new System.Drawing.Point(12, 36);
            this.lblConfigSteamUser.Name = "lblConfigSteamUser";
            this.lblConfigSteamUser.Size = new System.Drawing.Size(97, 20);
            this.lblConfigSteamUser.TabIndex = 6;
            this.lblConfigSteamUser.Text = "Steam Username";
            this.lblConfigSteamUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancelSettings
            // 
            this.btnCancelSettings.Location = new System.Drawing.Point(681, 555);
            this.btnCancelSettings.Name = "btnCancelSettings";
            this.btnCancelSettings.Size = new System.Drawing.Size(75, 23);
            this.btnCancelSettings.TabIndex = 1;
            this.btnCancelSettings.Text = "Cancel";
            this.btnCancelSettings.UseVisualStyleBackColor = true;
            this.btnCancelSettings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Close);
            // 
            // btnApplyAndCloseSettings
            // 
            this.btnApplyAndCloseSettings.Location = new System.Drawing.Point(584, 555);
            this.btnApplyAndCloseSettings.Name = "btnApplyAndCloseSettings";
            this.btnApplyAndCloseSettings.Size = new System.Drawing.Size(91, 23);
            this.btnApplyAndCloseSettings.TabIndex = 1;
            this.btnApplyAndCloseSettings.Text = "Apply and Close";
            this.btnApplyAndCloseSettings.UseVisualStyleBackColor = true;
            this.btnApplyAndCloseSettings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ApplyAndClose);
            // 
            // btnApplySettings
            // 
            this.btnApplySettings.Location = new System.Drawing.Point(503, 555);
            this.btnApplySettings.Name = "btnApplySettings";
            this.btnApplySettings.Size = new System.Drawing.Size(75, 23);
            this.btnApplySettings.TabIndex = 1;
            this.btnApplySettings.Text = "Apply";
            this.btnApplySettings.UseVisualStyleBackColor = true;
            this.btnApplySettings.Click += new System.EventHandler(this.Apply);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 590);
            this.Controls.Add(this.btnApplySettings);
            this.Controls.Add(this.btnApplyAndCloseSettings);
            this.Controls.Add(this.btnCancelSettings);
            this.Controls.Add(this.tbcSettings);
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsFormLoad);
            this.tbcSettings.ResumeLayout(false);
            this.tab2DViews.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NudgeUnits)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridHighlight1Distance)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HideGridLimit)).EndInit();
            this.tab3DViews.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.o_3DTimeToTopSpeed)).EndInit();
            this.groupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.o_3DForwardSpeed)).EndInit();
            this.groupBox14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.o_3DBackClippingPane)).EndInit();
            this.tabGame.ResumeLayout(false);
            this.tabGameSubTabs.ResumeLayout(false);
            this.tabConfigDirectories.ResumeLayout(false);
            this.tabConfigDirectories.PerformLayout();
            this.grpConfigGame.ResumeLayout(false);
            this.grpConfigGame.PerformLayout();
            this.grpConfigSaving.ResumeLayout(false);
            this.grpConfigSaving.PerformLayout();
            this.tabConfigEntities.ResumeLayout(false);
            this.tabConfigTextures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudGameLightmapScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGameTextureScale)).EndInit();
            this.tabBuild.ResumeLayout(false);
            this.tabBuildSubTabs.ResumeLayout(false);
            this.tabBuildGeneral.ResumeLayout(false);
            this.tabBuildGeneral.PerformLayout();
            this.tabBuildExecutables.ResumeLayout(false);
            this.tabBuildExecutables.PerformLayout();
            this.tabBuildPostCompile.ResumeLayout(false);
            this.tabBuildPostCompile.PerformLayout();
            this.tabBuildAdvanced.ResumeLayout(false);
            this.tabBuildAdvancedSubTabs.ResumeLayout(false);
            this.tabBuildAdvancedCSG.ResumeLayout(false);
            this.tabBuildAdvancedPreview.ResumeLayout(false);
            this.tabBuildAdvancedPreview.PerformLayout();
            this.tabSteam.ResumeLayout(false);
            this.tabSteam.PerformLayout();
            this.tabHotkeys.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.TextBox txtBuildAdvancedPreview;
		private System.Windows.Forms.TabPage tabBuildAdvancedPreview;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox15;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TabPage tabBuildAdvancedShared;
		private System.Windows.Forms.TabPage tabBuildAdvancedRAD;
		private System.Windows.Forms.TabPage tabBuildAdvancedVIS;
		private System.Windows.Forms.TabPage tabBuildAdvancedBSP;
		private System.Windows.Forms.TabPage tabBuildAdvancedCSG;
		private System.Windows.Forms.TabControl tabBuildAdvancedSubTabs;
		private System.Windows.Forms.TextBox txtGameMapDir;
		private System.Windows.Forms.TabPage tabGame;
		private System.Windows.Forms.Button btnGameAdd;
		private System.Windows.Forms.Button btnGameRemove;
		private System.Windows.Forms.Button btnGameAddFGD;
		private System.Windows.Forms.Button btnGameAddWAD;
		private System.Windows.Forms.Button btnGameRemoveFGD;
		private System.Windows.Forms.Button btnGameRemoveWAD;
		private System.Windows.Forms.Label lblGameWAD;
		private System.Windows.Forms.Label lblGameBuild;
		private System.Windows.Forms.ComboBox cmbGameBuild;
		private System.Windows.Forms.ListBox lstGameWAD;
		private System.Windows.Forms.Label lblGameFGD;
		private System.Windows.Forms.ComboBox cmbGamePointEnt;
		private System.Windows.Forms.ComboBox cmbGameBrushEnt;
		private System.Windows.Forms.ListBox lstGameFGD;
		private System.Windows.Forms.NumericUpDown nudGameTextureScale;
		private System.Windows.Forms.NumericUpDown nudGameLightmapScale;
		private System.Windows.Forms.Label lblGameMapSaveDir;
		private System.Windows.Forms.Button btnGameMapDirBrowse;
		private System.Windows.Forms.TextBox txtGameAutosaveDir;
		private System.Windows.Forms.Label lblGameAutosaveDir;
		private System.Windows.Forms.Button btnGameAutosaveDirBrowse;
		private System.Windows.Forms.Label lblGameMod;
		private System.Windows.Forms.ComboBox cmbGameMod;
		private System.Windows.Forms.TextBox txtGameWONDir;
		private System.Windows.Forms.Label lblGameWONDir;
		private System.Windows.Forms.Button btnGameDirBrowse;
		private System.Windows.Forms.Label lblGameSteamDir;
		private System.Windows.Forms.ComboBox cmbGameSteamDir;
		private System.Windows.Forms.TextBox txtGameName;
		private System.Windows.Forms.Label lblGameName;
		private System.Windows.Forms.Label lblGameEngine;
		private System.Windows.Forms.ComboBox cmbGameEngine;
		private System.Windows.Forms.CheckBox chkGameMapDiffAutosaveDir;
		private System.Windows.Forms.CheckBox chkGameEnableAutosave;
		private System.Windows.Forms.TabControl tabGameSubTabs;
		private System.Windows.Forms.Button btnGameChangeName;
		private System.Windows.Forms.TreeView tree_games;
		private System.Windows.Forms.Button btnBuildChangeName;
		private System.Windows.Forms.TreeView tree_builds;
		private System.Windows.Forms.ColumnHeader chTrigger;
		private System.Windows.Forms.ColumnHeader ckKeyCombo;
		private System.Windows.Forms.ColumnHeader chKey;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.TabPage tabHotkeys;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Panel GridBoundaryColour;
		private System.Windows.Forms.Button btnCancelSettings;
		private System.Windows.Forms.Button btnApplyAndCloseSettings;
		private System.Windows.Forms.Button btnSteamInstallDirBrowse;
		private System.Windows.Forms.TrackBar o_3DTimeToTopSpeed;
		private System.Windows.Forms.CheckBox o_3DInvertX;
		private System.Windows.Forms.CheckBox o_3DInvertY;
		private System.Windows.Forms.TrackBar o_3DForwardSpeed;
		private System.Windows.Forms.TrackBar o_3DBackClippingPane;
		private System.Windows.Forms.TextBox o_SteamInstallDir;
		private System.Windows.Forms.ComboBox o_SteamUsername;
		private System.Windows.Forms.Button btnApplySettings;
		private System.Windows.Forms.CheckBox CrosshairCursorIn2DViews;
		private System.Windows.Forms.RadioButton RotationStyle_SnapNever;
		private System.Windows.Forms.RadioButton RotationStyle_SnapOffShift;
		private System.Windows.Forms.RadioButton RotationStyle_SnapOnShift;
		private System.Windows.Forms.NumericUpDown NudgeUnits;
		private System.Windows.Forms.CheckBox ArrowKeysNudgeSelection;
		private System.Windows.Forms.RadioButton SnapStyle_SnapOnAlt;
		private System.Windows.Forms.RadioButton SnapStyle_SnapOffAlt;
		private System.Windows.Forms.DomainUpDown DefaultGridSize;
		private System.Windows.Forms.Panel GridHighlight2Colour;
		private System.Windows.Forms.Panel GridHighlight1Colour;
		private System.Windows.Forms.Panel GridColour;
		private System.Windows.Forms.Panel GridBackgroundColour;
		private System.Windows.Forms.Panel GridZeroAxisColour;
		private System.Windows.Forms.NumericUpDown GridHighlight1Distance;
		private System.Windows.Forms.CheckBox GridHighlight2On;
		private System.Windows.Forms.CheckBox GridHighlight1On;
		private System.Windows.Forms.DomainUpDown HideGridFactor;
		private System.Windows.Forms.NumericUpDown HideGridLimit;
		private System.Windows.Forms.TabPage tabBuildAdvanced;
		private System.Windows.Forms.TabPage tabBuildPostCompile;
		private System.Windows.Forms.TabPage tabBuildExecutables;
		private System.Windows.Forms.TabPage tabBuildGeneral;
		private System.Windows.Forms.TabControl tabBuildSubTabs;
		private System.Windows.Forms.TabPage tabConfigTextures;
		private System.Windows.Forms.TabPage tabConfigEntities;
		private System.Windows.Forms.TabPage tabConfigDirectories;
		private System.Windows.Forms.TabPage tab2DViews;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TabPage tabSteam;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox groupBox14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.TabPage tab3DViews;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackBar2;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TrackBar trackBar3;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtBuildCommandLine;
		private System.Windows.Forms.CheckBox chkBuildCopyBSP;
		private System.Windows.Forms.Label lblConfigLightmapScale;
		private System.Windows.Forms.Label lblConfigTextureScale;
		private System.Windows.Forms.Label lblConfigBrushEnt;
		private System.Windows.Forms.Label lblConfigPointEnt;
		private System.Windows.Forms.Label lblConfigSteamDir;
		private System.Windows.Forms.Button btnConfigListSteamGames;
		private System.Windows.Forms.Button btnConfigSteamDirBrowse;
		private System.Windows.Forms.ComboBox cmbConfigSteamUser;
		private System.Windows.Forms.Label lblConfigSteamUser;
		private System.Windows.Forms.Button btnBuildExeFolderBrowse;
		private System.Windows.Forms.ComboBox cmbBuildEngine;
		private System.Windows.Forms.Label lblBuildEngine;
		private System.Windows.Forms.Label lblBuildExeFolder;
		private System.Windows.Forms.Label lblBuildName;
		private System.Windows.Forms.TextBox txtBuildExeFolder;
		private System.Windows.Forms.TextBox txtBuildName;
		private System.Windows.Forms.Button btnBuildRemove;
		private System.Windows.Forms.Button btnBuildAdd;
		private System.Windows.Forms.CheckBox chkBuildShowLog;
		private System.Windows.Forms.RadioButton radBuildDontRunGame;
		private System.Windows.Forms.CheckBox chkBuildAskBeforeRun;
		private System.Windows.Forms.RadioButton radBuildRunGameOnChange;
		private System.Windows.Forms.RadioButton radBuildRunGame;
		private System.Windows.Forms.Label lblBuildCommandLine;
		private System.Windows.Forms.ListBox lstBuildPresets;
		private System.Windows.Forms.Label lblBuildDetectedPresets;
		private System.Windows.Forms.ComboBox cmbBuildRAD;
		private System.Windows.Forms.ComboBox cmbBuildVIS;
		private System.Windows.Forms.Label lblBuildRAD;
		private System.Windows.Forms.ComboBox cmbBuildCSG;
		private System.Windows.Forms.Label lblBuildVIS;
		private System.Windows.Forms.ComboBox cmbBuildBSP;
		private System.Windows.Forms.Label lblBuildCSG;
		private System.Windows.Forms.Label lblBuildBSP;
		private System.Windows.Forms.Label lblBuildTEMPAdvancedConfig;
		private System.Windows.Forms.GroupBox grpConfigGame;
		private System.Windows.Forms.GroupBox grpConfigSaving;
		private System.Windows.Forms.TabPage tabBuild;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.TabControl tbcSettings;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox HideGridOn;
        private System.Windows.Forms.DomainUpDown GridHighlight2UnitNum;
	}
}
