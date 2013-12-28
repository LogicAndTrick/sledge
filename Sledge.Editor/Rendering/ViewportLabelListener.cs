using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    // ReSharper disable CSharpWarnings::CS0612
    // OpenTK's TextPrinter is marked as obsolete but no suitable replacement exists
    public class ViewportLabelListener : IViewportEventListener, IDisposable
    {
        public ViewportBase Viewport { get; set; }
        private TextPrinter _printer;
        private RectangleF _rect;
        private string _text;
        private bool _showing;

        public ViewportLabelListener(ViewportBase viewport)
        {
            Viewport = viewport;
            _printer = new TextPrinter(TextQuality.Low);
            _text = "";
            if (viewport is Viewport2D)
            {
                var dir = ((Viewport2D) viewport).Direction;
                _text = dir.ToString();
            }
            else if (viewport is Viewport3D)
            {
                _text = "Textured";
            }
        }

        public void Render2D()
        {
            if (_rect.IsEmpty)
            {
                var te = _printer.Measure(_text, SystemFonts.MessageBoxFont, new RectangleF(0, 0, Viewport.Width, Viewport.Height));
                _rect = te.BoundingBox;
                _rect.X += 5;
                _rect.Y += 2;
                _rect.Width += 5;
                _rect.Height += 2;
            }

            GL.Disable(EnableCap.CullFace);

            _printer.Begin();
            if (_showing)
            {
                GL.Begin(BeginMode.Quads);
                GL.Color3(Viewport is Viewport3D ? View.ViewportBackground : Grid.Background);
                GL.Vertex2(0, 0);
                GL.Vertex2(_rect.Right, 0);
                GL.Vertex2(_rect.Right, _rect.Bottom);
                GL.Vertex2(0, _rect.Bottom);
                GL.End();
            }
            _printer.Print(_text, SystemFonts.MessageBoxFont, _showing ? Color.White : Grid.GridLines, _rect);
            _printer.End();

            GL.Enable(EnableCap.CullFace);
        }

        public void Dispose()
        {
            _printer.Dispose();
        }

        public void KeyUp(ViewportEvent e)
        {

        }

        public void KeyDown(ViewportEvent e)
        {

        }

        public void KeyPress(ViewportEvent e)
        {

        }

        public void MouseMove(ViewportEvent e)
        {
            if (_rect.IsEmpty) return;
            _showing = _rect.Contains(e.X, e.Y);
        }

        public void MouseWheel(ViewportEvent e)
        {

        }

        public void MouseUp(ViewportEvent e)
        {

        }

        public void MouseDown(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {

        }

        public void MouseLeave(ViewportEvent e)
        {
            _showing = false;
        }

        public void UpdateFrame()
        {

        }

        public void PreRender()
        {

        }

        public void Render3D()
        {

        }
    }
}
