using Sledge.Editor.UI;

namespace Sledge.Editor
{
    partial class Editor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.stsStatus = new System.Windows.Forms.StatusStrip();
            this.StatusTextLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusSelectionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusCoordinatesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBoxLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusZoomLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusSnapLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tscToolStrip = new System.Windows.Forms.ToolStripContainer();
            this.DockFill = new Sledge.Editor.UI.DockedPanel();
            this.tblQuadView = new Sledge.Editor.UI.QuadSplitControl();
            this.DocumentTabs = new Sledge.Editor.UI.ClosableTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DockRight = new Sledge.Editor.UI.DockedPanel();
            this.DockLeft = new Sledge.Editor.UI.DockedPanel();
            this.DockBottom = new Sledge.Editor.UI.DockedPanel();
            this.sidebarPanel = new Sledge.Editor.UI.DockedPanel();
            this.RightToolbar = new System.Windows.Forms.TableLayoutPanel();
            this.HistoryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.HistoryCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.VisgroupsPanel = new System.Windows.Forms.Panel();
            this.VisgroupToolbarPanel = new Sledge.Editor.Visgroups.VisgroupToolbarPanel();
            this.VisgroupsCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TextureCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TexturePanel = new System.Windows.Forms.Panel();
            this.TextureBrowseButton = new System.Windows.Forms.Button();
            this.TextureReplaceButton = new System.Windows.Forms.Button();
            this.TextureSizeLabel = new System.Windows.Forms.Label();
            this.TextureSelectionPictureBox = new System.Windows.Forms.PictureBox();
            this.TextureComboBox = new Sledge.Editor.UI.TextureComboBox();
            this.TextureGroupComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BrushCreatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.BrushTypeList = new System.Windows.Forms.ComboBox();
            this.EntityCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.EntityPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.EntityTypeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MoveToWorldButton = new System.Windows.Forms.Button();
            this.MoveToEntityButton = new System.Windows.Forms.Button();
            this.BrushCreateCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.sidebarPanel1 = new Sledge.Editor.UI.Sidebar.SidebarPanel();
            this.tspTools = new System.Windows.Forms.ToolStrip();
            this.stsStatus.SuspendLayout();
            this.tscToolStrip.ContentPanel.SuspendLayout();
            this.tscToolStrip.LeftToolStripPanel.SuspendLayout();
            this.tscToolStrip.SuspendLayout();
            this.DockFill.SuspendLayout();
            this.DocumentTabs.SuspendLayout();
            this.sidebarPanel.SuspendLayout();
            this.RightToolbar.SuspendLayout();
            this.VisgroupsPanel.SuspendLayout();
            this.TexturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextureSelectionPictureBox)).BeginInit();
            this.BrushCreatePanel.SuspendLayout();
            this.EntityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(879, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // stsStatus
            // 
            this.stsStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusTextLabel,
            this.StatusSelectionLabel,
            this.StatusCoordinatesLabel,
            this.StatusBoxLabel,
            this.StatusZoomLabel,
            this.StatusSnapLabel});
            this.stsStatus.Location = new System.Drawing.Point(0, 726);
            this.stsStatus.Name = "stsStatus";
            this.stsStatus.Size = new System.Drawing.Size(879, 24);
            this.stsStatus.TabIndex = 1;
            this.stsStatus.Text = "statusStrip1";
            // 
            // StatusTextLabel
            // 
            this.StatusTextLabel.Name = "StatusTextLabel";
            this.StatusTextLabel.Size = new System.Drawing.Size(314, 19);
            this.StatusTextLabel.Spring = true;
            this.StatusTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusSelectionLabel
            // 
            this.StatusSelectionLabel.AutoSize = false;
            this.StatusSelectionLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusSelectionLabel.Name = "StatusSelectionLabel";
            this.StatusSelectionLabel.Size = new System.Drawing.Size(130, 19);
            // 
            // StatusCoordinatesLabel
            // 
            this.StatusCoordinatesLabel.AutoSize = false;
            this.StatusCoordinatesLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusCoordinatesLabel.Name = "StatusCoordinatesLabel";
            this.StatusCoordinatesLabel.Size = new System.Drawing.Size(130, 19);
            // 
            // StatusBoxLabel
            // 
            this.StatusBoxLabel.AutoSize = false;
            this.StatusBoxLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusBoxLabel.Name = "StatusBoxLabel";
            this.StatusBoxLabel.Size = new System.Drawing.Size(130, 19);
            // 
            // StatusZoomLabel
            // 
            this.StatusZoomLabel.AutoSize = false;
            this.StatusZoomLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusZoomLabel.Name = "StatusZoomLabel";
            this.StatusZoomLabel.Size = new System.Drawing.Size(80, 19);
            // 
            // StatusSnapLabel
            // 
            this.StatusSnapLabel.AutoSize = false;
            this.StatusSnapLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusSnapLabel.Name = "StatusSnapLabel";
            this.StatusSnapLabel.Size = new System.Drawing.Size(80, 19);
            // 
            // tscToolStrip
            // 
            // 
            // tscToolStrip.ContentPanel
            // 
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockFill);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockRight);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockLeft);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockBottom);
            this.tscToolStrip.ContentPanel.Controls.Add(this.sidebarPanel);
            this.tscToolStrip.ContentPanel.Size = new System.Drawing.Size(845, 677);
            this.tscToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // tscToolStrip.LeftToolStripPanel
            // 
            this.tscToolStrip.LeftToolStripPanel.Controls.Add(this.tspTools);
            this.tscToolStrip.Location = new System.Drawing.Point(0, 24);
            this.tscToolStrip.Name = "tscToolStrip";
            this.tscToolStrip.Size = new System.Drawing.Size(879, 702);
            this.tscToolStrip.TabIndex = 2;
            this.tscToolStrip.Text = "tscToolStrip";
            // 
            // DockFill
            // 
            this.DockFill.Controls.Add(this.tblQuadView);
            this.DockFill.Controls.Add(this.DocumentTabs);
            this.DockFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockFill.DockDimension = 0;
            this.DockFill.Hidden = false;
            this.DockFill.Location = new System.Drawing.Point(26, 0);
            this.DockFill.Name = "DockFill";
            this.DockFill.Size = new System.Drawing.Size(434, 660);
            this.DockFill.TabIndex = 4;
            // 
            // tblQuadView
            // 
            this.tblQuadView.ColumnCount = 2;
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblQuadView.Location = new System.Drawing.Point(0, 24);
            this.tblQuadView.MinimumViewSize = 2;
            this.tblQuadView.Name = "tblQuadView";
            this.tblQuadView.RowCount = 2;
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblQuadView.Size = new System.Drawing.Size(434, 636);
            this.tblQuadView.TabIndex = 0;
            // 
            // DocumentTabs
            // 
            this.DocumentTabs.Controls.Add(this.tabPage1);
            this.DocumentTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.DocumentTabs.Location = new System.Drawing.Point(0, 0);
            this.DocumentTabs.Name = "DocumentTabs";
            this.DocumentTabs.SelectedIndex = 0;
            this.DocumentTabs.Size = new System.Drawing.Size(434, 24);
            this.DocumentTabs.TabIndex = 2;
            this.DocumentTabs.RequestClose += new Sledge.Editor.UI.ClosableTabControl.RequestCloseEventHandler(this.DocumentTabsRequestClose);
            this.DocumentTabs.SelectedIndexChanged += new System.EventHandler(this.DocumentTabsSelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(446, -5);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tab";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DockRight
            // 
            this.DockRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.DockRight.DockDimension = 230;
            this.DockRight.Hidden = false;
            this.DockRight.Location = new System.Drawing.Point(460, 0);
            this.DockRight.MinSize = 230;
            this.DockRight.Name = "DockRight";
            this.DockRight.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.DockRight.Size = new System.Drawing.Size(230, 660);
            this.DockRight.TabIndex = 3;
            // 
            // DockLeft
            // 
            this.DockLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.DockLeft.DockDimension = 26;
            this.DockLeft.Hidden = false;
            this.DockLeft.Location = new System.Drawing.Point(0, 0);
            this.DockLeft.Name = "DockLeft";
            this.DockLeft.Size = new System.Drawing.Size(26, 660);
            this.DockLeft.TabIndex = 3;
            // 
            // DockBottom
            // 
            this.DockBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DockBottom.DockDimension = 17;
            this.DockBottom.Hidden = false;
            this.DockBottom.Location = new System.Drawing.Point(0, 660);
            this.DockBottom.Name = "DockBottom";
            this.DockBottom.Size = new System.Drawing.Size(690, 17);
            this.DockBottom.TabIndex = 3;
            // 
            // sidebarPanel
            // 
            this.sidebarPanel.Controls.Add(this.RightToolbar);
            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.sidebarPanel.DockDimension = 155;
            this.sidebarPanel.Hidden = false;
            this.sidebarPanel.Location = new System.Drawing.Point(690, 0);
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.sidebarPanel.Size = new System.Drawing.Size(155, 677);
            this.sidebarPanel.TabIndex = 1;
            // 
            // RightToolbar
            // 
            this.RightToolbar.ColumnCount = 1;
            this.RightToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RightToolbar.Controls.Add(this.HistoryPanel, 0, 9);
            this.RightToolbar.Controls.Add(this.HistoryCollapse, 0, 8);
            this.RightToolbar.Controls.Add(this.VisgroupsPanel, 0, 3);
            this.RightToolbar.Controls.Add(this.VisgroupsCollapse, 0, 2);
            this.RightToolbar.Controls.Add(this.TextureCollapse, 0, 0);
            this.RightToolbar.Controls.Add(this.TexturePanel, 0, 1);
            this.RightToolbar.Controls.Add(this.BrushCreatePanel, 0, 7);
            this.RightToolbar.Controls.Add(this.EntityCollapse, 0, 4);
            this.RightToolbar.Controls.Add(this.EntityPanel, 0, 5);
            this.RightToolbar.Controls.Add(this.BrushCreateCollapse, 0, 6);
            this.RightToolbar.Controls.Add(this.sidebarPanel1, 0, 10);
            this.RightToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightToolbar.Location = new System.Drawing.Point(5, 0);
            this.RightToolbar.Name = "RightToolbar";
            this.RightToolbar.RowCount = 11;
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.RightToolbar.Size = new System.Drawing.Size(150, 677);
            this.RightToolbar.TabIndex = 2;
            // 
            // HistoryPanel
            // 
            this.HistoryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistoryPanel.Location = new System.Drawing.Point(3, 548);
            this.HistoryPanel.Name = "HistoryPanel";
            this.HistoryPanel.Size = new System.Drawing.Size(144, 14);
            this.HistoryPanel.TabIndex = 8;
            // 
            // HistoryCollapse
            // 
            this.HistoryCollapse.Collapsed = false;
            this.HistoryCollapse.ControlToCollapse = this.HistoryPanel;
            this.HistoryCollapse.LabelText = "History";
            this.HistoryCollapse.Location = new System.Drawing.Point(3, 528);
            this.HistoryCollapse.Name = "HistoryCollapse";
            this.HistoryCollapse.Size = new System.Drawing.Size(144, 14);
            this.HistoryCollapse.TabIndex = 7;
            // 
            // VisgroupsPanel
            // 
            this.VisgroupsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VisgroupsPanel.Controls.Add(this.VisgroupToolbarPanel);
            this.VisgroupsPanel.Location = new System.Drawing.Point(3, 189);
            this.VisgroupsPanel.Name = "VisgroupsPanel";
            this.VisgroupsPanel.Size = new System.Drawing.Size(144, 144);
            this.VisgroupsPanel.TabIndex = 0;
            // 
            // VisgroupToolbarPanel
            // 
            this.VisgroupToolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisgroupToolbarPanel.Location = new System.Drawing.Point(0, 0);
            this.VisgroupToolbarPanel.Name = "VisgroupToolbarPanel";
            this.VisgroupToolbarPanel.Size = new System.Drawing.Size(142, 142);
            this.VisgroupToolbarPanel.TabIndex = 2;
            // 
            // VisgroupsCollapse
            // 
            this.VisgroupsCollapse.Collapsed = false;
            this.VisgroupsCollapse.ControlToCollapse = this.VisgroupsPanel;
            this.VisgroupsCollapse.LabelText = "Visgroups";
            this.VisgroupsCollapse.Location = new System.Drawing.Point(3, 169);
            this.VisgroupsCollapse.Name = "VisgroupsCollapse";
            this.VisgroupsCollapse.Size = new System.Drawing.Size(144, 14);
            this.VisgroupsCollapse.TabIndex = 6;
            // 
            // TextureCollapse
            // 
            this.TextureCollapse.Collapsed = false;
            this.TextureCollapse.ControlToCollapse = this.TexturePanel;
            this.TextureCollapse.LabelText = "Textures";
            this.TextureCollapse.Location = new System.Drawing.Point(3, 3);
            this.TextureCollapse.Name = "TextureCollapse";
            this.TextureCollapse.Size = new System.Drawing.Size(144, 15);
            this.TextureCollapse.TabIndex = 0;
            // 
            // TexturePanel
            // 
            this.TexturePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TexturePanel.Controls.Add(this.TextureBrowseButton);
            this.TexturePanel.Controls.Add(this.TextureReplaceButton);
            this.TexturePanel.Controls.Add(this.TextureSizeLabel);
            this.TexturePanel.Controls.Add(this.TextureSelectionPictureBox);
            this.TexturePanel.Controls.Add(this.TextureComboBox);
            this.TexturePanel.Controls.Add(this.TextureGroupComboBox);
            this.TexturePanel.Controls.Add(this.label5);
            this.TexturePanel.Controls.Add(this.label4);
            this.TexturePanel.Location = new System.Drawing.Point(3, 24);
            this.TexturePanel.Name = "TexturePanel";
            this.TexturePanel.Size = new System.Drawing.Size(144, 139);
            this.TexturePanel.TabIndex = 3;
            // 
            // TextureBrowseButton
            // 
            this.TextureBrowseButton.Location = new System.Drawing.Point(73, 90);
            this.TextureBrowseButton.Name = "TextureBrowseButton";
            this.TextureBrowseButton.Size = new System.Drawing.Size(66, 20);
            this.TextureBrowseButton.TabIndex = 5;
            this.TextureBrowseButton.Text = "Browse...";
            this.TextureBrowseButton.UseVisualStyleBackColor = true;
            this.TextureBrowseButton.Click += new System.EventHandler(this.TextureBrowseButtonClicked);
            // 
            // TextureReplaceButton
            // 
            this.TextureReplaceButton.Location = new System.Drawing.Point(73, 113);
            this.TextureReplaceButton.Name = "TextureReplaceButton";
            this.TextureReplaceButton.Size = new System.Drawing.Size(66, 20);
            this.TextureReplaceButton.TabIndex = 5;
            this.TextureReplaceButton.Text = "Replace...";
            this.TextureReplaceButton.UseVisualStyleBackColor = true;
            this.TextureReplaceButton.Click += new System.EventHandler(this.TextureReplaceButtonClicked);
            // 
            // TextureSizeLabel
            // 
            this.TextureSizeLabel.AutoSize = true;
            this.TextureSizeLabel.Location = new System.Drawing.Point(73, 69);
            this.TextureSizeLabel.Name = "TextureSizeLabel";
            this.TextureSizeLabel.Size = new System.Drawing.Size(0, 13);
            this.TextureSizeLabel.TabIndex = 4;
            // 
            // TextureSelectionPictureBox
            // 
            this.TextureSelectionPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextureSelectionPictureBox.Location = new System.Drawing.Point(3, 69);
            this.TextureSelectionPictureBox.Name = "TextureSelectionPictureBox";
            this.TextureSelectionPictureBox.Size = new System.Drawing.Size(64, 64);
            this.TextureSelectionPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TextureSelectionPictureBox.TabIndex = 3;
            this.TextureSelectionPictureBox.TabStop = false;
            // 
            // TextureComboBox
            // 
            this.TextureComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.TextureComboBox.DropDownHeight = 600;
            this.TextureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextureComboBox.FormattingEnabled = true;
            this.TextureComboBox.IntegralHeight = false;
            this.TextureComboBox.ItemHeight = 15;
            this.TextureComboBox.Location = new System.Drawing.Point(3, 42);
            this.TextureComboBox.Name = "TextureComboBox";
            this.TextureComboBox.Size = new System.Drawing.Size(136, 21);
            this.TextureComboBox.TabIndex = 2;
            this.TextureComboBox.SelectionChangeCommitted += new System.EventHandler(this.TextureSelectionChanged);
            // 
            // TextureGroupComboBox
            // 
            this.TextureGroupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TextureGroupComboBox.FormattingEnabled = true;
            this.TextureGroupComboBox.Location = new System.Drawing.Point(41, 1);
            this.TextureGroupComboBox.Name = "TextureGroupComboBox";
            this.TextureGroupComboBox.Size = new System.Drawing.Size(98, 21);
            this.TextureGroupComboBox.TabIndex = 1;
            this.TextureGroupComboBox.SelectedIndexChanged += new System.EventHandler(this.TextureGroupSelected);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Texture:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Group:";
            // 
            // BrushCreatePanel
            // 
            this.BrushCreatePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BrushCreatePanel.Controls.Add(this.label3);
            this.BrushCreatePanel.Controls.Add(this.BrushTypeList);
            this.BrushCreatePanel.Location = new System.Drawing.Point(3, 474);
            this.BrushCreatePanel.Name = "BrushCreatePanel";
            this.BrushCreatePanel.Size = new System.Drawing.Size(144, 48);
            this.BrushCreatePanel.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Brush Type:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // BrushTypeList
            // 
            this.BrushTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BrushTypeList.FormattingEnabled = true;
            this.BrushTypeList.Location = new System.Drawing.Point(3, 20);
            this.BrushTypeList.Name = "BrushTypeList";
            this.BrushTypeList.Size = new System.Drawing.Size(137, 21);
            this.BrushTypeList.TabIndex = 0;
            // 
            // EntityCollapse
            // 
            this.EntityCollapse.Collapsed = false;
            this.EntityCollapse.ControlToCollapse = this.EntityPanel;
            this.EntityCollapse.LabelText = "Entities";
            this.EntityCollapse.Location = new System.Drawing.Point(3, 339);
            this.EntityCollapse.Name = "EntityCollapse";
            this.EntityCollapse.Size = new System.Drawing.Size(144, 14);
            this.EntityCollapse.TabIndex = 5;
            // 
            // EntityPanel
            // 
            this.EntityPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EntityPanel.Controls.Add(this.label2);
            this.EntityPanel.Controls.Add(this.EntityTypeList);
            this.EntityPanel.Controls.Add(this.label1);
            this.EntityPanel.Controls.Add(this.MoveToWorldButton);
            this.EntityPanel.Controls.Add(this.MoveToEntityButton);
            this.EntityPanel.Location = new System.Drawing.Point(3, 359);
            this.EntityPanel.Name = "EntityPanel";
            this.EntityPanel.Size = new System.Drawing.Size(144, 89);
            this.EntityPanel.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Entity Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // EntityTypeList
            // 
            this.EntityTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EntityTypeList.FormattingEnabled = true;
            this.EntityTypeList.Location = new System.Drawing.Point(3, 20);
            this.EntityTypeList.Name = "EntityTypeList";
            this.EntityTypeList.Size = new System.Drawing.Size(137, 21);
            this.EntityTypeList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Move selected:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MoveToWorldButton
            // 
            this.MoveToWorldButton.Location = new System.Drawing.Point(3, 60);
            this.MoveToWorldButton.Name = "MoveToWorldButton";
            this.MoveToWorldButton.Size = new System.Drawing.Size(65, 23);
            this.MoveToWorldButton.TabIndex = 1;
            this.MoveToWorldButton.Text = "To world";
            this.MoveToWorldButton.UseVisualStyleBackColor = true;
            this.MoveToWorldButton.Click += new System.EventHandler(this.MoveToWorldClicked);
            // 
            // MoveToEntityButton
            // 
            this.MoveToEntityButton.Location = new System.Drawing.Point(74, 60);
            this.MoveToEntityButton.Name = "MoveToEntityButton";
            this.MoveToEntityButton.Size = new System.Drawing.Size(65, 23);
            this.MoveToEntityButton.TabIndex = 2;
            this.MoveToEntityButton.Text = "To entity";
            this.MoveToEntityButton.UseVisualStyleBackColor = true;
            this.MoveToEntityButton.Click += new System.EventHandler(this.MoveToEntityClicked);
            // 
            // BrushCreateCollapse
            // 
            this.BrushCreateCollapse.Collapsed = false;
            this.BrushCreateCollapse.ControlToCollapse = this.BrushCreatePanel;
            this.BrushCreateCollapse.LabelText = "Brush Types";
            this.BrushCreateCollapse.Location = new System.Drawing.Point(3, 454);
            this.BrushCreateCollapse.Name = "BrushCreateCollapse";
            this.BrushCreateCollapse.Size = new System.Drawing.Size(144, 14);
            this.BrushCreateCollapse.TabIndex = 2;
            // 
            // sidebarPanel1
            // 
            this.sidebarPanel1.AutoSize = true;
            this.sidebarPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidebarPanel1.Hidden = false;
            this.sidebarPanel1.Location = new System.Drawing.Point(3, 568);
            this.sidebarPanel1.Name = "sidebarPanel1";
            this.sidebarPanel1.Size = new System.Drawing.Size(144, 106);
            this.sidebarPanel1.TabIndex = 9;
            this.sidebarPanel1.Text = "This is a test panel";
            // 
            // tspTools
            // 
            this.tspTools.Dock = System.Windows.Forms.DockStyle.None;
            this.tspTools.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tspTools.Location = new System.Drawing.Point(0, 3);
            this.tspTools.Name = "tspTools";
            this.tspTools.Padding = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.tspTools.Size = new System.Drawing.Size(34, 117);
            this.tspTools.TabIndex = 0;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 750);
            this.Controls.Add(this.tscToolStrip);
            this.Controls.Add(this.stsStatus);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMain;
            this.Name = "Editor";
            this.Text = "Sledge";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorClosing);
            this.Load += new System.EventHandler(this.EditorLoad);
            this.Shown += new System.EventHandler(this.EditorShown);
            this.stsStatus.ResumeLayout(false);
            this.stsStatus.PerformLayout();
            this.tscToolStrip.ContentPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.PerformLayout();
            this.tscToolStrip.ResumeLayout(false);
            this.tscToolStrip.PerformLayout();
            this.DockFill.ResumeLayout(false);
            this.DocumentTabs.ResumeLayout(false);
            this.sidebarPanel.ResumeLayout(false);
            this.RightToolbar.ResumeLayout(false);
            this.RightToolbar.PerformLayout();
            this.VisgroupsPanel.ResumeLayout(false);
            this.TexturePanel.ResumeLayout(false);
            this.TexturePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextureSelectionPictureBox)).EndInit();
            this.BrushCreatePanel.ResumeLayout(false);
            this.EntityPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.StatusStrip stsStatus;
        private System.Windows.Forms.ToolStripContainer tscToolStrip;
        private System.Windows.Forms.ToolStrip tspTools;
        private UI.QuadSplitControl tblQuadView;
        private DockedPanel sidebarPanel;
        private System.Windows.Forms.TableLayoutPanel RightToolbar;
        private UI.CollapsingLabel TextureCollapse;
        private UI.CollapsingLabel BrushCreateCollapse;
        private System.Windows.Forms.Panel TexturePanel;
        private System.Windows.Forms.FlowLayoutPanel BrushCreatePanel;
        private System.Windows.Forms.ComboBox BrushTypeList;
        private UI.CollapsingLabel EntityCollapse;
        private System.Windows.Forms.FlowLayoutPanel EntityPanel;
        private System.Windows.Forms.ComboBox EntityTypeList;
        private System.Windows.Forms.Button MoveToWorldButton;
        private System.Windows.Forms.Button MoveToEntityButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private UI.CollapsingLabel VisgroupsCollapse;
        private System.Windows.Forms.Panel VisgroupsPanel;
        private System.Windows.Forms.ComboBox TextureGroupComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private UI.TextureComboBox TextureComboBox;
        private System.Windows.Forms.Label TextureSizeLabel;
        private System.Windows.Forms.PictureBox TextureSelectionPictureBox;
        private System.Windows.Forms.Button TextureBrowseButton;
        private System.Windows.Forms.Button TextureReplaceButton;
        private UI.CollapsingLabel HistoryCollapse;
        private System.Windows.Forms.FlowLayoutPanel HistoryPanel;
        private Visgroups.VisgroupToolbarPanel VisgroupToolbarPanel;
        private System.Windows.Forms.ToolStripStatusLabel StatusTextLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusSelectionLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusCoordinatesLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusBoxLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusZoomLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusSnapLabel;
        private UI.ClosableTabControl DocumentTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private UI.DockedPanel DockFill;
        private UI.DockedPanel DockRight;
        private UI.DockedPanel DockLeft;
        private UI.DockedPanel DockBottom;
        private UI.Sidebar.SidebarPanel sidebarPanel1;
    }
}

