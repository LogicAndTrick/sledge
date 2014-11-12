using Sledge.Gui.Containers;

namespace Sledge.EditorNew.Brushes.Controls
{
    public class BrushControl : HorizontalBox
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
