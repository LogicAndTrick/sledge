using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using OpenTK.Input;

namespace Sledge.Gui.WinForms
{
    internal static class WinFormsOpenTkKeyMap
    {
        private static readonly Dictionary<Keys, Key> Mappings = new Dictionary<Keys, Key>
        {
            {Keys.ControlKey, Key.ControlLeft},
            {Keys.LControlKey, Key.ControlLeft},
            {Keys.RControlKey, Key.ControlRight},

            {Keys.ShiftKey, Key.ShiftLeft},
            {Keys.LShiftKey, Key.ShiftLeft},
            {Keys.RShiftKey, Key.ShiftRight},

            {Keys.Return, Key.Enter},
            {Keys.PageUp, Key.PageUp},
            {Keys.PageDown, Key.PageDown},

            {Keys.CapsLock, Key.CapsLock},
            {Keys.Scroll, Key.ScrollLock},

            {Keys.OemOpenBrackets, Key.BracketLeft},
            {Keys.OemCloseBrackets, Key.BracketRight},

            {Keys.Add, Key.Plus},
            {Keys.Oemplus, Key.Plus},
            {Keys.Subtract, Key.Minus},
            {Keys.OemMinus, Key.Minus},

            {Keys.OemSemicolon, Key.Semicolon},
            {Keys.OemQuotes, Key.Quote},
            {Keys.Oemcomma, Key.Comma},
            {Keys.OemPeriod, Key.Period},
            {Keys.OemBackslash, Key.BackSlash},
            {Keys.OemQuestion, Key.Slash},
            {Keys.PrintScreen, Key.PrintScreen},
            {Keys.Oemtilde, Key.Tilde},
        };

        static WinFormsOpenTkKeyMap()
        {
            Key ok;
            for (var i = 0; i <= 9; i++)
            {
                Keys wfk;

                var name1 = "NumPad" + i.ToString(CultureInfo.InvariantCulture);
                var name2 = "Keypad" + i.ToString(CultureInfo.InvariantCulture);
                if (Enum.TryParse(name1, out wfk) && Enum.TryParse(name2, out ok)) Mappings.Add(wfk, ok);

                name1 = "D" + i.ToString(CultureInfo.InvariantCulture);
                name2 = "Number" + i.ToString(CultureInfo.InvariantCulture);
                if (Enum.TryParse(name1, out wfk) && Enum.TryParse(name2, out ok)) Mappings.Add(wfk, ok);
            }

            foreach (Keys k in Enum.GetValues(typeof (Keys)))
            {
                if (!Mappings.ContainsKey(k) && Enum.TryParse(k.ToString(), true, out ok))
                {
                    Mappings.Add(k, ok);
                }
            }
        }

        public static Key Map(Keys key)
        {
            key = (key & ~Keys.Modifiers);
            return !Mappings.ContainsKey(key) ? Key.Unknown : Mappings[key];
        }
    }
}