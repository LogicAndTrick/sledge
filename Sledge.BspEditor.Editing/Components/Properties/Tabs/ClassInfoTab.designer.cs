using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    public partial class ClassInfoTab
    {
        private void InitializeComponent()
        {
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlSmartEdit = new System.Windows.Forms.Panel();
            this.btnSmartEdit = new System.Windows.Forms.CheckBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.lstKeyValues = new System.Windows.Forms.ListView();
            this.colPropertyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPropertyValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblKeyValues = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.angAngles = new Sledge.BspEditor.Editing.Controls.AngleControl();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(71, 352);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(62, 23);
            this.btnDelete.TabIndex = 24;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.DeleteKeyClicked);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(3, 352);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(62, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.AddKeyClicked);
            // 
            // pnlSmartEdit
            // 
            this.pnlSmartEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSmartEdit.Location = new System.Drawing.Point(399, 73);
            this.pnlSmartEdit.Name = "pnlSmartEdit";
            this.pnlSmartEdit.Size = new System.Drawing.Size(277, 69);
            this.pnlSmartEdit.TabIndex = 23;
            // 
            // btnSmartEdit
            // 
            this.btnSmartEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSmartEdit.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSmartEdit.AutoSize = true;
            this.btnSmartEdit.Checked = true;
            this.btnSmartEdit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSmartEdit.Location = new System.Drawing.Point(611, 3);
            this.btnSmartEdit.Name = "btnSmartEdit";
            this.btnSmartEdit.Size = new System.Drawing.Size(65, 23);
            this.btnSmartEdit.TabIndex = 22;
            this.btnSmartEdit.Text = "Smart Edit";
            this.btnSmartEdit.UseVisualStyleBackColor = true;
            this.btnSmartEdit.CheckedChanged += new System.EventHandler(this.SmartEditToggled);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(121, 30);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(44, 23);
            this.btnPaste.TabIndex = 20;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(71, 30);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(44, 23);
            this.btnCopy.TabIndex = 21;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            // 
            // txtComments
            // 
            this.txtComments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComments.Location = new System.Drawing.Point(399, 268);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(277, 78);
            this.txtComments.TabIndex = 18;
            this.txtComments.Visible = false;
            // 
            // txtHelp
            // 
            this.txtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHelp.Location = new System.Drawing.Point(399, 169);
            this.txtHelp.Multiline = true;
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.Size = new System.Drawing.Size(277, 72);
            this.txtHelp.TabIndex = 19;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(611, 30);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(65, 23);
            this.btnHelp.TabIndex = 17;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // lstKeyValues
            // 
            this.lstKeyValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstKeyValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPropertyName,
            this.colPropertyValue});
            this.lstKeyValues.FullRowSelect = true;
            this.lstKeyValues.GridLines = true;
            this.lstKeyValues.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstKeyValues.HideSelection = false;
            this.lstKeyValues.Location = new System.Drawing.Point(3, 59);
            this.lstKeyValues.MultiSelect = false;
            this.lstKeyValues.Name = "lstKeyValues";
            this.lstKeyValues.Size = new System.Drawing.Size(390, 287);
            this.lstKeyValues.TabIndex = 16;
            this.lstKeyValues.UseCompatibleStateImageBehavior = false;
            this.lstKeyValues.View = System.Windows.Forms.View.Details;
            this.lstKeyValues.SelectedIndexChanged += new System.EventHandler(this.SelectedPropertyChanged);
            // 
            // colPropertyName
            // 
            this.colPropertyName.Text = "Property Name";
            this.colPropertyName.Width = 162;
            // 
            // colPropertyValue
            // 
            this.colPropertyValue.Text = "Value";
            this.colPropertyValue.Width = 201;
            // 
            // cmbClass
            // 
            this.cmbClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(71, 3);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(322, 21);
            this.cmbClass.TabIndex = 15;
            this.cmbClass.TextChanged += new System.EventHandler(this.ClassChanged);
            // 
            // lblComments
            // 
            this.lblComments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblComments.Location = new System.Drawing.Point(399, 244);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(59, 21);
            this.lblComments.TabIndex = 11;
            this.lblComments.Text = "Comments:";
            this.lblComments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblComments.Visible = false;
            // 
            // lblHelp
            // 
            this.lblHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHelp.Location = new System.Drawing.Point(399, 145);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(45, 21);
            this.lblHelp.TabIndex = 12;
            this.lblHelp.Text = "Help:";
            this.lblHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKeyValues
            // 
            this.lblKeyValues.Location = new System.Drawing.Point(3, 30);
            this.lblKeyValues.Name = "lblKeyValues";
            this.lblKeyValues.Size = new System.Drawing.Size(62, 23);
            this.lblKeyValues.TabIndex = 13;
            this.lblKeyValues.Text = "Keyvalues:";
            this.lblKeyValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblClass
            // 
            this.lblClass.Location = new System.Drawing.Point(3, 3);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(45, 21);
            this.lblClass.TabIndex = 14;
            this.lblClass.Text = "Class:";
            this.lblClass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // angAngles
            // 
            this.angAngles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.angAngles.LabelText = "Angles:";
            this.angAngles.Location = new System.Drawing.Point(490, 3);
            this.angAngles.Name = "angAngles";
            this.angAngles.ShowLabel = true;
            this.angAngles.ShowTextBox = true;
            this.angAngles.Size = new System.Drawing.Size(115, 64);
            this.angAngles.TabIndex = 26;
            this.angAngles.AngleChangedEvent += new System.EventHandler(this.SetAngleValue);
            // 
            // ClassInfoTab
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.angAngles);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.pnlSmartEdit);
            this.Controls.Add(this.btnSmartEdit);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lstKeyValues);
            this.Controls.Add(this.cmbClass);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.lblKeyValues);
            this.Controls.Add(this.lblClass);
            this.Name = "ClassInfoTab";
            this.Size = new System.Drawing.Size(679, 378);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel pnlSmartEdit;
        private System.Windows.Forms.CheckBox btnSmartEdit;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.TextBox txtHelp;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ListView lstKeyValues;
        private System.Windows.Forms.ColumnHeader colPropertyName;
        private System.Windows.Forms.ColumnHeader colPropertyValue;
        private System.Windows.Forms.ComboBox cmbClass;
        private System.Windows.Forms.Label lblComments;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Label lblKeyValues;
        private System.Windows.Forms.Label lblClass;
        private Controls.AngleControl angAngles;
    }
}
