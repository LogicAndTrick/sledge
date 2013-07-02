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
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.stsStatus = new System.Windows.Forms.StatusStrip();
            this.tscToolStrip = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RightToolbar = new System.Windows.Forms.TableLayoutPanel();
            this.HistoryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.VisgroupsPanel = new System.Windows.Forms.Panel();
            this.TexturePanel = new System.Windows.Forms.Panel();
            this.TextureBrowseButton = new System.Windows.Forms.Button();
            this.TextureReplaceButton = new System.Windows.Forms.Button();
            this.TextureSizeLabel = new System.Windows.Forms.Label();
            this.TextureSelectionPictureBox = new System.Windows.Forms.PictureBox();
            this.TextureGroupComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BrushCreatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.BrushTypeList = new System.Windows.Forms.ComboBox();
            this.EntityPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.EntityTypeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MoveToWorldButton = new System.Windows.Forms.Button();
            this.MoveToEntityButton = new System.Windows.Forms.Button();
            this.tspTools = new System.Windows.Forms.ToolStrip();
            this.tblQuadView = new Sledge.Editor.UI.QuadSplitControl();
            this.HistoryCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.visgroupToolbarPanel1 = new Sledge.Editor.Visgroups.VisgroupToolbarPanel();
            this.VisgroupsCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TextureCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TextureComboBox = new Sledge.Editor.UI.TextureComboBox();
            this.EntityCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.BrushCreateCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.tscToolStrip.ContentPanel.SuspendLayout();
            this.tscToolStrip.LeftToolStripPanel.SuspendLayout();
            this.tscToolStrip.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.mnuMain.Size = new System.Drawing.Size(866, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // stsStatus
            // 
            this.stsStatus.Location = new System.Drawing.Point(0, 852);
            this.stsStatus.Name = "stsStatus";
            this.stsStatus.Size = new System.Drawing.Size(866, 22);
            this.stsStatus.TabIndex = 1;
            this.stsStatus.Text = "statusStrip1";
            // 
            // tscToolStrip
            // 
            // 
            // tscToolStrip.ContentPanel
            // 
            this.tscToolStrip.ContentPanel.Controls.Add(this.tblQuadView);
            this.tscToolStrip.ContentPanel.Controls.Add(this.panel1);
            this.tscToolStrip.ContentPanel.Size = new System.Drawing.Size(832, 828);
            this.tscToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // tscToolStrip.LeftToolStripPanel
            // 
            this.tscToolStrip.LeftToolStripPanel.Controls.Add(this.tspTools);
            this.tscToolStrip.Location = new System.Drawing.Point(0, 24);
            this.tscToolStrip.Name = "tscToolStrip";
            this.tscToolStrip.Size = new System.Drawing.Size(866, 828);
            this.tscToolStrip.TabIndex = 2;
            this.tscToolStrip.Text = "tscToolStrip";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RightToolbar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(682, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 828);
            this.panel1.TabIndex = 1;
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
            this.RightToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightToolbar.Location = new System.Drawing.Point(0, 0);
            this.RightToolbar.Name = "RightToolbar";
            this.RightToolbar.RowCount = 10;
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
            this.RightToolbar.Size = new System.Drawing.Size(150, 828);
            this.RightToolbar.TabIndex = 2;
            // 
            // HistoryPanel
            // 
            this.HistoryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistoryPanel.Location = new System.Drawing.Point(3, 548);
            this.HistoryPanel.Name = "HistoryPanel";
            this.HistoryPanel.Size = new System.Drawing.Size(144, 98);
            this.HistoryPanel.TabIndex = 8;
            // 
            // VisgroupsPanel
            // 
            this.VisgroupsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VisgroupsPanel.Controls.Add(this.visgroupToolbarPanel1);
            this.VisgroupsPanel.Location = new System.Drawing.Point(3, 189);
            this.VisgroupsPanel.Name = "VisgroupsPanel";
            this.VisgroupsPanel.Size = new System.Drawing.Size(144, 144);
            this.VisgroupsPanel.TabIndex = 0;
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
            this.tblQuadView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblQuadView.Location = new System.Drawing.Point(0, 0);
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
            this.tblQuadView.Size = new System.Drawing.Size(682, 828);
            this.tblQuadView.TabIndex = 0;
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
            // visgroupToolbarPanel1
            // 
            this.visgroupToolbarPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visgroupToolbarPanel1.Location = new System.Drawing.Point(0, 0);
            this.visgroupToolbarPanel1.Name = "visgroupToolbarPanel1";
            this.visgroupToolbarPanel1.Size = new System.Drawing.Size(142, 142);
            this.visgroupToolbarPanel1.TabIndex = 2;
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
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 874);
            this.Controls.Add(this.tscToolStrip);
            this.Controls.Add(this.stsStatus);
            this.Controls.Add(this.mnuMain);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMain;
            this.Name = "Editor";
            this.Text = "Sledge";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EditorLoad);
            this.tscToolStrip.ContentPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.PerformLayout();
            this.tscToolStrip.ResumeLayout(false);
            this.tscToolStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.RightToolbar.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panel1;
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
        private Visgroups.VisgroupToolbarPanel visgroupToolbarPanel1;
    }
}

