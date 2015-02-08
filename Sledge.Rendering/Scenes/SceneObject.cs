using System.ComponentModel;

namespace Sledge.Rendering.Scenes
{
    public abstract class SceneObject : INotifyPropertyChanged
    {
        public int ID { get; internal set; }
        public Scene Scene { get; set; }
        public bool IsVisible { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}