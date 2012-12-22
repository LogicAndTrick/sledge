namespace Sledge.Editor
{
    partial class GameSelectionForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstEngine = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstGame = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Engine";
            // 
            // lstEngine
            // 
            this.lstEngine.FormattingEnabled = true;
            this.lstEngine.IntegralHeight = false;
            this.lstEngine.Location = new System.Drawing.Point(15, 31);
            this.lstEngine.Name = "lstEngine";
            this.lstEngine.Size = new System.Drawing.Size(140, 260);
            this.lstEngine.TabIndex = 1;
            this.lstEngine.SelectedIndexChanged += new System.EventHandler(this.LstEngineSelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Game";
            // 
            // lstGame
            // 
            this.lstGame.FormattingEnabled = true;
            this.lstGame.IntegralHeight = false;
            this.lstGame.Location = new System.Drawing.Point(161, 31);
            this.lstGame.Name = "lstGame";
            this.lstGame.Size = new System.Drawing.Size(140, 260);
            this.lstGame.TabIndex = 1;
            this.lstGame.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LstGameMouseDoubleClick);
            // 
            // GameSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 303);
            this.Controls.Add(this.lstGame);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstEngine);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GameSelectionForm";
            this.Text = "Choose Your Game";
            this.Load += new System.EventHandler(this.GameSelectionFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstEngine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstGame;
    }
}