using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Structures;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsNumericSpinner : WinFormsControl, INumericSpinner
    {
        private readonly NumericUpDown _numeric;
        
        public WinFormsNumericSpinner()
            : base(new NumericUpDown())
        {
            _numeric = (NumericUpDown)Control;
        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                return new Size(100, FontSize * 2);
            }
        }

        public decimal Value
        {
            get { return _numeric.Value; }
            set { _numeric.Value = value; }
        }

        public decimal Minimum
        {
            get { return _numeric.Minimum; }
            set { _numeric.Minimum = value; }
        }

        public decimal Maximum
        {
            get { return _numeric.Maximum; }
            set { _numeric.Maximum = value; }
        }

        public decimal Increment
        {
            get { return _numeric.Increment; }
            set { _numeric.Increment = value; }
        }

        public int Precision
        {
            get { return _numeric.DecimalPlaces; }
            set { _numeric.DecimalPlaces = value; }
        }

        public event EventHandler ValueChanged
        {
            add { _numeric.ValueChanged += value; }
            remove { _numeric.ValueChanged -= value; }
        }
    }
}