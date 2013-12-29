using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using Sledge.Editor.Documents;
using Sledge.Editor.UI;
using Sledge.Graphics.Helpers;
using Sledge.Settings;
using Sledge.UI;
using View = Sledge.Settings.View;

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
        private ContextMenu _menu;

        public ViewportLabelListener(ViewportBase viewport)
        {
            Viewport = viewport;
            _printer = new TextPrinter(TextQuality.Low);
            Rebuild();
        }

        private void Rebuild()
        {
            _rect = RectangleF.Empty;
            _text = "";
            if (Viewport is Viewport2D)
            {
                var dir = ((Viewport2D)Viewport).Direction;
                _text = dir.ToString();
            }
            else if (Viewport is Viewport3D)
            {
                var type = ((Viewport3D) Viewport).Type;
                _text = type.ToString();
            }
            if (_menu != null) _menu.Dispose();
            _menu = new ContextMenu(new[]
                                        {
                                            CreateMenu("3D Textured", Viewport3D.ViewType.Textured, null),
                                            CreateMenu("3D Shaded", Viewport3D.ViewType.Shaded, null),
                                            CreateMenu("3D Flat", Viewport3D.ViewType.Flat, null),
                                            CreateMenu("3D Wireframe", Viewport3D.ViewType.Wireframe, null),
                                            new MenuItem("-"), 
                                            CreateMenu("2D Top", null, Viewport2D.ViewDirection.Top),
                                            CreateMenu("2D Side", null, Viewport2D.ViewDirection.Side),
                                            CreateMenu("2D Front", null, Viewport2D.ViewDirection.Front),
                                        });
        }

        private MenuItem CreateMenu(string text, Viewport3D.ViewType? type, Viewport2D.ViewDirection? dir)
        {
            var menu = new MenuItem(text);
            menu.Click += (sender, e) => SwitchType(type, dir);
            if (dir.HasValue && Viewport is Viewport2D)
            {
                var vpdir = ((Viewport2D) Viewport).Direction;
                menu.Checked = vpdir == dir.Value;
            }
            else if (type.HasValue && Viewport is Viewport3D)
            {
                var vptype = ((Viewport3D) Viewport).Type;
                menu.Checked = vptype == type.Value;
            }
            return menu;
        }

        private void SwitchType(Viewport3D.ViewType? type,  Viewport2D.ViewDirection? dir)
        {
            var doc = DocumentManager.CurrentDocument;
            if (doc == null) return;
            if (type.HasValue)
            {
                var vp = Viewport as Viewport3D;
                if (vp == null)
                {
                    doc.Make3D(Viewport, type.Value);
                    return;
                }
                vp.Type = type.Value;
            }
            else if (dir.HasValue)
            {
                var vp = Viewport as Viewport2D;
                if (vp == null)
                {
                    doc.Make2D(Viewport, dir.Value);
                    return;
                }
                vp.Direction = dir.Value;
            }
            Rebuild();
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
            if (_showing)
            {
                _menu.Show(Viewport, new Point(e.X, e.Y));
                e.Handled = true;
            }
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
