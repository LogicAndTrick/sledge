using System;
using System.Reflection;
using System.Windows.Forms;
using Sledge.UI;

namespace Sledge.Editor.UI
{
    public static class Hotkeys
    {
        public static bool HotkeyDown(Editor editor, Keys keyData)
        {
            var keyCombination = KeyboardState.KeysToString(keyData);
            var hotkeyDefinition = Sledge.Settings.Hotkeys.GetHotkeyFor(keyCombination);
            if (hotkeyDefinition == null) return false;

            var action = hotkeyDefinition.Action;
            var method = typeof (Hotkeys).GetMethod(action, BindingFlags.Static | BindingFlags.Public);
            if (method == null) return false;

            method.Invoke(null, new object[] { editor });
            return true;
        }

        public static void FourViewAutosize(Editor editor)
        {
            editor.GetFourView().ResetViews();
        }

        public static void FourViewFocusTopLeft(Editor editor)
        {
            editor.GetFourView().FocusOn(0, 0);
        }

        public static void FourViewFocusTopRight(Editor editor)
        {
            editor.GetFourView().FocusOn(0, 1);
        }

        public static void FourViewFocusBottomLeft(Editor editor)
        {
            editor.GetFourView().FocusOn(1, 0);
        }

        public static void FourViewFocusBottomRight(Editor editor)
        {
            editor.GetFourView().FocusOn(1, 1);
        }

        public static void HistoryUndo(Editor editor)
        {
            Document.Undo();
        }

        public static void HistoryRedo(Editor editor)
        {
            Document.Redo();
        }
    }
}
