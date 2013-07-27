using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.Settings;
using Sledge.UI;
using System.Drawing;

namespace Sledge.Editor.Tools
{
    public abstract class BaseTool : IMediatorListener
    {
        public enum ToolUsage
        {
            View2D,
            View3D,
            Both
        }

        protected Coordinate SnapIfNeeded(Coordinate c)
        {
            return Document.Snap(c);
        }

        protected Documents.Document Document { get; set; }
        public ViewportBase Viewport { get; set; }
        public ToolUsage Usage { get; set; }

        public abstract Image GetIcon();
        public abstract string GetName();

        protected BaseTool()
        {
            Viewport = null;
            Usage = ToolUsage.View2D;
        }

        public void SetDocument(Documents.Document document)
        {
            Document = document;
            DocumentChanged();
        }

        public virtual void ToolSelected()
        {
            // Virtual
        }

        public virtual void ToolDeselected()
        {
            // Virtual
        }

        public virtual void DocumentChanged()
        {
            // Virtual
        }

        public virtual void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        public abstract void MouseEnter(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseLeave(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseDown(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseUp(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseWheel(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseMove(ViewportBase viewport, ViewportEvent e);
        public abstract void KeyPress(ViewportBase viewport, ViewportEvent e);
        public abstract void KeyDown(ViewportBase viewport, ViewportEvent e);
        public abstract void KeyUp(ViewportBase viewport, ViewportEvent e);
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

        /// <summary>
        /// Intercepts a document hotkey. Returns false if the hotkey should not be executed.
        /// </summary>
        /// <param name="hotkeyMessage">The hotkey message</param>
        /// <returns>False to prevent execution of the document hotkey</returns>
        public abstract HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage);
    }

    public enum HotkeyInterceptResult
    {
        Continue,
        Abort,
        SwitchToSelectTool
    }
}
