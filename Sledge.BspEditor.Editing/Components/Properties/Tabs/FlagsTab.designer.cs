namespace Sledge.BspEditor.Editing.Components.Properties.Tabs
{
    public sealed partial class FlagsTab
    {
        private void InitializeComponent()
        {
            this.FlagsTable = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // FlagsTable
            // 
            this.FlagsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlagsTable.CheckOnClick = true;
            this.FlagsTable.FormattingEnabled = true;
            this.FlagsTable.IntegralHeight = false;
            this.FlagsTable.Location = new System.Drawing.Point(3, 3);
            this.FlagsTable.Name = "FlagsTable";
            this.FlagsTable.Size = new System.Drawing.Size(673, 372);
            this.FlagsTable.TabIndex = 1;
            this.FlagsTable.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.FlagsTableChanged);
            // 
            // FlagsTab
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.FlagsTable);
            this.Name = "FlagsTab";
            this.Size = new System.Drawing.Size(679, 378);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckedListBox FlagsTable;
    }
}
