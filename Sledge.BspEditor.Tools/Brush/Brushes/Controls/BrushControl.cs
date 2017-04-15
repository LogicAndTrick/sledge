using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.Brush.Brushes.Controls
{
    public class BrushControl : UserControl
    {
        public delegate void ValuesChangedEventHandler(object sender, IBrush brush);

        public event ValuesChangedEventHandler ValuesChanged;

        protected virtual void OnValuesChanged(IBrush brush)
        {
            if (ValuesChanged != null)
            {
                ValuesChanged(this, brush);
            }
        }

        protected readonly IBrush Brush;

        private BrushControl()
        {
        }

        protected BrushControl(IBrush brush)
        {
            Brush = brush;
        }
    }
}
