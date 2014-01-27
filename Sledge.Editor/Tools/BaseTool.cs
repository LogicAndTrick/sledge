using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.UI;
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

        protected Coordinate GetNudgeValue(Keys k)
        {
            var ctrl = KeyboardState.Ctrl;
            var gridoff = Select.NudgeStyle == NudgeStyle.GridOffCtrl;
            var grid = (gridoff && !ctrl) || (!gridoff && ctrl);
            var val = grid ? Document.Map.GridSpacing : Select.NudgeUnits;
            switch (k)
            {
                case Keys.Left:
                    return new Coordinate(-val, 0, 0);
                case Keys.Right:
                    return new Coordinate(val, 0, 0);
                case Keys.Up:
                    return new Coordinate(0, val, 0);
                case Keys.Down:
                    return new Coordinate(0, -val, 0);
            }
            return null;
        }

        protected Documents.Document Document { get; set; }
        public ViewportBase Viewport { get; set; }
        public ToolUsage Usage { get; set; }

        public abstract Image GetIcon();
        public abstract string GetName();
        public abstract HotkeyTool? GetHotkeyToolType();

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

        public virtual void ToolSelected(bool preventHistory)
        {
            // Virtual
        }

        public virtual void ToolDeselected(bool preventHistory)
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
        public abstract void MouseClick(ViewportBase viewport, ViewportEvent e);
        public abstract void MouseDoubleClick(ViewportBase viewport, ViewportEvent e);
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

        public virtual void OverrideViewportContextMenu(ViewportContextMenu menu, Viewport2D vp, ViewportEvent e)
        {
            // Default: nothing...
        }
    }

    public enum HotkeyInterceptResult
    {
        Continue,
        Abort,
        SwitchToSelectTool
    }
}
