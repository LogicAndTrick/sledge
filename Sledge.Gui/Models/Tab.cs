using Sledge.Gui.Interfaces.Models;

namespace Sledge.Gui.Models
{
    public class Tab : ListItem, ITab
    {
        private bool _dirty;
        private bool _closable;

        public bool Dirty
        {
            get { return _dirty; }
            set
            {
                if (_dirty == value) return;
                _dirty = value;
                OnPropertyChanged("Dirty");
            }
        }

        public bool Closable
        {
            get { return _closable; }
            set
            {
                if (_closable == value) return;
                _closable = value;
                OnPropertyChanged("Closable");
            }
        }

        public Tab()
        {
            _closable = true;
        }
    }
}