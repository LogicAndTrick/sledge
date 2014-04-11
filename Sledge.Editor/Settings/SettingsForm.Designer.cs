﻿/*
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
            this.tbcSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tab2DViews = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.CenterHandlesOnlyNearCursor = new System.Windows.Forms.CheckBox();
            this.CenterHandlesActiveViewportOnly = new System.Windows.Forms.CheckBox();
            this.DrawCenterHandles = new System.Windows.Forms.CheckBox();
            this.BoxSelectByHandlesOnly = new System.Windows.Forms.CheckBox();
            this.ClickSelectByHandlesOnly = new System.Windows.Forms.CheckBox();
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
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.DrawEntityNames = new System.Windows.Forms.CheckBox();
            this.DrawBoxText = new System.Windows.Forms.CheckBox();
            this.KeepVisgroupsWhenCloning = new System.Windows.Forms.CheckBox();
            this.AutoSelectBox = new System.Windows.Forms.CheckBox();
            this.CrosshairCursorIn2DViews = new System.Windows.Forms.CheckBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.SelectionBoxBackgroundOpacity = new System.Windows.Forms.NumericUpDown();
            this.ScrollWheelZoomMultiplier = new System.Windows.Forms.NumericUpDown();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.ArrowKeysNudgeSelection = new System.Windows.Forms.CheckBox();
            this.NudgeStyle_GridOnCtrl = new System.Windows.Forms.RadioButton();
            this.NudgeStyle_GridOffCtrl = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.NudgeUnits = new System.Windows.Forms.NumericUpDown();
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
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.ViewportBackgroundColour = new System.Windows.Forms.Panel();
            this.CameraFOV = new System.Windows.Forms.NumericUpDown();
            this.label29 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label43 = new System.Windows.Forms.Label();
            this.MouseWheelMoveDistance = new System.Windows.Forms.NumericUpDown();
            this.InvertMouseX = new System.Windows.Forms.CheckBox();
            this.TimeToTopSpeedLabel = new System.Windows.Forms.Label();
            this.InvertMouseY = new System.Windows.Forms.CheckBox();
            this.TimeToTopSpeed = new System.Windows.Forms.TrackBar();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.ForwardSpeedLabel = new System.Windows.Forms.Label();
            this.ForwardSpeed = new System.Windows.Forms.TrackBar();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.DetailRenderDistance = new System.Windows.Forms.TrackBar();
            this.DetailRenderDistanceLabel = new System.Windows.Forms.Label();
            this.ModelRenderDistance = new System.Windows.Forms.TrackBar();
            this.ModelRenderDistanceLabel = new System.Windows.Forms.Label();
            this.BackClippingPane = new System.Windows.Forms.TrackBar();
            this.BackClippingPaneLabel = new System.Windows.Forms.Label();
            this.tabGame = new System.Windows.Forms.TabPage();
            this.GameSubTabs = new System.Windows.Forms.TabControl();
            this.tabConfigDirectories = new System.Windows.Forms.TabPage();
            this.SelectedGameRunArguments = new System.Windows.Forms.TextBox();
            this.lblBaseGame = new System.Windows.Forms.Label();
            this.SelectedGameSteamInstall = new System.Windows.Forms.CheckBox();
            this.SelectedGameBase = new System.Windows.Forms.ComboBox();
            this.SelectedGameWonDir = new System.Windows.Forms.TextBox();
            this.lblGameName = new System.Windows.Forms.Label();
            this.lblGameWONDir = new System.Windows.Forms.Label();
            this.SelectedGameBuild = new System.Windows.Forms.ComboBox();
            this.SelectedGameDirBrowse = new System.Windows.Forms.Button();
            this.lblGameSteamDir = new System.Windows.Forms.Label();
            this.lblGameBuild = new System.Windows.Forms.Label();
            this.SelectedGameSteamDir = new System.Windows.Forms.ComboBox();
            this.SelectedGameName = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.SelectedGameEngine = new System.Windows.Forms.ComboBox();
            this.lblGameMod = new System.Windows.Forms.Label();
            this.lblGameEngine = new System.Windows.Forms.Label();
            this.SelectedGameExecutable = new System.Windows.Forms.ComboBox();
            this.SelectedGameMod = new System.Windows.Forms.ComboBox();
            this.tabConfigSaving = new System.Windows.Forms.TabPage();
            this.SelectedGameAutosaveLimit = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.SelectedGameMapDirBrowse = new System.Windows.Forms.Button();
            this.SelectedGameAutosaveTime = new System.Windows.Forms.NumericUpDown();
            this.SelectedGameUseDiffAutosaveDir = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.SelectedGameAutosaveOnlyOnChange = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.SelectedGameDiffAutosaveDirBrowse = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.lblGameAutosaveDir = new System.Windows.Forms.Label();
            this.lblGameMapSaveDir = new System.Windows.Forms.Label();
            this.SelectedGameDiffAutosaveDir = new System.Windows.Forms.TextBox();
            this.SelectedGameEnableAutosave = new System.Windows.Forms.CheckBox();
            this.SelectedGameMapDir = new System.Windows.Forms.TextBox();
            this.tabConfigEntities = new System.Windows.Forms.TabPage();
            this.SelectedGameFgdList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label38 = new System.Windows.Forms.Label();
            this.SelectedGameDetectedSizeHigh = new System.Windows.Forms.Label();
            this.SelectedGameDetectedSizeLow = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.SelectedGameOverrideSizeHigh = new System.Windows.Forms.ComboBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.SelectedGameOverrideSizeLow = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.SelectedGameOverrideMapSize = new System.Windows.Forms.CheckBox();
            this.lblGameFGD = new System.Windows.Forms.Label();
            this.SelectedGameAddFgd = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.lblConfigBrushEnt = new System.Windows.Forms.Label();
            this.SelectedGameRemoveFgd = new System.Windows.Forms.Button();
            this.lblConfigPointEnt = new System.Windows.Forms.Label();
            this.SelectedGameDefaultBrushEnt = new System.Windows.Forms.ComboBox();
            this.SelectedGameDefaultPointEnt = new System.Windows.Forms.ComboBox();
            this.tabConfigTextures = new System.Windows.Forms.TabPage();
            this.SelectedGameWadList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblGameWAD = new System.Windows.Forms.Label();
            this.SelectedGameLightmapScale = new System.Windows.Forms.NumericUpDown();
            this.lblConfigLightmapScale = new System.Windows.Forms.Label();
            this.SelectedGameAddWad = new System.Windows.Forms.Button();
            this.SelectedGameTextureScale = new System.Windows.Forms.NumericUpDown();
            this.lblConfigTextureScale = new System.Windows.Forms.Label();
            this.SelectedGameRemoveWad = new System.Windows.Forms.Button();
            this.GameTree = new System.Windows.Forms.TreeView();
            this.RemoveGame = new System.Windows.Forms.Button();
            this.AddGame = new System.Windows.Forms.Button();
            this.tabBuild = new System.Windows.Forms.TabPage();
            this.BuildSubTabs = new System.Windows.Forms.TabControl();
            this.tabBuildGeneral = new System.Windows.Forms.TabPage();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.SelectedBuildCopyLin = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyErr = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyLog = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyPts = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyRes = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyMap = new System.Windows.Forms.CheckBox();
            this.SelectedBuildCopyBsp = new System.Windows.Forms.CheckBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.SelectedBuildWorkingDirSub = new System.Windows.Forms.RadioButton();
            this.SelectedBuildWorkingDirSame = new System.Windows.Forms.RadioButton();
            this.SelectedBuildWorkingDirTemp = new System.Windows.Forms.RadioButton();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.SelectedBuildAfterRunGame = new System.Windows.Forms.CheckBox();
            this.SelectedBuildAfterCopyBsp = new System.Windows.Forms.CheckBox();
            this.SelectedBuildAskBeforeRun = new System.Windows.Forms.CheckBox();
            this.lblBuildName = new System.Windows.Forms.Label();
            this.SelectedBuildName = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.lblBuildEngine = new System.Windows.Forms.Label();
            this.SelectedBuildSpecification = new System.Windows.Forms.ComboBox();
            this.SelectedBuildEngine = new System.Windows.Forms.ComboBox();
            this.tabBuildExecutables = new System.Windows.Forms.TabPage();
            this.SelectedBuildIncludeWads = new System.Windows.Forms.CheckBox();
            this.lblBuildExeFolder = new System.Windows.Forms.Label();
            this.lblBuildBSP = new System.Windows.Forms.Label();
            this.SelectedBuildExeFolder = new System.Windows.Forms.TextBox();
            this.lblBuildCSG = new System.Windows.Forms.Label();
            this.SelectedBuildRad = new System.Windows.Forms.ComboBox();
            this.SelectedBuildBsp = new System.Windows.Forms.ComboBox();
            this.lblBuildVIS = new System.Windows.Forms.Label();
            this.SelectedBuildVis = new System.Windows.Forms.ComboBox();
            this.SelectedBuildCsg = new System.Windows.Forms.ComboBox();
            this.lblBuildRAD = new System.Windows.Forms.Label();
            this.SelectedBuildExeFolderBrowse = new System.Windows.Forms.Button();
            this.tabBuildAdvanced = new System.Windows.Forms.TabPage();
            this.SelectedBuildNewProfileButton = new System.Windows.Forms.Button();
            this.SelectedBuildRenameProfileButton = new System.Windows.Forms.Button();
            this.SelectedBuildDeleteProfileButton = new System.Windows.Forms.Button();
            this.SelectedBuildProfile = new System.Windows.Forms.ComboBox();
            this.label40 = new System.Windows.Forms.Label();
            this.tabBuildAdvancedSubTabs = new System.Windows.Forms.TabControl();
            this.tabBuildAdvancedCSG = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedBSP = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedVIS = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedRAD = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedShared = new System.Windows.Forms.TabPage();
            this.tabBuildAdvancedPreview = new System.Windows.Forms.TabPage();
            this.SelectedBuildProfilePreview = new System.Windows.Forms.TextBox();
            this.RemoveBuild = new System.Windows.Forms.Button();
            this.AddBuild = new System.Windows.Forms.Button();
            this.BuildTree = new System.Windows.Forms.TreeView();
            this.tabSteam = new System.Windows.Forms.TabPage();
            this.SteamInstallDir = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.ListAvailableGamesButton = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.SteamInstallDirBrowseButton = new System.Windows.Forms.Button();
            this.SteamUsername = new System.Windows.Forms.ComboBox();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.HotkeyResetButton = new System.Windows.Forms.Button();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.HotkeyActionList = new System.Windows.Forms.ComboBox();
            this.HotkeyAddButton = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.HotkeyCombination = new System.Windows.Forms.TextBox();
            this.HotkeyReassignButton = new System.Windows.Forms.Button();
            this.HotkeyRemoveButton = new System.Windows.Forms.Button();
            this.HotkeyList = new System.Windows.Forms.ListView();
            this.chAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ckKeyCombo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SelectedBuildCsgParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.SelectedBuildBspParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.SelectedBuildVisParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.SelectedBuildRadParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.SelectedBuildSharedParameters = new Sledge.Editor.Compiling.CompileParameterPanel();
            this.tbcSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tab2DViews.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridHighlight1Distance)).BeginInit();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBackgroundOpacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScrollWheelZoomMultiplier)).BeginInit();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudgeUnits)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HideGridLimit)).BeginInit();
            this.tab3DViews.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraFOV)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MouseWheelMoveDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeToTopSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ForwardSpeed)).BeginInit();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DetailRenderDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ModelRenderDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackClippingPane)).BeginInit();
            this.tabGame.SuspendLayout();
            this.GameSubTabs.SuspendLayout();
            this.tabConfigDirectories.SuspendLayout();
            this.tabConfigSaving.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameAutosaveLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameAutosaveTime)).BeginInit();
            this.tabConfigEntities.SuspendLayout();
            this.tabConfigTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameLightmapScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameTextureScale)).BeginInit();
            this.tabBuild.SuspendLayout();
            this.BuildSubTabs.SuspendLayout();
            this.tabBuildGeneral.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.tabBuildExecutables.SuspendLayout();
            this.tabBuildAdvanced.SuspendLayout();
            this.tabBuildAdvancedSubTabs.SuspendLayout();
            this.tabBuildAdvancedCSG.SuspendLayout();
            this.tabBuildAdvancedBSP.SuspendLayout();
            this.tabBuildAdvancedVIS.SuspendLayout();
            this.tabBuildAdvancedRAD.SuspendLayout();
            this.tabBuildAdvancedShared.SuspendLayout();
            this.tabBuildAdvancedPreview.SuspendLayout();
            this.tabSteam.SuspendLayout();
            this.tabHotkeys.SuspendLayout();
            this.groupBox22.SuspendLayout();
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
            this.tbcSettings.SelectedIndexChanged += new System.EventHandler(this.TabChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.flowLayoutPanel1);
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
            this.tab2DViews.Controls.Add(this.groupBox11);
            this.tab2DViews.Controls.Add(this.groupBox6);
            this.tab2DViews.Controls.Add(this.groupBox3);
            this.tab2DViews.Controls.Add(this.groupBox17);
            this.tab2DViews.Controls.Add(this.groupBox16);
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
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.CenterHandlesOnlyNearCursor);
            this.groupBox11.Controls.Add(this.CenterHandlesActiveViewportOnly);
            this.groupBox11.Controls.Add(this.DrawCenterHandles);
            this.groupBox11.Controls.Add(this.BoxSelectByHandlesOnly);
            this.groupBox11.Controls.Add(this.ClickSelectByHandlesOnly);
            this.groupBox11.Location = new System.Drawing.Point(6, 401);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(468, 107);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Center Handles";
            // 
            // CenterHandlesOnlyNearCursor
            // 
            this.CenterHandlesOnlyNearCursor.Location = new System.Drawing.Point(10, 77);
            this.CenterHandlesOnlyNearCursor.Name = "CenterHandlesOnlyNearCursor";
            this.CenterHandlesOnlyNearCursor.Size = new System.Drawing.Size(182, 24);
            this.CenterHandlesOnlyNearCursor.TabIndex = 0;
            this.CenterHandlesOnlyNearCursor.Tag = "";
            this.CenterHandlesOnlyNearCursor.Text = "Render only near cursor position";
            this.CenterHandlesOnlyNearCursor.UseVisualStyleBackColor = true;
            // 
            // CenterHandlesActiveViewportOnly
            // 
            this.CenterHandlesActiveViewportOnly.Location = new System.Drawing.Point(10, 47);
            this.CenterHandlesActiveViewportOnly.Name = "CenterHandlesActiveViewportOnly";
            this.CenterHandlesActiveViewportOnly.Size = new System.Drawing.Size(182, 24);
            this.CenterHandlesActiveViewportOnly.TabIndex = 0;
            this.CenterHandlesActiveViewportOnly.Tag = "";
            this.CenterHandlesActiveViewportOnly.Text = "Render only in active viewport";
            this.CenterHandlesActiveViewportOnly.UseVisualStyleBackColor = true;
            // 
            // DrawCenterHandles
            // 
            this.DrawCenterHandles.Location = new System.Drawing.Point(10, 19);
            this.DrawCenterHandles.Name = "DrawCenterHandles";
            this.DrawCenterHandles.Size = new System.Drawing.Size(182, 24);
            this.DrawCenterHandles.TabIndex = 0;
            this.DrawCenterHandles.Tag = "";
            this.DrawCenterHandles.Text = "Render brush center handles";
            this.DrawCenterHandles.UseVisualStyleBackColor = true;
            // 
            // BoxSelectByHandlesOnly
            // 
            this.BoxSelectByHandlesOnly.Location = new System.Drawing.Point(198, 19);
            this.BoxSelectByHandlesOnly.Name = "BoxSelectByHandlesOnly";
            this.BoxSelectByHandlesOnly.Size = new System.Drawing.Size(257, 24);
            this.BoxSelectByHandlesOnly.TabIndex = 0;
            this.BoxSelectByHandlesOnly.Tag = "";
            this.BoxSelectByHandlesOnly.Text = "Selection box selects by center handles only";
            this.BoxSelectByHandlesOnly.UseVisualStyleBackColor = true;
            // 
            // ClickSelectByHandlesOnly
            // 
            this.ClickSelectByHandlesOnly.Location = new System.Drawing.Point(198, 47);
            this.ClickSelectByHandlesOnly.Name = "ClickSelectByHandlesOnly";
            this.ClickSelectByHandlesOnly.Size = new System.Drawing.Size(278, 24);
            this.ClickSelectByHandlesOnly.TabIndex = 0;
            this.ClickSelectByHandlesOnly.Tag = "";
            this.ClickSelectByHandlesOnly.Text = "Clicking in 2D view selects by center handles only";
            this.ClickSelectByHandlesOnly.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.RotationStyle_SnapNever);
            this.groupBox6.Controls.Add(this.RotationStyle_SnapOnShift);
            this.groupBox6.Controls.Add(this.RotationStyle_SnapOffShift);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(239, 114);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Rotation Style";
            // 
            // RotationStyle_SnapNever
            // 
            this.RotationStyle_SnapNever.Location = new System.Drawing.Point(11, 79);
            this.RotationStyle_SnapNever.Name = "RotationStyle_SnapNever";
            this.RotationStyle_SnapNever.Size = new System.Drawing.Size(137, 24);
            this.RotationStyle_SnapNever.TabIndex = 2;
            this.RotationStyle_SnapNever.Text = "No rotational snapping";
            this.RotationStyle_SnapNever.UseVisualStyleBackColor = true;
            // 
            // RotationStyle_SnapOnShift
            // 
            this.RotationStyle_SnapOnShift.Checked = true;
            this.RotationStyle_SnapOnShift.Location = new System.Drawing.Point(11, 19);
            this.RotationStyle_SnapOnShift.Name = "RotationStyle_SnapOnShift";
            this.RotationStyle_SnapOnShift.Size = new System.Drawing.Size(182, 24);
            this.RotationStyle_SnapOnShift.TabIndex = 2;
            this.RotationStyle_SnapOnShift.TabStop = true;
            this.RotationStyle_SnapOnShift.Text = "Press shift to snap to 15 degrees";
            this.RotationStyle_SnapOnShift.UseVisualStyleBackColor = true;
            // 
            // RotationStyle_SnapOffShift
            // 
            this.RotationStyle_SnapOffShift.Location = new System.Drawing.Point(11, 49);
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
            this.groupBox3.Location = new System.Drawing.Point(6, 217);
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
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.DrawEntityNames);
            this.groupBox17.Controls.Add(this.DrawBoxText);
            this.groupBox17.Controls.Add(this.KeepVisgroupsWhenCloning);
            this.groupBox17.Controls.Add(this.AutoSelectBox);
            this.groupBox17.Controls.Add(this.CrosshairCursorIn2DViews);
            this.groupBox17.Controls.Add(this.label31);
            this.groupBox17.Controls.Add(this.label30);
            this.groupBox17.Controls.Add(this.SelectionBoxBackgroundOpacity);
            this.groupBox17.Controls.Add(this.ScrollWheelZoomMultiplier);
            this.groupBox17.Location = new System.Drawing.Point(421, 126);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(304, 269);
            this.groupBox17.TabIndex = 0;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Options";
            // 
            // DrawEntityNames
            // 
            this.DrawEntityNames.Location = new System.Drawing.Point(10, 109);
            this.DrawEntityNames.Name = "DrawEntityNames";
            this.DrawEntityNames.Size = new System.Drawing.Size(225, 24);
            this.DrawEntityNames.TabIndex = 4;
            this.DrawEntityNames.Tag = "";
            this.DrawEntityNames.Text = "Draw entity names in the viewport";
            this.DrawEntityNames.UseVisualStyleBackColor = true;
            // 
            // DrawBoxText
            // 
            this.DrawBoxText.Location = new System.Drawing.Point(10, 79);
            this.DrawBoxText.Name = "DrawBoxText";
            this.DrawBoxText.Size = new System.Drawing.Size(225, 24);
            this.DrawBoxText.TabIndex = 4;
            this.DrawBoxText.Tag = "";
            this.DrawBoxText.Text = "Draw selection box size in the viewport";
            this.DrawBoxText.UseVisualStyleBackColor = true;
            // 
            // KeepVisgroupsWhenCloning
            // 
            this.KeepVisgroupsWhenCloning.Location = new System.Drawing.Point(10, 139);
            this.KeepVisgroupsWhenCloning.Name = "KeepVisgroupsWhenCloning";
            this.KeepVisgroupsWhenCloning.Size = new System.Drawing.Size(278, 24);
            this.KeepVisgroupsWhenCloning.TabIndex = 2;
            this.KeepVisgroupsWhenCloning.Tag = "";
            this.KeepVisgroupsWhenCloning.Text = "Keep visgroups when cloning";
            this.KeepVisgroupsWhenCloning.UseVisualStyleBackColor = true;
            // 
            // AutoSelectBox
            // 
            this.AutoSelectBox.Location = new System.Drawing.Point(10, 49);
            this.AutoSelectBox.Name = "AutoSelectBox";
            this.AutoSelectBox.Size = new System.Drawing.Size(225, 24);
            this.AutoSelectBox.TabIndex = 0;
            this.AutoSelectBox.Tag = "";
            this.AutoSelectBox.Text = "Automatically select when box is drawn";
            this.AutoSelectBox.UseVisualStyleBackColor = true;
            // 
            // CrosshairCursorIn2DViews
            // 
            this.CrosshairCursorIn2DViews.Location = new System.Drawing.Point(10, 19);
            this.CrosshairCursorIn2DViews.Name = "CrosshairCursorIn2DViews";
            this.CrosshairCursorIn2DViews.Size = new System.Drawing.Size(225, 24);
            this.CrosshairCursorIn2DViews.TabIndex = 0;
            this.CrosshairCursorIn2DViews.Tag = "";
            this.CrosshairCursorIn2DViews.Text = "Crosshair cursor in 2D views";
            this.CrosshairCursorIn2DViews.UseVisualStyleBackColor = true;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(12, 192);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(204, 20);
            this.label31.TabIndex = 3;
            this.label31.Text = "Selection box background opacity";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(12, 166);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(204, 20);
            this.label30.TabIndex = 3;
            this.label30.Text = "Scroll wheel zoom multiplier (default 1.20)";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectionBoxBackgroundOpacity
            // 
            this.SelectionBoxBackgroundOpacity.Location = new System.Drawing.Point(222, 194);
            this.SelectionBoxBackgroundOpacity.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.SelectionBoxBackgroundOpacity.Name = "SelectionBoxBackgroundOpacity";
            this.SelectionBoxBackgroundOpacity.Size = new System.Drawing.Size(50, 20);
            this.SelectionBoxBackgroundOpacity.TabIndex = 2;
            this.SelectionBoxBackgroundOpacity.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // ScrollWheelZoomMultiplier
            // 
            this.ScrollWheelZoomMultiplier.DecimalPlaces = 2;
            this.ScrollWheelZoomMultiplier.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScrollWheelZoomMultiplier.Location = new System.Drawing.Point(222, 168);
            this.ScrollWheelZoomMultiplier.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ScrollWheelZoomMultiplier.Minimum = new decimal(new int[] {
            101,
            0,
            0,
            131072});
            this.ScrollWheelZoomMultiplier.Name = "ScrollWheelZoomMultiplier";
            this.ScrollWheelZoomMultiplier.Size = new System.Drawing.Size(50, 20);
            this.ScrollWheelZoomMultiplier.TabIndex = 2;
            this.ScrollWheelZoomMultiplier.Value = new decimal(new int[] {
            12,
            0,
            0,
            65536});
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.ArrowKeysNudgeSelection);
            this.groupBox16.Controls.Add(this.NudgeStyle_GridOnCtrl);
            this.groupBox16.Controls.Add(this.NudgeStyle_GridOffCtrl);
            this.groupBox16.Controls.Add(this.label2);
            this.groupBox16.Controls.Add(this.NudgeUnits);
            this.groupBox16.Location = new System.Drawing.Point(469, 6);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(256, 114);
            this.groupBox16.TabIndex = 0;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Nudge Grid";
            // 
            // ArrowKeysNudgeSelection
            // 
            this.ArrowKeysNudgeSelection.Checked = true;
            this.ArrowKeysNudgeSelection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ArrowKeysNudgeSelection.Location = new System.Drawing.Point(12, 19);
            this.ArrowKeysNudgeSelection.Name = "ArrowKeysNudgeSelection";
            this.ArrowKeysNudgeSelection.Size = new System.Drawing.Size(126, 24);
            this.ArrowKeysNudgeSelection.TabIndex = 0;
            this.ArrowKeysNudgeSelection.Text = "Arrow keys nudge by";
            this.ArrowKeysNudgeSelection.UseVisualStyleBackColor = true;
            // 
            // NudgeStyle_GridOnCtrl
            // 
            this.NudgeStyle_GridOnCtrl.Checked = true;
            this.NudgeStyle_GridOnCtrl.Location = new System.Drawing.Point(12, 49);
            this.NudgeStyle_GridOnCtrl.Name = "NudgeStyle_GridOnCtrl";
            this.NudgeStyle_GridOnCtrl.Size = new System.Drawing.Size(236, 24);
            this.NudgeStyle_GridOnCtrl.TabIndex = 2;
            this.NudgeStyle_GridOnCtrl.TabStop = true;
            this.NudgeStyle_GridOnCtrl.Text = "Press control to nudge by grid width";
            this.NudgeStyle_GridOnCtrl.UseVisualStyleBackColor = true;
            // 
            // NudgeStyle_GridOffCtrl
            // 
            this.NudgeStyle_GridOffCtrl.Location = new System.Drawing.Point(12, 79);
            this.NudgeStyle_GridOffCtrl.Name = "NudgeStyle_GridOffCtrl";
            this.NudgeStyle_GridOffCtrl.Size = new System.Drawing.Size(238, 24);
            this.NudgeStyle_GridOffCtrl.TabIndex = 2;
            this.NudgeStyle_GridOffCtrl.Text = "Nudge by grid width unless control is pressed";
            this.NudgeStyle_GridOffCtrl.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(195, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "unit(s)";
            // 
            // NudgeUnits
            // 
            this.NudgeUnits.DecimalPlaces = 2;
            this.NudgeUnits.Location = new System.Drawing.Point(138, 20);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SnapStyle_SnapOffAlt);
            this.groupBox4.Controls.Add(this.SnapStyle_SnapOnAlt);
            this.groupBox4.Location = new System.Drawing.Point(251, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(212, 114);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Snap to Grid";
            // 
            // SnapStyle_SnapOffAlt
            // 
            this.SnapStyle_SnapOffAlt.Checked = true;
            this.SnapStyle_SnapOffAlt.Location = new System.Drawing.Point(10, 19);
            this.SnapStyle_SnapOffAlt.Name = "SnapStyle_SnapOffAlt";
            this.SnapStyle_SnapOffAlt.Size = new System.Drawing.Size(182, 24);
            this.SnapStyle_SnapOffAlt.TabIndex = 2;
            this.SnapStyle_SnapOffAlt.TabStop = true;
            this.SnapStyle_SnapOffAlt.Text = "Hold alt to ignore snapping";
            this.SnapStyle_SnapOffAlt.UseVisualStyleBackColor = true;
            // 
            // SnapStyle_SnapOnAlt
            // 
            this.SnapStyle_SnapOnAlt.Location = new System.Drawing.Point(10, 49);
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
            this.groupBox5.Location = new System.Drawing.Point(6, 126);
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
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label39);
            this.groupBox12.Controls.Add(this.ViewportBackgroundColour);
            this.groupBox12.Controls.Add(this.CameraFOV);
            this.groupBox12.Controls.Add(this.label29);
            this.groupBox12.Location = new System.Drawing.Point(438, 6);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(292, 91);
            this.groupBox12.TabIndex = 4;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "General";
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(12, 46);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(74, 17);
            this.label39.TabIndex = 3;
            this.label39.Text = "Background:";
            // 
            // ViewportBackgroundColour
            // 
            this.ViewportBackgroundColour.BackColor = System.Drawing.Color.Black;
            this.ViewportBackgroundColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ViewportBackgroundColour.Location = new System.Drawing.Point(92, 46);
            this.ViewportBackgroundColour.Name = "ViewportBackgroundColour";
            this.ViewportBackgroundColour.Size = new System.Drawing.Size(51, 17);
            this.ViewportBackgroundColour.TabIndex = 4;
            // 
            // CameraFOV
            // 
            this.CameraFOV.Location = new System.Drawing.Point(92, 20);
            this.CameraFOV.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.CameraFOV.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.CameraFOV.Name = "CameraFOV";
            this.CameraFOV.Size = new System.Drawing.Size(50, 20);
            this.CameraFOV.TabIndex = 1;
            this.CameraFOV.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(12, 22);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(70, 13);
            this.label29.TabIndex = 0;
            this.label29.Text = "Camera FOV:";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.label43);
            this.groupBox13.Controls.Add(this.MouseWheelMoveDistance);
            this.groupBox13.Controls.Add(this.InvertMouseX);
            this.groupBox13.Controls.Add(this.TimeToTopSpeedLabel);
            this.groupBox13.Controls.Add(this.InvertMouseY);
            this.groupBox13.Controls.Add(this.TimeToTopSpeed);
            this.groupBox13.Controls.Add(this.label28);
            this.groupBox13.Controls.Add(this.label27);
            this.groupBox13.Controls.Add(this.ForwardSpeedLabel);
            this.groupBox13.Controls.Add(this.ForwardSpeed);
            this.groupBox13.Location = new System.Drawing.Point(6, 187);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(426, 180);
            this.groupBox13.TabIndex = 2;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Navigation";
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(211, 119);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(148, 20);
            this.label43.TabIndex = 6;
            this.label43.Text = "Mouse wheel move distance";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MouseWheelMoveDistance
            // 
            this.MouseWheelMoveDistance.Location = new System.Drawing.Point(365, 121);
            this.MouseWheelMoveDistance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MouseWheelMoveDistance.Name = "MouseWheelMoveDistance";
            this.MouseWheelMoveDistance.Size = new System.Drawing.Size(50, 20);
            this.MouseWheelMoveDistance.TabIndex = 5;
            this.MouseWheelMoveDistance.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // InvertMouseX
            // 
            this.InvertMouseX.Location = new System.Drawing.Point(12, 148);
            this.InvertMouseX.Name = "InvertMouseX";
            this.InvertMouseX.Size = new System.Drawing.Size(149, 24);
            this.InvertMouseX.TabIndex = 0;
            this.InvertMouseX.Text = "Invert Mouselook X Axis";
            this.InvertMouseX.UseVisualStyleBackColor = true;
            // 
            // TimeToTopSpeedLabel
            // 
            this.TimeToTopSpeedLabel.Location = new System.Drawing.Point(354, 68);
            this.TimeToTopSpeedLabel.Name = "TimeToTopSpeedLabel";
            this.TimeToTopSpeedLabel.Size = new System.Drawing.Size(65, 42);
            this.TimeToTopSpeedLabel.TabIndex = 1;
            this.TimeToTopSpeedLabel.Text = "0.5\r\nseconds";
            this.TimeToTopSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InvertMouseY
            // 
            this.InvertMouseY.Location = new System.Drawing.Point(12, 118);
            this.InvertMouseY.Name = "InvertMouseY";
            this.InvertMouseY.Size = new System.Drawing.Size(149, 24);
            this.InvertMouseY.TabIndex = 0;
            this.InvertMouseY.Text = "Invert Mouselook Y Axis";
            this.InvertMouseY.UseVisualStyleBackColor = true;
            // 
            // TimeToTopSpeed
            // 
            this.TimeToTopSpeed.AutoSize = false;
            this.TimeToTopSpeed.BackColor = System.Drawing.SystemColors.Window;
            this.TimeToTopSpeed.Location = new System.Drawing.Point(125, 68);
            this.TimeToTopSpeed.Maximum = 50;
            this.TimeToTopSpeed.Name = "TimeToTopSpeed";
            this.TimeToTopSpeed.Size = new System.Drawing.Size(231, 42);
            this.TimeToTopSpeed.TabIndex = 0;
            this.TimeToTopSpeed.TickFrequency = 10000;
            this.TimeToTopSpeed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TimeToTopSpeed.Value = 5;
            this.TimeToTopSpeed.Scroll += new System.EventHandler(this.TimeToTopSpeedChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(9, 83);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(92, 13);
            this.label28.TabIndex = 4;
            this.label28.Text = "Time to top speed";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(9, 35);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(77, 13);
            this.label27.TabIndex = 4;
            this.label27.Text = "Forward speed";
            // 
            // ForwardSpeedLabel
            // 
            this.ForwardSpeedLabel.Location = new System.Drawing.Point(362, 20);
            this.ForwardSpeedLabel.Name = "ForwardSpeedLabel";
            this.ForwardSpeedLabel.Size = new System.Drawing.Size(57, 42);
            this.ForwardSpeedLabel.TabIndex = 1;
            this.ForwardSpeedLabel.Text = "1000\r\nunits/sec";
            this.ForwardSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ForwardSpeed
            // 
            this.ForwardSpeed.AutoSize = false;
            this.ForwardSpeed.BackColor = System.Drawing.SystemColors.Window;
            this.ForwardSpeed.Location = new System.Drawing.Point(125, 20);
            this.ForwardSpeed.Maximum = 5000;
            this.ForwardSpeed.Minimum = 100;
            this.ForwardSpeed.Name = "ForwardSpeed";
            this.ForwardSpeed.Size = new System.Drawing.Size(231, 42);
            this.ForwardSpeed.TabIndex = 0;
            this.ForwardSpeed.TickFrequency = 10000;
            this.ForwardSpeed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ForwardSpeed.Value = 1000;
            this.ForwardSpeed.Scroll += new System.EventHandler(this.ForwardSpeedChanged);
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label26);
            this.groupBox14.Controls.Add(this.label24);
            this.groupBox14.Controls.Add(this.label22);
            this.groupBox14.Controls.Add(this.DetailRenderDistance);
            this.groupBox14.Controls.Add(this.DetailRenderDistanceLabel);
            this.groupBox14.Controls.Add(this.ModelRenderDistance);
            this.groupBox14.Controls.Add(this.ModelRenderDistanceLabel);
            this.groupBox14.Controls.Add(this.BackClippingPane);
            this.groupBox14.Controls.Add(this.BackClippingPaneLabel);
            this.groupBox14.Location = new System.Drawing.Point(6, 6);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(426, 175);
            this.groupBox14.TabIndex = 2;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Performance";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(7, 136);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(110, 13);
            this.label26.TabIndex = 4;
            this.label26.Text = "Detail render distance";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(7, 81);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(112, 13);
            this.label24.TabIndex = 4;
            this.label24.Text = "Model render distance";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 34);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(100, 13);
            this.label22.TabIndex = 4;
            this.label22.Text = "Back clipping plane";
            // 
            // DetailRenderDistance
            // 
            this.DetailRenderDistance.AutoSize = false;
            this.DetailRenderDistance.BackColor = System.Drawing.SystemColors.Window;
            this.DetailRenderDistance.Location = new System.Drawing.Point(125, 122);
            this.DetailRenderDistance.Maximum = 10000;
            this.DetailRenderDistance.Minimum = 200;
            this.DetailRenderDistance.Name = "DetailRenderDistance";
            this.DetailRenderDistance.Size = new System.Drawing.Size(232, 41);
            this.DetailRenderDistance.TabIndex = 0;
            this.DetailRenderDistance.TickFrequency = 10000;
            this.DetailRenderDistance.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.DetailRenderDistance.Value = 5000;
            this.DetailRenderDistance.Scroll += new System.EventHandler(this.DetailRenderDistanceChanged);
            // 
            // DetailRenderDistanceLabel
            // 
            this.DetailRenderDistanceLabel.Location = new System.Drawing.Point(351, 131);
            this.DetailRenderDistanceLabel.Name = "DetailRenderDistanceLabel";
            this.DetailRenderDistanceLabel.Size = new System.Drawing.Size(69, 23);
            this.DetailRenderDistanceLabel.TabIndex = 1;
            this.DetailRenderDistanceLabel.Text = "4000";
            this.DetailRenderDistanceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ModelRenderDistance
            // 
            this.ModelRenderDistance.AutoSize = false;
            this.ModelRenderDistance.BackColor = System.Drawing.SystemColors.Window;
            this.ModelRenderDistance.Location = new System.Drawing.Point(125, 67);
            this.ModelRenderDistance.Maximum = 10000;
            this.ModelRenderDistance.Minimum = 200;
            this.ModelRenderDistance.Name = "ModelRenderDistance";
            this.ModelRenderDistance.Size = new System.Drawing.Size(232, 41);
            this.ModelRenderDistance.TabIndex = 0;
            this.ModelRenderDistance.TickFrequency = 10000;
            this.ModelRenderDistance.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.ModelRenderDistance.Value = 5000;
            this.ModelRenderDistance.Scroll += new System.EventHandler(this.ModelRenderDistanceChanged);
            // 
            // ModelRenderDistanceLabel
            // 
            this.ModelRenderDistanceLabel.Location = new System.Drawing.Point(351, 76);
            this.ModelRenderDistanceLabel.Name = "ModelRenderDistanceLabel";
            this.ModelRenderDistanceLabel.Size = new System.Drawing.Size(69, 23);
            this.ModelRenderDistanceLabel.TabIndex = 1;
            this.ModelRenderDistanceLabel.Text = "4000";
            this.ModelRenderDistanceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BackClippingPane
            // 
            this.BackClippingPane.AutoSize = false;
            this.BackClippingPane.BackColor = System.Drawing.SystemColors.Window;
            this.BackClippingPane.Location = new System.Drawing.Point(124, 20);
            this.BackClippingPane.Maximum = 10000;
            this.BackClippingPane.Minimum = 2000;
            this.BackClippingPane.Name = "BackClippingPane";
            this.BackClippingPane.Size = new System.Drawing.Size(232, 41);
            this.BackClippingPane.TabIndex = 0;
            this.BackClippingPane.TickFrequency = 10000;
            this.BackClippingPane.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.BackClippingPane.Value = 5000;
            this.BackClippingPane.Scroll += new System.EventHandler(this.BackClippingPaneChanged);
            // 
            // BackClippingPaneLabel
            // 
            this.BackClippingPaneLabel.Location = new System.Drawing.Point(350, 29);
            this.BackClippingPaneLabel.Name = "BackClippingPaneLabel";
            this.BackClippingPaneLabel.Size = new System.Drawing.Size(69, 23);
            this.BackClippingPaneLabel.TabIndex = 1;
            this.BackClippingPaneLabel.Text = "4000";
            this.BackClippingPaneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabGame
            // 
            this.tabGame.Controls.Add(this.GameSubTabs);
            this.tabGame.Controls.Add(this.GameTree);
            this.tabGame.Controls.Add(this.RemoveGame);
            this.tabGame.Controls.Add(this.AddGame);
            this.tabGame.Location = new System.Drawing.Point(4, 22);
            this.tabGame.Name = "tabGame";
            this.tabGame.Padding = new System.Windows.Forms.Padding(3);
            this.tabGame.Size = new System.Drawing.Size(736, 511);
            this.tabGame.TabIndex = 2;
            this.tabGame.Text = "Game Configurations";
            this.tabGame.UseVisualStyleBackColor = true;
            // 
            // GameSubTabs
            // 
            this.GameSubTabs.Controls.Add(this.tabConfigDirectories);
            this.GameSubTabs.Controls.Add(this.tabConfigSaving);
            this.GameSubTabs.Controls.Add(this.tabConfigEntities);
            this.GameSubTabs.Controls.Add(this.tabConfigTextures);
            this.GameSubTabs.ItemSize = new System.Drawing.Size(58, 18);
            this.GameSubTabs.Location = new System.Drawing.Point(245, 6);
            this.GameSubTabs.Name = "GameSubTabs";
            this.GameSubTabs.SelectedIndex = 0;
            this.GameSubTabs.Size = new System.Drawing.Size(477, 472);
            this.GameSubTabs.TabIndex = 23;
            this.GameSubTabs.Visible = false;
            // 
            // tabConfigDirectories
            // 
            this.tabConfigDirectories.Controls.Add(this.SelectedGameRunArguments);
            this.tabConfigDirectories.Controls.Add(this.lblBaseGame);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameSteamInstall);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameBase);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameWonDir);
            this.tabConfigDirectories.Controls.Add(this.lblGameName);
            this.tabConfigDirectories.Controls.Add(this.lblGameWONDir);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameBuild);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameDirBrowse);
            this.tabConfigDirectories.Controls.Add(this.lblGameSteamDir);
            this.tabConfigDirectories.Controls.Add(this.lblGameBuild);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameSteamDir);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameName);
            this.tabConfigDirectories.Controls.Add(this.label42);
            this.tabConfigDirectories.Controls.Add(this.label41);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameEngine);
            this.tabConfigDirectories.Controls.Add(this.lblGameMod);
            this.tabConfigDirectories.Controls.Add(this.lblGameEngine);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameExecutable);
            this.tabConfigDirectories.Controls.Add(this.SelectedGameMod);
            this.tabConfigDirectories.Location = new System.Drawing.Point(4, 22);
            this.tabConfigDirectories.Name = "tabConfigDirectories";
            this.tabConfigDirectories.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigDirectories.Size = new System.Drawing.Size(469, 446);
            this.tabConfigDirectories.TabIndex = 0;
            this.tabConfigDirectories.Text = "General";
            this.tabConfigDirectories.UseVisualStyleBackColor = true;
            // 
            // SelectedGameRunArguments
            // 
            this.SelectedGameRunArguments.Location = new System.Drawing.Point(15, 233);
            this.SelectedGameRunArguments.Name = "SelectedGameRunArguments";
            this.SelectedGameRunArguments.Size = new System.Drawing.Size(437, 20);
            this.SelectedGameRunArguments.TabIndex = 34;
            this.SelectedGameRunArguments.Text = "-dev -console";
            // 
            // lblBaseGame
            // 
            this.lblBaseGame.Location = new System.Drawing.Point(9, 127);
            this.lblBaseGame.Name = "lblBaseGame";
            this.lblBaseGame.Size = new System.Drawing.Size(198, 20);
            this.lblBaseGame.TabIndex = 11;
            this.lblBaseGame.Text = "Base Game Directory (e.g. \'valve\')";
            this.lblBaseGame.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameSteamInstall
            // 
            this.SelectedGameSteamInstall.Checked = true;
            this.SelectedGameSteamInstall.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedGameSteamInstall.Location = new System.Drawing.Point(217, 35);
            this.SelectedGameSteamInstall.Name = "SelectedGameSteamInstall";
            this.SelectedGameSteamInstall.Size = new System.Drawing.Size(109, 24);
            this.SelectedGameSteamInstall.TabIndex = 21;
            this.SelectedGameSteamInstall.Text = "Steam Install";
            this.SelectedGameSteamInstall.UseVisualStyleBackColor = true;
            this.SelectedGameSteamInstall.CheckedChanged += new System.EventHandler(this.SelectedGameEngineChanged);
            // 
            // SelectedGameBase
            // 
            this.SelectedGameBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameBase.FormattingEnabled = true;
            this.SelectedGameBase.Items.AddRange(new object[] {
            "(Steam only) Half-Life",
            "Counter-Strike"});
            this.SelectedGameBase.Location = new System.Drawing.Point(217, 128);
            this.SelectedGameBase.Name = "SelectedGameBase";
            this.SelectedGameBase.Size = new System.Drawing.Size(152, 21);
            this.SelectedGameBase.TabIndex = 12;
            // 
            // SelectedGameWonDir
            // 
            this.SelectedGameWonDir.Location = new System.Drawing.Point(81, 100);
            this.SelectedGameWonDir.Name = "SelectedGameWonDir";
            this.SelectedGameWonDir.Size = new System.Drawing.Size(288, 20);
            this.SelectedGameWonDir.TabIndex = 5;
            this.SelectedGameWonDir.Text = "(WON only) example: C:\\Sierra\\Half-Life";
            this.SelectedGameWonDir.TextChanged += new System.EventHandler(this.SelectedGameWonDirChanged);
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
            // lblGameWONDir
            // 
            this.lblGameWONDir.Location = new System.Drawing.Point(12, 100);
            this.lblGameWONDir.Name = "lblGameWONDir";
            this.lblGameWONDir.Size = new System.Drawing.Size(63, 20);
            this.lblGameWONDir.TabIndex = 6;
            this.lblGameWONDir.Text = "Game Dir";
            this.lblGameWONDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameBuild
            // 
            this.SelectedGameBuild.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameBuild.FormattingEnabled = true;
            this.SelectedGameBuild.Location = new System.Drawing.Point(81, 62);
            this.SelectedGameBuild.Name = "SelectedGameBuild";
            this.SelectedGameBuild.Size = new System.Drawing.Size(121, 21);
            this.SelectedGameBuild.TabIndex = 10;
            // 
            // SelectedGameDirBrowse
            // 
            this.SelectedGameDirBrowse.Location = new System.Drawing.Point(375, 99);
            this.SelectedGameDirBrowse.Name = "SelectedGameDirBrowse";
            this.SelectedGameDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.SelectedGameDirBrowse.TabIndex = 8;
            this.SelectedGameDirBrowse.Text = "Browse...";
            this.SelectedGameDirBrowse.UseVisualStyleBackColor = true;
            this.SelectedGameDirBrowse.Click += new System.EventHandler(this.SelectedGameDirBrowseClicked);
            // 
            // lblGameSteamDir
            // 
            this.lblGameSteamDir.Location = new System.Drawing.Point(22, 100);
            this.lblGameSteamDir.Name = "lblGameSteamDir";
            this.lblGameSteamDir.Size = new System.Drawing.Size(45, 20);
            this.lblGameSteamDir.TabIndex = 9;
            this.lblGameSteamDir.Text = "Game";
            this.lblGameSteamDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // SelectedGameSteamDir
            // 
            this.SelectedGameSteamDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameSteamDir.FormattingEnabled = true;
            this.SelectedGameSteamDir.Items.AddRange(new object[] {
            "(Steam only) Half-Life",
            "Counter-Strike"});
            this.SelectedGameSteamDir.Location = new System.Drawing.Point(81, 99);
            this.SelectedGameSteamDir.Name = "SelectedGameSteamDir";
            this.SelectedGameSteamDir.Size = new System.Drawing.Size(259, 21);
            this.SelectedGameSteamDir.TabIndex = 10;
            this.SelectedGameSteamDir.SelectedIndexChanged += new System.EventHandler(this.SelectedGameSteamDirChanged);
            // 
            // SelectedGameName
            // 
            this.SelectedGameName.Location = new System.Drawing.Point(81, 9);
            this.SelectedGameName.Name = "SelectedGameName";
            this.SelectedGameName.Size = new System.Drawing.Size(133, 20);
            this.SelectedGameName.TabIndex = 5;
            this.SelectedGameName.TextChanged += new System.EventHandler(this.SelectedGameNameChanged);
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(6, 210);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(211, 20);
            this.label42.TabIndex = 9;
            this.label42.Text = "Game Run Arguments (e.g. -dev -console)";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(10, 181);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(198, 20);
            this.label41.TabIndex = 9;
            this.label41.Text = "Game Executable (e.g. \'hl.exe\')";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameEngine
            // 
            this.SelectedGameEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameEngine.FormattingEnabled = true;
            this.SelectedGameEngine.Items.AddRange(new object[] {
            "Goldsource (WON)",
            "Goldsource (Steam)",
            "Source"});
            this.SelectedGameEngine.Location = new System.Drawing.Point(81, 35);
            this.SelectedGameEngine.Name = "SelectedGameEngine";
            this.SelectedGameEngine.Size = new System.Drawing.Size(121, 21);
            this.SelectedGameEngine.TabIndex = 7;
            this.SelectedGameEngine.SelectedIndexChanged += new System.EventHandler(this.SelectedGameEngineChanged);
            // 
            // lblGameMod
            // 
            this.lblGameMod.Location = new System.Drawing.Point(9, 154);
            this.lblGameMod.Name = "lblGameMod";
            this.lblGameMod.Size = new System.Drawing.Size(198, 20);
            this.lblGameMod.TabIndex = 9;
            this.lblGameMod.Text = "Mod Directory (e.g. \'cstrike\')";
            this.lblGameMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // SelectedGameExecutable
            // 
            this.SelectedGameExecutable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameExecutable.FormattingEnabled = true;
            this.SelectedGameExecutable.Items.AddRange(new object[] {
            "Valve"});
            this.SelectedGameExecutable.Location = new System.Drawing.Point(217, 182);
            this.SelectedGameExecutable.Name = "SelectedGameExecutable";
            this.SelectedGameExecutable.Size = new System.Drawing.Size(152, 21);
            this.SelectedGameExecutable.TabIndex = 10;
            // 
            // SelectedGameMod
            // 
            this.SelectedGameMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameMod.FormattingEnabled = true;
            this.SelectedGameMod.Items.AddRange(new object[] {
            "Valve"});
            this.SelectedGameMod.Location = new System.Drawing.Point(216, 155);
            this.SelectedGameMod.Name = "SelectedGameMod";
            this.SelectedGameMod.Size = new System.Drawing.Size(152, 21);
            this.SelectedGameMod.TabIndex = 10;
            // 
            // tabConfigSaving
            // 
            this.tabConfigSaving.Controls.Add(this.SelectedGameAutosaveLimit);
            this.tabConfigSaving.Controls.Add(this.label16);
            this.tabConfigSaving.Controls.Add(this.SelectedGameMapDirBrowse);
            this.tabConfigSaving.Controls.Add(this.SelectedGameAutosaveTime);
            this.tabConfigSaving.Controls.Add(this.SelectedGameUseDiffAutosaveDir);
            this.tabConfigSaving.Controls.Add(this.label15);
            this.tabConfigSaving.Controls.Add(this.SelectedGameAutosaveOnlyOnChange);
            this.tabConfigSaving.Controls.Add(this.label14);
            this.tabConfigSaving.Controls.Add(this.SelectedGameDiffAutosaveDirBrowse);
            this.tabConfigSaving.Controls.Add(this.label12);
            this.tabConfigSaving.Controls.Add(this.lblGameAutosaveDir);
            this.tabConfigSaving.Controls.Add(this.lblGameMapSaveDir);
            this.tabConfigSaving.Controls.Add(this.SelectedGameDiffAutosaveDir);
            this.tabConfigSaving.Controls.Add(this.SelectedGameEnableAutosave);
            this.tabConfigSaving.Controls.Add(this.SelectedGameMapDir);
            this.tabConfigSaving.Location = new System.Drawing.Point(4, 22);
            this.tabConfigSaving.Name = "tabConfigSaving";
            this.tabConfigSaving.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigSaving.Size = new System.Drawing.Size(469, 446);
            this.tabConfigSaving.TabIndex = 3;
            this.tabConfigSaving.Text = "Saving";
            this.tabConfigSaving.UseVisualStyleBackColor = true;
            // 
            // SelectedGameAutosaveLimit
            // 
            this.SelectedGameAutosaveLimit.Location = new System.Drawing.Point(99, 147);
            this.SelectedGameAutosaveLimit.Name = "SelectedGameAutosaveLimit";
            this.SelectedGameAutosaveLimit.Size = new System.Drawing.Size(50, 20);
            this.SelectedGameAutosaveLimit.TabIndex = 20;
            this.SelectedGameAutosaveLimit.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(155, 149);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(166, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "autosave(s) on disk (0 to keep all)";
            // 
            // SelectedGameMapDirBrowse
            // 
            this.SelectedGameMapDirBrowse.Location = new System.Drawing.Point(329, 7);
            this.SelectedGameMapDirBrowse.Name = "SelectedGameMapDirBrowse";
            this.SelectedGameMapDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.SelectedGameMapDirBrowse.TabIndex = 8;
            this.SelectedGameMapDirBrowse.Text = "Browse...";
            this.SelectedGameMapDirBrowse.UseVisualStyleBackColor = true;
            this.SelectedGameMapDirBrowse.Click += new System.EventHandler(this.SelectedGameMapDirBrowseClicked);
            // 
            // SelectedGameAutosaveTime
            // 
            this.SelectedGameAutosaveTime.Location = new System.Drawing.Point(99, 121);
            this.SelectedGameAutosaveTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.SelectedGameAutosaveTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SelectedGameAutosaveTime.Name = "SelectedGameAutosaveTime";
            this.SelectedGameAutosaveTime.Size = new System.Drawing.Size(50, 20);
            this.SelectedGameAutosaveTime.TabIndex = 20;
            this.SelectedGameAutosaveTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // SelectedGameUseDiffAutosaveDir
            // 
            this.SelectedGameUseDiffAutosaveDir.Checked = true;
            this.SelectedGameUseDiffAutosaveDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedGameUseDiffAutosaveDir.Location = new System.Drawing.Point(98, 65);
            this.SelectedGameUseDiffAutosaveDir.Name = "SelectedGameUseDiffAutosaveDir";
            this.SelectedGameUseDiffAutosaveDir.Size = new System.Drawing.Size(225, 20);
            this.SelectedGameUseDiffAutosaveDir.TabIndex = 14;
            this.SelectedGameUseDiffAutosaveDir.Text = "Use a different directory for autosaves";
            this.SelectedGameUseDiffAutosaveDir.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 149);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "Keep the last";
            // 
            // SelectedGameAutosaveOnlyOnChange
            // 
            this.SelectedGameAutosaveOnlyOnChange.Checked = true;
            this.SelectedGameAutosaveOnlyOnChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedGameAutosaveOnlyOnChange.Location = new System.Drawing.Point(99, 173);
            this.SelectedGameAutosaveOnlyOnChange.Name = "SelectedGameAutosaveOnlyOnChange";
            this.SelectedGameAutosaveOnlyOnChange.Size = new System.Drawing.Size(225, 20);
            this.SelectedGameAutosaveOnlyOnChange.TabIndex = 14;
            this.SelectedGameAutosaveOnlyOnChange.Text = "Only autosave if changes detected";
            this.SelectedGameAutosaveOnlyOnChange.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(155, 123);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 13);
            this.label14.TabIndex = 19;
            this.label14.Text = "minute(s)";
            // 
            // SelectedGameDiffAutosaveDirBrowse
            // 
            this.SelectedGameDiffAutosaveDirBrowse.Location = new System.Drawing.Point(329, 89);
            this.SelectedGameDiffAutosaveDirBrowse.Name = "SelectedGameDiffAutosaveDirBrowse";
            this.SelectedGameDiffAutosaveDirBrowse.Size = new System.Drawing.Size(67, 23);
            this.SelectedGameDiffAutosaveDirBrowse.TabIndex = 13;
            this.SelectedGameDiffAutosaveDirBrowse.Text = "Browse...";
            this.SelectedGameDiffAutosaveDirBrowse.UseVisualStyleBackColor = true;
            this.SelectedGameDiffAutosaveDirBrowse.Click += new System.EventHandler(this.SelectedGameDiffAutosaveDirBrowseClicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Autosave every";
            // 
            // lblGameAutosaveDir
            // 
            this.lblGameAutosaveDir.Location = new System.Drawing.Point(12, 91);
            this.lblGameAutosaveDir.Name = "lblGameAutosaveDir";
            this.lblGameAutosaveDir.Size = new System.Drawing.Size(80, 20);
            this.lblGameAutosaveDir.TabIndex = 12;
            this.lblGameAutosaveDir.Text = "Autosave Dir";
            this.lblGameAutosaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblGameMapSaveDir
            // 
            this.lblGameMapSaveDir.Location = new System.Drawing.Point(12, 9);
            this.lblGameMapSaveDir.Name = "lblGameMapSaveDir";
            this.lblGameMapSaveDir.Size = new System.Drawing.Size(80, 20);
            this.lblGameMapSaveDir.TabIndex = 6;
            this.lblGameMapSaveDir.Text = "Map Save Dir";
            this.lblGameMapSaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameDiffAutosaveDir
            // 
            this.SelectedGameDiffAutosaveDir.BackColor = System.Drawing.SystemColors.Window;
            this.SelectedGameDiffAutosaveDir.Location = new System.Drawing.Point(98, 91);
            this.SelectedGameDiffAutosaveDir.Name = "SelectedGameDiffAutosaveDir";
            this.SelectedGameDiffAutosaveDir.Size = new System.Drawing.Size(225, 20);
            this.SelectedGameDiffAutosaveDir.TabIndex = 11;
            this.SelectedGameDiffAutosaveDir.Text = "Folder to put autosaves in";
            // 
            // SelectedGameEnableAutosave
            // 
            this.SelectedGameEnableAutosave.Checked = true;
            this.SelectedGameEnableAutosave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedGameEnableAutosave.Location = new System.Drawing.Point(98, 35);
            this.SelectedGameEnableAutosave.Name = "SelectedGameEnableAutosave";
            this.SelectedGameEnableAutosave.Size = new System.Drawing.Size(225, 24);
            this.SelectedGameEnableAutosave.TabIndex = 18;
            this.SelectedGameEnableAutosave.Text = "Enable autosave for this config";
            this.SelectedGameEnableAutosave.UseVisualStyleBackColor = true;
            // 
            // SelectedGameMapDir
            // 
            this.SelectedGameMapDir.Location = new System.Drawing.Point(98, 9);
            this.SelectedGameMapDir.Name = "SelectedGameMapDir";
            this.SelectedGameMapDir.Size = new System.Drawing.Size(225, 20);
            this.SelectedGameMapDir.TabIndex = 5;
            this.SelectedGameMapDir.Text = "Default folder to save VMF/RMF files";
            // 
            // tabConfigEntities
            // 
            this.tabConfigEntities.Controls.Add(this.SelectedGameFgdList);
            this.tabConfigEntities.Controls.Add(this.label38);
            this.tabConfigEntities.Controls.Add(this.SelectedGameDetectedSizeHigh);
            this.tabConfigEntities.Controls.Add(this.SelectedGameDetectedSizeLow);
            this.tabConfigEntities.Controls.Add(this.label36);
            this.tabConfigEntities.Controls.Add(this.SelectedGameOverrideSizeHigh);
            this.tabConfigEntities.Controls.Add(this.label37);
            this.tabConfigEntities.Controls.Add(this.label35);
            this.tabConfigEntities.Controls.Add(this.SelectedGameOverrideSizeLow);
            this.tabConfigEntities.Controls.Add(this.label33);
            this.tabConfigEntities.Controls.Add(this.SelectedGameOverrideMapSize);
            this.tabConfigEntities.Controls.Add(this.lblGameFGD);
            this.tabConfigEntities.Controls.Add(this.SelectedGameAddFgd);
            this.tabConfigEntities.Controls.Add(this.label34);
            this.tabConfigEntities.Controls.Add(this.lblConfigBrushEnt);
            this.tabConfigEntities.Controls.Add(this.SelectedGameRemoveFgd);
            this.tabConfigEntities.Controls.Add(this.lblConfigPointEnt);
            this.tabConfigEntities.Controls.Add(this.SelectedGameDefaultBrushEnt);
            this.tabConfigEntities.Controls.Add(this.SelectedGameDefaultPointEnt);
            this.tabConfigEntities.Location = new System.Drawing.Point(4, 22);
            this.tabConfigEntities.Name = "tabConfigEntities";
            this.tabConfigEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigEntities.Size = new System.Drawing.Size(469, 446);
            this.tabConfigEntities.TabIndex = 1;
            this.tabConfigEntities.Text = "Entities";
            this.tabConfigEntities.UseVisualStyleBackColor = true;
            // 
            // SelectedGameFgdList
            // 
            this.SelectedGameFgdList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4});
            this.SelectedGameFgdList.FullRowSelect = true;
            this.SelectedGameFgdList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SelectedGameFgdList.Location = new System.Drawing.Point(15, 26);
            this.SelectedGameFgdList.Name = "SelectedGameFgdList";
            this.SelectedGameFgdList.ShowItemToolTips = true;
            this.SelectedGameFgdList.Size = new System.Drawing.Size(368, 160);
            this.SelectedGameFgdList.TabIndex = 26;
            this.SelectedGameFgdList.UseCompatibleStateImageBehavior = false;
            this.SelectedGameFgdList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Path";
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(18, 303);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(137, 20);
            this.label38.TabIndex = 25;
            this.label38.Text = "Detected map dimensions:";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameDetectedSizeHigh
            // 
            this.SelectedGameDetectedSizeHigh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectedGameDetectedSizeHigh.Location = new System.Drawing.Point(306, 303);
            this.SelectedGameDetectedSizeHigh.Name = "SelectedGameDetectedSizeHigh";
            this.SelectedGameDetectedSizeHigh.Size = new System.Drawing.Size(57, 20);
            this.SelectedGameDetectedSizeHigh.TabIndex = 24;
            this.SelectedGameDetectedSizeHigh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectedGameDetectedSizeLow
            // 
            this.SelectedGameDetectedSizeLow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectedGameDetectedSizeLow.Location = new System.Drawing.Point(202, 303);
            this.SelectedGameDetectedSizeLow.Name = "SelectedGameDetectedSizeLow";
            this.SelectedGameDetectedSizeLow.Size = new System.Drawing.Size(57, 20);
            this.SelectedGameDetectedSizeLow.TabIndex = 24;
            this.SelectedGameDetectedSizeLow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(161, 303);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(35, 20);
            this.label36.TabIndex = 24;
            this.label36.Text = "Low";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameOverrideSizeHigh
            // 
            this.SelectedGameOverrideSizeHigh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameOverrideSizeHigh.FormattingEnabled = true;
            this.SelectedGameOverrideSizeHigh.Items.AddRange(new object[] {
            "4096",
            "8192",
            "16384",
            "32768",
            "65536"});
            this.SelectedGameOverrideSizeHigh.Location = new System.Drawing.Point(306, 331);
            this.SelectedGameOverrideSizeHigh.Name = "SelectedGameOverrideSizeHigh";
            this.SelectedGameOverrideSizeHigh.Size = new System.Drawing.Size(57, 21);
            this.SelectedGameOverrideSizeHigh.TabIndex = 23;
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(265, 303);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(35, 20);
            this.label37.TabIndex = 22;
            this.label37.Text = "High";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(265, 330);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(35, 20);
            this.label35.TabIndex = 22;
            this.label35.Text = "High";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedGameOverrideSizeLow
            // 
            this.SelectedGameOverrideSizeLow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameOverrideSizeLow.FormattingEnabled = true;
            this.SelectedGameOverrideSizeLow.Items.AddRange(new object[] {
            "-4096",
            "-8192",
            "-16384",
            "-32768",
            "-65536"});
            this.SelectedGameOverrideSizeLow.Location = new System.Drawing.Point(202, 331);
            this.SelectedGameOverrideSizeLow.Name = "SelectedGameOverrideSizeLow";
            this.SelectedGameOverrideSizeLow.Size = new System.Drawing.Size(57, 21);
            this.SelectedGameOverrideSizeLow.TabIndex = 21;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(15, 258);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(368, 40);
            this.label33.TabIndex = 20;
            this.label33.Text = "Sledge uses the @mapsize syntax in the map FGDs to set the map size limits. If us" +
    "ing a non-source FGD that doesn\'t have this syntax, the map size can be overridd" +
    "en below.";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectedGameOverrideMapSize
            // 
            this.SelectedGameOverrideMapSize.Checked = true;
            this.SelectedGameOverrideMapSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedGameOverrideMapSize.Location = new System.Drawing.Point(18, 329);
            this.SelectedGameOverrideMapSize.Name = "SelectedGameOverrideMapSize";
            this.SelectedGameOverrideMapSize.Size = new System.Drawing.Size(137, 24);
            this.SelectedGameOverrideMapSize.TabIndex = 19;
            this.SelectedGameOverrideMapSize.Text = "Override FGD map size";
            this.SelectedGameOverrideMapSize.UseVisualStyleBackColor = true;
            this.SelectedGameOverrideMapSize.CheckedChanged += new System.EventHandler(this.SelectedGameOverrideMapSizeChanged);
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
            // SelectedGameAddFgd
            // 
            this.SelectedGameAddFgd.Location = new System.Drawing.Point(389, 26);
            this.SelectedGameAddFgd.Name = "SelectedGameAddFgd";
            this.SelectedGameAddFgd.Size = new System.Drawing.Size(74, 23);
            this.SelectedGameAddFgd.TabIndex = 1;
            this.SelectedGameAddFgd.Text = "Add...";
            this.SelectedGameAddFgd.UseVisualStyleBackColor = true;
            this.SelectedGameAddFgd.Click += new System.EventHandler(this.SelectedGameAddFgdClicked);
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(161, 330);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(35, 20);
            this.label34.TabIndex = 9;
            this.label34.Text = "Low";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // SelectedGameRemoveFgd
            // 
            this.SelectedGameRemoveFgd.Location = new System.Drawing.Point(389, 55);
            this.SelectedGameRemoveFgd.Name = "SelectedGameRemoveFgd";
            this.SelectedGameRemoveFgd.Size = new System.Drawing.Size(74, 23);
            this.SelectedGameRemoveFgd.TabIndex = 3;
            this.SelectedGameRemoveFgd.Text = "Remove";
            this.SelectedGameRemoveFgd.UseVisualStyleBackColor = true;
            this.SelectedGameRemoveFgd.Click += new System.EventHandler(this.SelectedGameRemoveFgdClicked);
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
            // SelectedGameDefaultBrushEnt
            // 
            this.SelectedGameDefaultBrushEnt.DropDownHeight = 300;
            this.SelectedGameDefaultBrushEnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameDefaultBrushEnt.FormattingEnabled = true;
            this.SelectedGameDefaultBrushEnt.IntegralHeight = false;
            this.SelectedGameDefaultBrushEnt.Items.AddRange(new object[] {
            "Valve"});
            this.SelectedGameDefaultBrushEnt.Location = new System.Drawing.Point(184, 219);
            this.SelectedGameDefaultBrushEnt.Name = "SelectedGameDefaultBrushEnt";
            this.SelectedGameDefaultBrushEnt.Size = new System.Drawing.Size(199, 21);
            this.SelectedGameDefaultBrushEnt.TabIndex = 10;
            // 
            // SelectedGameDefaultPointEnt
            // 
            this.SelectedGameDefaultPointEnt.DropDownHeight = 300;
            this.SelectedGameDefaultPointEnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedGameDefaultPointEnt.FormattingEnabled = true;
            this.SelectedGameDefaultPointEnt.IntegralHeight = false;
            this.SelectedGameDefaultPointEnt.Items.AddRange(new object[] {
            "Valve"});
            this.SelectedGameDefaultPointEnt.Location = new System.Drawing.Point(184, 192);
            this.SelectedGameDefaultPointEnt.Name = "SelectedGameDefaultPointEnt";
            this.SelectedGameDefaultPointEnt.Size = new System.Drawing.Size(199, 21);
            this.SelectedGameDefaultPointEnt.TabIndex = 10;
            // 
            // tabConfigTextures
            // 
            this.tabConfigTextures.Controls.Add(this.SelectedGameWadList);
            this.tabConfigTextures.Controls.Add(this.lblGameWAD);
            this.tabConfigTextures.Controls.Add(this.SelectedGameLightmapScale);
            this.tabConfigTextures.Controls.Add(this.lblConfigLightmapScale);
            this.tabConfigTextures.Controls.Add(this.SelectedGameAddWad);
            this.tabConfigTextures.Controls.Add(this.SelectedGameTextureScale);
            this.tabConfigTextures.Controls.Add(this.lblConfigTextureScale);
            this.tabConfigTextures.Controls.Add(this.SelectedGameRemoveWad);
            this.tabConfigTextures.Location = new System.Drawing.Point(4, 22);
            this.tabConfigTextures.Name = "tabConfigTextures";
            this.tabConfigTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfigTextures.Size = new System.Drawing.Size(469, 446);
            this.tabConfigTextures.TabIndex = 2;
            this.tabConfigTextures.Text = "Textures";
            this.tabConfigTextures.UseVisualStyleBackColor = true;
            // 
            // SelectedGameWadList
            // 
            this.SelectedGameWadList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.SelectedGameWadList.FullRowSelect = true;
            this.SelectedGameWadList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SelectedGameWadList.Location = new System.Drawing.Point(15, 26);
            this.SelectedGameWadList.Name = "SelectedGameWadList";
            this.SelectedGameWadList.ShowItemToolTips = true;
            this.SelectedGameWadList.Size = new System.Drawing.Size(368, 160);
            this.SelectedGameWadList.TabIndex = 18;
            this.SelectedGameWadList.UseCompatibleStateImageBehavior = false;
            this.SelectedGameWadList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Path";
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
            // SelectedGameLightmapScale
            // 
            this.SelectedGameLightmapScale.Location = new System.Drawing.Point(332, 218);
            this.SelectedGameLightmapScale.Name = "SelectedGameLightmapScale";
            this.SelectedGameLightmapScale.Size = new System.Drawing.Size(51, 20);
            this.SelectedGameLightmapScale.TabIndex = 17;
            this.SelectedGameLightmapScale.Value = new decimal(new int[] {
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
            // SelectedGameAddWad
            // 
            this.SelectedGameAddWad.Location = new System.Drawing.Point(389, 26);
            this.SelectedGameAddWad.Name = "SelectedGameAddWad";
            this.SelectedGameAddWad.Size = new System.Drawing.Size(74, 23);
            this.SelectedGameAddWad.TabIndex = 1;
            this.SelectedGameAddWad.Text = "Add...";
            this.SelectedGameAddWad.UseVisualStyleBackColor = true;
            this.SelectedGameAddWad.Click += new System.EventHandler(this.SelectedGameAddWadClicked);
            // 
            // SelectedGameTextureScale
            // 
            this.SelectedGameTextureScale.DecimalPlaces = 2;
            this.SelectedGameTextureScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.SelectedGameTextureScale.Location = new System.Drawing.Point(332, 192);
            this.SelectedGameTextureScale.Name = "SelectedGameTextureScale";
            this.SelectedGameTextureScale.Size = new System.Drawing.Size(51, 20);
            this.SelectedGameTextureScale.TabIndex = 17;
            this.SelectedGameTextureScale.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
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
            // SelectedGameRemoveWad
            // 
            this.SelectedGameRemoveWad.Location = new System.Drawing.Point(389, 55);
            this.SelectedGameRemoveWad.Name = "SelectedGameRemoveWad";
            this.SelectedGameRemoveWad.Size = new System.Drawing.Size(74, 23);
            this.SelectedGameRemoveWad.TabIndex = 3;
            this.SelectedGameRemoveWad.Text = "Remove";
            this.SelectedGameRemoveWad.UseVisualStyleBackColor = true;
            this.SelectedGameRemoveWad.Click += new System.EventHandler(this.SelectedGameRemoveWadClicked);
            // 
            // GameTree
            // 
            this.GameTree.HideSelection = false;
            this.GameTree.Location = new System.Drawing.Point(6, 6);
            this.GameTree.Name = "GameTree";
            this.GameTree.Size = new System.Drawing.Size(154, 448);
            this.GameTree.TabIndex = 0;
            this.GameTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GameSelected);
            // 
            // RemoveGame
            // 
            this.RemoveGame.Location = new System.Drawing.Point(166, 35);
            this.RemoveGame.Name = "RemoveGame";
            this.RemoveGame.Size = new System.Drawing.Size(73, 23);
            this.RemoveGame.TabIndex = 3;
            this.RemoveGame.Text = "Remove";
            this.RemoveGame.UseVisualStyleBackColor = true;
            this.RemoveGame.Click += new System.EventHandler(this.RemoveGameClicked);
            // 
            // AddGame
            // 
            this.AddGame.Location = new System.Drawing.Point(166, 6);
            this.AddGame.Name = "AddGame";
            this.AddGame.Size = new System.Drawing.Size(73, 23);
            this.AddGame.TabIndex = 1;
            this.AddGame.Text = "Add New";
            this.AddGame.UseVisualStyleBackColor = true;
            this.AddGame.Click += new System.EventHandler(this.AddGameClicked);
            // 
            // tabBuild
            // 
            this.tabBuild.Controls.Add(this.BuildSubTabs);
            this.tabBuild.Controls.Add(this.RemoveBuild);
            this.tabBuild.Controls.Add(this.AddBuild);
            this.tabBuild.Controls.Add(this.BuildTree);
            this.tabBuild.Location = new System.Drawing.Point(4, 22);
            this.tabBuild.Name = "tabBuild";
            this.tabBuild.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuild.Size = new System.Drawing.Size(736, 511);
            this.tabBuild.TabIndex = 3;
            this.tabBuild.Text = "Build Programs";
            this.tabBuild.UseVisualStyleBackColor = true;
            // 
            // BuildSubTabs
            // 
            this.BuildSubTabs.Controls.Add(this.tabBuildGeneral);
            this.BuildSubTabs.Controls.Add(this.tabBuildExecutables);
            this.BuildSubTabs.Controls.Add(this.tabBuildAdvanced);
            this.BuildSubTabs.Location = new System.Drawing.Point(245, 6);
            this.BuildSubTabs.Name = "BuildSubTabs";
            this.BuildSubTabs.SelectedIndex = 0;
            this.BuildSubTabs.Size = new System.Drawing.Size(477, 499);
            this.BuildSubTabs.TabIndex = 29;
            this.BuildSubTabs.Visible = false;
            // 
            // tabBuildGeneral
            // 
            this.tabBuildGeneral.Controls.Add(this.groupBox26);
            this.tabBuildGeneral.Controls.Add(this.groupBox25);
            this.tabBuildGeneral.Controls.Add(this.groupBox24);
            this.tabBuildGeneral.Controls.Add(this.lblBuildName);
            this.tabBuildGeneral.Controls.Add(this.SelectedBuildName);
            this.tabBuildGeneral.Controls.Add(this.label20);
            this.tabBuildGeneral.Controls.Add(this.lblBuildEngine);
            this.tabBuildGeneral.Controls.Add(this.SelectedBuildSpecification);
            this.tabBuildGeneral.Controls.Add(this.SelectedBuildEngine);
            this.tabBuildGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabBuildGeneral.Name = "tabBuildGeneral";
            this.tabBuildGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildGeneral.Size = new System.Drawing.Size(469, 473);
            this.tabBuildGeneral.TabIndex = 0;
            this.tabBuildGeneral.Text = "General";
            this.tabBuildGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.SelectedBuildCopyLin);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyErr);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyLog);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyPts);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyRes);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyMap);
            this.groupBox26.Controls.Add(this.SelectedBuildCopyBsp);
            this.groupBox26.Location = new System.Drawing.Point(6, 295);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(457, 107);
            this.groupBox26.TabIndex = 38;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Copy files back into map file\'s directory";
            // 
            // SelectedBuildCopyLin
            // 
            this.SelectedBuildCopyLin.Checked = true;
            this.SelectedBuildCopyLin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyLin.Location = new System.Drawing.Point(6, 77);
            this.SelectedBuildCopyLin.Name = "SelectedBuildCopyLin";
            this.SelectedBuildCopyLin.Size = new System.Drawing.Size(133, 23);
            this.SelectedBuildCopyLin.TabIndex = 36;
            this.SelectedBuildCopyLin.Text = "LIN (linear pointfile)";
            this.SelectedBuildCopyLin.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyErr
            // 
            this.SelectedBuildCopyErr.Checked = true;
            this.SelectedBuildCopyErr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyErr.Location = new System.Drawing.Point(284, 48);
            this.SelectedBuildCopyErr.Name = "SelectedBuildCopyErr";
            this.SelectedBuildCopyErr.Size = new System.Drawing.Size(146, 23);
            this.SelectedBuildCopyErr.TabIndex = 36;
            this.SelectedBuildCopyErr.Text = "ERR (compile error log)";
            this.SelectedBuildCopyErr.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyLog
            // 
            this.SelectedBuildCopyLog.Checked = true;
            this.SelectedBuildCopyLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyLog.Location = new System.Drawing.Point(284, 19);
            this.SelectedBuildCopyLog.Name = "SelectedBuildCopyLog";
            this.SelectedBuildCopyLog.Size = new System.Drawing.Size(146, 23);
            this.SelectedBuildCopyLog.TabIndex = 36;
            this.SelectedBuildCopyLog.Text = "LOG (compile output log)";
            this.SelectedBuildCopyLog.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyPts
            // 
            this.SelectedBuildCopyPts.Checked = true;
            this.SelectedBuildCopyPts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyPts.Location = new System.Drawing.Point(145, 48);
            this.SelectedBuildCopyPts.Name = "SelectedBuildCopyPts";
            this.SelectedBuildCopyPts.Size = new System.Drawing.Size(133, 23);
            this.SelectedBuildCopyPts.TabIndex = 36;
            this.SelectedBuildCopyPts.Text = "PTS (pointfile)";
            this.SelectedBuildCopyPts.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyRes
            // 
            this.SelectedBuildCopyRes.Checked = true;
            this.SelectedBuildCopyRes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyRes.Location = new System.Drawing.Point(6, 48);
            this.SelectedBuildCopyRes.Name = "SelectedBuildCopyRes";
            this.SelectedBuildCopyRes.Size = new System.Drawing.Size(133, 23);
            this.SelectedBuildCopyRes.TabIndex = 36;
            this.SelectedBuildCopyRes.Text = "RES (resource file)";
            this.SelectedBuildCopyRes.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyMap
            // 
            this.SelectedBuildCopyMap.Checked = true;
            this.SelectedBuildCopyMap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyMap.Location = new System.Drawing.Point(145, 19);
            this.SelectedBuildCopyMap.Name = "SelectedBuildCopyMap";
            this.SelectedBuildCopyMap.Size = new System.Drawing.Size(133, 23);
            this.SelectedBuildCopyMap.TabIndex = 36;
            this.SelectedBuildCopyMap.Text = "MAP (exported map)";
            this.SelectedBuildCopyMap.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildCopyBsp
            // 
            this.SelectedBuildCopyBsp.Checked = true;
            this.SelectedBuildCopyBsp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildCopyBsp.Location = new System.Drawing.Point(6, 19);
            this.SelectedBuildCopyBsp.Name = "SelectedBuildCopyBsp";
            this.SelectedBuildCopyBsp.Size = new System.Drawing.Size(133, 23);
            this.SelectedBuildCopyBsp.TabIndex = 36;
            this.SelectedBuildCopyBsp.Text = "BSP (compiled result)";
            this.SelectedBuildCopyBsp.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.SelectedBuildWorkingDirSub);
            this.groupBox25.Controls.Add(this.SelectedBuildWorkingDirSame);
            this.groupBox25.Controls.Add(this.SelectedBuildWorkingDirTemp);
            this.groupBox25.Location = new System.Drawing.Point(6, 86);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(457, 90);
            this.groupBox25.TabIndex = 37;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Compile working directory";
            // 
            // SelectedBuildWorkingDirSub
            // 
            this.SelectedBuildWorkingDirSub.AutoSize = true;
            this.SelectedBuildWorkingDirSub.Location = new System.Drawing.Point(6, 65);
            this.SelectedBuildWorkingDirSub.Name = "SelectedBuildWorkingDirSub";
            this.SelectedBuildWorkingDirSub.Size = new System.Drawing.Size(234, 17);
            this.SelectedBuildWorkingDirSub.TabIndex = 0;
            this.SelectedBuildWorkingDirSub.TabStop = true;
            this.SelectedBuildWorkingDirSub.Text = "Use a sub-directory in the map file\'s directory";
            this.SelectedBuildWorkingDirSub.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildWorkingDirSame
            // 
            this.SelectedBuildWorkingDirSame.AutoSize = true;
            this.SelectedBuildWorkingDirSame.Location = new System.Drawing.Point(6, 42);
            this.SelectedBuildWorkingDirSame.Name = "SelectedBuildWorkingDirSame";
            this.SelectedBuildWorkingDirSame.Size = new System.Drawing.Size(204, 17);
            this.SelectedBuildWorkingDirSame.TabIndex = 0;
            this.SelectedBuildWorkingDirSame.TabStop = true;
            this.SelectedBuildWorkingDirSame.Text = "Use the same directory as the map file";
            this.SelectedBuildWorkingDirSame.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildWorkingDirTemp
            // 
            this.SelectedBuildWorkingDirTemp.AutoSize = true;
            this.SelectedBuildWorkingDirTemp.Location = new System.Drawing.Point(6, 19);
            this.SelectedBuildWorkingDirTemp.Name = "SelectedBuildWorkingDirTemp";
            this.SelectedBuildWorkingDirTemp.Size = new System.Drawing.Size(145, 17);
            this.SelectedBuildWorkingDirTemp.TabIndex = 0;
            this.SelectedBuildWorkingDirTemp.TabStop = true;
            this.SelectedBuildWorkingDirTemp.Text = "Use a temporary directory";
            this.SelectedBuildWorkingDirTemp.UseVisualStyleBackColor = true;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.SelectedBuildAfterRunGame);
            this.groupBox24.Controls.Add(this.SelectedBuildAfterCopyBsp);
            this.groupBox24.Controls.Add(this.SelectedBuildAskBeforeRun);
            this.groupBox24.Location = new System.Drawing.Point(6, 182);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(457, 107);
            this.groupBox24.TabIndex = 20;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "After compiling has finished";
            // 
            // SelectedBuildAfterRunGame
            // 
            this.SelectedBuildAfterRunGame.Checked = true;
            this.SelectedBuildAfterRunGame.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildAfterRunGame.Location = new System.Drawing.Point(6, 48);
            this.SelectedBuildAfterRunGame.Name = "SelectedBuildAfterRunGame";
            this.SelectedBuildAfterRunGame.Size = new System.Drawing.Size(174, 23);
            this.SelectedBuildAfterRunGame.TabIndex = 35;
            this.SelectedBuildAfterRunGame.Text = "Run the game";
            this.SelectedBuildAfterRunGame.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildAfterCopyBsp
            // 
            this.SelectedBuildAfterCopyBsp.Checked = true;
            this.SelectedBuildAfterCopyBsp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildAfterCopyBsp.Location = new System.Drawing.Point(6, 19);
            this.SelectedBuildAfterCopyBsp.Name = "SelectedBuildAfterCopyBsp";
            this.SelectedBuildAfterCopyBsp.Size = new System.Drawing.Size(241, 23);
            this.SelectedBuildAfterCopyBsp.TabIndex = 36;
            this.SelectedBuildAfterCopyBsp.Text = "Copy BSP into the game\'s map directory";
            this.SelectedBuildAfterCopyBsp.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildAskBeforeRun
            // 
            this.SelectedBuildAskBeforeRun.Checked = true;
            this.SelectedBuildAskBeforeRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildAskBeforeRun.Location = new System.Drawing.Point(6, 77);
            this.SelectedBuildAskBeforeRun.Name = "SelectedBuildAskBeforeRun";
            this.SelectedBuildAskBeforeRun.Size = new System.Drawing.Size(171, 23);
            this.SelectedBuildAskBeforeRun.TabIndex = 34;
            this.SelectedBuildAskBeforeRun.Text = "Ask before running the game";
            this.SelectedBuildAskBeforeRun.UseVisualStyleBackColor = true;
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
            // SelectedBuildName
            // 
            this.SelectedBuildName.Location = new System.Drawing.Point(84, 6);
            this.SelectedBuildName.Name = "SelectedBuildName";
            this.SelectedBuildName.Size = new System.Drawing.Size(133, 20);
            this.SelectedBuildName.TabIndex = 14;
            this.SelectedBuildName.Text = "ZHLT";
            this.SelectedBuildName.TextChanged += new System.EventHandler(this.SelectedBuildNameChanged);
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(6, 60);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 20);
            this.label20.TabIndex = 16;
            this.label20.Text = "Specification";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // SelectedBuildSpecification
            // 
            this.SelectedBuildSpecification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildSpecification.FormattingEnabled = true;
            this.SelectedBuildSpecification.Items.AddRange(new object[] {
            "Goldsource",
            "Source"});
            this.SelectedBuildSpecification.Location = new System.Drawing.Point(84, 59);
            this.SelectedBuildSpecification.Name = "SelectedBuildSpecification";
            this.SelectedBuildSpecification.Size = new System.Drawing.Size(229, 21);
            this.SelectedBuildSpecification.TabIndex = 19;
            this.SelectedBuildSpecification.SelectedIndexChanged += new System.EventHandler(this.SelectedBuildSpecificationChanged);
            // 
            // SelectedBuildEngine
            // 
            this.SelectedBuildEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildEngine.FormattingEnabled = true;
            this.SelectedBuildEngine.Items.AddRange(new object[] {
            "Goldsource",
            "Source"});
            this.SelectedBuildEngine.Location = new System.Drawing.Point(84, 32);
            this.SelectedBuildEngine.Name = "SelectedBuildEngine";
            this.SelectedBuildEngine.Size = new System.Drawing.Size(121, 21);
            this.SelectedBuildEngine.TabIndex = 19;
            this.SelectedBuildEngine.SelectedIndexChanged += new System.EventHandler(this.SelectedBuildEngineChanged);
            // 
            // tabBuildExecutables
            // 
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildIncludeWads);
            this.tabBuildExecutables.Controls.Add(this.lblBuildExeFolder);
            this.tabBuildExecutables.Controls.Add(this.lblBuildBSP);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildExeFolder);
            this.tabBuildExecutables.Controls.Add(this.lblBuildCSG);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildRad);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildBsp);
            this.tabBuildExecutables.Controls.Add(this.lblBuildVIS);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildVis);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildCsg);
            this.tabBuildExecutables.Controls.Add(this.lblBuildRAD);
            this.tabBuildExecutables.Controls.Add(this.SelectedBuildExeFolderBrowse);
            this.tabBuildExecutables.Location = new System.Drawing.Point(4, 22);
            this.tabBuildExecutables.Name = "tabBuildExecutables";
            this.tabBuildExecutables.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildExecutables.Size = new System.Drawing.Size(469, 473);
            this.tabBuildExecutables.TabIndex = 1;
            this.tabBuildExecutables.Text = "Build Programs";
            this.tabBuildExecutables.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildIncludeWads
            // 
            this.SelectedBuildIncludeWads.Checked = true;
            this.SelectedBuildIncludeWads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SelectedBuildIncludeWads.Location = new System.Drawing.Point(95, 163);
            this.SelectedBuildIncludeWads.Name = "SelectedBuildIncludeWads";
            this.SelectedBuildIncludeWads.Size = new System.Drawing.Size(293, 24);
            this.SelectedBuildIncludeWads.TabIndex = 21;
            this.SelectedBuildIncludeWads.Text = "Automatically include WAD files found in this directory";
            this.SelectedBuildIncludeWads.UseVisualStyleBackColor = true;
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
            this.lblBuildBSP.Location = new System.Drawing.Point(55, 56);
            this.lblBuildBSP.Name = "lblBuildBSP";
            this.lblBuildBSP.Size = new System.Drawing.Size(34, 20);
            this.lblBuildBSP.TabIndex = 16;
            this.lblBuildBSP.Text = "BSP";
            this.lblBuildBSP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedBuildExeFolder
            // 
            this.SelectedBuildExeFolder.Location = new System.Drawing.Point(6, 29);
            this.SelectedBuildExeFolder.Name = "SelectedBuildExeFolder";
            this.SelectedBuildExeFolder.Size = new System.Drawing.Size(323, 20);
            this.SelectedBuildExeFolder.TabIndex = 15;
            this.SelectedBuildExeFolder.Text = "example: C:\\hammer_alt";
            this.SelectedBuildExeFolder.TextChanged += new System.EventHandler(this.SelectedBuildPathChanged);
            // 
            // lblBuildCSG
            // 
            this.lblBuildCSG.Location = new System.Drawing.Point(55, 83);
            this.lblBuildCSG.Name = "lblBuildCSG";
            this.lblBuildCSG.Size = new System.Drawing.Size(34, 20);
            this.lblBuildCSG.TabIndex = 16;
            this.lblBuildCSG.Text = "CSG";
            this.lblBuildCSG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedBuildRad
            // 
            this.SelectedBuildRad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildRad.FormattingEnabled = true;
            this.SelectedBuildRad.Location = new System.Drawing.Point(95, 136);
            this.SelectedBuildRad.Name = "SelectedBuildRad";
            this.SelectedBuildRad.Size = new System.Drawing.Size(234, 21);
            this.SelectedBuildRad.TabIndex = 19;
            // 
            // SelectedBuildBsp
            // 
            this.SelectedBuildBsp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildBsp.FormattingEnabled = true;
            this.SelectedBuildBsp.Location = new System.Drawing.Point(95, 55);
            this.SelectedBuildBsp.Name = "SelectedBuildBsp";
            this.SelectedBuildBsp.Size = new System.Drawing.Size(234, 21);
            this.SelectedBuildBsp.TabIndex = 19;
            // 
            // lblBuildVIS
            // 
            this.lblBuildVIS.Location = new System.Drawing.Point(55, 110);
            this.lblBuildVIS.Name = "lblBuildVIS";
            this.lblBuildVIS.Size = new System.Drawing.Size(34, 20);
            this.lblBuildVIS.TabIndex = 16;
            this.lblBuildVIS.Text = "VIS";
            this.lblBuildVIS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedBuildVis
            // 
            this.SelectedBuildVis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildVis.FormattingEnabled = true;
            this.SelectedBuildVis.Location = new System.Drawing.Point(95, 109);
            this.SelectedBuildVis.Name = "SelectedBuildVis";
            this.SelectedBuildVis.Size = new System.Drawing.Size(234, 21);
            this.SelectedBuildVis.TabIndex = 19;
            // 
            // SelectedBuildCsg
            // 
            this.SelectedBuildCsg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildCsg.FormattingEnabled = true;
            this.SelectedBuildCsg.Location = new System.Drawing.Point(95, 82);
            this.SelectedBuildCsg.Name = "SelectedBuildCsg";
            this.SelectedBuildCsg.Size = new System.Drawing.Size(234, 21);
            this.SelectedBuildCsg.TabIndex = 19;
            // 
            // lblBuildRAD
            // 
            this.lblBuildRAD.Location = new System.Drawing.Point(55, 137);
            this.lblBuildRAD.Name = "lblBuildRAD";
            this.lblBuildRAD.Size = new System.Drawing.Size(34, 20);
            this.lblBuildRAD.TabIndex = 16;
            this.lblBuildRAD.Text = "RAD";
            this.lblBuildRAD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectedBuildExeFolderBrowse
            // 
            this.SelectedBuildExeFolderBrowse.Location = new System.Drawing.Point(335, 27);
            this.SelectedBuildExeFolderBrowse.Name = "SelectedBuildExeFolderBrowse";
            this.SelectedBuildExeFolderBrowse.Size = new System.Drawing.Size(67, 23);
            this.SelectedBuildExeFolderBrowse.TabIndex = 20;
            this.SelectedBuildExeFolderBrowse.Text = "Browse...";
            this.SelectedBuildExeFolderBrowse.UseVisualStyleBackColor = true;
            this.SelectedBuildExeFolderBrowse.Click += new System.EventHandler(this.SelectedBuildExeFolderBrowseClicked);
            // 
            // tabBuildAdvanced
            // 
            this.tabBuildAdvanced.Controls.Add(this.SelectedBuildNewProfileButton);
            this.tabBuildAdvanced.Controls.Add(this.SelectedBuildRenameProfileButton);
            this.tabBuildAdvanced.Controls.Add(this.SelectedBuildDeleteProfileButton);
            this.tabBuildAdvanced.Controls.Add(this.SelectedBuildProfile);
            this.tabBuildAdvanced.Controls.Add(this.label40);
            this.tabBuildAdvanced.Controls.Add(this.tabBuildAdvancedSubTabs);
            this.tabBuildAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvanced.Name = "tabBuildAdvanced";
            this.tabBuildAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildAdvanced.Size = new System.Drawing.Size(469, 473);
            this.tabBuildAdvanced.TabIndex = 3;
            this.tabBuildAdvanced.Text = "Advanced";
            this.tabBuildAdvanced.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildNewProfileButton
            // 
            this.SelectedBuildNewProfileButton.Location = new System.Drawing.Point(382, 4);
            this.SelectedBuildNewProfileButton.Name = "SelectedBuildNewProfileButton";
            this.SelectedBuildNewProfileButton.Size = new System.Drawing.Size(81, 23);
            this.SelectedBuildNewProfileButton.TabIndex = 22;
            this.SelectedBuildNewProfileButton.Text = "New Profile...";
            this.SelectedBuildNewProfileButton.UseVisualStyleBackColor = true;
            this.SelectedBuildNewProfileButton.Click += new System.EventHandler(this.SelectedBuildNewProfileButtonClicked);
            // 
            // SelectedBuildRenameProfileButton
            // 
            this.SelectedBuildRenameProfileButton.Location = new System.Drawing.Point(175, 4);
            this.SelectedBuildRenameProfileButton.Name = "SelectedBuildRenameProfileButton";
            this.SelectedBuildRenameProfileButton.Size = new System.Drawing.Size(56, 23);
            this.SelectedBuildRenameProfileButton.TabIndex = 22;
            this.SelectedBuildRenameProfileButton.Text = "Rename";
            this.SelectedBuildRenameProfileButton.UseVisualStyleBackColor = true;
            this.SelectedBuildRenameProfileButton.Click += new System.EventHandler(this.SelectedBuildRenameProfileButtonClicked);
            // 
            // SelectedBuildDeleteProfileButton
            // 
            this.SelectedBuildDeleteProfileButton.Location = new System.Drawing.Point(237, 4);
            this.SelectedBuildDeleteProfileButton.Name = "SelectedBuildDeleteProfileButton";
            this.SelectedBuildDeleteProfileButton.Size = new System.Drawing.Size(56, 23);
            this.SelectedBuildDeleteProfileButton.TabIndex = 22;
            this.SelectedBuildDeleteProfileButton.Text = "Delete";
            this.SelectedBuildDeleteProfileButton.UseVisualStyleBackColor = true;
            this.SelectedBuildDeleteProfileButton.Click += new System.EventHandler(this.SelectedBuildDeleteProfileButtonClicked);
            // 
            // SelectedBuildProfile
            // 
            this.SelectedBuildProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectedBuildProfile.FormattingEnabled = true;
            this.SelectedBuildProfile.Location = new System.Drawing.Point(49, 6);
            this.SelectedBuildProfile.Name = "SelectedBuildProfile";
            this.SelectedBuildProfile.Size = new System.Drawing.Size(121, 21);
            this.SelectedBuildProfile.TabIndex = 21;
            this.SelectedBuildProfile.SelectedIndexChanged += new System.EventHandler(this.SelectedBuildProfileChanged);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(6, 9);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(36, 13);
            this.label40.TabIndex = 20;
            this.label40.Text = "Profile";
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
            this.tabBuildAdvancedCSG.Controls.Add(this.SelectedBuildCsgParameters);
            this.tabBuildAdvancedCSG.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedCSG.Name = "tabBuildAdvancedCSG";
            this.tabBuildAdvancedCSG.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedCSG.TabIndex = 0;
            this.tabBuildAdvancedCSG.Text = "CSG";
            this.tabBuildAdvancedCSG.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedBSP
            // 
            this.tabBuildAdvancedBSP.Controls.Add(this.SelectedBuildBspParameters);
            this.tabBuildAdvancedBSP.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedBSP.Name = "tabBuildAdvancedBSP";
            this.tabBuildAdvancedBSP.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedBSP.TabIndex = 1;
            this.tabBuildAdvancedBSP.Text = "BSP";
            this.tabBuildAdvancedBSP.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedVIS
            // 
            this.tabBuildAdvancedVIS.Controls.Add(this.SelectedBuildVisParameters);
            this.tabBuildAdvancedVIS.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedVIS.Name = "tabBuildAdvancedVIS";
            this.tabBuildAdvancedVIS.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedVIS.TabIndex = 2;
            this.tabBuildAdvancedVIS.Text = "VIS";
            this.tabBuildAdvancedVIS.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedRAD
            // 
            this.tabBuildAdvancedRAD.Controls.Add(this.SelectedBuildRadParameters);
            this.tabBuildAdvancedRAD.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedRAD.Name = "tabBuildAdvancedRAD";
            this.tabBuildAdvancedRAD.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedRAD.TabIndex = 3;
            this.tabBuildAdvancedRAD.Text = "RAD";
            this.tabBuildAdvancedRAD.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedShared
            // 
            this.tabBuildAdvancedShared.Controls.Add(this.SelectedBuildSharedParameters);
            this.tabBuildAdvancedShared.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedShared.Name = "tabBuildAdvancedShared";
            this.tabBuildAdvancedShared.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedShared.TabIndex = 4;
            this.tabBuildAdvancedShared.Text = "Shared";
            this.tabBuildAdvancedShared.UseVisualStyleBackColor = true;
            // 
            // tabBuildAdvancedPreview
            // 
            this.tabBuildAdvancedPreview.Controls.Add(this.SelectedBuildProfilePreview);
            this.tabBuildAdvancedPreview.Location = new System.Drawing.Point(4, 22);
            this.tabBuildAdvancedPreview.Name = "tabBuildAdvancedPreview";
            this.tabBuildAdvancedPreview.Size = new System.Drawing.Size(449, 408);
            this.tabBuildAdvancedPreview.TabIndex = 5;
            this.tabBuildAdvancedPreview.Text = "Preview";
            this.tabBuildAdvancedPreview.UseVisualStyleBackColor = true;
            // 
            // SelectedBuildProfilePreview
            // 
            this.SelectedBuildProfilePreview.BackColor = System.Drawing.SystemColors.Window;
            this.SelectedBuildProfilePreview.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectedBuildProfilePreview.Location = new System.Drawing.Point(6, 6);
            this.SelectedBuildProfilePreview.Multiline = true;
            this.SelectedBuildProfilePreview.Name = "SelectedBuildProfilePreview";
            this.SelectedBuildProfilePreview.ReadOnly = true;
            this.SelectedBuildProfilePreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SelectedBuildProfilePreview.Size = new System.Drawing.Size(437, 396);
            this.SelectedBuildProfilePreview.TabIndex = 3;
            // 
            // RemoveBuild
            // 
            this.RemoveBuild.Location = new System.Drawing.Point(166, 35);
            this.RemoveBuild.Name = "RemoveBuild";
            this.RemoveBuild.Size = new System.Drawing.Size(73, 23);
            this.RemoveBuild.TabIndex = 12;
            this.RemoveBuild.Text = "Remove";
            this.RemoveBuild.UseVisualStyleBackColor = true;
            this.RemoveBuild.Click += new System.EventHandler(this.RemoveBuildClicked);
            // 
            // AddBuild
            // 
            this.AddBuild.Location = new System.Drawing.Point(166, 6);
            this.AddBuild.Name = "AddBuild";
            this.AddBuild.Size = new System.Drawing.Size(73, 23);
            this.AddBuild.TabIndex = 10;
            this.AddBuild.Text = "Add New";
            this.AddBuild.UseVisualStyleBackColor = true;
            this.AddBuild.Click += new System.EventHandler(this.AddBuildClicked);
            // 
            // BuildTree
            // 
            this.BuildTree.Location = new System.Drawing.Point(6, 6);
            this.BuildTree.Name = "BuildTree";
            this.BuildTree.Size = new System.Drawing.Size(154, 448);
            this.BuildTree.TabIndex = 9;
            this.BuildTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.BuildSelected);
            // 
            // tabSteam
            // 
            this.tabSteam.Controls.Add(this.SteamInstallDir);
            this.tabSteam.Controls.Add(this.label17);
            this.tabSteam.Controls.Add(this.ListAvailableGamesButton);
            this.tabSteam.Controls.Add(this.label18);
            this.tabSteam.Controls.Add(this.SteamInstallDirBrowseButton);
            this.tabSteam.Controls.Add(this.SteamUsername);
            this.tabSteam.Location = new System.Drawing.Point(4, 22);
            this.tabSteam.Name = "tabSteam";
            this.tabSteam.Padding = new System.Windows.Forms.Padding(3);
            this.tabSteam.Size = new System.Drawing.Size(736, 511);
            this.tabSteam.TabIndex = 5;
            this.tabSteam.Text = "Steam";
            this.tabSteam.UseVisualStyleBackColor = true;
            // 
            // SteamInstallDir
            // 
            this.SteamInstallDir.Location = new System.Drawing.Point(109, 20);
            this.SteamInstallDir.Name = "SteamInstallDir";
            this.SteamInstallDir.Size = new System.Drawing.Size(225, 20);
            this.SteamInstallDir.TabIndex = 5;
            this.SteamInstallDir.TextChanged += new System.EventHandler(this.SteamDirectoryChanged);
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
            // ListAvailableGamesButton
            // 
            this.ListAvailableGamesButton.Location = new System.Drawing.Point(237, 44);
            this.ListAvailableGamesButton.Name = "ListAvailableGamesButton";
            this.ListAvailableGamesButton.Size = new System.Drawing.Size(115, 25);
            this.ListAvailableGamesButton.TabIndex = 8;
            this.ListAvailableGamesButton.Text = "List Available Games";
            this.ListAvailableGamesButton.UseVisualStyleBackColor = true;
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
            // SteamInstallDirBrowseButton
            // 
            this.SteamInstallDirBrowseButton.Location = new System.Drawing.Point(340, 18);
            this.SteamInstallDirBrowseButton.Name = "SteamInstallDirBrowseButton";
            this.SteamInstallDirBrowseButton.Size = new System.Drawing.Size(67, 23);
            this.SteamInstallDirBrowseButton.TabIndex = 8;
            this.SteamInstallDirBrowseButton.Text = "Browse...";
            this.SteamInstallDirBrowseButton.UseVisualStyleBackColor = true;
            this.SteamInstallDirBrowseButton.Click += new System.EventHandler(this.SteamInstallDirBrowseClicked);
            // 
            // SteamUsername
            // 
            this.SteamUsername.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SteamUsername.FormattingEnabled = true;
            this.SteamUsername.Location = new System.Drawing.Point(109, 46);
            this.SteamUsername.Name = "SteamUsername";
            this.SteamUsername.Size = new System.Drawing.Size(121, 21);
            this.SteamUsername.TabIndex = 7;
            this.SteamUsername.SelectedIndexChanged += new System.EventHandler(this.SteamUsernameChanged);
            // 
            // tabHotkeys
            // 
            this.tabHotkeys.Controls.Add(this.HotkeyResetButton);
            this.tabHotkeys.Controls.Add(this.groupBox22);
            this.tabHotkeys.Controls.Add(this.HotkeyReassignButton);
            this.tabHotkeys.Controls.Add(this.HotkeyRemoveButton);
            this.tabHotkeys.Controls.Add(this.HotkeyList);
            this.tabHotkeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotkeys.Name = "tabHotkeys";
            this.tabHotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.tabHotkeys.Size = new System.Drawing.Size(736, 511);
            this.tabHotkeys.TabIndex = 6;
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // HotkeyResetButton
            // 
            this.HotkeyResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HotkeyResetButton.Location = new System.Drawing.Point(537, 482);
            this.HotkeyResetButton.Name = "HotkeyResetButton";
            this.HotkeyResetButton.Size = new System.Drawing.Size(118, 23);
            this.HotkeyResetButton.TabIndex = 5;
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
            this.groupBox22.Controls.Add(this.HotkeyCombination);
            this.groupBox22.Location = new System.Drawing.Point(6, 455);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(437, 50);
            this.groupBox22.TabIndex = 4;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "New Hotkey";
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
            this.HotkeyAddButton.Text = "Add";
            this.HotkeyAddButton.UseVisualStyleBackColor = true;
            this.HotkeyAddButton.Click += new System.EventHandler(this.HotkeyAddButtonClicked);
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
            // HotkeyCombination
            // 
            this.HotkeyCombination.Location = new System.Drawing.Point(252, 19);
            this.HotkeyCombination.Name = "HotkeyCombination";
            this.HotkeyCombination.Size = new System.Drawing.Size(100, 20);
            this.HotkeyCombination.TabIndex = 1;
            this.HotkeyCombination.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyCombinationKeyDown);
            // 
            // HotkeyReassignButton
            // 
            this.HotkeyReassignButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HotkeyReassignButton.Location = new System.Drawing.Point(661, 455);
            this.HotkeyReassignButton.Name = "HotkeyReassignButton";
            this.HotkeyReassignButton.Size = new System.Drawing.Size(69, 23);
            this.HotkeyReassignButton.TabIndex = 3;
            this.HotkeyReassignButton.Text = "Reassign";
            this.HotkeyReassignButton.UseVisualStyleBackColor = true;
            this.HotkeyReassignButton.Click += new System.EventHandler(this.HotkeyReassignButtonClicked);
            // 
            // HotkeyRemoveButton
            // 
            this.HotkeyRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HotkeyRemoveButton.Location = new System.Drawing.Point(661, 482);
            this.HotkeyRemoveButton.Name = "HotkeyRemoveButton";
            this.HotkeyRemoveButton.Size = new System.Drawing.Size(69, 23);
            this.HotkeyRemoveButton.TabIndex = 3;
            this.HotkeyRemoveButton.Text = "Remove";
            this.HotkeyRemoveButton.UseVisualStyleBackColor = true;
            this.HotkeyRemoveButton.Click += new System.EventHandler(this.HotkeyRemoveButtonClicked);
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
            this.HotkeyList.Location = new System.Drawing.Point(6, 6);
            this.HotkeyList.MultiSelect = false;
            this.HotkeyList.Name = "HotkeyList";
            this.HotkeyList.Size = new System.Drawing.Size(724, 443);
            this.HotkeyList.TabIndex = 0;
            this.HotkeyList.UseCompatibleStateImageBehavior = false;
            this.HotkeyList.View = System.Windows.Forms.View.Details;
            this.HotkeyList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyListKeyDown);
            this.HotkeyList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HotkeyListDoubleClicked);
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
            this.btnCancelSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnApplyAndCloseSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnApplySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplySettings.Location = new System.Drawing.Point(503, 555);
            this.btnApplySettings.Name = "btnApplySettings";
            this.btnApplySettings.Size = new System.Drawing.Size(75, 23);
            this.btnApplySettings.TabIndex = 1;
            this.btnApplySettings.Text = "Apply";
            this.btnApplySettings.UseVisualStyleBackColor = true;
            this.btnApplySettings.Click += new System.EventHandler(this.Apply);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(730, 505);
            this.flowLayoutPanel1.TabIndex = 6;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // SelectedBuildCsgParameters
            // 
            this.SelectedBuildCsgParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedBuildCsgParameters.Location = new System.Drawing.Point(0, 0);
            this.SelectedBuildCsgParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SelectedBuildCsgParameters.Name = "SelectedBuildCsgParameters";
            this.SelectedBuildCsgParameters.Size = new System.Drawing.Size(449, 408);
            this.SelectedBuildCsgParameters.TabIndex = 0;
            // 
            // SelectedBuildBspParameters
            // 
            this.SelectedBuildBspParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedBuildBspParameters.Location = new System.Drawing.Point(0, 0);
            this.SelectedBuildBspParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SelectedBuildBspParameters.Name = "SelectedBuildBspParameters";
            this.SelectedBuildBspParameters.Size = new System.Drawing.Size(449, 408);
            this.SelectedBuildBspParameters.TabIndex = 1;
            // 
            // SelectedBuildVisParameters
            // 
            this.SelectedBuildVisParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedBuildVisParameters.Location = new System.Drawing.Point(0, 0);
            this.SelectedBuildVisParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SelectedBuildVisParameters.Name = "SelectedBuildVisParameters";
            this.SelectedBuildVisParameters.Size = new System.Drawing.Size(449, 408);
            this.SelectedBuildVisParameters.TabIndex = 1;
            // 
            // SelectedBuildRadParameters
            // 
            this.SelectedBuildRadParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedBuildRadParameters.Location = new System.Drawing.Point(0, 0);
            this.SelectedBuildRadParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SelectedBuildRadParameters.Name = "SelectedBuildRadParameters";
            this.SelectedBuildRadParameters.Size = new System.Drawing.Size(449, 408);
            this.SelectedBuildRadParameters.TabIndex = 1;
            // 
            // SelectedBuildSharedParameters
            // 
            this.SelectedBuildSharedParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedBuildSharedParameters.Location = new System.Drawing.Point(0, 0);
            this.SelectedBuildSharedParameters.MinimumSize = new System.Drawing.Size(300, 250);
            this.SelectedBuildSharedParameters.Name = "SelectedBuildSharedParameters";
            this.SelectedBuildSharedParameters.Size = new System.Drawing.Size(449, 408);
            this.SelectedBuildSharedParameters.TabIndex = 1;
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sledge Settings";
            this.Load += new System.EventHandler(this.SettingsFormLoad);
            this.tbcSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tab2DViews.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridHighlight1Distance)).EndInit();
            this.groupBox17.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SelectionBoxBackgroundOpacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScrollWheelZoomMultiplier)).EndInit();
            this.groupBox16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NudgeUnits)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HideGridLimit)).EndInit();
            this.tab3DViews.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CameraFOV)).EndInit();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MouseWheelMoveDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimeToTopSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ForwardSpeed)).EndInit();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DetailRenderDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ModelRenderDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackClippingPane)).EndInit();
            this.tabGame.ResumeLayout(false);
            this.GameSubTabs.ResumeLayout(false);
            this.tabConfigDirectories.ResumeLayout(false);
            this.tabConfigDirectories.PerformLayout();
            this.tabConfigSaving.ResumeLayout(false);
            this.tabConfigSaving.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameAutosaveLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameAutosaveTime)).EndInit();
            this.tabConfigEntities.ResumeLayout(false);
            this.tabConfigTextures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameLightmapScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedGameTextureScale)).EndInit();
            this.tabBuild.ResumeLayout(false);
            this.BuildSubTabs.ResumeLayout(false);
            this.tabBuildGeneral.ResumeLayout(false);
            this.tabBuildGeneral.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.tabBuildExecutables.ResumeLayout(false);
            this.tabBuildExecutables.PerformLayout();
            this.tabBuildAdvanced.ResumeLayout(false);
            this.tabBuildAdvanced.PerformLayout();
            this.tabBuildAdvancedSubTabs.ResumeLayout(false);
            this.tabBuildAdvancedCSG.ResumeLayout(false);
            this.tabBuildAdvancedBSP.ResumeLayout(false);
            this.tabBuildAdvancedVIS.ResumeLayout(false);
            this.tabBuildAdvancedRAD.ResumeLayout(false);
            this.tabBuildAdvancedShared.ResumeLayout(false);
            this.tabBuildAdvancedPreview.ResumeLayout(false);
            this.tabBuildAdvancedPreview.PerformLayout();
            this.tabSteam.ResumeLayout(false);
            this.tabSteam.PerformLayout();
            this.tabHotkeys.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.TextBox SelectedBuildProfilePreview;
		private System.Windows.Forms.TabPage tabBuildAdvancedPreview;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox15;
		private System.Windows.Forms.TabPage tabBuildAdvancedShared;
		private System.Windows.Forms.TabPage tabBuildAdvancedRAD;
		private System.Windows.Forms.TabPage tabBuildAdvancedVIS;
		private System.Windows.Forms.TabPage tabBuildAdvancedBSP;
		private System.Windows.Forms.TabPage tabBuildAdvancedCSG;
        private System.Windows.Forms.TabControl tabBuildAdvancedSubTabs;
		private System.Windows.Forms.TabPage tabGame;
		private System.Windows.Forms.Button AddGame;
		private System.Windows.Forms.Button RemoveGame;
		private System.Windows.Forms.Button SelectedGameAddFgd;
		private System.Windows.Forms.Button SelectedGameAddWad;
		private System.Windows.Forms.Button SelectedGameRemoveFgd;
		private System.Windows.Forms.Button SelectedGameRemoveWad;
		private System.Windows.Forms.Label lblGameWAD;
		private System.Windows.Forms.Label lblGameBuild;
        private System.Windows.Forms.ComboBox SelectedGameBuild;
		private System.Windows.Forms.Label lblGameFGD;
		private System.Windows.Forms.ComboBox SelectedGameDefaultPointEnt;
        private System.Windows.Forms.ComboBox SelectedGameDefaultBrushEnt;
		private System.Windows.Forms.NumericUpDown SelectedGameTextureScale;
        private System.Windows.Forms.NumericUpDown SelectedGameLightmapScale;
		private System.Windows.Forms.Label lblGameMod;
		private System.Windows.Forms.ComboBox SelectedGameMod;
		private System.Windows.Forms.TextBox SelectedGameWonDir;
		private System.Windows.Forms.Label lblGameWONDir;
		private System.Windows.Forms.Button SelectedGameDirBrowse;
		private System.Windows.Forms.Label lblGameSteamDir;
		private System.Windows.Forms.ComboBox SelectedGameSteamDir;
		private System.Windows.Forms.TextBox SelectedGameName;
		private System.Windows.Forms.Label lblGameName;
		private System.Windows.Forms.Label lblGameEngine;
        private System.Windows.Forms.ComboBox SelectedGameEngine;
        private System.Windows.Forms.TabControl GameSubTabs;
        private System.Windows.Forms.TreeView GameTree;
        private System.Windows.Forms.TreeView BuildTree;
		private System.Windows.Forms.ColumnHeader ckKeyCombo;
		private System.Windows.Forms.ColumnHeader chAction;
		private System.Windows.Forms.ListView HotkeyList;
		private System.Windows.Forms.TabPage tabHotkeys;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Panel GridBoundaryColour;
		private System.Windows.Forms.Button btnCancelSettings;
		private System.Windows.Forms.Button btnApplyAndCloseSettings;
		private System.Windows.Forms.Button SteamInstallDirBrowseButton;
		private System.Windows.Forms.TrackBar TimeToTopSpeed;
		private System.Windows.Forms.CheckBox InvertMouseX;
		private System.Windows.Forms.CheckBox InvertMouseY;
		private System.Windows.Forms.TrackBar ForwardSpeed;
		private System.Windows.Forms.TrackBar BackClippingPane;
		private System.Windows.Forms.TextBox SteamInstallDir;
		private System.Windows.Forms.ComboBox SteamUsername;
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
		private System.Windows.Forms.TabPage tabBuildExecutables;
		private System.Windows.Forms.TabPage tabBuildGeneral;
		private System.Windows.Forms.TabControl BuildSubTabs;
		private System.Windows.Forms.TabPage tabConfigTextures;
		private System.Windows.Forms.TabPage tabConfigEntities;
		private System.Windows.Forms.TabPage tabConfigDirectories;
		private System.Windows.Forms.TabPage tab2DViews;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Button ListAvailableGamesButton;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TabPage tabSteam;
		private System.Windows.Forms.Label BackClippingPaneLabel;
		private System.Windows.Forms.GroupBox groupBox14;
		private System.Windows.Forms.Label ForwardSpeedLabel;
		private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label TimeToTopSpeedLabel;
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
		private System.Windows.Forms.Label lblConfigLightmapScale;
		private System.Windows.Forms.Label lblConfigTextureScale;
		private System.Windows.Forms.Label lblConfigBrushEnt;
		private System.Windows.Forms.Label lblConfigPointEnt;
		private System.Windows.Forms.Label lblConfigSteamDir;
		private System.Windows.Forms.Button btnConfigListSteamGames;
		private System.Windows.Forms.Button btnConfigSteamDirBrowse;
		private System.Windows.Forms.ComboBox cmbConfigSteamUser;
		private System.Windows.Forms.Label lblConfigSteamUser;
		private System.Windows.Forms.Button SelectedBuildExeFolderBrowse;
		private System.Windows.Forms.ComboBox SelectedBuildEngine;
		private System.Windows.Forms.Label lblBuildEngine;
		private System.Windows.Forms.Label lblBuildExeFolder;
		private System.Windows.Forms.Label lblBuildName;
		private System.Windows.Forms.TextBox SelectedBuildExeFolder;
		private System.Windows.Forms.TextBox SelectedBuildName;
		private System.Windows.Forms.Button RemoveBuild;
        private System.Windows.Forms.Button AddBuild;
		private System.Windows.Forms.ComboBox SelectedBuildRad;
		private System.Windows.Forms.ComboBox SelectedBuildVis;
		private System.Windows.Forms.Label lblBuildRAD;
		private System.Windows.Forms.ComboBox SelectedBuildCsg;
		private System.Windows.Forms.Label lblBuildVIS;
		private System.Windows.Forms.ComboBox SelectedBuildBsp;
		private System.Windows.Forms.Label lblBuildCSG;
        private System.Windows.Forms.Label lblBuildBSP;
		private System.Windows.Forms.TabPage tabBuild;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.TabControl tbcSettings;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox HideGridOn;
        private System.Windows.Forms.DomainUpDown GridHighlight2UnitNum;
        private System.Windows.Forms.CheckBox SelectedGameSteamInstall;
        private System.Windows.Forms.CheckBox SelectedBuildIncludeWads;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.RadioButton NudgeStyle_GridOnCtrl;
        private System.Windows.Forms.RadioButton NudgeStyle_GridOffCtrl;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.CheckBox AutoSelectBox;
        private System.Windows.Forms.CheckBox BoxSelectByHandlesOnly;
        private System.Windows.Forms.CheckBox DrawCenterHandles;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TrackBar DetailRenderDistance;
        private System.Windows.Forms.Label DetailRenderDistanceLabel;
        private System.Windows.Forms.TrackBar ModelRenderDistance;
        private System.Windows.Forms.Label ModelRenderDistanceLabel;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.NumericUpDown CameraFOV;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox ClickSelectByHandlesOnly;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.CheckBox CenterHandlesOnlyNearCursor;
        private System.Windows.Forms.CheckBox CenterHandlesActiveViewportOnly;
        private System.Windows.Forms.CheckBox KeepVisgroupsWhenCloning;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.NumericUpDown ScrollWheelZoomMultiplier;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.NumericUpDown SelectionBoxBackgroundOpacity;
        private System.Windows.Forms.Label lblBaseGame;
        private System.Windows.Forms.ComboBox SelectedGameBase;
        private System.Windows.Forms.ComboBox SelectedGameOverrideSizeHigh;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.ComboBox SelectedGameOverrideSizeLow;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox SelectedGameOverrideMapSize;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label SelectedGameDetectedSizeHigh;
        private System.Windows.Forms.Label SelectedGameDetectedSizeLow;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.ListView SelectedGameWadList;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView SelectedGameFgdList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button HotkeyRemoveButton;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox HotkeyCombination;
        private System.Windows.Forms.Button HotkeyReassignButton;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.ComboBox HotkeyActionList;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button HotkeyAddButton;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Panel ViewportBackgroundColour;
        private System.Windows.Forms.Button HotkeyResetButton;
        private Compiling.CompileParameterPanel SelectedBuildCsgParameters;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox SelectedBuildSpecification;
        private System.Windows.Forms.Button SelectedBuildNewProfileButton;
        private System.Windows.Forms.Button SelectedBuildDeleteProfileButton;
        private System.Windows.Forms.ComboBox SelectedBuildProfile;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Button SelectedBuildRenameProfileButton;
        private Compiling.CompileParameterPanel SelectedBuildBspParameters;
        private Compiling.CompileParameterPanel SelectedBuildVisParameters;
        private Compiling.CompileParameterPanel SelectedBuildRadParameters;
        private Compiling.CompileParameterPanel SelectedBuildSharedParameters;
        private System.Windows.Forms.GroupBox groupBox24;
        private System.Windows.Forms.CheckBox SelectedBuildAfterRunGame;
        private System.Windows.Forms.CheckBox SelectedBuildAfterCopyBsp;
        private System.Windows.Forms.CheckBox SelectedBuildAskBeforeRun;
        private System.Windows.Forms.GroupBox groupBox25;
        private System.Windows.Forms.RadioButton SelectedBuildWorkingDirSub;
        private System.Windows.Forms.RadioButton SelectedBuildWorkingDirSame;
        private System.Windows.Forms.RadioButton SelectedBuildWorkingDirTemp;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.CheckBox SelectedBuildCopyLin;
        private System.Windows.Forms.CheckBox SelectedBuildCopyPts;
        private System.Windows.Forms.CheckBox SelectedBuildCopyRes;
        private System.Windows.Forms.CheckBox SelectedBuildCopyMap;
        private System.Windows.Forms.CheckBox SelectedBuildCopyBsp;
        private System.Windows.Forms.CheckBox SelectedBuildCopyErr;
        private System.Windows.Forms.CheckBox SelectedBuildCopyLog;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.ComboBox SelectedGameExecutable;
        private System.Windows.Forms.TabPage tabConfigSaving;
        private System.Windows.Forms.NumericUpDown SelectedGameAutosaveLimit;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown SelectedGameAutosaveTime;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblGameMapSaveDir;
        private System.Windows.Forms.CheckBox SelectedGameEnableAutosave;
        private System.Windows.Forms.TextBox SelectedGameMapDir;
        private System.Windows.Forms.Button SelectedGameMapDirBrowse;
        private System.Windows.Forms.TextBox SelectedGameDiffAutosaveDir;
        private System.Windows.Forms.Label lblGameAutosaveDir;
        private System.Windows.Forms.Button SelectedGameDiffAutosaveDirBrowse;
        private System.Windows.Forms.CheckBox SelectedGameAutosaveOnlyOnChange;
        private System.Windows.Forms.CheckBox SelectedGameUseDiffAutosaveDir;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox SelectedGameRunArguments;
        private System.Windows.Forms.CheckBox DrawBoxText;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.NumericUpDown MouseWheelMoveDistance;
        private System.Windows.Forms.CheckBox DrawEntityNames;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
