namespace Sledge.BspEditor.Rendering.Scene
{
    partial class SceneBufferAllocationSidebarPanel
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
            this.BufferList = new System.Windows.Forms.ListBox();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BufferList
            // 
            this.BufferList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BufferList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.BufferList.FormattingEnabled = true;
            this.BufferList.IntegralHeight = false;
            this.BufferList.ItemHeight = 30;
            this.BufferList.Location = new System.Drawing.Point(3, 3);
            this.BufferList.Name = "BufferList";
            this.BufferList.Size = new System.Drawing.Size(220, 111);
            this.BufferList.TabIndex = 0;
            this.BufferList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.DrawItem);
            // 
            // chkAutoRefresh
            // 
            this.chkAutoRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Checked = true;
            this.chkAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRefresh.Location = new System.Drawing.Point(4, 120);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(83, 17);
            this.chkAutoRefresh.TabIndex = 1;
            this.chkAutoRefresh.Text = "Auto-refresh";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            this.chkAutoRefresh.CheckedChanged += new System.EventHandler(this.ChangeAutoRefresh);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshButton.Location = new System.Drawing.Point(150, 115);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(73, 22);
            this.RefreshButton.TabIndex = 2;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.UpdateList);
            // 
            // SceneBufferAllocationSidebarPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.chkAutoRefresh);
            this.Controls.Add(this.BufferList);
            this.Name = "SceneBufferAllocationSidebarPanel";
            this.Size = new System.Drawing.Size(226, 140);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox BufferList;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Button RefreshButton;
    }
}
