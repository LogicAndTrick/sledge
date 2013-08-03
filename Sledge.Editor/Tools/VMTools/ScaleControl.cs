using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.Tools.VMTools
{
    public partial class ScaleControl : UserControl
    {
        public delegate void ValueChangedEventHandler(object sender, decimal value, bool relative);
        public delegate void ValueResetEventHandler(object sender, decimal value, bool relative);
        public delegate void ResetOriginEventHandler(object sender);

        public event ValueChangedEventHandler ValueChanged;
        public event ValueResetEventHandler ValueReset;
        public event ResetOriginEventHandler ResetOrigin;

        protected virtual void OnValueChanged(decimal value, bool relative)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, value, relative);
            }
        }

        protected void OnValueReset(decimal value, bool relative)
        {
            if (ValueReset != null)
            {
                ValueReset(this, value, relative);
            }
        }

        protected void OnResetOrigin()
        {
            if (ResetOrigin != null)
            {
                ResetOrigin(this);
            }
        }

        private bool _freeze;

        public ScaleControl()
        {
            _freeze = true;
            InitializeComponent();
            _freeze = false;
        }

        public void ResetValue()
        {
            _freeze = true;
            if (UseRelative.Checked)
            {
                DistanceValue.Value = 100;
                DistanceValue.Minimum = 0;
                DistanceValue.Maximum = 10000;
                DistanceValue.Increment = 10;
            }
            else
            {
                DistanceValue.Value = 0;
                DistanceValue.Minimum = -10000;
                DistanceValue.Maximum = 10000;
                DistanceValue.Increment = _gridSpacing;
            }
            _freeze = false;
            OnValueReset(DistanceValue.Value, UseRelative.Checked);
        }

        private decimal _gridSpacing = 1;

        public void SetGridSpacing(decimal value)
        {
            _gridSpacing = value;
            if (!UseRelative.Checked) DistanceValue.Increment = _gridSpacing;
        }

        private void DistanceValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            OnValueChanged(DistanceValue.Value, UseRelative.Checked);
        }

        private void UseRelativeChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            ResetValue();
        }

        private void ResetDistanceClicked(object sender, EventArgs e)
        {
            ResetValue();
        }

        private void ResetOriginClicked(object sender, EventArgs e)
        {
            OnResetOrigin();
        }
    }
}
