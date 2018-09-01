using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sledge.Shell.Input
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
                                            {"Back", "Backspace"},
                                            {"Return", "Enter"},
                                            {"Next", "PageDown"},
                                            {"Prior", "PageUp"},
                                            {"D1", "1"},
                                            {"D2", "2"},
                                            {"D3", "3"},
                                            {"D4", "4"},
                                            {"D5", "5"},
                                            {"D6", "6"},
                                            {"D7", "7"},
                                            {"D8", "8"},
                                            {"D9", "9"},
                                            {"D0", "0"},
                                            {"Delete", "Del"}
                                        };
        }

        public static bool Ctrl => IsModifierKeyDown(Keys.Control);
        public static bool Shift => IsModifierKeyDown(Keys.Shift);
        public static bool Alt => IsModifierKeyDown(Keys.Alt);
        public static bool CapsLocked => IsKeyToggled(Keys.CapsLock);
        public static bool ScrollLocked => IsKeyToggled(Keys.Scroll);
        public static bool NumLocked => IsKeyToggled(Keys.NumLock);

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
            // KeysConverter seems to ignore the invariant culture, manually replicate the results
            var mods = key & Keys.Modifiers;
            var keycode = key & Keys.KeyCode;
            if (keycode == Keys.None) return "";

            var str = keycode.ToString();
            if (KeyStringReplacements.ContainsKey(str)) str = KeyStringReplacements[str];

            // Modifier order: Ctrl+Alt+Shift+Key
            return (mods.HasFlag(Keys.Control) ? "Ctrl+" : "")
                   + (mods.HasFlag(Keys.Alt) ? "Alt+" : "")
                   + (mods.HasFlag(Keys.Shift) ? "Shift+" : "")
                   + str;
        }
    }
}
