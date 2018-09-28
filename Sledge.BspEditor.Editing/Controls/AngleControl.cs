using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common;

namespace Sledge.BspEditor.Editing.Controls
{
    /// <summary>
    /// An AngleControl is used as a convenient way for a user
    /// to set an angle of between 0 and 360 degrees, using a
    /// mouse drag-and-drop approach.
    /// </summary>
    public partial class AngleControl : UserControl
    {
        public event EventHandler AngleChangedEvent;

        private int _angle;
        private bool _draggedinside;

        public int Angle
        {
            get => _angle;
            set
            {
                while (value < 0) value += 360;
                value = value % 360;

                _angle = value;

                UpdateAngle();
            }
        }

        public bool Up
        {
            get => _angle == -1;
            set
            {
                _angle = -1;
                UpdateAngle();
            }
        }

        public bool Down
        {
            get => _angle == -2;
            set
            {
                _angle = -2;
                UpdateAngle();
            }
        }

        public override string Text
        {
            get
            {
                if (_angle == -1) return @"Up";
                else if (_angle == -2) return @"Down";
                return _angle.ToString();
            }
        }

        public string AnglePropertyString
        {
            get
            {
                if (_angle == -1) return "-90 0 0";
                if (_angle == -2) return "90 0 0";
                return "0 " + _angle + " 0";
            }
            set
            {
                var split = value.Split(' ');

                if (split.Length != 3) return;
                if (!int.TryParse(split[0], out var a1)) return;
                if (!int.TryParse(split[1], out var a2)) return;
                if (!int.TryParse(split[2], out var a3)) return;

                if (a1 == 0 && a3 == 0) Angle = a2;
                else if (a1 == 90 && a2 == 0 && a3 == 0) Down = true;
                else if (a1 == -90 && a2 == 0 && a3 == 0) Up = true;
                else cmbAngles.Text = "";
            }
        }

        private bool _showTextBox;
        public bool ShowTextBox
        {
            get => _showTextBox;
            set
            {
                _showTextBox = value;
                lblAngles.Visible = value;
                cmbAngles.Visible = value;
            }
        }

        public bool ShowLabel
        {
            get => lblAngle.Visible;
            set => lblAngle.Visible = value;
        }

        public string LabelText
        {
            get => lblAngles.Text;
            set => lblAngles.Text = value;
        }

        public AngleControl()
        {
            InitializeComponent();
            _angle = 0;
            _draggedinside = false;
            _showTextBox = true;
        }

        private void FireAngleChangedEvent()
        {
            AngleChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            var fill = Enabled ? Brushes.Black : Brushes.LightGray;
            var top = new Pen(Color.FromArgb(167, 166, 170), 4);
            var bottom = new Pen(Color.White, 4);

            var x = Width - 38;
            g.DrawArc(bottom, x, 2, 36, 36, 315, 180);
            g.DrawArc(top, x, 2, 36, 36, 135, 180);
            g.FillEllipse(fill, x, 2, 36, 36);
            UpdateAngle(g);
        }

        private void UpdateAngle()
        {
            lblAngle.Text = Text;
            cmbAngles.Text = Text;
            Refresh();
        }

        /// <summary>
        /// Updates the line indicating the angle.
        /// </summary>
        void UpdateAngle(Graphics g)
        {
            var x = Width - 40;
            var fill = Enabled ? Brushes.Black : Brushes.LightGray;
            var line = Enabled ? Pens.White : Pens.LightGray;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.FillEllipse(fill, x + 4, 4, 32, 32);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (_angle < 0)
            {
                // Draw a single pixel in the center - a bit of a pain to do
                var pt = new Bitmap(1, 1);
                pt.SetPixel(0, 0, Color.White);
                g.DrawImageUnscaled(pt, 20, 20);
                pt.Dispose();
            }
            else
            {
                var rad = MathHelper.DegreesToRadians(_angle);
                var xcoord = x + (int)Math.Round(Math.Cos(rad) * 15, 0) + 20;
                var ycoord = -(int)Math.Round(Math.Sin(rad) * 15, 0) + 20;
                g.DrawLine(line, x + 20, 20, xcoord, ycoord);
            }
        }

        private void AngleControlMouseMove(object sender, MouseEventArgs e)
        {
            if (!_draggedinside) return;
            var x = Width - 40;
            var xcoord = (e.X - 20) - x;
            var ycoord = -(e.Y - 20);
            var ang = Math.Atan2(ycoord, xcoord);
            while (ang < 0) ang += 2 * Math.PI;
            Angle = (int) MathHelper.RadiansToDegrees(ang);
        }

        private void AngleControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var x = Width - 40;
            var xcoord = (e.X - 20) - x;
            var ycoord = -(e.Y - 20);
            var dist = (float)Math.Sqrt(Math.Pow(xcoord, 2) + Math.Pow(ycoord, 2));
            if (dist >= 20) return;
            _draggedinside = true;
            AngleControlMouseMove(sender, e);
        }

        void AngleControlMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _draggedinside = false;

            FireAngleChangedEvent();
        }

        void CmbAnglesSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAngles.SelectedIndex == 0) Up = true;
            else Down = true;

            FireAngleChangedEvent();
        }

        void CmbAnglesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || !int.TryParse(cmbAngles.Text, out var i) || i < 0 || i > 359) return;

            Angle = i;
            FireAngleChangedEvent();
        }
    }
}
