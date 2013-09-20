using System.Windows.Forms;

namespace Sledge.Editor.UI.ObjectProperties
{
    internal abstract class SmartEditControl : FlowLayoutPanel
    {
        public string OriginalName { get; private set; }
        public string PropertyName { get; private set; }
        public string PropertyValue { get; private set; }
        public DataStructures.GameData.Property Property { get; private set; }

        public delegate void ValueChangedEventHandler(object sender, string propertyName, string propertyValue);
        public delegate void NameChangedEventHandler(object sender, string oldName, string newName);

        public event ValueChangedEventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            if (_setting) return;
            PropertyValue = GetValue();
            if (ValueChanged != null)
            {
                ValueChanged(this, PropertyName, PropertyValue);
            }
        }

        public event ValueChangedEventHandler NameChanged;

        protected virtual void OnNameChanged()
        {
            if (_setting) return;
            PropertyName = GetName();
            if (NameChanged != null)
            {
                NameChanged(this, OriginalName, PropertyName);
            }
        }

        protected SmartEditControl()
        {
            Dock = DockStyle.Fill;
        }

        private bool _setting;

        public void SetProperty(string originalName, string newName, string currentValue, DataStructures.GameData.Property property)
        {
            _setting = true;
            OriginalName = originalName;
            PropertyName = newName;
            PropertyValue = currentValue;
            Property = property;
            OnSetProperty();
            _setting = false;
        }

        protected abstract string GetName();
        protected abstract string GetValue();
        protected abstract void OnSetProperty();
    }
}