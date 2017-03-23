namespace Sledge.Shell.Forms
{
    partial class Shell
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
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.BottomSidebar = new Sledge.Shell.Controls.DockedPanel();
            this.LeftSidebar = new Sledge.Shell.Controls.DockedPanel();
            this.RightSidebar = new Sledge.Shell.Controls.DockedPanel();
            this.DocumentContainer = new System.Windows.Forms.Panel();
            this.DocumentTabs = new Sledge.Shell.Controls.ClosableTabControl();
            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            this.DocumentTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(571, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 381);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(571, 22);
            this.StatusStrip.TabIndex = 1;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.Controls.Add(this.DocumentContainer);
            this.ToolStripContainer.ContentPanel.Controls.Add(this.DocumentTabs);
            this.ToolStripContainer.ContentPanel.Controls.Add(this.RightSidebar);
            this.ToolStripContainer.ContentPanel.Controls.Add(this.LeftSidebar);
            this.ToolStripContainer.ContentPanel.Controls.Add(this.BottomSidebar);
            this.ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(546, 332);
            this.ToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStripContainer.Location = new System.Drawing.Point(0, 24);
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.Size = new System.Drawing.Size(571, 357);
            this.ToolStripContainer.TabIndex = 2;
            this.ToolStripContainer.Text = "ToolStripContainer";
            // 
            // BottomSidebar
            // 
            this.BottomSidebar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomSidebar.DockDimension = 8;
            this.BottomSidebar.Hidden = true;
            this.BottomSidebar.Location = new System.Drawing.Point(0, 324);
            this.BottomSidebar.Name = "BottomSidebar";
            this.BottomSidebar.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.BottomSidebar.Size = new System.Drawing.Size(546, 8);
            this.BottomSidebar.TabIndex = 0;
            // 
            // LeftSidebar
            // 
            this.LeftSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftSidebar.DockDimension = 8;
            this.LeftSidebar.Hidden = true;
            this.LeftSidebar.Location = new System.Drawing.Point(0, 0);
            this.LeftSidebar.Name = "LeftSidebar";
            this.LeftSidebar.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.LeftSidebar.Size = new System.Drawing.Size(8, 324);
            this.LeftSidebar.TabIndex = 1;
            // 
            // RightSidebar
            // 
            this.RightSidebar.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightSidebar.DockDimension = 8;
            this.RightSidebar.Hidden = true;
            this.RightSidebar.Location = new System.Drawing.Point(538, 0);
            this.RightSidebar.Name = "RightSidebar";
            this.RightSidebar.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.RightSidebar.Size = new System.Drawing.Size(8, 324);
            this.RightSidebar.TabIndex = 2;
            // 
            // DocumentContainer
            // 
            this.DocumentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DocumentContainer.Location = new System.Drawing.Point(8, 24);
            this.DocumentContainer.Name = "DocumentContainer";
            this.DocumentContainer.Size = new System.Drawing.Size(530, 300);
            this.DocumentContainer.TabIndex = 3;
            // 
            // DocumentTabs
            // 
            this.DocumentTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.DocumentTabs.Location = new System.Drawing.Point(8, 0);
            this.DocumentTabs.Name = "DocumentTabs";
            this.DocumentTabs.SelectedIndex = 0;
            this.DocumentTabs.Size = new System.Drawing.Size(530, 24);
            this.DocumentTabs.TabIndex = 4;
            this.DocumentTabs.SelectedIndexChanged += TabChanged;
            this.DocumentTabs.RequestClose += RequestClose;
            // 
            // Shell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 403);
            this.Controls.Add(this.ToolStripContainer);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "Shell";
            this.Text = "Sledge Shell";
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.DocumentTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripContainer ToolStripContainer;
        private Controls.DockedPanel BottomSidebar;
        private Controls.DockedPanel RightSidebar;
        private Controls.DockedPanel LeftSidebar;
        private System.Windows.Forms.Panel DocumentContainer;
        private Controls.ClosableTabControl DocumentTabs;
    }
}