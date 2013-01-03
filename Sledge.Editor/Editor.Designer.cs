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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuToolsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.compileMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.etcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stsStatus = new System.Windows.Forms.StatusStrip();
            this.tscToolStrip = new System.Windows.Forms.ToolStripContainer();
            this.tblQuadView = new Sledge.Editor.UI.QuadSplitControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RightToolbar = new System.Windows.Forms.TableLayoutPanel();
            this.VisgroupsPanel = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.VisgroupTree = new Sledge.Editor.Visgroups.VisgroupPanel();
            this.VisgroupsCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TextureCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.TexturePanel = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BrushCreatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.BrushTypeList = new System.Windows.Forms.ComboBox();
            this.EntityCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.EntityPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.EntityTypeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.BrushCreateCollapse = new Sledge.Editor.UI.CollapsingLabel();
            this.tspTools = new System.Windows.Forms.ToolStrip();
            this.tspFile = new System.Windows.Forms.ToolStrip();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRandom = new System.Windows.Forms.ToolStripButton();
            this.mnuMain.SuspendLayout();
            this.tscToolStrip.ContentPanel.SuspendLayout();
            this.tscToolStrip.LeftToolStripPanel.SuspendLayout();
            this.tscToolStrip.TopToolStripPanel.SuspendLayout();
            this.tscToolStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.RightToolbar.SuspendLayout();
            this.VisgroupsPanel.SuspendLayout();
            this.TexturePanel.SuspendLayout();
            this.BrushCreatePanel.SuspendLayout();
            this.EntityPanel.SuspendLayout();
            this.tspFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.etcToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(866, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuToolsSettings,
            this.compileMapToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // MenuToolsSettings
            // 
            this.MenuToolsSettings.Name = "MenuToolsSettings";
            this.MenuToolsSettings.Size = new System.Drawing.Size(146, 22);
            this.MenuToolsSettings.Text = "Settings";
            this.MenuToolsSettings.Click += new System.EventHandler(this.OpenSettingsDialog);
            // 
            // compileMapToolStripMenuItem
            // 
            this.compileMapToolStripMenuItem.Name = "compileMapToolStripMenuItem";
            this.compileMapToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.compileMapToolStripMenuItem.Text = "Compile Map";
            this.compileMapToolStripMenuItem.Click += new System.EventHandler(this.CompileMapClicked);
            // 
            // etcToolStripMenuItem
            // 
            this.etcToolStripMenuItem.Name = "etcToolStripMenuItem";
            this.etcToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.etcToolStripMenuItem.Text = "Etc.";
            // 
            // stsStatus
            // 
            this.stsStatus.Location = new System.Drawing.Point(0, 506);
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
            this.tscToolStrip.ContentPanel.Size = new System.Drawing.Size(832, 457);
            this.tscToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // tscToolStrip.LeftToolStripPanel
            // 
            this.tscToolStrip.LeftToolStripPanel.Controls.Add(this.tspTools);
            this.tscToolStrip.Location = new System.Drawing.Point(0, 24);
            this.tscToolStrip.Name = "tscToolStrip";
            this.tscToolStrip.Size = new System.Drawing.Size(866, 482);
            this.tscToolStrip.TabIndex = 2;
            this.tscToolStrip.Text = "tscToolStrip";
            // 
            // tscToolStrip.TopToolStripPanel
            // 
            this.tscToolStrip.TopToolStripPanel.Controls.Add(this.tspFile);
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
            this.tblQuadView.Size = new System.Drawing.Size(682, 457);
            this.tblQuadView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RightToolbar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(682, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 457);
            this.panel1.TabIndex = 1;
            // 
            // RightToolbar
            // 
            this.RightToolbar.ColumnCount = 1;
            this.RightToolbar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
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
            this.RightToolbar.RowCount = 7;
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RightToolbar.Size = new System.Drawing.Size(150, 457);
            this.RightToolbar.TabIndex = 2;
            // 
            // VisgroupsPanel
            // 
            this.VisgroupsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VisgroupsPanel.Controls.Add(this.button4);
            this.VisgroupsPanel.Controls.Add(this.button3);
            this.VisgroupsPanel.Controls.Add(this.editButton);
            this.VisgroupsPanel.Controls.Add(this.VisgroupTree);
            this.VisgroupsPanel.Location = new System.Drawing.Point(3, 150);
            this.VisgroupsPanel.Name = "VisgroupsPanel";
            this.VisgroupsPanel.Size = new System.Drawing.Size(144, 144);
            this.VisgroupsPanel.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(84, 120);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(57, 22);
            this.button4.TabIndex = 1;
            this.button4.Tag = "ShowAll";
            this.button4.Text = "Show All";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(39, 120);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(46, 22);
            this.button3.TabIndex = 1;
            this.button3.Tag = "Select";
            this.button3.Text = "Select";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(1, 120);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(39, 22);
            this.editButton.TabIndex = 1;
            this.editButton.Tag = "Edit";
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            // 
            // VisgroupTree
            // 
            this.VisgroupTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisgroupTree.Location = new System.Drawing.Point(1, 1);
            this.VisgroupTree.Name = "VisgroupTree";
            this.VisgroupTree.ShowCheckboxes = true;
            this.VisgroupTree.Size = new System.Drawing.Size(140, 119);
            this.VisgroupTree.TabIndex = 0;
            // 
            // VisgroupsCollapse
            // 
            this.VisgroupsCollapse.Collapsed = false;
            this.VisgroupsCollapse.ControlToCollapse = this.VisgroupsPanel;
            this.VisgroupsCollapse.LabelText = "Visgroups";
            this.VisgroupsCollapse.Location = new System.Drawing.Point(3, 130);
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
            this.TexturePanel.Controls.Add(this.textBox1);
            this.TexturePanel.Location = new System.Drawing.Point(3, 24);
            this.TexturePanel.Name = "TexturePanel";
            this.TexturePanel.Size = new System.Drawing.Size(144, 100);
            this.TexturePanel.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(23, 43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // BrushCreatePanel
            // 
            this.BrushCreatePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BrushCreatePanel.Controls.Add(this.label3);
            this.BrushCreatePanel.Controls.Add(this.BrushTypeList);
            this.BrushCreatePanel.Location = new System.Drawing.Point(3, 435);
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
            this.EntityCollapse.Location = new System.Drawing.Point(3, 300);
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
            this.EntityPanel.Controls.Add(this.button1);
            this.EntityPanel.Controls.Add(this.button2);
            this.EntityPanel.Location = new System.Drawing.Point(3, 320);
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "To world";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(74, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "To entity";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // BrushCreateCollapse
            // 
            this.BrushCreateCollapse.Collapsed = false;
            this.BrushCreateCollapse.ControlToCollapse = this.BrushCreatePanel;
            this.BrushCreateCollapse.LabelText = "Brush Types";
            this.BrushCreateCollapse.Location = new System.Drawing.Point(3, 415);
            this.BrushCreateCollapse.Name = "BrushCreateCollapse";
            this.BrushCreateCollapse.Size = new System.Drawing.Size(144, 14);
            this.BrushCreateCollapse.TabIndex = 2;
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
            // tspFile
            // 
            this.tspFile.Dock = System.Windows.Forms.DockStyle.None;
            this.tspFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbOpen,
            this.tsbSave,
            this.toolStripSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.tsbRandom});
            this.tspFile.Location = new System.Drawing.Point(3, 0);
            this.tspFile.Name = "tspFile";
            this.tspFile.Size = new System.Drawing.Size(185, 25);
            this.tspFile.TabIndex = 0;
            // 
            // tsbNew
            // 
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.Image = ((System.Drawing.Image)(resources.GetObject("tsbNew.Image")));
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(23, 22);
            this.tsbNew.Text = "&New";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(23, 22);
            this.tsbOpen.Text = "&Open";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "&Save";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cutToolStripButton.Text = "C&ut";
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copyToolStripButton.Text = "&Copy";
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
            this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pasteToolStripButton.Text = "&Paste";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbRandom
            // 
            this.tsbRandom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRandom.Image = ((System.Drawing.Image)(resources.GetObject("tsbRandom.Image")));
            this.tsbRandom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRandom.Name = "tsbRandom";
            this.tsbRandom.Size = new System.Drawing.Size(23, 22);
            this.tsbRandom.Text = "He&lp";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 528);
            this.Controls.Add(this.tscToolStrip);
            this.Controls.Add(this.stsStatus);
            this.Controls.Add(this.mnuMain);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMain;
            this.Name = "Editor";
            this.Text = "Sledge";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EditorLoad);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.tscToolStrip.ContentPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.ResumeLayout(false);
            this.tscToolStrip.LeftToolStripPanel.PerformLayout();
            this.tscToolStrip.TopToolStripPanel.ResumeLayout(false);
            this.tscToolStrip.TopToolStripPanel.PerformLayout();
            this.tscToolStrip.ResumeLayout(false);
            this.tscToolStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.RightToolbar.ResumeLayout(false);
            this.VisgroupsPanel.ResumeLayout(false);
            this.TexturePanel.ResumeLayout(false);
            this.TexturePanel.PerformLayout();
            this.BrushCreatePanel.ResumeLayout(false);
            this.EntityPanel.ResumeLayout(false);
            this.tspFile.ResumeLayout(false);
            this.tspFile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem etcToolStripMenuItem;
        private System.Windows.Forms.StatusStrip stsStatus;
        private System.Windows.Forms.ToolStripContainer tscToolStrip;
        private System.Windows.Forms.ToolStrip tspFile;
        private System.Windows.Forms.ToolStripButton tsbNew;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbRandom;
        private System.Windows.Forms.ToolStrip tspTools;
        private UI.QuadSplitControl tblQuadView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel RightToolbar;
        private UI.CollapsingLabel TextureCollapse;
        private UI.CollapsingLabel BrushCreateCollapse;
        private System.Windows.Forms.Panel TexturePanel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuToolsSettings;
        private System.Windows.Forms.FlowLayoutPanel BrushCreatePanel;
        private System.Windows.Forms.ComboBox BrushTypeList;
        private UI.CollapsingLabel EntityCollapse;
        private System.Windows.Forms.FlowLayoutPanel EntityPanel;
        private System.Windows.Forms.ComboBox EntityTypeList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private UI.CollapsingLabel VisgroupsCollapse;
        private System.Windows.Forms.Panel VisgroupsPanel;
        private Visgroups.VisgroupPanel VisgroupTree;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.ToolStripMenuItem compileMapToolStripMenuItem;
    }
}

