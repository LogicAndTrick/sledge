using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    class DockedPanel : Panel
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
                    DockDimension = 0;
                }
                else
                {
                    DockDimension = Math.Max(_savedDimension, 10);
                }
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

        private const int ResizeHandleSize = 4;

        private bool IsInResizeArea(MouseEventArgs e)
        {
            switch (Dock)
            {
                case DockStyle.Left:
                    return Width >= e.X && Width - ResizeHandleSize <= e.X;
                    break;
                case DockStyle.Right:
                    return e.X >= 0 && e.X <= ResizeHandleSize;
                case DockStyle.Top:
                    return Height >= e.Y && Height - ResizeHandleSize <= e.Y;
                case DockStyle.Bottom:
                    return e.Y >= 0 && e.Y <= ResizeHandleSize;
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

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Hidden) return;
            if (_resizing)
            {
                SetDockSize(e);
            } 
            else
            {
                if (IsInResizeArea(e))
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
            if (IsInResizeArea(e)) _resizing = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _resizing = false;
            base.OnMouseUp(e);
        }
    }
}

