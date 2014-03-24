namespace Sledge.Editor.UI.Sidebar
{
    partial class BrushSidebarPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.BrushTypeList = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RoundCreatedVerticesCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Brush Type:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BrushTypeList
            // 
            this.BrushTypeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BrushTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BrushTypeList.FormattingEnabled = true;
            this.BrushTypeList.Location = new System.Drawing.Point(73, 5);
            this.BrushTypeList.Name = "BrushTypeList";
            this.BrushTypeList.Size = new System.Drawing.Size(146, 21);
            this.BrushTypeList.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RoundCreatedVerticesCheckbox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.BrushTypeList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 56);
            this.panel1.TabIndex = 6;
            // 
            // RoundCreatedVerticesCheckbox
            // 
            this.RoundCreatedVerticesCheckbox.AutoSize = true;
            this.RoundCreatedVerticesCheckbox.Location = new System.Drawing.Point(73, 33);
            this.RoundCreatedVerticesCheckbox.Name = "RoundCreatedVerticesCheckbox";
            this.RoundCreatedVerticesCheckbox.Size = new System.Drawing.Size(137, 17);
            this.RoundCreatedVerticesCheckbox.TabIndex = 6;
            this.RoundCreatedVerticesCheckbox.Text = "Round created vertices";
            this.RoundCreatedVerticesCheckbox.UseVisualStyleBackColor = true;
            this.RoundCreatedVerticesCheckbox.CheckedChanged += new System.EventHandler(this.RoundCreatedVerticesChanged);
            // 
            // BrushSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panel1);
            this.Name = "BrushSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(232, 137);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox BrushTypeList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox RoundCreatedVerticesCheckbox;

    }
}
