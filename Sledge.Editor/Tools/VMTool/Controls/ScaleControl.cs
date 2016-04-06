using System;
using System.Windows.Forms;

namespace Sledge.Editor.Tools.VMTool.Controls
{
    public partial class ScaleControl : UserControl
    {
        public delegate void ValueChangedEventHandler(object sender, decimal value);
        public delegate void ValueResetEventHandler(object sender, decimal value);
        public delegate void ResetOriginEventHandler(object sender);

        public event ValueChangedEventHandler ValueChanged;
        public event ValueResetEventHandler ValueReset;
        public event ResetOriginEventHandler ResetOrigin;

        protected virtual void OnValueChanged(decimal value)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, value);
            }
        }

        protected void OnValueReset(decimal value)
        {
            if (ValueReset != null)
            {
                ValueReset(this, value);
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
            DistanceValue.Value = 100;
            OnValueReset(DistanceValue.Value);
            _freeze = false;
        }

        private void DistanceValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            OnValueChanged(DistanceValue.Value);
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
