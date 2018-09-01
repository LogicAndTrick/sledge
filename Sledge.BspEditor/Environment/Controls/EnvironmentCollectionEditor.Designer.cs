namespace Sledge.BspEditor.Environment.Controls
{
    partial class EnvironmentCollectionEditor
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
            this.components = new System.ComponentModel.Container();
            this.treEnvironments = new System.Windows.Forms.TreeView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.pnlSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new Sledge.Shell.Controls.DropdownButton();
            this.ctxEnvironmentMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.noEnvironmentsFoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxEnvironmentMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treEnvironments
            // 
            this.treEnvironments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treEnvironments.HideSelection = false;
            this.treEnvironments.Location = new System.Drawing.Point(3, 3);
            this.treEnvironments.Name = "treEnvironments";
            this.treEnvironments.Size = new System.Drawing.Size(157, 291);
            this.treEnvironments.TabIndex = 0;
            this.treEnvironments.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.EnvironmentSelected);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(85, 300);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.RemoveEnvironment);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.AutoScroll = true;
            this.pnlSettings.Location = new System.Drawing.Point(166, 3);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(506, 320);
            this.pnlSettings.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(4, 300);
            this.btnAdd.Menu = this.ctxEnvironmentMenu;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // ctxEnvironmentMenu
            // 
            this.ctxEnvironmentMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noEnvironmentsFoundToolStripMenuItem});
            this.ctxEnvironmentMenu.Name = "ctxEnvironmentMenu";
            this.ctxEnvironmentMenu.Size = new System.Drawing.Size(205, 26);
            // 
            // noEnvironmentsFoundToolStripMenuItem
            // 
            this.noEnvironmentsFoundToolStripMenuItem.Enabled = false;
            this.noEnvironmentsFoundToolStripMenuItem.Name = "noEnvironmentsFoundToolStripMenuItem";
            this.noEnvironmentsFoundToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.noEnvironmentsFoundToolStripMenuItem.Text = "No environments found!";
            // 
            // EnvironmentCollectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.treEnvironments);
            this.Name = "EnvironmentCollectionEditor";
            this.Size = new System.Drawing.Size(675, 326);
            this.ctxEnvironmentMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treEnvironments;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.FlowLayoutPanel pnlSettings;
        private Shell.Controls.DropdownButton btnAdd;
        private System.Windows.Forms.ContextMenuStrip ctxEnvironmentMenu;
        private System.Windows.Forms.ToolStripMenuItem noEnvironmentsFoundToolStripMenuItem;
    }
}
