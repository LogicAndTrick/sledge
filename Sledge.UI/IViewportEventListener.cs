using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.UI
{
    public interface IViewportEventListener
    {
        ViewportBase Viewport { get; set; }

        void KeyUp(KeyEventArgs e);
        void KeyDown(KeyEventArgs e);
        void KeyPress(KeyPressEventArgs e);

        void MouseMove(MouseEventArgs e);
        void MouseWheel(MouseEventArgs e);
        void MouseUp(MouseEventArgs e);
        void MouseDown(MouseEventArgs e);

        void MouseEnter(EventArgs e);
        void MouseLeave(EventArgs e);

        void UpdateFrame();
        void PreRender();
        void Render3D();
        void Render2D();
    }
}
