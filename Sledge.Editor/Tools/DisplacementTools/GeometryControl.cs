using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sledge.Common.Easings;
using Sledge.Editor.Extensions;

namespace Sledge.Editor.Tools.DisplacementTools
{
    public partial class GeometryControl : UserControl
    {
        #region Events

        public delegate void ResetAllPointsEventHandler(object sender);

        public event ResetAllPointsEventHandler ResetAllPoints;

        protected virtual void OnResetAllPoints()
        {
            if (ResetAllPoints != null)
            {
                ResetAllPoints(this);
            }
        }

        #endregion

        #region Enums

        public enum Axis
        {
            [Description("X Axis")] XAxis,
            [Description("Y Axis")] YAxis,
            [Description("Z Axis")] ZAxis,
            [Description("Face Normal")] FaceNormal,
            [Description("Point Normal")] PointNormal,
            [Description("Towards Viewport")] TowardsViewport
        }

        public enum Effect
        {
            RelativeDistance,
            AbsoluteDistance,
            SmoothPoints
        }

        public enum Brush
        {
            Spatial,
            Point
        }

        #endregion

        private bool _freeze;

        public GeometryControl()
        {
            InitializeComponent();
            foreach (Enum value in Enum.GetValues(typeof(Axis)))
            {
                AxisCombo.Items.Add(value.GetDescription());
            }
            AxisCombo.SelectedItem = Axis.FaceNormal.GetDescription();

            foreach (Enum value in Enum.GetValues(typeof(EasingType)))
            {
                SoftEdgeBrushModeCombo.Items.Add(value.GetDescription());
            }
            SoftEdgeBrushModeCombo.SelectedItem = EasingType.Sinusoidal.GetDescription();
        }

        private void ResetAllPointsButtonClick(object sender, EventArgs e)
        {
            OnResetAllPoints();
        }

        private void DistanceSliderScroll(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            DistanceUpDown.Value = DistanceSlider.Value / 100m;
            _freeze = false;
        }

        private void DistanceUpDownValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            var val = (int) (DistanceUpDown.Value * 100);
            DistanceSlider.Value = Math.Min(DistanceSlider.Maximum, val);
            _freeze = false;
        }

        private void SpatialRadiusSliderScroll(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            SpatialBrushRadiusUpDown.Value = SpatialRadiusSlider.Value / 100m;
            _freeze = false;
        }

        private void SpatialBrushRadiusUpDownValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            var val = (int)(SpatialBrushRadiusUpDown.Value * 100);
            SpatialRadiusSlider.Value = Math.Min(SpatialRadiusSlider.Maximum, val);
            _freeze = false;
        }

        private void PointBrushSizeSliderScroll(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            PointBrushSizeUpDown.Value = PointBrushSizeSlider.Value;
            _freeze = false;
        }

        private void PointBrushSizeUpDownValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _freeze = true;
            PointBrushSizeSlider.Value = (int) PointBrushSizeUpDown.Value;
            _freeze = false;
        }

        #region Properties

        public Effect SelectedEffect
        {
            get
            {
                if (AbsoluteDistanceRadio.Checked) return Effect.AbsoluteDistance;
                else if (SmoothPointsRadio.Checked) return Effect.SmoothPoints;
                else return Effect.RelativeDistance;
            }
        }

        public Brush SelectedBrush
        {
            get
            {
                if (SpatialBrushRadio.Checked) return Brush.Spatial;
                else return Brush.Point;
            }
        }

        public decimal SpatialBrushRadius
        {
            get { return SpatialBrushRadiusUpDown.Value; }
        }

        public int PointBrushSize
        {
            get { return (int)PointBrushSizeUpDown.Value; }
        }

        public bool SoftEdgeBrush
        {
            get { return SoftEdgeCheckbox.Checked; }
        }

        public Axis SelectedAxis
        {
            get { return EnumExtensions.FromDescription<Axis>((string) AxisCombo.SelectedItem); }
        }

        public bool AutoSew
        {
            get { return AutoSewCheckbox.Checked; }
        }

        public decimal Distance
        {
            get { return DistanceUpDown.Value; }
        }

        public Easing SoftEdgeEasing
        {
            get
            {
                // https://github.com/jquery/jquery-ui/blob/master/ui/jquery.ui.effect.js
                var easing = SoftEdgeBrush
                                 ? EnumExtensions.FromDescription<EasingType>((string) SoftEdgeBrushModeCombo.SelectedItem)
                                 : EasingType.Constant;
                return Easing.FromType(easing, EasingDirection.In);
            }
        }

        #endregion
    }
}
