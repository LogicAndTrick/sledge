using System;
using System.Collections.Generic;
using Gdk;

namespace Sledge.UI
{
    internal static class GtkOpenTkKeyMap
    {
        private static readonly Dictionary<Key, OpenTK.Input.Key> Mappings = new Dictionary<Gdk.Key, OpenTK.Input.Key>
        {
            {Gdk.Key.Control_L, OpenTK.Input.Key.ControlLeft},
            {Gdk.Key.Control_R, OpenTK.Input.Key.ControlRight},
            {Gdk.Key.Shift_L, OpenTK.Input.Key.ShiftLeft},
            {Gdk.Key.Shift_R, OpenTK.Input.Key.ShiftRight},
            {Gdk.Key.Alt_L, OpenTK.Input.Key.AltLeft},
            {Gdk.Key.Alt_R, OpenTK.Input.Key.AltRight},
            {Gdk.Key.Meta_L, OpenTK.Input.Key.WinLeft},
            {Gdk.Key.Meta_R, OpenTK.Input.Key.WinRight},

            {Gdk.Key.Caps_Lock, OpenTK.Input.Key.CapsLock},
            {Gdk.Key.Num_Lock, OpenTK.Input.Key.NumLock},
            {Gdk.Key.Scroll_Lock, OpenTK.Input.Key.ScrollLock},
            {Gdk.Key.KP_Divide, OpenTK.Input.Key.KeypadDivide},
            {Gdk.Key.KP_Multiply, OpenTK.Input.Key.KeypadMultiply},
            {Gdk.Key.KP_Enter, OpenTK.Input.Key.KeypadEnter},

            {Gdk.Key.Return, OpenTK.Input.Key.Enter},
            {Gdk.Key.Page_Up, OpenTK.Input.Key.PageUp},
            {Gdk.Key.Page_Down, OpenTK.Input.Key.PageDown},
            {Gdk.Key.BackSpace, OpenTK.Input.Key.BackSpace},
            {Gdk.Key.Print, OpenTK.Input.Key.PrintScreen},
            {Gdk.Key.KP_Subtract, OpenTK.Input.Key.KeypadSubtract},
            {Gdk.Key.KP_Add, OpenTK.Input.Key.KeypadAdd},
            {Gdk.Key.KP_Decimal, OpenTK.Input.Key.KeypadDecimal},
            {Gdk.Key.grave, OpenTK.Input.Key.Grave},
            {Gdk.Key.bracketleft, OpenTK.Input.Key.BracketLeft},
            {Gdk.Key.bracketright, OpenTK.Input.Key.BracketRight},
            {Gdk.Key.apostrophe, OpenTK.Input.Key.Quote},

            {Gdk.Key.VoidSymbol, OpenTK.Input.Key.Unknown},
        };

        static GtkOpenTkKeyMap()
        {
            Gdk.Key gk;
            OpenTK.Input.Key ok;
            for (var i = 0; i <= 9; i++)
            {
                var name1 = "KP_" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var name2 = "Keypad" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (Enum.TryParse(name1, out gk) && Enum.TryParse(name2, out ok)) Mappings.Add(gk, ok);

                name1 = "Key_" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                name2 = "Number" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (Enum.TryParse(name1, out gk) && Enum.TryParse(name2, out ok)) Mappings.Add(gk, ok);
            }
            for (var i = 11; i <= 35; i++)
            {
                var name = "F" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (Enum.TryParse(name, out gk) && Enum.TryParse(name, out ok)) Mappings.Add(gk, ok);
            }

            foreach (Gdk.Key k in Enum.GetValues(typeof(Gdk.Key)))
            {
                if (!Mappings.ContainsKey(k) && Enum.TryParse(k.ToString(), true, out ok))
                {
                    Mappings.Add(k, ok);
                }
            }
        }

        public static OpenTK.Input.Key Map(Gdk.Key key)
        {
            return !Mappings.ContainsKey(key) ? OpenTK.Input.Key.Unknown : Mappings[key];
        }
    }
}