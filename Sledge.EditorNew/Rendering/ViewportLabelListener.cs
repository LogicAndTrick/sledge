using System;
using System.Drawing;
using OpenTK.Graphics;
using Sledge.Common.Mediator;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Structures;
using Sledge.Settings;
using Point = System.Drawing.Point;

namespace Sledge.EditorNew.Rendering
{
    // ReSharper disable CSharpWarnings::CS0612
    // OpenTK's TextPrinter is marked as obsolete but no suitable replacement exists
    public class ViewportLabelListener : IViewportEventListener, IDisposable
    {
        public IMapViewport Viewport { get; set; }
        private TextPrinter _printer;
        private RectangleF _rect;
        private string _text;
        private bool _showing;
        private ContextMenu _menu;

        public ViewportLabelListener(IMapViewport viewport)
        {
            Viewport = viewport;
            _printer = new TextPrinter(TextQuality.Low);
            Rebuild();
        }

        private void Rebuild()
        {
            _rect = RectangleF.Empty;
            _text = "";
            if (Viewport.Is2D)
            {
                var dir = ((IViewport2D)Viewport).Direction;
                _text = "";
                switch (dir)
                {
                    case ViewDirection.Top:
                        _text = "Top (x/y)";
                        break;
                    case ViewDirection.Front:
                        _text = "Front (y/z)";
                        break;
                    case ViewDirection.Side:
                        _text = "Side (x/z)";
                        break;
                }
            }
            else if (Viewport.Is3D)
            {
                var type = ((IViewport3D) Viewport).Type;
                _text = type.ToString();
            }
            if (_menu != null) _menu.Dispose();
            _menu = new ContextMenu(new[]
                                        {
                                            CreateMenu("3D Textured", ViewType.Textured, null),
                                            CreateMenu("3D Shaded", ViewType.Shaded, null),
                                            CreateMenu("3D Flat", ViewType.Flat, null),
                                            CreateMenu("3D Wireframe", ViewType.Wireframe, null),
                                            new MenuItem("-"), 
                                            CreateMenu("2D Top (x/y)", null, ViewDirection.Top),
                                            CreateMenu("2D Side (x/z)", null, ViewDirection.Side),
                                            CreateMenu("2D Front (y/z)", null, ViewDirection.Front),
                                            new MenuItem("-"), 
                                            ScreenshotMenuItem()
                                        });
        }

        private MenuItem ScreenshotMenuItem()
        {
            var menu = new MenuItem("Take Screenshot...");
            menu.Click += (s, e) => Mediator.Publish(HotkeysMediator.ScreenshotViewport, Viewport);
            return menu;
        }

        private MenuItem CreateMenu(string text, ViewType? type, ViewDirection? dir)
        {
            var menu = new MenuItem(text);
            menu.Click += (sender, e) => SwitchType(type, dir);
            if (dir.HasValue && Viewport.Is2D)
            {
                var vpdir = ((IViewport2D) Viewport).Direction;
                menu.Checked = vpdir == dir.Value;
            }
            else if (type.HasValue && Viewport.Is3D)
            {
                var vptype = ((IViewport3D) Viewport).Type;
                menu.Checked = vptype == type.Value;
            }
            return menu;
        }

        private void SwitchType(ViewType? type,  ViewDirection? dir)
        {
            var doc = DocumentManager.CurrentDocument;
            if (doc == null) return;
            if (type.HasValue)
            {
                if (!Viewport.Is3D)
                {
                    doc.Make3D(Viewport, type.Value);
                    return;
                }
                else
                {
                    ((IViewport3D)Viewport).Type = type.Value;
                }
            }
            else if (dir.HasValue)
            {
                if (!Viewport.Is2D)
                {
                    doc.Make2D(Viewport, dir.Value);
                    return;
                }
                else
                {
                    ((IViewport2D)Viewport).Direction = dir.Value;
                }
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
                GL.Color3(Viewport.Is3D ? View.ViewportBackground : Grid.Background);
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

        public void PostRender()
        {
            // 
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

        public void MouseClick(ViewportEvent e)
        {
            
        }

        public void MouseDoubleClick(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {

        }

        public void MouseLeave(ViewportEvent e)
        {
            _showing = false;
        }

        public void UpdateFrame(Frame frame)
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
