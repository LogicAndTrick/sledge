using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.Shell.Controls
{
    // https://stackoverflow.com/a/24087828
    public class DropdownButton : Button
    {
        [DefaultValue(null)]
        public ContextMenuStrip Menu { get; set; }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (Menu == null || mevent.Button != MouseButtons.Left) return;

            var menuLocation = new Point(0, Height);
            Menu.Show(this, menuLocation);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (Menu == null) return;

            var arrowX = ClientRectangle.Width - 14;
            var arrowY = ClientRectangle.Height / 2 - 1;

            var brush = Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow;
            Point[] arrows = { new Point(arrowX, arrowY), new Point(arrowX + 7, arrowY), new Point(arrowX + 3, arrowY + 4) };
            pevent.Graphics.FillPolygon(brush, arrows);
        }
    }
}
