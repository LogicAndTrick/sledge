using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    public partial class EmptyTab
    {
        private void InitializeComponent()
        {
            this.lblNothing = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNothing
            // 
            this.lblNothing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNothing.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblNothing.Location = new System.Drawing.Point(0, 0);
            this.lblNothing.Name = "lblNothing";
            this.lblNothing.Size = new System.Drawing.Size(391, 222);
            this.lblNothing.TabIndex = 0;
            this.lblNothing.Text = "Nothing is selected";
            this.lblNothing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EmptyTab
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.lblNothing);
            this.Name = "EmptyTab";
            this.Size = new System.Drawing.Size(391, 222);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblNothing;
    }
}
