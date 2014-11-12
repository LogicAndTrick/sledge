using System;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class NumericSpinner : ControlBase<INumericSpinner>, INumericSpinner
    {
        public decimal Value
        {
            get { return Control.Value; }
            set { Control.Value = value; }
        }

        public decimal Minimum
        {
            get { return Control.Minimum; }
            set { Control.Minimum = value; }
        }

        public decimal Maximum
        {
            get { return Control.Maximum; }
            set { Control.Maximum = value; }
        }

        public decimal Increment
        {
            get { return Control.Increment; }
            set { Control.Increment = value; }
        }

        public int Precision
        {
            get { return Control.Precision; }
            set { Control.Precision = value; }
        }

        public event EventHandler ValueChanged
        {
            add { Control.ValueChanged += value; }
            remove { Control.ValueChanged -= value; }
        }
    }
}