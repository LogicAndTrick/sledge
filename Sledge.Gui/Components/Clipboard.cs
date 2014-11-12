using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Gui.Interfaces.Components;

namespace Sledge.Gui.Components
{
    public static class Clipboard
    {
        private static readonly IClipboard Instance;

        static Clipboard()
        {
            Instance = UIManager.Manager.ConstructComponent<IClipboard>();
        }

        public static void SetText(string text)
        {
            Instance.SetText(text);
        }

        public static string GetText()
        {
            return Instance.GetText();
        }

        public static bool HasText()
        {
            return Instance.HasText();
        }
    }
}
