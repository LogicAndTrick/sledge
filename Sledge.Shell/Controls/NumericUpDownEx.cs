using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Shell.Controls
{
    /// <summary>
    /// A <see cref="NumericUpDown"/> that has a few extra features.
    /// </summary>
    public class NumericUpDownEx : NumericUpDown
    {
        /// <summary>
        /// The multiplier to use for mouse wheel when control is down. Default is 0.1.
        /// </summary>
        [Category("Data")]
        [Description("The multiplier to use for mouse wheel when control is down.")]
        [DefaultValue(typeof(decimal), "0.1")]
        public decimal CtrlWheelMultiplier { get; set; } = 0.1m;

        /// <summary>
        /// The multiplier to use for mouse wheel when shift is down. Default is 10.
        /// </summary>
        [Category("Data")]
        [Description("The multiplier to use for mouse wheel when shift is down.")]
        [DefaultValue(typeof(decimal), "10")]
        public decimal ShiftWheelMultiplier { get; set; } = 10;

        /// <summary>
        /// The increment to use for mouse wheel operations. If 0, the Increment value will be used.
        /// </summary>
        [Category("Data")]
        [Description("The increment to use for mouse wheel operations. If 0, the Increment will be used")]
        [DefaultValue(typeof(decimal), "0")]
        public decimal WheelIncrement { get; set; } = 0;

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e is HandledMouseEventArgs hme) hme.Handled = true;

            // Determine the increment
            var wi = WheelIncrement == 0 ? Increment : WheelIncrement;
            var inc = 0m;
            if (e.Delta < 0) inc = -wi;
            else if (e.Delta > 0) inc = wi;

            // If multipliers are enabled, apply them
            if (ShiftWheelMultiplier != 0 && ModifierKeys.HasFlag(Keys.Shift))
            {
                inc *= ShiftWheelMultiplier;
            }
            else if (CtrlWheelMultiplier != 0 && ModifierKeys.HasFlag(Keys.Control))
            {
                inc *= CtrlWheelMultiplier;
            }

            // Change the value
            if (inc != 0) Value += inc;

            // The event is handled, but pass to the base class anyway
            base.OnMouseWheel(e);
        }
    }
}
