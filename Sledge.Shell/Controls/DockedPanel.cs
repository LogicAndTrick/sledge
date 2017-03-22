using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.Shell.Properties;

namespace Sledge.Shell.Controls
{
    public class DockedPanel : Panel
    {
        public int DockDimension
        {
            get
            {
                switch (Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return Height;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        return Width;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        Height = value;
                        break;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        Width = value;
                        break;
                }
            }
        }

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                if (value)
                {
                    _savedDimension = DockDimension;
                    DockDimension = HandleWidth;
                }
                else
                {
                    DockDimension = Math.Max(_savedDimension, 10);
                }
                Refresh();
            }
        }

        private int _savedDimension = 100;
        private bool _resizing;

        [DefaultValue(500)]
        public int MaxSize { get; set; }

        [DefaultValue(10)]
        public int MinSize { get; set; }

        public DockedPanel()
        {
            MinSize = 10;
            MaxSize = 500;
            _resizing = false;
        }

        private static readonly Bitmap _arrow;

        static DockedPanel()
        {
            _arrow = new Bitmap(Resources.Arrow_Down.Width, Resources.Arrow_Down.Height);
            using (var g = System.Drawing.Graphics.FromImage(_arrow))
            {
                using (var attrs = new ImageAttributes())
                {
                    var colorMatrix = new ColorMatrix(new[]
                    {
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, 0.5f, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                    attrs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    g.DrawImage(Resources.Arrow_Down, new Rectangle(0, 0, _arrow.Width, _arrow.Height), 0, 0, _arrow.Width, _arrow.Height, GraphicsUnit.Pixel, attrs);
                }
            }
        }

        private const int ResizeHandleSize = 4;

        private bool IsInResizeArea(MouseEventArgs e)
        {
            switch (Dock)
            {
                case DockStyle.Left:
                    return Width >= e.X && Width - ResizeHandleSize <= e.X;
                case DockStyle.Right:
                    return e.X >= 0 && e.X <= ResizeHandleSize;
                case DockStyle.Top:
                    return Height >= e.Y && Height - ResizeHandleSize <= e.Y;
                case DockStyle.Bottom:
                    return e.Y >= 0 && e.Y <= ResizeHandleSize;
            }
            return false;
        }

        private bool IsInButtonArea(MouseEventArgs e)
        {
            switch (Dock)
            {
                case DockStyle.Left:
                    return Width >= e.X && Width - ButtonHeight <= e.X && e.Y <= ButtonHeight;
                case DockStyle.Right:
                    return e.X >= 0 && e.X <= ButtonHeight && e.Y <= ButtonHeight;
                case DockStyle.Top:
                    return Height >= e.Y && Height - ButtonHeight <= e.Y && e.X <= ButtonHeight;
                case DockStyle.Bottom:
                    return e.Y >= 0 && e.Y <= ButtonHeight && e.X <= ButtonHeight;
            }
            return false;
        }

        private void SetDockSize(MouseEventArgs e)
        {
            int width = Width, height = Height;
            switch (Dock)
            {
                case DockStyle.Left:
                    width = e.X;
                    break;
                case DockStyle.Right:
                    width -= e.X;
                    break;
                case DockStyle.Top:
                    height = e.Y;
                    break;
                case DockStyle.Bottom:
                    height -= e.Y;
                    break;
            }
            Width = Math.Min(Math.Max(width, Math.Max(MinSize, ResizeHandleSize + 1)), MaxSize);
            Height = Math.Min(Math.Max(height, Math.Max(MinSize, ResizeHandleSize + 1)), MaxSize);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_resizing)
            {
                SetDockSize(e);
            } 
            else
            {
                var ba = IsInButtonArea(e);
                var ra = IsInResizeArea(e);
                if (ba || (_hidden && ra))
                {
                    Cursor = Cursors.Hand;
                }
                else if (ra && !_hidden)
                {
                    Cursor = (Dock == DockStyle.Left || Dock == DockStyle.Right) ? Cursors.SizeWE : Cursors.SizeNS;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_resizing) Cursor = Cursors.Default;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var ba = IsInButtonArea(e);
            var ra = IsInResizeArea(e);
            if (ba || (ra && _hidden)) Hidden = !Hidden;
            else if (!_hidden && IsInResizeArea(e)) _resizing = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _resizing = false;
            base.OnMouseUp(e);
        }

        private const int ButtonHeight = 12;
        private const int HandleWidth = 8;
        private const int RenderHandleWidth = 3;

        protected override void OnDockChanged(EventArgs e)
        {
            var padding = new Padding(0);
            switch (Dock)
            {
                case DockStyle.Top:
                    padding.Bottom = HandleWidth;
                    break;
                case DockStyle.Bottom:
                    padding.Top = HandleWidth;
                    break;
                case DockStyle.Left:
                    padding.Right = HandleWidth;
                    break;
                case DockStyle.Right:
                    padding.Left = HandleWidth;
                    break;
            }
            Padding = padding;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            const int padding = 4;
            var rect = Rectangle.Empty;
            var rotflip = RotateFlipType.RotateNoneFlipNone;
            int buttonX = 0, buttonY = 0;
            switch (Dock)
            {
                case DockStyle.Top:
                    rect = new Rectangle(padding + ButtonHeight, Height - RenderHandleWidth - 1, Width - padding - padding - ButtonHeight, RenderHandleWidth);
                    rotflip = _hidden ? RotateFlipType.RotateNoneFlipNone : RotateFlipType.RotateNoneFlipY;
                    buttonX = padding;
                    buttonY = Height - HandleWidth;
                    break;
                case DockStyle.Bottom:
                    rect = new Rectangle(padding + ButtonHeight, 1, Width - padding - padding - ButtonHeight, RenderHandleWidth);
                    rotflip = !_hidden ? RotateFlipType.RotateNoneFlipNone : RotateFlipType.RotateNoneFlipY;
                    buttonX = padding;
                    break;
                case DockStyle.Left:
                    rect = new Rectangle(Width - RenderHandleWidth - 1, padding + ButtonHeight, RenderHandleWidth, Height - padding - padding - ButtonHeight);
                    rotflip = _hidden ? RotateFlipType.Rotate90FlipX : RotateFlipType.Rotate90FlipNone;
                    buttonY = padding;
                    buttonX = Width - HandleWidth;
                    break;
                case DockStyle.Right:
                    rect = new Rectangle(1, padding + ButtonHeight, RenderHandleWidth, Height - padding - padding - ButtonHeight);
                    rotflip = !_hidden ? RotateFlipType.Rotate90FlipX : RotateFlipType.Rotate90FlipNone;
                    buttonY = padding;
                    break;
            }
            if (!rect.IsEmpty)
            {
                using (var b = new SolidBrush(BackColor.Darken(_hidden ? 10 : 40)))
                {
                    e.Graphics.FillRectangle(b, rect);
                }
                using (var cl = new Bitmap(_arrow))
                {
                    cl.RotateFlip(rotflip);
                    e.Graphics.DrawImage(cl, buttonX, buttonY);
                }
            }
        }
    }
}

