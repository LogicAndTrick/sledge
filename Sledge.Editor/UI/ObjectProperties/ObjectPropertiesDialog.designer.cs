/*
 * Created by SharpDevelop.
 * User: Dan
 * Date: 19/12/2008
 * Time: 6:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sledge.Editor.UI.ObjectProperties
{
	partial class ObjectPropertiesDialog
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
            this.Tabs = new System.Windows.Forms.TabControl();
            this.ClassInfoTab = new System.Windows.Forms.TabPage();
            this.ChangingClassWarning = new System.Windows.Forms.Label();
            this.CancelClassChangeButton = new System.Windows.Forms.Button();
            this.DeletePropertyButton = new System.Windows.Forms.Button();
            this.AddPropertyButton = new System.Windows.Forms.Button();
            this.ConfirmClassChangeButton = new System.Windows.Forms.Button();
            this.SmartEditControlPanel = new System.Windows.Forms.Panel();
            this.SmartEditButton = new System.Windows.Forms.CheckBox();
            this.PasteKeyValues = new System.Windows.Forms.Button();
            this.CopyKeyValues = new System.Windows.Forms.Button();
            this.CommentsTextbox = new System.Windows.Forms.TextBox();
            this.HelpTextbox = new System.Windows.Forms.TextBox();
            this.HelpButton = new System.Windows.Forms.Button();
            this.KeyValuesList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Class = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Angles = new Sledge.Editor.UI.AngleControl();
            this.OutputsTab = new System.Windows.Forms.TabPage();
            this.OutputDelete = new System.Windows.Forms.Button();
            this.OutputPaste = new System.Windows.Forms.Button();
            this.OutputCopy = new System.Windows.Forms.Button();
            this.OutputAdd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.OutputOnce = new System.Windows.Forms.CheckBox();
            this.OutputDelay = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.OutputParameter = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.OutputInputName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.OutputTargetName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.OutputName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.OutputsList = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InputsTab = new System.Windows.Forms.TabPage();
            this.InputsList = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FlagsTab = new System.Windows.Forms.TabPage();
            this.FlagsTable = new System.Windows.Forms.CheckedListBox();
            this.VisgroupTab = new System.Windows.Forms.TabPage();
            this.EditVisgroupsButton = new System.Windows.Forms.Button();
            this.VisgroupPanel = new Sledge.Editor.Visgroups.VisgroupPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Tabs.SuspendLayout();
            this.ClassInfoTab.SuspendLayout();
            this.OutputsTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputDelay)).BeginInit();
            this.InputsTab.SuspendLayout();
            this.FlagsTab.SuspendLayout();
            this.VisgroupTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.ClassInfoTab);
            this.Tabs.Controls.Add(this.OutputsTab);
            this.Tabs.Controls.Add(this.InputsTab);
            this.Tabs.Controls.Add(this.FlagsTab);
            this.Tabs.Controls.Add(this.VisgroupTab);
            this.Tabs.Location = new System.Drawing.Point(12, 12);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(682, 406);
            this.Tabs.TabIndex = 1;
            // 
            // ClassInfoTab
            // 
            this.ClassInfoTab.Controls.Add(this.ChangingClassWarning);
            this.ClassInfoTab.Controls.Add(this.CancelClassChangeButton);
            this.ClassInfoTab.Controls.Add(this.DeletePropertyButton);
            this.ClassInfoTab.Controls.Add(this.AddPropertyButton);
            this.ClassInfoTab.Controls.Add(this.ConfirmClassChangeButton);
            this.ClassInfoTab.Controls.Add(this.SmartEditControlPanel);
            this.ClassInfoTab.Controls.Add(this.SmartEditButton);
            this.ClassInfoTab.Controls.Add(this.PasteKeyValues);
            this.ClassInfoTab.Controls.Add(this.CopyKeyValues);
            this.ClassInfoTab.Controls.Add(this.CommentsTextbox);
            this.ClassInfoTab.Controls.Add(this.HelpTextbox);
            this.ClassInfoTab.Controls.Add(this.HelpButton);
            this.ClassInfoTab.Controls.Add(this.KeyValuesList);
            this.ClassInfoTab.Controls.Add(this.Class);
            this.ClassInfoTab.Controls.Add(this.label4);
            this.ClassInfoTab.Controls.Add(this.label3);
            this.ClassInfoTab.Controls.Add(this.label5);
            this.ClassInfoTab.Controls.Add(this.label1);
            this.ClassInfoTab.Controls.Add(this.Angles);
            this.ClassInfoTab.Location = new System.Drawing.Point(4, 22);
            this.ClassInfoTab.Name = "ClassInfoTab";
            this.ClassInfoTab.Padding = new System.Windows.Forms.Padding(3);
            this.ClassInfoTab.Size = new System.Drawing.Size(674, 380);
            this.ClassInfoTab.TabIndex = 0;
            this.ClassInfoTab.Text = "Class Info";
            this.ClassInfoTab.UseVisualStyleBackColor = true;
            // 
            // ChangingClassWarning
            // 
            this.ChangingClassWarning.AutoSize = true;
            this.ChangingClassWarning.ForeColor = System.Drawing.Color.Brown;
            this.ChangingClassWarning.Location = new System.Drawing.Point(220, 38);
            this.ChangingClassWarning.Name = "ChangingClassWarning";
            this.ChangingClassWarning.Size = new System.Drawing.Size(277, 26);
            this.ChangingClassWarning.TabIndex = 11;
            this.ChangingClassWarning.Text = "Changing class - click \"Change\" or \"Cancel\" to continue.\r\nThe object will not be " +
    "affected until you click \"Apply\".\r\n";
            this.ChangingClassWarning.Visible = false;
            // 
            // CancelClassChangeButton
            // 
            this.CancelClassChangeButton.Enabled = false;
            this.CancelClassChangeButton.Location = new System.Drawing.Point(310, 7);
            this.CancelClassChangeButton.Name = "CancelClassChangeButton";
            this.CancelClassChangeButton.Size = new System.Drawing.Size(53, 21);
            this.CancelClassChangeButton.TabIndex = 10;
            this.CancelClassChangeButton.Text = "Cancel";
            this.CancelClassChangeButton.UseVisualStyleBackColor = true;
            this.CancelClassChangeButton.Click += new System.EventHandler(this.CancelClassChange);
            // 
            // DeletePropertyButton
            // 
            this.DeletePropertyButton.Location = new System.Drawing.Point(74, 353);
            this.DeletePropertyButton.Name = "DeletePropertyButton";
            this.DeletePropertyButton.Size = new System.Drawing.Size(62, 21);
            this.DeletePropertyButton.TabIndex = 10;
            this.DeletePropertyButton.Text = "Delete";
            this.DeletePropertyButton.UseVisualStyleBackColor = true;
            this.DeletePropertyButton.Click += new System.EventHandler(this.RemovePropertyClicked);
            // 
            // AddPropertyButton
            // 
            this.AddPropertyButton.Location = new System.Drawing.Point(6, 353);
            this.AddPropertyButton.Name = "AddPropertyButton";
            this.AddPropertyButton.Size = new System.Drawing.Size(62, 21);
            this.AddPropertyButton.TabIndex = 10;
            this.AddPropertyButton.Text = "Add";
            this.AddPropertyButton.UseVisualStyleBackColor = true;
            this.AddPropertyButton.Click += new System.EventHandler(this.AddPropertyClicked);
            // 
            // ConfirmClassChangeButton
            // 
            this.ConfirmClassChangeButton.Enabled = false;
            this.ConfirmClassChangeButton.Location = new System.Drawing.Point(251, 7);
            this.ConfirmClassChangeButton.Name = "ConfirmClassChangeButton";
            this.ConfirmClassChangeButton.Size = new System.Drawing.Size(53, 21);
            this.ConfirmClassChangeButton.TabIndex = 10;
            this.ConfirmClassChangeButton.Text = "Change";
            this.ConfirmClassChangeButton.UseVisualStyleBackColor = true;
            this.ConfirmClassChangeButton.Click += new System.EventHandler(this.ConfirmClassChange);
            // 
            // SmartEditControlPanel
            // 
            this.SmartEditControlPanel.Location = new System.Drawing.Point(402, 73);
            this.SmartEditControlPanel.Name = "SmartEditControlPanel";
            this.SmartEditControlPanel.Size = new System.Drawing.Size(266, 57);
            this.SmartEditControlPanel.TabIndex = 9;
            // 
            // SmartEditButton
            // 
            this.SmartEditButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.SmartEditButton.AutoSize = true;
            this.SmartEditButton.Checked = true;
            this.SmartEditButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SmartEditButton.Location = new System.Drawing.Point(405, 7);
            this.SmartEditButton.Name = "SmartEditButton";
            this.SmartEditButton.Size = new System.Drawing.Size(65, 23);
            this.SmartEditButton.TabIndex = 8;
            this.SmartEditButton.Text = "Smart Edit";
            this.SmartEditButton.UseVisualStyleBackColor = true;
            this.SmartEditButton.CheckedChanged += new System.EventHandler(this.SmartEditToggled);
            // 
            // PasteKeyValues
            // 
            this.PasteKeyValues.Location = new System.Drawing.Point(124, 47);
            this.PasteKeyValues.Name = "PasteKeyValues";
            this.PasteKeyValues.Size = new System.Drawing.Size(44, 20);
            this.PasteKeyValues.TabIndex = 7;
            this.PasteKeyValues.Text = "Paste";
            this.PasteKeyValues.UseVisualStyleBackColor = true;
            // 
            // CopyKeyValues
            // 
            this.CopyKeyValues.Location = new System.Drawing.Point(74, 47);
            this.CopyKeyValues.Name = "CopyKeyValues";
            this.CopyKeyValues.Size = new System.Drawing.Size(44, 20);
            this.CopyKeyValues.TabIndex = 7;
            this.CopyKeyValues.Text = "Copy";
            this.CopyKeyValues.UseVisualStyleBackColor = true;
            // 
            // CommentsTextbox
            // 
            this.CommentsTextbox.Location = new System.Drawing.Point(402, 275);
            this.CommentsTextbox.Multiline = true;
            this.CommentsTextbox.Name = "CommentsTextbox";
            this.CommentsTextbox.Size = new System.Drawing.Size(266, 91);
            this.CommentsTextbox.TabIndex = 6;
            // 
            // HelpTextbox
            // 
            this.HelpTextbox.Location = new System.Drawing.Point(402, 157);
            this.HelpTextbox.Multiline = true;
            this.HelpTextbox.Name = "HelpTextbox";
            this.HelpTextbox.ReadOnly = true;
            this.HelpTextbox.Size = new System.Drawing.Size(266, 91);
            this.HelpTextbox.TabIndex = 6;
            // 
            // HelpButton
            // 
            this.HelpButton.Location = new System.Drawing.Point(473, 7);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(74, 23);
            this.HelpButton.TabIndex = 4;
            this.HelpButton.Text = "Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            // 
            // KeyValuesList
            // 
            this.KeyValuesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.KeyValuesList.FullRowSelect = true;
            this.KeyValuesList.GridLines = true;
            this.KeyValuesList.Location = new System.Drawing.Point(6, 73);
            this.KeyValuesList.MultiSelect = false;
            this.KeyValuesList.Name = "KeyValuesList";
            this.KeyValuesList.Size = new System.Drawing.Size(390, 274);
            this.KeyValuesList.TabIndex = 3;
            this.KeyValuesList.UseCompatibleStateImageBehavior = false;
            this.KeyValuesList.View = System.Windows.Forms.View.Details;
            this.KeyValuesList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.KeyValuesListItemSelectionChanged);
            this.KeyValuesList.SelectedIndexChanged += new System.EventHandler(this.KeyValuesListSelectedIndexChanged);
            this.KeyValuesList.Leave += new System.EventHandler(this.KeyValuesListSelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Property Name";
            this.columnHeader1.Width = 162;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 201;
            // 
            // Class
            // 
            this.Class.FormattingEnabled = true;
            this.Class.Location = new System.Drawing.Point(57, 7);
            this.Class.Name = "Class";
            this.Class.Size = new System.Drawing.Size(187, 21);
            this.Class.TabIndex = 2;
            this.Class.TextChanged += new System.EventHandler(this.StartClassChange);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(402, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "Comments:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(402, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "Help:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "Keyvalues:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Class:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Angles
            // 
            this.Angles.Location = new System.Drawing.Point(553, 6);
            this.Angles.Name = "Angles";
            this.Angles.ShowLabel = false;
            this.Angles.ShowTextBox = true;
            this.Angles.Size = new System.Drawing.Size(115, 46);
            this.Angles.TabIndex = 0;
            this.Angles.AngleChangedEvent += new Sledge.Editor.UI.AngleControl.AngleChangedEventHandler(this.AnglesChanged);
            // 
            // OutputsTab
            // 
            this.OutputsTab.Controls.Add(this.OutputDelete);
            this.OutputsTab.Controls.Add(this.OutputPaste);
            this.OutputsTab.Controls.Add(this.OutputCopy);
            this.OutputsTab.Controls.Add(this.OutputAdd);
            this.OutputsTab.Controls.Add(this.groupBox1);
            this.OutputsTab.Controls.Add(this.OutputsList);
            this.OutputsTab.Location = new System.Drawing.Point(4, 22);
            this.OutputsTab.Name = "OutputsTab";
            this.OutputsTab.Padding = new System.Windows.Forms.Padding(3);
            this.OutputsTab.Size = new System.Drawing.Size(674, 380);
            this.OutputsTab.TabIndex = 1;
            this.OutputsTab.Text = "Outputs";
            this.OutputsTab.UseVisualStyleBackColor = true;
            // 
            // OutputDelete
            // 
            this.OutputDelete.Location = new System.Drawing.Point(593, 205);
            this.OutputDelete.Name = "OutputDelete";
            this.OutputDelete.Size = new System.Drawing.Size(75, 23);
            this.OutputDelete.TabIndex = 6;
            this.OutputDelete.Text = "Delete";
            this.OutputDelete.UseVisualStyleBackColor = true;
            // 
            // OutputPaste
            // 
            this.OutputPaste.Location = new System.Drawing.Point(512, 205);
            this.OutputPaste.Name = "OutputPaste";
            this.OutputPaste.Size = new System.Drawing.Size(75, 23);
            this.OutputPaste.TabIndex = 6;
            this.OutputPaste.Text = "Paste";
            this.OutputPaste.UseVisualStyleBackColor = true;
            // 
            // OutputCopy
            // 
            this.OutputCopy.Location = new System.Drawing.Point(431, 205);
            this.OutputCopy.Name = "OutputCopy";
            this.OutputCopy.Size = new System.Drawing.Size(75, 23);
            this.OutputCopy.TabIndex = 6;
            this.OutputCopy.Text = "Copy";
            this.OutputCopy.UseVisualStyleBackColor = true;
            // 
            // OutputAdd
            // 
            this.OutputAdd.Location = new System.Drawing.Point(350, 205);
            this.OutputAdd.Name = "OutputAdd";
            this.OutputAdd.Size = new System.Drawing.Size(75, 23);
            this.OutputAdd.TabIndex = 6;
            this.OutputAdd.Text = "Add";
            this.OutputAdd.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OutputOnce);
            this.groupBox1.Controls.Add(this.OutputDelay);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.OutputParameter);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.OutputInputName);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.OutputTargetName);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.OutputName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(6, 205);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 169);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Entry";
            // 
            // OutputOnce
            // 
            this.OutputOnce.Location = new System.Drawing.Point(171, 122);
            this.OutputOnce.Name = "OutputOnce";
            this.OutputOnce.Size = new System.Drawing.Size(104, 20);
            this.OutputOnce.TabIndex = 3;
            this.OutputOnce.Text = "Once Only";
            this.OutputOnce.UseVisualStyleBackColor = true;
            // 
            // OutputDelay
            // 
            this.OutputDelay.DecimalPlaces = 2;
            this.OutputDelay.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.OutputDelay.Location = new System.Drawing.Point(115, 122);
            this.OutputDelay.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.OutputDelay.Name = "OutputDelay";
            this.OutputDelay.Size = new System.Drawing.Size(50, 20);
            this.OutputDelay.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(6, 120);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "Delay (seconds):";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutputParameter
            // 
            this.OutputParameter.Location = new System.Drawing.Point(115, 94);
            this.OutputParameter.Name = "OutputParameter";
            this.OutputParameter.Size = new System.Drawing.Size(197, 20);
            this.OutputParameter.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Parameter Override:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutputInputName
            // 
            this.OutputInputName.Location = new System.Drawing.Point(115, 68);
            this.OutputInputName.Name = "OutputInputName";
            this.OutputInputName.Size = new System.Drawing.Size(197, 20);
            this.OutputInputName.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Input:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutputTargetName
            // 
            this.OutputTargetName.Location = new System.Drawing.Point(115, 42);
            this.OutputTargetName.Name = "OutputTargetName";
            this.OutputTargetName.Size = new System.Drawing.Size(197, 20);
            this.OutputTargetName.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Target Name:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutputName
            // 
            this.OutputName.Location = new System.Drawing.Point(115, 16);
            this.OutputName.Name = "OutputName";
            this.OutputName.Size = new System.Drawing.Size(197, 20);
            this.OutputName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Output Name:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutputsList
            // 
            this.OutputsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader7,
            this.columnHeader10});
            this.OutputsList.GridLines = true;
            this.OutputsList.Location = new System.Drawing.Point(6, 6);
            this.OutputsList.Name = "OutputsList";
            this.OutputsList.Size = new System.Drawing.Size(662, 193);
            this.OutputsList.TabIndex = 4;
            this.OutputsList.UseCompatibleStateImageBehavior = false;
            this.OutputsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Output Name";
            this.columnHeader5.Width = 92;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Target";
            this.columnHeader6.Width = 91;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Input";
            this.columnHeader8.Width = 101;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Parameter";
            this.columnHeader9.Width = 103;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Delay";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Only Once";
            this.columnHeader10.Width = 79;
            // 
            // InputsTab
            // 
            this.InputsTab.Controls.Add(this.InputsList);
            this.InputsTab.Location = new System.Drawing.Point(4, 22);
            this.InputsTab.Name = "InputsTab";
            this.InputsTab.Padding = new System.Windows.Forms.Padding(3);
            this.InputsTab.Size = new System.Drawing.Size(674, 380);
            this.InputsTab.TabIndex = 2;
            this.InputsTab.Text = "Inputs";
            this.InputsTab.UseVisualStyleBackColor = true;
            // 
            // InputsList
            // 
            this.InputsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader3,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14});
            this.InputsList.GridLines = true;
            this.InputsList.Location = new System.Drawing.Point(6, 6);
            this.InputsList.Name = "InputsList";
            this.InputsList.Size = new System.Drawing.Size(662, 368);
            this.InputsList.TabIndex = 5;
            this.InputsList.UseCompatibleStateImageBehavior = false;
            this.InputsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Source";
            this.columnHeader4.Width = 91;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Output Name";
            this.columnHeader3.Width = 92;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Input";
            this.columnHeader11.Width = 101;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Parameter";
            this.columnHeader12.Width = 103;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Delay";
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Only Once";
            this.columnHeader14.Width = 79;
            // 
            // FlagsTab
            // 
            this.FlagsTab.Controls.Add(this.FlagsTable);
            this.FlagsTab.Location = new System.Drawing.Point(4, 22);
            this.FlagsTab.Name = "FlagsTab";
            this.FlagsTab.Padding = new System.Windows.Forms.Padding(3);
            this.FlagsTab.Size = new System.Drawing.Size(674, 380);
            this.FlagsTab.TabIndex = 3;
            this.FlagsTab.Text = "Flags";
            this.FlagsTab.UseVisualStyleBackColor = true;
            // 
            // FlagsTable
            // 
            this.FlagsTable.CheckOnClick = true;
            this.FlagsTable.FormattingEnabled = true;
            this.FlagsTable.Location = new System.Drawing.Point(6, 6);
            this.FlagsTable.Name = "FlagsTable";
            this.FlagsTable.Size = new System.Drawing.Size(662, 364);
            this.FlagsTable.TabIndex = 0;
            // 
            // VisgroupTab
            // 
            this.VisgroupTab.Controls.Add(this.EditVisgroupsButton);
            this.VisgroupTab.Controls.Add(this.VisgroupPanel);
            this.VisgroupTab.Controls.Add(this.label11);
            this.VisgroupTab.Location = new System.Drawing.Point(4, 22);
            this.VisgroupTab.Name = "VisgroupTab";
            this.VisgroupTab.Padding = new System.Windows.Forms.Padding(3);
            this.VisgroupTab.Size = new System.Drawing.Size(674, 380);
            this.VisgroupTab.TabIndex = 4;
            this.VisgroupTab.Text = "Visgroup";
            this.VisgroupTab.UseVisualStyleBackColor = true;
            // 
            // EditVisgroupsButton
            // 
            this.EditVisgroupsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditVisgroupsButton.Location = new System.Drawing.Point(570, 351);
            this.EditVisgroupsButton.Name = "EditVisgroupsButton";
            this.EditVisgroupsButton.Size = new System.Drawing.Size(98, 23);
            this.EditVisgroupsButton.TabIndex = 3;
            this.EditVisgroupsButton.Text = "Edit Visgroups";
            this.EditVisgroupsButton.UseVisualStyleBackColor = true;
            this.EditVisgroupsButton.Click += new System.EventHandler(this.EditVisgroupsClicked);
            // 
            // VisgroupPanel
            // 
            this.VisgroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisgroupPanel.DisableAutomatic = true;
            this.VisgroupPanel.HideAutomatic = false;
            this.VisgroupPanel.Location = new System.Drawing.Point(6, 26);
            this.VisgroupPanel.Name = "VisgroupPanel";
            this.VisgroupPanel.ShowCheckboxes = true;
            this.VisgroupPanel.ShowHidden = false;
            this.VisgroupPanel.Size = new System.Drawing.Size(662, 319);
            this.VisgroupPanel.SortAutomaticFirst = false;
            this.VisgroupPanel.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 20);
            this.label11.TabIndex = 1;
            this.label11.Text = "Member of group:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(619, 424);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelButtonClicked);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(538, 424);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButtonClicked);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(457, 424);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ApplyButtonClicked);
            // 
            // EntityEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 459);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.Tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ObjectPropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Object Properties";
            this.Tabs.ResumeLayout(false);
            this.ClassInfoTab.ResumeLayout(false);
            this.ClassInfoTab.PerformLayout();
            this.OutputsTab.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputDelay)).EndInit();
            this.InputsTab.ResumeLayout(false);
            this.FlagsTab.ResumeLayout(false);
            this.VisgroupTab.ResumeLayout(false);
            this.ResumeLayout(false);

		}
        private Sledge.Editor.UI.AngleControl Angles;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TabPage VisgroupTab;
		private System.Windows.Forms.TabPage FlagsTab;
		private System.Windows.Forms.ColumnHeader columnHeader14;
		private System.Windows.Forms.ColumnHeader columnHeader13;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ListView InputsList;
		private System.Windows.Forms.TabPage InputsTab;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ListView OutputsList;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox OutputName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox OutputTargetName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox OutputInputName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox OutputParameter;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown OutputDelay;
		private System.Windows.Forms.CheckBox OutputOnce;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button OutputAdd;
		private System.Windows.Forms.Button OutputCopy;
		private System.Windows.Forms.Button OutputPaste;
		private System.Windows.Forms.Button OutputDelete;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.ListView KeyValuesList;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox Class;
		private System.Windows.Forms.TextBox HelpTextbox;
		private System.Windows.Forms.TextBox CommentsTextbox;
		private System.Windows.Forms.Button CopyKeyValues;
		private System.Windows.Forms.Button PasteKeyValues;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TabPage OutputsTab;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.TabPage ClassInfoTab;
		private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.CheckBox SmartEditButton;
        private System.Windows.Forms.Panel SmartEditControlPanel;
        private System.Windows.Forms.CheckedListBox FlagsTable;
        private System.Windows.Forms.Button CancelClassChangeButton;
        private System.Windows.Forms.Button ConfirmClassChangeButton;
        private System.Windows.Forms.Label ChangingClassWarning;
        private System.Windows.Forms.Button DeletePropertyButton;
        private System.Windows.Forms.Button AddPropertyButton;
        private System.Windows.Forms.Button button1;
        private Visgroups.VisgroupPanel VisgroupPanel;
        private System.Windows.Forms.Button EditVisgroupsButton;
	}
}
