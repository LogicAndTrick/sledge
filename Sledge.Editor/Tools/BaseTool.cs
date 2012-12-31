using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.UI;
using System.Drawing;

namespace Sledge.Editor.Tools
{
    public abstract class BaseTool
    {
        public enum ToolUsage
        {
            View2D,
            View3D,
            Both
        }

        protected static Coordinate SnapIfNeeded(Coordinate c)
        {
            return KeyboardState.Alt ? c : c.Snap(Document.GridSpacing);
        }

        public ViewportBase Viewport { get; set; }
        public ToolUsage Usage { get; set; }

        public abstract Image GetIcon();
        public abstract string GetName();

        protected BaseTool()
        {
            Viewport = null;
            Usage = ToolUsage.View2D;
        }

        public virtual void ToolSelected()
        {
            // Virtual
        }

        public virtual void ToolDeselected()
        {
            // Virtual
        }

        public abstract void MouseEnter(ViewportBase viewport, EventArgs e);
        public abstract void MouseLeave(ViewportBase viewport, EventArgs e);
        public abstract void MouseDown(ViewportBase viewport, MouseEventArgs e);
        public abstract void MouseUp(ViewportBase viewport, MouseEventArgs e);
        public abstract void MouseWheel(ViewportBase viewport, MouseEventArgs e);
        public abstract void MouseMove(ViewportBase viewport, MouseEventArgs e);
        public abstract void KeyPress(ViewportBase viewport, KeyPressEventArgs e);
        public abstract void KeyDown(ViewportBase viewport, KeyEventArgs e);
        public abstract void KeyUp(ViewportBase viewport, KeyEventArgs e);
        public abstract void UpdateFrame(ViewportBase viewport);
        public abstract void Render(ViewportBase viewport);

        public virtual void PreRender(ViewportBase viewport)
        {
            return;
        }

        public virtual bool IsCapturingMouseWheel()
        {
            return false;
        }
    }
}
