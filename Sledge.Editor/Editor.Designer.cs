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
            Sledge.Editor.UI.TableSplitConfiguration tableSplitConfiguration1 = new Sledge.Editor.UI.TableSplitConfiguration();
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
            this.TableSplitView = new Sledge.Editor.UI.TableSplitControl();
            this.DocumentTabs = new Sledge.Editor.UI.ClosableTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RightSidebar = new Sledge.Editor.UI.DockedPanel();
            this.DockLeft = new Sledge.Editor.UI.DockedPanel();
            this.DockBottom = new Sledge.Editor.UI.DockedPanel();
            this.outputMessagePanel1 = new Sledge.Editor.UI.DockPanels.OutputMessagePanel();
            this.DockRight = new Sledge.Editor.UI.DockedPanel();
            this.tspTools = new System.Windows.Forms.ToolStrip();
            this.stsStatus.SuspendLayout();
            this.tscToolStrip.ContentPanel.SuspendLayout();
            this.tscToolStrip.LeftToolStripPanel.SuspendLayout();
            this.tscToolStrip.SuspendLayout();
            this.DockFill.SuspendLayout();
            this.DocumentTabs.SuspendLayout();
            this.DockBottom.SuspendLayout();
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
            this.StatusTextLabel.Size = new System.Drawing.Size(283, 19);
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
            this.tscToolStrip.ContentPanel.Controls.Add(this.RightSidebar);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockLeft);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockBottom);
            this.tscToolStrip.ContentPanel.Controls.Add(this.DockRight);
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
            this.DockFill.Controls.Add(this.TableSplitView);
            this.DockFill.Controls.Add(this.DocumentTabs);
            this.DockFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockFill.DockDimension = 0;
            this.DockFill.Hidden = false;
            this.DockFill.Location = new System.Drawing.Point(26, 0);
            this.DockFill.Name = "DockFill";
            this.DockFill.Size = new System.Drawing.Size(567, 528);
            this.DockFill.TabIndex = 4;
            // 
            // TableSplitView
            // 
            this.TableSplitView.ColumnCount = 2;
            this.TableSplitView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableSplitConfiguration1.Columns = 2;
            tableSplitConfiguration1.Rectangles = ((System.Collections.Generic.List<System.Drawing.Rectangle>)(resources.GetObject("tableSplitConfiguration1.Rectangles")));
            tableSplitConfiguration1.Rows = 2;
            this.TableSplitView.Configuration = tableSplitConfiguration1;
            this.TableSplitView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableSplitView.Location = new System.Drawing.Point(0, 24);
            this.TableSplitView.MinimumViewSize = 2;
            this.TableSplitView.Name = "TableSplitView";
            this.TableSplitView.RowCount = 2;
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableSplitView.Size = new System.Drawing.Size(567, 504);
            this.TableSplitView.TabIndex = 0;
            // 
            // DocumentTabs
            // 
            this.DocumentTabs.Controls.Add(this.tabPage1);
            this.DocumentTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.DocumentTabs.Location = new System.Drawing.Point(0, 0);
            this.DocumentTabs.Name = "DocumentTabs";
            this.DocumentTabs.SelectedIndex = 0;
            this.DocumentTabs.Size = new System.Drawing.Size(567, 24);
            this.DocumentTabs.TabIndex = 2;
            this.DocumentTabs.RequestClose += new Sledge.Editor.UI.ClosableTabControl.RequestCloseEventHandler(this.DocumentTabsRequestClose);
            this.DocumentTabs.SelectedIndexChanged += new System.EventHandler(this.DocumentTabsSelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(559, -8);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tab";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RightSidebar
            // 
            this.RightSidebar.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightSidebar.DockDimension = 230;
            this.RightSidebar.Hidden = false;
            this.RightSidebar.Location = new System.Drawing.Point(593, 0);
            this.RightSidebar.MinSize = 230;
            this.RightSidebar.Name = "RightSidebar";
            this.RightSidebar.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.RightSidebar.Size = new System.Drawing.Size(230, 528);
            this.RightSidebar.TabIndex = 3;
            // 
            // DockLeft
            // 
            this.DockLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.DockLeft.DockDimension = 26;
            this.DockLeft.Hidden = false;
            this.DockLeft.Location = new System.Drawing.Point(0, 0);
            this.DockLeft.Name = "DockLeft";
            this.DockLeft.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.DockLeft.Size = new System.Drawing.Size(26, 528);
            this.DockLeft.TabIndex = 3;
            // 
            // DockBottom
            // 
            this.DockBottom.Controls.Add(this.outputMessagePanel1);
            this.DockBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DockBottom.DockDimension = 149;
            this.DockBottom.Hidden = false;
            this.DockBottom.Location = new System.Drawing.Point(0, 528);
            this.DockBottom.Name = "DockBottom";
            this.DockBottom.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.DockBottom.Size = new System.Drawing.Size(823, 149);
            this.DockBottom.TabIndex = 3;
            // 
            // outputMessagePanel1
            // 
            this.outputMessagePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputMessagePanel1.Location = new System.Drawing.Point(0, 8);
            this.outputMessagePanel1.Name = "outputMessagePanel1";
            this.outputMessagePanel1.Size = new System.Drawing.Size(823, 141);
            this.outputMessagePanel1.TabIndex = 0;
            // 
            // DockRight
            // 
            this.DockRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.DockRight.DockDimension = 22;
            this.DockRight.Hidden = false;
            this.DockRight.Location = new System.Drawing.Point(823, 0);
            this.DockRight.Name = "DockRight";
            this.DockRight.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.DockRight.Size = new System.Drawing.Size(22, 677);
            this.DockRight.TabIndex = 1;
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
            this.AllowDrop = true;
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
            this.DockBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.StatusStrip stsStatus;
        private System.Windows.Forms.ToolStripContainer tscToolStrip;
        private System.Windows.Forms.ToolStrip tspTools;
        private UI.TableSplitControl TableSplitView;
        private DockedPanel DockRight;
        private System.Windows.Forms.ToolStripStatusLabel StatusTextLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusSelectionLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusCoordinatesLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusBoxLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusZoomLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusSnapLabel;
        private UI.ClosableTabControl DocumentTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private UI.DockedPanel DockFill;
        private UI.DockedPanel RightSidebar;
        private UI.DockedPanel DockLeft;
        private UI.DockedPanel DockBottom;
        private UI.DockPanels.OutputMessagePanel outputMessagePanel1;
    }
}

