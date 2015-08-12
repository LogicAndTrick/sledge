using System.ComponentModel;

namespace Sledge.Rendering.Scenes
{
    public abstract class SceneObject : INotifyPropertyChanged
    {
        private static int _nextID = 1;

        public int ID { get; internal set; }
        public Scene Scene { get; set; }
        public bool IsVisible { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected SceneObject()
        {
            ID = _nextID++;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}