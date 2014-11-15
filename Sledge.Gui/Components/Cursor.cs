using System.Drawing;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Components;

namespace Sledge.Gui.Components
{
    public static class Cursor
    {
        private static readonly ICursor Instance;

        static Cursor()
        {
            Instance = UIManager.Manager.ConstructComponent<ICursor>();
        }

        public static void SetCursor(IControl control, CursorType type)
        {
            Instance.SetCursor(control, type);
        }

        public static void SetCursor(IControl control, Image cursorImage)
        {
            Instance.SetCursor(control, cursorImage);
        }

        public static void HideCursor(IControl control)
        {
            Instance.HideCursor(control);
        }

        public static void ShowCursor(IControl control)
        {
            Instance.ShowCursor(control);
        }
    }
}