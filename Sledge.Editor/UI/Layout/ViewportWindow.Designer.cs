namespace Sledge.Editor.UI.Layout
{
    partial class ViewportWindow
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
            Sledge.Editor.UI.TableSplitConfiguration tableSplitConfiguration1 = new Sledge.Editor.UI.TableSplitConfiguration();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewportWindow));
            this.SplitControl = new Sledge.Editor.UI.TableSplitControl();
            this.SuspendLayout();
            // 
            // SplitControl
            // 
            this.SplitControl.ColumnCount = 2;
            this.SplitControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SplitControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableSplitConfiguration1.Columns = 2;
            tableSplitConfiguration1.Rectangles = ((System.Collections.Generic.List<System.Drawing.Rectangle>)(resources.GetObject("tableSplitConfiguration1.Rectangles")));
            tableSplitConfiguration1.Rows = 2;
            this.SplitControl.Configuration = tableSplitConfiguration1;
            this.SplitControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitControl.Location = new System.Drawing.Point(0, 0);
            this.SplitControl.MinimumViewSize = 2;
            this.SplitControl.Name = "SplitControl";
            this.SplitControl.RowCount = 2;
            this.SplitControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SplitControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SplitControl.Size = new System.Drawing.Size(872, 537);
            this.SplitControl.TabIndex = 0;
            // 
            // ViewportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 537);
            this.Controls.Add(this.SplitControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewportWindow";
            this.ShowInTaskbar = false;
            this.Text = "Sledge Viewport Window";
            this.ResumeLayout(false);

        }

        #endregion

        private TableSplitControl SplitControl;
    }
}