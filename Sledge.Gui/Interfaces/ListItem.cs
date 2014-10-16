using System;
using System.ComponentModel;

namespace Sledge.Gui.Interfaces
{
    public class ListItem : IListItem
    {
        private string _text;
        private object _value;
        private bool _selected;

        public virtual string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public virtual object Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value) return;
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public override string ToString()
        {
            return Text ?? (Value != null ? Value.ToString() : String.Empty);
        }

        public static implicit operator ListItem(string text)
        {
            return new ListItem { Text = text };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}