namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    partial class VertexSidebarPanel
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
            this.ButtonLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ControlPanel = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ResetButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonLayoutPanel
            // 
            this.ButtonLayoutPanel.AutoSize = true;
            this.ButtonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonLayoutPanel.Location = new System.Drawing.Point(2, 2);
            this.ButtonLayoutPanel.Name = "ButtonLayoutPanel";
            this.ButtonLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.ButtonLayoutPanel.Size = new System.Drawing.Size(197, 5);
            this.ButtonLayoutPanel.TabIndex = 7;
            // 
            // ControlPanel
            // 
            this.ControlPanel.AutoSize = true;
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ControlPanel.Location = new System.Drawing.Point(2, 7);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(197, 5);
            this.ControlPanel.TabIndex = 10;
            this.ControlPanel.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ResetButton);
            this.panel1.Controls.Add(this.DeselectAllButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(2, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 34);
            this.panel1.TabIndex = 11;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(92, 6);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(95, 23);
            this.ResetButton.TabIndex = 6;
            this.ResetButton.Text = "Reset to Original";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButtonClicked);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(3, 6);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(83, 23);
            this.DeselectAllButton.TabIndex = 5;
            this.DeselectAllButton.Text = "Deselect All";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButtonClicked);
            // 
            // VertexSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.ButtonLayoutPanel);
            this.Name = "VertexSidebarPanel";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(201, 68);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonLayoutPanel;
        private System.Windows.Forms.GroupBox ControlPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button DeselectAllButton;

    }
}
