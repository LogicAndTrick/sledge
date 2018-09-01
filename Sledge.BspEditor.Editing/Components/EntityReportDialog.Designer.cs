namespace Sledge.BspEditor.Editing.Components
{
    partial class EntityReportDialog
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
            this.EntityList = new System.Windows.Forms.ListView();
            this.ClassNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EntityNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilterGroup = new System.Windows.Forms.GroupBox();
            this.ResetFiltersButton = new System.Windows.Forms.Button();
            this.IncludeHidden = new System.Windows.Forms.CheckBox();
            this.FilterClassExact = new System.Windows.Forms.CheckBox();
            this.FilterKeyValueExact = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterValue = new System.Windows.Forms.TextBox();
            this.FilterClass = new System.Windows.Forms.TextBox();
            this.FilterByClassLabel = new System.Windows.Forms.Label();
            this.FilterKey = new System.Windows.Forms.TextBox();
            this.FilterByKeyValueLabel = new System.Windows.Forms.Label();
            this.TypeBrush = new System.Windows.Forms.RadioButton();
            this.TypePoint = new System.Windows.Forms.RadioButton();
            this.TypeAll = new System.Windows.Forms.RadioButton();
            this.GoToButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.PropertiesButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.FollowSelection = new System.Windows.Forms.CheckBox();
            this.FilterGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // EntityList
            // 
            this.EntityList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EntityList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClassNameHeader,
            this.EntityNameHeader});
            this.EntityList.FullRowSelect = true;
            this.EntityList.HideSelection = false;
            this.EntityList.Location = new System.Drawing.Point(12, 12);
            this.EntityList.MultiSelect = false;
            this.EntityList.Name = "EntityList";
            this.EntityList.Size = new System.Drawing.Size(308, 242);
            this.EntityList.TabIndex = 0;
            this.EntityList.UseCompatibleStateImageBehavior = false;
            this.EntityList.View = System.Windows.Forms.View.Details;
            this.EntityList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.SortByColumn);
            // 
            // ClassNameHeader
            // 
            this.ClassNameHeader.Text = "Class";
            this.ClassNameHeader.Width = 107;
            // 
            // EntityNameHeader
            // 
            this.EntityNameHeader.Text = "Name";
            this.EntityNameHeader.Width = 153;
            // 
            // FilterGroup
            // 
            this.FilterGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterGroup.Controls.Add(this.ResetFiltersButton);
            this.FilterGroup.Controls.Add(this.IncludeHidden);
            this.FilterGroup.Controls.Add(this.FilterClassExact);
            this.FilterGroup.Controls.Add(this.FilterKeyValueExact);
            this.FilterGroup.Controls.Add(this.label2);
            this.FilterGroup.Controls.Add(this.FilterValue);
            this.FilterGroup.Controls.Add(this.FilterClass);
            this.FilterGroup.Controls.Add(this.FilterByClassLabel);
            this.FilterGroup.Controls.Add(this.FilterKey);
            this.FilterGroup.Controls.Add(this.FilterByKeyValueLabel);
            this.FilterGroup.Controls.Add(this.TypeBrush);
            this.FilterGroup.Controls.Add(this.TypePoint);
            this.FilterGroup.Controls.Add(this.TypeAll);
            this.FilterGroup.Location = new System.Drawing.Point(326, 12);
            this.FilterGroup.Name = "FilterGroup";
            this.FilterGroup.Size = new System.Drawing.Size(178, 242);
            this.FilterGroup.TabIndex = 1;
            this.FilterGroup.TabStop = false;
            this.FilterGroup.Text = "Filter";
            // 
            // ResetFiltersButton
            // 
            this.ResetFiltersButton.Location = new System.Drawing.Point(49, 204);
            this.ResetFiltersButton.Name = "ResetFiltersButton";
            this.ResetFiltersButton.Size = new System.Drawing.Size(75, 23);
            this.ResetFiltersButton.TabIndex = 6;
            this.ResetFiltersButton.Text = "Reset Filters";
            this.ResetFiltersButton.UseVisualStyleBackColor = true;
            this.ResetFiltersButton.Click += new System.EventHandler(this.ResetFilters);
            // 
            // IncludeHidden
            // 
            this.IncludeHidden.AutoSize = true;
            this.IncludeHidden.Checked = true;
            this.IncludeHidden.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludeHidden.Location = new System.Drawing.Point(11, 88);
            this.IncludeHidden.Name = "IncludeHidden";
            this.IncludeHidden.Size = new System.Drawing.Size(133, 17);
            this.IncludeHidden.TabIndex = 5;
            this.IncludeHidden.Text = "Include hidden objects";
            this.IncludeHidden.UseVisualStyleBackColor = true;
            this.IncludeHidden.CheckedChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // FilterClassExact
            // 
            this.FilterClassExact.AutoSize = true;
            this.FilterClassExact.Location = new System.Drawing.Point(119, 158);
            this.FilterClassExact.Name = "FilterClassExact";
            this.FilterClassExact.Size = new System.Drawing.Size(53, 17);
            this.FilterClassExact.TabIndex = 4;
            this.FilterClassExact.Text = "Exact";
            this.FilterClassExact.UseVisualStyleBackColor = true;
            this.FilterClassExact.CheckedChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // FilterKeyValueExact
            // 
            this.FilterKeyValueExact.AutoSize = true;
            this.FilterKeyValueExact.Location = new System.Drawing.Point(119, 112);
            this.FilterKeyValueExact.Name = "FilterKeyValueExact";
            this.FilterKeyValueExact.Size = new System.Drawing.Size(53, 17);
            this.FilterKeyValueExact.TabIndex = 4;
            this.FilterKeyValueExact.Text = "Exact";
            this.FilterKeyValueExact.UseVisualStyleBackColor = true;
            this.FilterKeyValueExact.CheckedChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(83, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "=";
            // 
            // FilterValue
            // 
            this.FilterValue.Location = new System.Drawing.Point(102, 132);
            this.FilterValue.Name = "FilterValue";
            this.FilterValue.Size = new System.Drawing.Size(66, 20);
            this.FilterValue.TabIndex = 2;
            this.FilterValue.TextChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // FilterClass
            // 
            this.FilterClass.Location = new System.Drawing.Point(11, 178);
            this.FilterClass.Name = "FilterClass";
            this.FilterClass.Size = new System.Drawing.Size(157, 20);
            this.FilterClass.TabIndex = 2;
            this.FilterClass.TextChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // FilterByClassLabel
            // 
            this.FilterByClassLabel.AutoSize = true;
            this.FilterByClassLabel.Location = new System.Drawing.Point(8, 159);
            this.FilterByClassLabel.Name = "FilterByClassLabel";
            this.FilterByClassLabel.Size = new System.Drawing.Size(73, 13);
            this.FilterByClassLabel.TabIndex = 1;
            this.FilterByClassLabel.Text = "Filter by class:";
            // 
            // FilterKey
            // 
            this.FilterKey.Location = new System.Drawing.Point(11, 132);
            this.FilterKey.Name = "FilterKey";
            this.FilterKey.Size = new System.Drawing.Size(66, 20);
            this.FilterKey.TabIndex = 2;
            this.FilterKey.TextChanged += new System.EventHandler(this.FiltersChanged);
            // 
            // FilterByKeyValueLabel
            // 
            this.FilterByKeyValueLabel.AutoSize = true;
            this.FilterByKeyValueLabel.Location = new System.Drawing.Point(8, 113);
            this.FilterByKeyValueLabel.Name = "FilterByKeyValueLabel";
            this.FilterByKeyValueLabel.Size = new System.Drawing.Size(97, 13);
            this.FilterByKeyValueLabel.TabIndex = 1;
            this.FilterByKeyValueLabel.Text = "Filter by key/value:";
            // 
            // TypeBrush
            // 
            this.TypeBrush.AutoSize = true;
            this.TypeBrush.Location = new System.Drawing.Point(11, 65);
            this.TypeBrush.Name = "TypeBrush";
            this.TypeBrush.Size = new System.Drawing.Size(113, 17);
            this.TypeBrush.TabIndex = 0;
            this.TypeBrush.Text = "Brush Entities Only";
            this.TypeBrush.UseVisualStyleBackColor = true;
            this.TypeBrush.Click += new System.EventHandler(this.FiltersChanged);
            // 
            // TypePoint
            // 
            this.TypePoint.AutoSize = true;
            this.TypePoint.Location = new System.Drawing.Point(11, 42);
            this.TypePoint.Name = "TypePoint";
            this.TypePoint.Size = new System.Drawing.Size(110, 17);
            this.TypePoint.TabIndex = 0;
            this.TypePoint.Text = "Point Entities Only";
            this.TypePoint.UseVisualStyleBackColor = true;
            this.TypePoint.Click += new System.EventHandler(this.FiltersChanged);
            // 
            // TypeAll
            // 
            this.TypeAll.AutoSize = true;
            this.TypeAll.Checked = true;
            this.TypeAll.Location = new System.Drawing.Point(11, 19);
            this.TypeAll.Name = "TypeAll";
            this.TypeAll.Size = new System.Drawing.Size(66, 17);
            this.TypeAll.TabIndex = 0;
            this.TypeAll.TabStop = true;
            this.TypeAll.Text = "Show All";
            this.TypeAll.UseVisualStyleBackColor = true;
            this.TypeAll.Click += new System.EventHandler(this.FiltersChanged);
            // 
            // GoToButton
            // 
            this.GoToButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GoToButton.Location = new System.Drawing.Point(12, 260);
            this.GoToButton.Name = "GoToButton";
            this.GoToButton.Size = new System.Drawing.Size(75, 23);
            this.GoToButton.TabIndex = 2;
            this.GoToButton.Text = "Go to";
            this.GoToButton.UseVisualStyleBackColor = true;
            this.GoToButton.Click += new System.EventHandler(this.GoToSelectedEntity);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeleteButton.Location = new System.Drawing.Point(93, 260);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 3;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteSelectedEntity);
            // 
            // PropertiesButton
            // 
            this.PropertiesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PropertiesButton.Location = new System.Drawing.Point(174, 260);
            this.PropertiesButton.Name = "PropertiesButton";
            this.PropertiesButton.Size = new System.Drawing.Size(75, 23);
            this.PropertiesButton.TabIndex = 4;
            this.PropertiesButton.Text = "Properties";
            this.PropertiesButton.UseVisualStyleBackColor = true;
            this.PropertiesButton.Click += new System.EventHandler(this.OpenEntityProperties);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(429, 260);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 5;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // FollowSelection
            // 
            this.FollowSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FollowSelection.AutoSize = true;
            this.FollowSelection.Checked = true;
            this.FollowSelection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FollowSelection.Location = new System.Drawing.Point(255, 264);
            this.FollowSelection.Name = "FollowSelection";
            this.FollowSelection.Size = new System.Drawing.Size(101, 17);
            this.FollowSelection.TabIndex = 6;
            this.FollowSelection.Text = "Follow selection";
            this.FollowSelection.UseVisualStyleBackColor = true;
            // 
            // EntityReportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 292);
            this.Controls.Add(this.FollowSelection);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.PropertiesButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.GoToButton);
            this.Controls.Add(this.FilterGroup);
            this.Controls.Add(this.EntityList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 330);
            this.Name = "EntityReportDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entity Report";
            this.FilterGroup.ResumeLayout(false);
            this.FilterGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView EntityList;
        private System.Windows.Forms.ColumnHeader ClassNameHeader;
        private System.Windows.Forms.ColumnHeader EntityNameHeader;
        private System.Windows.Forms.GroupBox FilterGroup;
        private System.Windows.Forms.RadioButton TypeBrush;
        private System.Windows.Forms.RadioButton TypePoint;
        private System.Windows.Forms.RadioButton TypeAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterValue;
        private System.Windows.Forms.TextBox FilterKey;
        private System.Windows.Forms.Label FilterByKeyValueLabel;
        private System.Windows.Forms.TextBox FilterClass;
        private System.Windows.Forms.Label FilterByClassLabel;
        private System.Windows.Forms.CheckBox FilterClassExact;
        private System.Windows.Forms.CheckBox FilterKeyValueExact;
        private System.Windows.Forms.CheckBox IncludeHidden;
        private System.Windows.Forms.Button ResetFiltersButton;
        private System.Windows.Forms.Button GoToButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button PropertiesButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.CheckBox FollowSelection;
    }
}