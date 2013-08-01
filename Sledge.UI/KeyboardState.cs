using System.Collections.Generic;
using System.Linq;
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

        private static readonly Dictionary<string, string> KeyStringReplacements;

        static KeyboardState()
        {
            KeyStringReplacements = new Dictionary<string, string>
                                        {
                                            {"Add", "+"},
                                            {"Oemplus", "+"},
                                            {"Subtract", "-"},
                                            {"OemMinus", "-"},
                                            {"Separator", "-"},
                                            {"Decimal", "."},
                                            {"OemPeriod", "."},
                                            {"Divide", "/"},
                                            {"OemQuestion", "/"},
                                            {"Multiply", "*"},
                                            {"OemBackslash", "\\"},
                                            {"Oem5", "\\"},
                                            {"OemCloseBrackets", "]"},
                                            {"Oem6", "]"},
                                            {"OemOpenBrackets", "["},
                                            {"OemPipe", "|"},
                                            {"OemQuotes", "'"},
                                            {"Oem7", "'"},
                                            {"OemSemicolon", ";"},
                                            {"Oem1", ";"},
                                            {"Oemcomma", ","},
                                            {"Oemtilde", "`"},
                                            {"Back", "Backspace"}
                                        };
        }

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

        public static bool IsAnyKeyDown(params Keys[] keys)
        {
            return keys.Any(IsKeyDown);
        }

        private static bool IsKeyToggled(Keys key)
        {
            // Key is toggled if the low bit is 1
            return (GetKeyState((int) key) & 0x0001) == 0x0001;
        }

        public static string KeysToString(Keys key)
        {
            var kc = new KeysConverter();
            var str = kc.ConvertToString(key) ?? "";
            foreach (var rep in KeyStringReplacements) str = str.Replace(rep.Key, rep.Value);
            return str;
        }
    }
}
