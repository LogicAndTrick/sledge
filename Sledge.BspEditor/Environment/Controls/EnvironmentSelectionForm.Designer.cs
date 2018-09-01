namespace Sledge.BspEditor.Environment.Controls
{
    partial class EnvironmentSelectionForm
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
            this.GameTable = new System.Windows.Forms.TableLayoutPanel();
            this.GamePanel = new System.Windows.Forms.Panel();
            this.GamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameTable
            // 
            this.GameTable.AutoSize = true;
            this.GameTable.ColumnCount = 2;
            this.GameTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.GameTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.GameTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.GameTable.Location = new System.Drawing.Point(0, 0);
            this.GameTable.Name = "GameTable";
            this.GameTable.Padding = new System.Windows.Forms.Padding(10);
            this.GameTable.RowCount = 1;
            this.GameTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.GameTable.Size = new System.Drawing.Size(272, 50);
            this.GameTable.TabIndex = 2;
            // 
            // GamePanel
            // 
            this.GamePanel.AutoScroll = true;
            this.GamePanel.Controls.Add(this.GameTable);
            this.GamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GamePanel.Location = new System.Drawing.Point(0, 0);
            this.GamePanel.Name = "GamePanel";
            this.GamePanel.Size = new System.Drawing.Size(272, 303);
            this.GamePanel.TabIndex = 3;
            // 
            // EnvironmentSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 303);
            this.Controls.Add(this.GamePanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnvironmentSelectionForm";
            this.ShowIcon = false;
            this.Text = "Choose Your Game";
            this.Load += new System.EventHandler(this.GameSelectionFormLoad);
            this.GamePanel.ResumeLayout(false);
            this.GamePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel GameTable;
        private System.Windows.Forms.Panel GamePanel;
    }
}