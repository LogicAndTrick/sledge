using System.Drawing;
using System.Windows.Forms;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public sealed class HeadingButton : Button
    {
        public string HeadingText { get; set; }
        public Font HeadingFont { get; set; }

        public HeadingButton()
        {
            Height = 80;
            Padding = new Padding(0, 30, 0, 0);
            TextAlign = ContentAlignment.TopLeft;
            HeadingFont = new Font(Font.FontFamily, 16, FontStyle.Bold);
        }

        protected override void Dispose(bool disposing)
        {
            HeadingFont.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.DrawString(HeadingText, HeadingFont, Brushes.Black, 3, 7);
        }
    }
}