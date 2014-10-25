using Sledge.Gui.Interfaces.Models;

namespace Sledge.Gui.Models
{
    public class CheckboxListItem : ListItem, ICheckboxListItem
    {
        private bool _checked;
        private bool _indeterminate;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                _checked = value;
                OnPropertyChanged("Checked");
            }
        }

        public bool Indeterminate
        {
            get { return _indeterminate; }
            set
            {
                if (_indeterminate == value) return;
                _indeterminate = value;
                OnPropertyChanged("Indeterminate");
            }
        }

        public static implicit operator CheckboxListItem(string text)
        {
            return new CheckboxListItem { Text = text };
        }
    }
}