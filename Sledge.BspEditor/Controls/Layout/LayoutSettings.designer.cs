namespace Sledge.BspEditor.Controls.Layout
{
    partial class LayoutSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutSettings));
            this.TableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Rows = new System.Windows.Forms.NumericUpDown();
            this.lblRows = new System.Windows.Forms.Label();
            this.Columns = new System.Windows.Forms.NumericUpDown();
            this.lblColumns = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.WindowDropDown = new System.Windows.Forms.ComboBox();
            this.lblWindow = new System.Windows.Forms.Label();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblPreset = new System.Windows.Forms.Label();
            this.PresetButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.Rows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Columns)).BeginInit();
            this.SuspendLayout();
            // 
            // TableLayout
            // 
            this.TableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayout.ColumnCount = 2;
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayout.Location = new System.Drawing.Point(12, 39);
            this.TableLayout.Name = "TableLayout";
            this.TableLayout.RowCount = 2;
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayout.Size = new System.Drawing.Size(290, 230);
            this.TableLayout.TabIndex = 0;
            // 
            // Rows
            // 
            this.Rows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Rows.Location = new System.Drawing.Point(308, 61);
            this.Rows.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.Rows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Rows.Name = "Rows";
            this.Rows.Size = new System.Drawing.Size(45, 20);
            this.Rows.TabIndex = 1;
            this.Rows.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Rows.ValueChanged += new System.EventHandler(this.RowsValueChanged);
            // 
            // lblRows
            // 
            this.lblRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(305, 45);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(34, 13);
            this.lblRows.TabIndex = 2;
            this.lblRows.Text = "Rows";
            // 
            // Columns
            // 
            this.Columns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Columns.Location = new System.Drawing.Point(96, 272);
            this.Columns.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.Columns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Columns.Name = "Columns";
            this.Columns.Size = new System.Drawing.Size(45, 20);
            this.Columns.TabIndex = 1;
            this.Columns.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Columns.ValueChanged += new System.EventHandler(this.ColumnsValueChanged);
            // 
            // lblColumns
            // 
            this.lblColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblColumns.Location = new System.Drawing.Point(12, 272);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(78, 20);
            this.lblColumns.TabIndex = 2;
            this.lblColumns.Text = "Columns";
            this.lblColumns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(278, 272);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.ApplyButtonClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(197, 272);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // WindowDropDown
            // 
            this.WindowDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WindowDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WindowDropDown.FormattingEnabled = true;
            this.WindowDropDown.Location = new System.Drawing.Point(96, 12);
            this.WindowDropDown.Name = "WindowDropDown";
            this.WindowDropDown.Size = new System.Drawing.Size(206, 21);
            this.WindowDropDown.TabIndex = 4;
            this.WindowDropDown.SelectedIndexChanged += new System.EventHandler(this.WindowDropDownSelectedIndexChanged);
            // 
            // lblWindow
            // 
            this.lblWindow.Location = new System.Drawing.Point(9, 12);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(81, 21);
            this.lblWindow.TabIndex = 2;
            this.lblWindow.Text = "Window";
            this.lblWindow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblInstructions
            // 
            this.lblInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(12, 298);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(233, 39);
            this.lblInstructions.TabIndex = 2;
            this.lblInstructions.Text = "Each rectangle represents a viewport.\r\nClick and drag rectangles to combine viewp" +
    "orts.\r\nClick a combined viewport to un-combine it.";
            // 
            // lblPreset
            // 
            this.lblPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(12, 346);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(131, 13);
            this.lblPreset.TabIndex = 2;
            this.lblPreset.Text = "Click to try a preset layout:";
            // 
            // PresetButtonPanel
            // 
            this.PresetButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetButtonPanel.Location = new System.Drawing.Point(15, 364);
            this.PresetButtonPanel.Name = "PresetButtonPanel";
            this.PresetButtonPanel.Size = new System.Drawing.Size(338, 24);
            this.PresetButtonPanel.TabIndex = 5;
            this.PresetButtonPanel.WrapContents = false;
            // 
            // LayoutSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 400);
            this.Controls.Add(this.PresetButtonPanel);
            this.Controls.Add(this.WindowDropDown);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblWindow);
            this.Controls.Add(this.lblColumns);
            this.Controls.Add(this.Columns);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.lblPreset);
            this.Controls.Add(this.lblRows);
            this.Controls.Add(this.Rows);
            this.Controls.Add(this.TableLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 380);
            this.Name = "LayoutSettings";
            this.Text = "Layout Settings";
            ((System.ComponentModel.ISupportInitialize)(this.Rows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Columns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayout;
        private System.Windows.Forms.NumericUpDown Rows;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.NumericUpDown Columns;
        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox WindowDropDown;
        private System.Windows.Forms.Label lblWindow;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.FlowLayoutPanel PresetButtonPanel;
    }
}