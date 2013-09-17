using Sledge.Editor.UI;

namespace Sledge.Editor.Tools
{
    partial class VMForm
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
            this.HoverTip = new System.Windows.Forms.ToolTip(this.components);
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ErrorList = new System.Windows.Forms.ListBox();
            this.radioLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.groupRadios = new System.Windows.Forms.GroupBox();
            this.ControlPanel = new System.Windows.Forms.GroupBox();
            this.groupRadios.SuspendLayout();
            this.SuspendLayout();
            // 
            // HoverTip
            // 
            this.HoverTip.AutoPopDelay = 5000;
            this.HoverTip.InitialDelay = 200;
            this.HoverTip.IsBalloon = true;
            this.HoverTip.ReshowDelay = 100;
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(12, 126);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(134, 23);
            this.DeselectAllButton.TabIndex = 3;
            this.DeselectAllButton.Text = "Deselect All";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButtonClicked);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 155);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(134, 23);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset to Original";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButtonClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(445, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Errors";
            // 
            // ErrorList
            // 
            this.ErrorList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorList.FormattingEnabled = true;
            this.ErrorList.Location = new System.Drawing.Point(445, 27);
            this.ErrorList.Name = "ErrorList";
            this.ErrorList.Size = new System.Drawing.Size(134, 160);
            this.ErrorList.TabIndex = 6;
            this.ErrorList.SelectedIndexChanged += new System.EventHandler(this.ErrorListSelectionChanged);
            // 
            // radioLayoutPanel
            // 
            this.radioLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radioLayoutPanel.Location = new System.Drawing.Point(6, 19);
            this.radioLayoutPanel.Name = "radioLayoutPanel";
            this.radioLayoutPanel.Size = new System.Drawing.Size(122, 83);
            this.radioLayoutPanel.TabIndex = 0;
            // 
            // groupRadios
            // 
            this.groupRadios.Controls.Add(this.radioLayoutPanel);
            this.groupRadios.Location = new System.Drawing.Point(12, 12);
            this.groupRadios.Name = "groupRadios";
            this.groupRadios.Size = new System.Drawing.Size(134, 108);
            this.groupRadios.TabIndex = 8;
            this.groupRadios.TabStop = false;
            this.groupRadios.Text = "Mode";
            // 
            // ControlPanel
            // 
            this.ControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlPanel.Location = new System.Drawing.Point(152, 12);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(287, 178);
            this.ControlPanel.TabIndex = 9;
            this.ControlPanel.TabStop = false;
            // 
            // VMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 202);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.groupRadios);
            this.Controls.Add(this.ErrorList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.DeselectAllButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VMForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vertex Manipulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.groupRadios.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip HoverTip;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ErrorList;
        private System.Windows.Forms.FlowLayoutPanel radioLayoutPanel;
        private System.Windows.Forms.GroupBox groupRadios;
        private System.Windows.Forms.GroupBox ControlPanel;
    }
}