using System;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.EditorNew.Brushes.Controls
{
    public class BooleanControl : BrushControl
    {
        public bool Checked
        {
            get { return Checkbox.Checked; }
            set { Checkbox.Checked = value; }
        }

        // todo translate
        public string LabelText
        {
            get { return Checkbox.Text; }
            set { Checkbox.Text = value; }
        }

        public string LabelTextKey
        {
            get { return Checkbox.TextKey; }
            set { Checkbox.TextKey = value; }
        }

        public bool ControlEnabled
        {
            get { return Checkbox.Enabled; }
            set { Checkbox.Enabled = value; }
        }

        public CheckBox Checkbox { get; set; }

        public BooleanControl(IBrush brush) : base(brush)
        {
            Checkbox = new CheckBox();
            Checkbox.Toggled += ValueChanged;
            this.Add(Checkbox, true);
        }

        public bool GetValue()
        {
            return Checkbox.Checked;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            OnValuesChanged(Brush);
        }
    }
}
