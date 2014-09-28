using System.Linq;
using System.Text;
using Sledge.Gui.Controls;

namespace Sledge.Gui.WinForms.Controls
{
    public static class WinFormsExtensions
    {
        public static Rectangle ToRectangle(this System.Drawing.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Padding ToPadding(this System.Windows.Forms.Padding padding)
        {
            return new Padding(padding.Top, padding.Left, padding.Bottom, padding.Right);
        }
    }
}
