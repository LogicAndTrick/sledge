using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common.Easings;

namespace Sledge.Editor.Tools.VMTool
{
    public partial class StandardControl : UserControl
    {
        public delegate void SplitEventHandler(object sender);
        public delegate void MergeEventHandler(object sender);

        public event SplitEventHandler Split;
        public event MergeEventHandler Merge;

        protected virtual void OnSplit()
        {
            if (Split != null)
            {
                Split(this);
            }
        }

        protected virtual void OnMerge()
        {
            if (Merge != null)
            {
                Merge(this);
            }
        }

        public bool AutomaticallyMerge
        {
            get { return AutoMerge.Checked; }
            set { AutoMerge.Checked = value; }
        }

        public bool SplitEnabled
        {
            get { return SplitButton.Enabled; }
            set { SplitButton.Enabled = value; }
        }

        public StandardControl()
        {
            InitializeComponent();
        }

        private void SplitButtonClicked(object sender, EventArgs e)
        {
            OnSplit();
        }

        private void MergeButtonClicked(object sender, EventArgs e)
        {
            OnMerge();
        }

        public void ShowMergeResult(int mergedVertices, int removedFaces)
        {
            if (mergedVertices + removedFaces <= 0) return;
            MergeResultsLabel.Text = String.Format("{0} vert{1} merged, {2} face{3} removed",
                mergedVertices, mergedVertices == 1 ? "ex" : "ices",
                removedFaces, removedFaces == 1 ? "" : "s");
            MergeResultsLabel.Trigger();
        }

    }
    public class FadeLabel : Label
    {
        private long _lastTick;
        private long _remaining;
        private Timer _timer;
        private int _fadeTime = 1000;
        private Easing _easing;

        public int FadeTime
        {
            get { return _fadeTime; }
            set
            {
                _fadeTime = value;
            }
        }

        public FadeLabel()
        {
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);
            _timer = new Timer
            {
                Enabled = false,
                Interval = 50
            };
            _timer.Tick += (s, e) => Tick();
            _remaining = 0;
        }

        private void Tick()
        {
            var tick = DateTime.Now.Ticks / 10000;
            _remaining -= (tick - _lastTick);
            _lastTick = tick;
            if (_remaining <= 0) _timer.Stop();
            Invalidate();
            Refresh();
        }

        public void Trigger()
        {
            _remaining = _fadeTime;
            _lastTick = DateTime.Now.Ticks / 10000;
            _timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var val = _easing.Evaluate((_remaining * 1m) / _fadeTime);
            val = Math.Min(1, Math.Max(0, val));
            var a = (int) (val * 255);
            var c = Color.FromArgb(a, ForeColor);
            
            using (var brush = new SolidBrush(c))
            {
                e.Graphics.DrawString(Text, Font, brush, ClientRectangle);
            }
        }
    }
}
