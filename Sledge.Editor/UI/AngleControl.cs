using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
	/// <summary>
	/// An AngleControl is used as a convenient way for a user
	/// to set an angle of between 0 and 360 degrees, using a
	/// mouse drag-and-drop approach.
	/// </summary>
	public partial class AngleControl : UserControl
    {
        public delegate void AngleChangedEventHandler(object sender, AngleChangedEventArgs e);

		public event AngleChangedEventHandler AngleChangedEvent;

        public class AngleChangedEventArgs : EventArgs
        {
            public string Text { get; private set; }
            public int Degrees { get; private set; }
            public double Radians { get; private set; }

            public AngleChangedEventArgs(double rad)
            {
                Radians = rad;
                Degrees = (int)(rad * 180 / Math.PI);
                Text = Degrees.ToString();
            }
        }

		private double _angle;
		private int _elevation;
		private bool _draggedinside;
		private string _anglestring;

		public int Degrees
        {
            get { return (int) (_angle * 180 / Math.PI); }
		}

		public override string Text
        {
			get { return _anglestring; }
		}

		private bool _showTextBox;

		public bool ShowTextBox
        {
			get
			{
			    return _showTextBox;
			}
			set
            {
				_showTextBox = value;
				lblAngles.Visible = value;
				cmbAngles.Visible = value;
			}
		}

		public bool ShowLabel
        {
			get { return lblAngle.Visible; }
			set { lblAngle.Visible = value; }
		}

		public AngleControl()
		{
			InitializeComponent();
			_angle = 0;
			_elevation = 0;
			_draggedinside = false;
			_anglestring = Degrees.ToString();
			_showTextBox = true;
		}
		
		private void FireAngleChangedEvent()
		{
			OnAngleChangedEvent(new AngleChangedEventArgs(_angle));
		}

		protected virtual void OnAngleChangedEvent(AngleChangedEventArgs e)
		{
			if (AngleChangedEvent == null) return;
			AngleChangedEvent(this,e);
		}
		
		/// <summary>
		/// Sets the current angle in degrees<br/>
		/// Valid inputs:<br/>
		/// -1: Sets the angle to Up<br/>
		/// -2: Sets the angle to Down<br/>
		/// 0-360: Sets the angle to the supplied value in degrees.
		/// </summary>
		/// <param name="ang">The angle in degrees.</param>
		/// <exception cref="ArgumentException">If the angle is not in the valid range.</exception>
		public void SetAngle(int ang)
		{
			if (ang < -2 || ang > 359) throw new ArgumentException("The angle must be between -2 and 359.");
			if (ang < 0)
            {
				_elevation = -ang;
				if (_elevation == 1) _anglestring = "Up";
				if (_elevation == 2) _anglestring = "Down";
			}
			else
            {
				_angle = ang * Math.PI / 180;
				_elevation = 0;
				_anglestring = Degrees.ToString();
			}
			UpdateAngle();
			FireAngleChangedEvent();
		}
		
		/// <summary>
		/// Sets the current angle in radians
		/// </summary>
		/// <param name="ang">The angle in radians</param>
		public void SetAngle(double ang)
		{
		    SetAngle((int) Math.Floor(ang * 180 / Math.PI));
		}
		
		/// <summary>
		/// Sets the current angle to Up.
		/// </summary>
		public void SetUp()
		{
			SetAngle(-1);
		}
		
		/// <summary>
		/// Sets the current angle to Down.
		/// </summary>
		public void SetDown()
		{
			SetAngle(-2);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			System.Drawing.Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			var fill = new SolidBrush(Color.Black);
			var top = new Pen(Color.FromArgb(167,166,170),4);
			var bottom = new Pen(Color.White,4);

			var x = Width - 38;
			g.DrawArc(bottom,x,2,36,36,315,180);
			g.DrawArc(top,x,2,36,36,135,180);
			g.FillEllipse(fill, x, 2, 36, 36);
			UpdateAngle(g);
		}
		
		private void UpdateAngle()
		{
			System.Drawing.Graphics g = CreateGraphics();
			UpdateAngle(g);
			g.Dispose();
		}
		
		/// <summary>
		/// Updates the line indicating the angle.
		/// </summary>
		void UpdateAngle(System.Drawing.Graphics g)
		{
			var x = Width - 40;
		    var fill = new SolidBrush(Color.Black);
		    var line = new Pen(Color.White, 1);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
		    g.FillEllipse(fill, x + 4, 4, 32, 32);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			if (_elevation > 0)
            {
				// Draw a single pixel in the center - a bit of a pain to do
				var pt = new Bitmap(1, 1);
				pt.SetPixel(0, 0, Color.White);
				g.DrawImageUnscaled(pt, 20, 20);
				pt.Dispose();
			}
			else
			{
			    var xcoord = x + (int) Math.Round(Math.Cos(_angle) * 15, 0) + 20;
			    var ycoord = -(int) Math.Round(Math.Sin(_angle) * 15, 0) + 20;
			    g.DrawLine(line, x + 20, 20, xcoord, ycoord);
			}
			lblAngle.Text = _anglestring;
			cmbAngles.Text = _anglestring;
		}
		
		private void AngleControlMouseMove(object sender, MouseEventArgs e)
		{
		    if (!_draggedinside) return;
		    var x = Width - 40;
		    var xcoord = (e.X-20) - x;
		    var ycoord = -(e.Y-20);
		    var ang = Math.Atan2(ycoord,xcoord);
            while (ang < 0) ang += 2 * Math.PI;
		    SetAngle(ang);
		}

		private void AngleControlMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;
			var x = Width - 40;
		    var xcoord = (e.X - 20) - x;
		    var ycoord = -(e.Y - 20);
		    var dist = (float) Math.Sqrt(Math.Pow(xcoord, 2) + Math.Pow(ycoord, 2));
		    if (dist >= 20) return;
		    _draggedinside = true;
		    AngleControlMouseMove(sender,e);
		}

		void AngleControlMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;
			_draggedinside = false;
		}
		
		void CmbAnglesSelectedIndexChanged(object sender, EventArgs e)
		{
		    SetAngle(-(cmbAngles.SelectedIndex + 1));
		}
		
		void CmbAnglesKeyDown(object sender, KeyEventArgs e)
		{
		    int i;
		    if (e.KeyCode == Keys.Enter && int.TryParse(cmbAngles.Text, out i) && i >= 0 && i <= 359)
            {
		        SetAngle(i);
		    }
		}
	}
}
