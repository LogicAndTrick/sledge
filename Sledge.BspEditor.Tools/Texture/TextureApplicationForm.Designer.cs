namespace Sledge.BspEditor.Tools.Texture
{
    partial class TextureApplicationForm
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
            this.HideMaskCheckbox = new System.Windows.Forms.CheckBox();
            this.RecentFilterTextbox = new System.Windows.Forms.TextBox();
            this.FilterRecentLabel = new System.Windows.Forms.Label();
            this.SmoothingGroupsButton = new System.Windows.Forms.Button();
            this.AlignGroup = new System.Windows.Forms.GroupBox();
            this.AlignToFaceCheckbox = new System.Windows.Forms.CheckBox();
            this.AlignToWorldCheckbox = new System.Windows.Forms.CheckBox();
            this.JustifyGroup = new System.Windows.Forms.GroupBox();
            this.JustifyTopButton = new System.Windows.Forms.Button();
            this.JustifyFitButton = new System.Windows.Forms.Button();
            this.TreatAsOneCheckbox = new System.Windows.Forms.CheckBox();
            this.JustifyRightButton = new System.Windows.Forms.Button();
            this.JustifyBottomButton = new System.Windows.Forms.Button();
            this.JustifyCenterButton = new System.Windows.Forms.Button();
            this.JustifyLeftButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.RotationValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.ReplaceButton = new System.Windows.Forms.Button();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.RotationLabel = new System.Windows.Forms.Label();
            this.TextureDetailsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ScaleXValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.ScaleLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ShiftLabel = new System.Windows.Forms.Label();
            this.ScaleYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.ShiftXValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.ShiftYValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.LightmapLabel = new System.Windows.Forms.Label();
            this.LightmapValue = new Sledge.Shell.Controls.NumericUpDownEx();
            this.HoverTip = new System.Windows.Forms.ToolTip(this.components);
            this.SelectedTextureListPanel = new System.Windows.Forms.Panel();
            this.RecentTextureListPanel = new System.Windows.Forms.Panel();
            this.LeftClickActionButton = new Sledge.Shell.Controls.DropdownButton();
            this.LeftClickActionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RightClickActionButton = new Sledge.Shell.Controls.DropdownButton();
            this.RightClickActionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AlignGroup.SuspendLayout();
            this.JustifyGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RotationValue)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleXValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleYValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftXValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftYValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapValue)).BeginInit();
            this.SuspendLayout();
            // 
            // HideMaskCheckbox
            // 
            this.HideMaskCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.HideMaskCheckbox.Location = new System.Drawing.Point(303, 152);
            this.HideMaskCheckbox.Name = "HideMaskCheckbox";
            this.HideMaskCheckbox.Size = new System.Drawing.Size(102, 23);
            this.HideMaskCheckbox.TabIndex = 34;
            this.HideMaskCheckbox.Text = "Hide Mask";
            this.HideMaskCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.HideMaskCheckbox.UseVisualStyleBackColor = true;
            this.HideMaskCheckbox.CheckedChanged += new System.EventHandler(this.HideMaskCheckboxToggled);
            // 
            // RecentFilterTextbox
            // 
            this.RecentFilterTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RecentFilterTextbox.Location = new System.Drawing.Point(318, 390);
            this.RecentFilterTextbox.Name = "RecentFilterTextbox";
            this.RecentFilterTextbox.Size = new System.Drawing.Size(87, 20);
            this.RecentFilterTextbox.TabIndex = 33;
            this.RecentFilterTextbox.TextChanged += new System.EventHandler(this.RecentFilterTextChanged);
            // 
            // FilterRecentLabel
            // 
            this.FilterRecentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterRecentLabel.Location = new System.Drawing.Point(318, 360);
            this.FilterRecentLabel.Name = "FilterRecentLabel";
            this.FilterRecentLabel.Size = new System.Drawing.Size(87, 24);
            this.FilterRecentLabel.TabIndex = 32;
            this.FilterRecentLabel.Text = "Filter Recent:";
            this.FilterRecentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SmoothingGroupsButton
            // 
            this.SmoothingGroupsButton.Enabled = false;
            this.SmoothingGroupsButton.Location = new System.Drawing.Point(180, 69);
            this.SmoothingGroupsButton.Name = "SmoothingGroupsButton";
            this.SmoothingGroupsButton.Size = new System.Drawing.Size(117, 23);
            this.SmoothingGroupsButton.TabIndex = 31;
            this.SmoothingGroupsButton.Text = "Smoothing Groups";
            this.SmoothingGroupsButton.UseVisualStyleBackColor = true;
            this.SmoothingGroupsButton.Click += new System.EventHandler(this.SmoothingGroupsButtonClicked);
            // 
            // AlignGroup
            // 
            this.AlignGroup.Controls.Add(this.AlignToFaceCheckbox);
            this.AlignGroup.Controls.Add(this.AlignToWorldCheckbox);
            this.AlignGroup.Location = new System.Drawing.Point(174, 97);
            this.AlignGroup.Name = "AlignGroup";
            this.AlignGroup.Size = new System.Drawing.Size(122, 52);
            this.AlignGroup.TabIndex = 30;
            this.AlignGroup.TabStop = false;
            this.AlignGroup.Text = "Align";
            // 
            // AlignToFaceCheckbox
            // 
            this.AlignToFaceCheckbox.AutoSize = true;
            this.AlignToFaceCheckbox.Location = new System.Drawing.Point(63, 22);
            this.AlignToFaceCheckbox.Name = "AlignToFaceCheckbox";
            this.AlignToFaceCheckbox.Size = new System.Drawing.Size(50, 17);
            this.AlignToFaceCheckbox.TabIndex = 0;
            this.AlignToFaceCheckbox.Text = "Face";
            this.AlignToFaceCheckbox.UseVisualStyleBackColor = true;
            this.AlignToFaceCheckbox.Click += new System.EventHandler(this.AlignToFaceClicked);
            // 
            // AlignToWorldCheckbox
            // 
            this.AlignToWorldCheckbox.AutoSize = true;
            this.AlignToWorldCheckbox.Location = new System.Drawing.Point(7, 22);
            this.AlignToWorldCheckbox.Name = "AlignToWorldCheckbox";
            this.AlignToWorldCheckbox.Size = new System.Drawing.Size(54, 17);
            this.AlignToWorldCheckbox.TabIndex = 0;
            this.AlignToWorldCheckbox.Text = "World";
            this.AlignToWorldCheckbox.UseVisualStyleBackColor = true;
            this.AlignToWorldCheckbox.Click += new System.EventHandler(this.AlignToWorldClicked);
            // 
            // JustifyGroup
            // 
            this.JustifyGroup.Controls.Add(this.JustifyTopButton);
            this.JustifyGroup.Controls.Add(this.JustifyFitButton);
            this.JustifyGroup.Controls.Add(this.TreatAsOneCheckbox);
            this.JustifyGroup.Controls.Add(this.JustifyRightButton);
            this.JustifyGroup.Controls.Add(this.JustifyBottomButton);
            this.JustifyGroup.Controls.Add(this.JustifyCenterButton);
            this.JustifyGroup.Controls.Add(this.JustifyLeftButton);
            this.JustifyGroup.Location = new System.Drawing.Point(303, 12);
            this.JustifyGroup.Name = "JustifyGroup";
            this.JustifyGroup.Size = new System.Drawing.Size(102, 137);
            this.JustifyGroup.TabIndex = 29;
            this.JustifyGroup.TabStop = false;
            this.JustifyGroup.Text = "Justify";
            // 
            // JustifyTopButton
            // 
            this.JustifyTopButton.Location = new System.Drawing.Point(40, 15);
            this.JustifyTopButton.Name = "JustifyTopButton";
            this.JustifyTopButton.Size = new System.Drawing.Size(20, 20);
            this.JustifyTopButton.TabIndex = 3;
            this.JustifyTopButton.Text = "T";
            this.JustifyTopButton.UseVisualStyleBackColor = true;
            this.JustifyTopButton.Click += new System.EventHandler(this.JustifyTopClicked);
            // 
            // JustifyFitButton
            // 
            this.JustifyFitButton.Location = new System.Drawing.Point(16, 87);
            this.JustifyFitButton.Name = "JustifyFitButton";
            this.JustifyFitButton.Size = new System.Drawing.Size(68, 20);
            this.JustifyFitButton.TabIndex = 4;
            this.JustifyFitButton.Text = "Fit";
            this.JustifyFitButton.UseVisualStyleBackColor = true;
            this.JustifyFitButton.Click += new System.EventHandler(this.JustifyFitClicked);
            // 
            // TreatAsOneCheckbox
            // 
            this.TreatAsOneCheckbox.Location = new System.Drawing.Point(6, 113);
            this.TreatAsOneCheckbox.Name = "TreatAsOneCheckbox";
            this.TreatAsOneCheckbox.Size = new System.Drawing.Size(90, 21);
            this.TreatAsOneCheckbox.TabIndex = 5;
            this.TreatAsOneCheckbox.Text = "Treat as One";
            this.TreatAsOneCheckbox.UseVisualStyleBackColor = true;
            this.TreatAsOneCheckbox.CheckedChanged += new System.EventHandler(this.TreatAsOneCheckboxToggled);
            // 
            // JustifyRightButton
            // 
            this.JustifyRightButton.Location = new System.Drawing.Point(64, 39);
            this.JustifyRightButton.Name = "JustifyRightButton";
            this.JustifyRightButton.Size = new System.Drawing.Size(20, 20);
            this.JustifyRightButton.TabIndex = 3;
            this.JustifyRightButton.Text = "R";
            this.JustifyRightButton.UseVisualStyleBackColor = true;
            this.JustifyRightButton.Click += new System.EventHandler(this.JustifyRightClicked);
            // 
            // JustifyBottomButton
            // 
            this.JustifyBottomButton.Location = new System.Drawing.Point(40, 63);
            this.JustifyBottomButton.Name = "JustifyBottomButton";
            this.JustifyBottomButton.Size = new System.Drawing.Size(20, 20);
            this.JustifyBottomButton.TabIndex = 3;
            this.JustifyBottomButton.Text = "B";
            this.JustifyBottomButton.UseVisualStyleBackColor = true;
            this.JustifyBottomButton.Click += new System.EventHandler(this.JustifyBottomClicked);
            // 
            // JustifyCenterButton
            // 
            this.JustifyCenterButton.Location = new System.Drawing.Point(40, 39);
            this.JustifyCenterButton.Name = "JustifyCenterButton";
            this.JustifyCenterButton.Size = new System.Drawing.Size(20, 20);
            this.JustifyCenterButton.TabIndex = 3;
            this.JustifyCenterButton.Text = "C";
            this.JustifyCenterButton.UseVisualStyleBackColor = true;
            this.JustifyCenterButton.Click += new System.EventHandler(this.JustifyCenterClicked);
            // 
            // JustifyLeftButton
            // 
            this.JustifyLeftButton.Location = new System.Drawing.Point(16, 39);
            this.JustifyLeftButton.Name = "JustifyLeftButton";
            this.JustifyLeftButton.Size = new System.Drawing.Size(20, 20);
            this.JustifyLeftButton.TabIndex = 3;
            this.JustifyLeftButton.Text = "L";
            this.JustifyLeftButton.UseVisualStyleBackColor = true;
            this.JustifyLeftButton.Click += new System.EventHandler(this.JustifyLeftClicked);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(93, 97);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 52);
            this.ApplyButton.TabIndex = 22;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButtonClicked);
            // 
            // RotationValue
            // 
            this.RotationValue.BackColor = System.Drawing.SystemColors.Window;
            this.RotationValue.DecimalPlaces = 2;
            this.RotationValue.Location = new System.Drawing.Point(239, 17);
            this.RotationValue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationValue.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationValue.Name = "RotationValue";
            this.RotationValue.Size = new System.Drawing.Size(57, 20);
            this.RotationValue.TabIndex = 18;
            this.RotationValue.ValueChanged += new System.EventHandler(this.RotationValueChanged);
            this.RotationValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // ReplaceButton
            // 
            this.ReplaceButton.Location = new System.Drawing.Point(12, 126);
            this.ReplaceButton.Name = "ReplaceButton";
            this.ReplaceButton.Size = new System.Drawing.Size(75, 23);
            this.ReplaceButton.TabIndex = 24;
            this.ReplaceButton.Text = "Replace...";
            this.ReplaceButton.UseVisualStyleBackColor = true;
            this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButtonClicked);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(12, 97);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 23;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClicked);
            // 
            // RotationLabel
            // 
            this.RotationLabel.Location = new System.Drawing.Point(174, 13);
            this.RotationLabel.Name = "RotationLabel";
            this.RotationLabel.Size = new System.Drawing.Size(59, 25);
            this.RotationLabel.TabIndex = 17;
            this.RotationLabel.Text = "Rotation";
            this.RotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextureDetailsLabel
            // 
            this.TextureDetailsLabel.Location = new System.Drawing.Point(12, 152);
            this.TextureDetailsLabel.Name = "TextureDetailsLabel";
            this.TextureDetailsLabel.Size = new System.Drawing.Size(393, 23);
            this.TextureDetailsLabel.TabIndex = 21;
            this.TextureDetailsLabel.Text = "";
            this.TextureDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 226F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.ScaleXValue, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ScaleLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ShiftLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ScaleYValue, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ShiftXValue, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ShiftYValue, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(156, 80);
            this.tableLayoutPanel1.TabIndex = 20;
            // 
            // ScaleXValue
            // 
            this.ScaleXValue.DecimalPlaces = 4;
            this.ScaleXValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ScaleXValue.Location = new System.Drawing.Point(25, 30);
            this.ScaleXValue.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.ScaleXValue.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.ScaleXValue.Name = "ScaleXValue";
            this.ScaleXValue.Size = new System.Drawing.Size(57, 20);
            this.ScaleXValue.TabIndex = 1;
            this.ScaleXValue.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.ScaleXValue.WheelIncrement = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleXValue.ValueChanged += new System.EventHandler(this.ScaleXValueChanged);
            this.ScaleXValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // ScaleLabel
            // 
            this.ScaleLabel.Location = new System.Drawing.Point(25, 1);
            this.ScaleLabel.Name = "ScaleLabel";
            this.ScaleLabel.Size = new System.Drawing.Size(59, 25);
            this.ScaleLabel.TabIndex = 0;
            this.ScaleLabel.Text = "Scale";
            this.ScaleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "X";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ShiftLabel
            // 
            this.ShiftLabel.Location = new System.Drawing.Point(91, 1);
            this.ShiftLabel.Name = "ShiftLabel";
            this.ShiftLabel.Size = new System.Drawing.Size(59, 25);
            this.ShiftLabel.TabIndex = 0;
            this.ShiftLabel.Text = "Shift";
            this.ShiftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScaleYValue
            // 
            this.ScaleYValue.DecimalPlaces = 4;
            this.ScaleYValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.ScaleYValue.Location = new System.Drawing.Point(25, 56);
            this.ScaleYValue.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.ScaleYValue.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.ScaleYValue.Name = "ScaleYValue";
            this.ScaleYValue.Size = new System.Drawing.Size(57, 20);
            this.ScaleYValue.TabIndex = 1;
            this.ScaleYValue.Value = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.ScaleYValue.WheelIncrement = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleYValue.ValueChanged += new System.EventHandler(this.ScaleYValueChanged);
            this.ScaleYValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // ShiftXValue
            // 
            this.ShiftXValue.CtrlWheelMultiplier = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ShiftXValue.Location = new System.Drawing.Point(91, 30);
            this.ShiftXValue.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.ShiftXValue.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.ShiftXValue.Name = "ShiftXValue";
            this.ShiftXValue.Size = new System.Drawing.Size(58, 20);
            this.ShiftXValue.TabIndex = 1;
            this.ShiftXValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ShiftXValue.ValueChanged += new System.EventHandler(this.ShiftXValueChanged);
            this.ShiftXValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // ShiftYValue
            // 
            this.ShiftYValue.CtrlWheelMultiplier = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ShiftYValue.Location = new System.Drawing.Point(91, 56);
            this.ShiftYValue.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.ShiftYValue.Minimum = new decimal(new int[] {
            4096,
            0,
            0,
            -2147483648});
            this.ShiftYValue.Name = "ShiftYValue";
            this.ShiftYValue.Size = new System.Drawing.Size(58, 20);
            this.ShiftYValue.TabIndex = 1;
            this.ShiftYValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ShiftYValue.ValueChanged += new System.EventHandler(this.ShiftYValueChanged);
            this.ShiftYValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // LightmapLabel
            // 
            this.LightmapLabel.Location = new System.Drawing.Point(174, 38);
            this.LightmapLabel.Name = "LightmapLabel";
            this.LightmapLabel.Size = new System.Drawing.Size(59, 25);
            this.LightmapLabel.TabIndex = 16;
            this.LightmapLabel.Text = "Lightmap";
            this.LightmapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LightmapValue
            // 
            this.LightmapValue.CtrlWheelMultiplier = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.LightmapValue.Enabled = false;
            this.LightmapValue.Location = new System.Drawing.Point(239, 43);
            this.LightmapValue.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.LightmapValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.LightmapValue.Name = "LightmapValue";
            this.LightmapValue.ShiftWheelMultiplier = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.LightmapValue.Size = new System.Drawing.Size(58, 20);
            this.LightmapValue.TabIndex = 19;
            this.LightmapValue.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.LightmapValue.ValueChanged += new System.EventHandler(this.LightmapValueChanged);
            this.LightmapValue.Enter += new System.EventHandler(this.FocusTextInControl);
            // 
            // HoverTip
            // 
            this.HoverTip.AutoPopDelay = 5000;
            this.HoverTip.InitialDelay = 200;
            this.HoverTip.IsBalloon = true;
            this.HoverTip.ReshowDelay = 100;
            // 
            // SelectedTextureListPanel
            // 
            this.SelectedTextureListPanel.Location = new System.Drawing.Point(12, 178);
            this.SelectedTextureListPanel.Name = "SelectedTextureListPanel";
            this.SelectedTextureListPanel.Size = new System.Drawing.Size(300, 206);
            this.SelectedTextureListPanel.TabIndex = 35;
            // 
            // RecentTextureListPanel
            // 
            this.RecentTextureListPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecentTextureListPanel.Location = new System.Drawing.Point(318, 178);
            this.RecentTextureListPanel.Name = "RecentTextureListPanel";
            this.RecentTextureListPanel.Size = new System.Drawing.Size(87, 179);
            this.RecentTextureListPanel.TabIndex = 36;
            // 
            // LeftClickActionButton
            // 
            this.LeftClickActionButton.Location = new System.Drawing.Point(12, 390);
            this.LeftClickActionButton.Menu = this.LeftClickActionMenu;
            this.LeftClickActionButton.Name = "LeftClickActionButton";
            this.LeftClickActionButton.Size = new System.Drawing.Size(148, 23);
            this.LeftClickActionButton.TabIndex = 37;
            this.LeftClickActionButton.Text = "Left click: Lift";
            this.LeftClickActionButton.UseVisualStyleBackColor = true;
            // 
            // LeftClickActionMenu
            // 
            this.LeftClickActionMenu.Name = "LeftClickActionMenu";
            this.LeftClickActionMenu.Size = new System.Drawing.Size(61, 4);
            this.LeftClickActionMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SetLeftClickAction);
            // 
            // RightClickActionButton
            // 
            this.RightClickActionButton.Location = new System.Drawing.Point(164, 390);
            this.RightClickActionButton.Menu = this.RightClickActionMenu;
            this.RightClickActionButton.Name = "RightClickActionButton";
            this.RightClickActionButton.Size = new System.Drawing.Size(148, 23);
            this.RightClickActionButton.TabIndex = 37;
            this.RightClickActionButton.Text = "Right click: Apply";
            this.RightClickActionButton.UseVisualStyleBackColor = true;
            // 
            // RightClickActionMenu
            // 
            this.RightClickActionMenu.Name = "RightClickActionMenu";
            this.RightClickActionMenu.Size = new System.Drawing.Size(61, 4);
            this.RightClickActionMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SetRightClickAction);
            // 
            // TextureApplicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 420);
            this.Controls.Add(this.RightClickActionButton);
            this.Controls.Add(this.LeftClickActionButton);
            this.Controls.Add(this.RecentTextureListPanel);
            this.Controls.Add(this.SelectedTextureListPanel);
            this.Controls.Add(this.HideMaskCheckbox);
            this.Controls.Add(this.RecentFilterTextbox);
            this.Controls.Add(this.FilterRecentLabel);
            this.Controls.Add(this.SmoothingGroupsButton);
            this.Controls.Add(this.AlignGroup);
            this.Controls.Add(this.JustifyGroup);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.RotationValue);
            this.Controls.Add(this.ReplaceButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.RotationLabel);
            this.Controls.Add(this.TextureDetailsLabel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.LightmapLabel);
            this.Controls.Add(this.LightmapValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TextureApplicationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Texture Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.AlignGroup.ResumeLayout(false);
            this.AlignGroup.PerformLayout();
            this.JustifyGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RotationValue)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScaleXValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleYValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftXValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftYValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox HideMaskCheckbox;
        private System.Windows.Forms.TextBox RecentFilterTextbox;
        private System.Windows.Forms.Label FilterRecentLabel;
        private System.Windows.Forms.Button SmoothingGroupsButton;
        private System.Windows.Forms.GroupBox AlignGroup;
        private System.Windows.Forms.GroupBox JustifyGroup;
        private System.Windows.Forms.Button JustifyTopButton;
        private System.Windows.Forms.Button JustifyFitButton;
        private System.Windows.Forms.CheckBox TreatAsOneCheckbox;
        private System.Windows.Forms.Button JustifyRightButton;
        private System.Windows.Forms.Button JustifyBottomButton;
        private System.Windows.Forms.Button JustifyCenterButton;
        private System.Windows.Forms.Button JustifyLeftButton;
        private System.Windows.Forms.Button ApplyButton;
        private Sledge.Shell.Controls.NumericUpDownEx RotationValue;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label RotationLabel;
        private System.Windows.Forms.Label TextureDetailsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleXValue;
        private System.Windows.Forms.Label ScaleLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ShiftLabel;
        private Sledge.Shell.Controls.NumericUpDownEx ScaleYValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftXValue;
        private Sledge.Shell.Controls.NumericUpDownEx ShiftYValue;
        private System.Windows.Forms.Label LightmapLabel;
        private Sledge.Shell.Controls.NumericUpDownEx LightmapValue;
        private System.Windows.Forms.ToolTip HoverTip;
        private System.Windows.Forms.CheckBox AlignToFaceCheckbox;
        private System.Windows.Forms.CheckBox AlignToWorldCheckbox;
        private System.Windows.Forms.Panel SelectedTextureListPanel;
        private System.Windows.Forms.Panel RecentTextureListPanel;
        private Shell.Controls.DropdownButton LeftClickActionButton;
        private Shell.Controls.DropdownButton RightClickActionButton;
        private System.Windows.Forms.ContextMenuStrip LeftClickActionMenu;
        private System.Windows.Forms.ContextMenuStrip RightClickActionMenu;
    }
}