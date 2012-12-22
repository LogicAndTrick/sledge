using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sledge.UI
{
    /// <summary>
    /// Performs polling on the current keyboard state.
    /// </summary>
    /// Most of this is adapted from:
    /// http://www.switchonthecode.com/tutorials/winforms-accessing-mouse-and-keyboard-state
    public static class KeyboardState
    {

        public static bool Ctrl
        {
            get { return IsModifierKeyDown(Keys.Control); }
        }

        public static bool Shift
        {
            get { return IsModifierKeyDown(Keys.Shift); }
        }

        public static bool Alt
        {
            get { return IsModifierKeyDown(Keys.Alt); }
        }

        public static bool CapsLocked
        {
            get { return IsKeyToggled(Keys.CapsLock); }
        }

        public static bool ScrollLocked
        {
            get { return IsKeyToggled(Keys.Scroll); }
        }

        public static bool NumLocked
        {
            get { return IsKeyToggled(Keys.NumLock); }
        }

        private static bool IsModifierKeyDown(Keys k)
        {
            return (Control.ModifierKeys & k) == k;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        public static bool IsKeyDown(Keys key)
        {
            // Key is down if the high bit is 1
            return (GetKeyState((int)key) & 0x8000) == 0x8000;
        }

        private static bool IsKeyToggled(Keys key)
        {
            // Key is toggled if the low bit is 1
            return (GetKeyState((int) key) & 0x0001) == 0x0001;
        }
    }
}
