namespace Sledge.Gui.Interfaces
{
    public class ComboBoxItem : ImageListItem, IComboBoxItem
    {
        private bool _drawBorder;
        private string _displayText;

        public virtual bool DrawBorder
        {
            get { return _drawBorder; }
            set
            {
                if (_drawBorder == value) return;
                _drawBorder = value;
                OnPropertyChanged("DrawBorder");
            }
        }

        public virtual string DisplayText
        {
            get { return _displayText; }
            set
            {
                if (_displayText == value) return;
                _displayText = value;
                OnPropertyChanged("DisplayText");
            }
        }

        public static implicit operator ComboBoxItem(string text)
        {
            return new ComboBoxItem { Text = text };
        }
    }
}