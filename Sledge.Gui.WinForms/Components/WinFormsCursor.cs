using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Components;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Components;
using Sledge.Gui.WinForms.Controls;

namespace Sledge.Gui.WinForms.Components
{

    /*
    // GTK mappings
    Arrow  // default/lptr
    Cross  // crosshair
    Default  // default
    IBeam // xterm
    No // x_cursor
    SizeAll // fleur
    SizeNESW // top_right_corner + bottom_left_corner
    SizeNS // top_side + bottom_side
    SizeNWSE // top_left_corner + bottom_right_corner
    SizeWE // left_side + right_side
    UpArrow // center_ptr
    WaitCursor // watch
    Help // question_arrow
    HSplit // sb_v_double_arrow
    VSplit // sb_h_double_arrow
    Hand // hand2
     */
    [ControlImplementation("WinForms")]
    public class WinFormsCursor : ICursor
    {
        private bool _hidden = false;

        public void SetCursor(IControl control, CursorType type)
        {
            var wfc = control.Implementation as WinFormsControl;
            if (wfc != null)
            {
                var ty = MapCursorType(type);
                wfc.Control.Cursor = ty;
            }
        }

        private System.Windows.Forms.Cursor MapCursorType(CursorType type)
        {
            switch (type)
            {
                case CursorType.Default:
                    return Cursors.Default;
                case CursorType.Pointer:
                    return Cursors.Hand;
                case CursorType.Crosshair:
                    return Cursors.Cross;
                case CursorType.IBeam:
                    return Cursors.IBeam;
                case CursorType.No:
                    return Cursors.No;
                case CursorType.Up:
                    return Cursors.UpArrow;
                case CursorType.Wait:
                    return Cursors.WaitCursor;
                case CursorType.Help:
                    return Cursors.Help;
                case CursorType.SizeAll:
                    return Cursors.SizeAll;
                case CursorType.SizeTop:
                case CursorType.SizeBottom:
                    return Cursors.SizeNS;
                case CursorType.SizeTopRight:
                case CursorType.SizeBottomLeft:
                    return Cursors.SizeNESW;
                case CursorType.SizeRight:
                case CursorType.SizeLeft:
                    return Cursors.SizeWE;
                case CursorType.SizeBottomRight:
                case CursorType.SizeTopLeft:
                    return Cursors.SizeNWSE;
                case CursorType.HSplit:
                    return Cursors.HSplit;
                case CursorType.VSplit:
                    return Cursors.VSplit;
                case CursorType.Custom:
                case CursorType.None:
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public void SetCursor(IControl control, Image cursorImage)
        {
            var wfc = control.Implementation as WinFormsControl;
            if (wfc != null)
            {
                throw new NotImplementedException(); // todo
            }
        }

        public void HideCursor(IControl control)
        {
            if (_hidden) return;
            System.Windows.Forms.Cursor.Hide();
            _hidden = true;
        }

        public void ShowCursor(IControl control)
        {
            if (!_hidden) return;
            System.Windows.Forms.Cursor.Show();
            _hidden = false;
        }
    }
}