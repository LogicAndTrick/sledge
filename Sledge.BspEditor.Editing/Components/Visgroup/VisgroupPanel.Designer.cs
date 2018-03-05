namespace Sledge.BspEditor.Editing.Components.Visgroup
{
    partial class VisgroupPanel
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
            this.VisgroupTree = new System.Windows.Forms.TreeView();
            this.CheckboxImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // VisgroupTree
            // 
            this.VisgroupTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VisgroupTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisgroupTree.Indent = 16;
            this.VisgroupTree.Location = new System.Drawing.Point(0, 0);
            this.VisgroupTree.Name = "VisgroupTree";
            this.VisgroupTree.Size = new System.Drawing.Size(244, 228);
            this.VisgroupTree.StateImageList = this.CheckboxImages;
            this.VisgroupTree.TabIndex = 0;
            this.VisgroupTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.OnItemDrag);
            this.VisgroupTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.NodeSelected);
            this.VisgroupTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.NodeMouseClick);
            this.VisgroupTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.NodeMouseClick);
            this.VisgroupTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this.VisgroupTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            // 
            // CheckboxImages
            // 
            this.CheckboxImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.CheckboxImages.ImageSize = new System.Drawing.Size(16, 16);
            this.CheckboxImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // VisgroupPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VisgroupTree);
            this.Name = "VisgroupPanel";
            this.Size = new System.Drawing.Size(244, 228);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView VisgroupTree;
        private System.Windows.Forms.ImageList CheckboxImages;
    }
}
