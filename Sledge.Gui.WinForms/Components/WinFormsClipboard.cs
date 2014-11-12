using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Components;

namespace Sledge.Gui.WinForms.Components
{
    [ControlImplementation("WinForms")]
    public class WinFormsClipboard : IClipboard
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }

        public string GetText()
        {
            return Clipboard.GetText();
        }

        public bool HasText()
        {
            return Clipboard.ContainsText();
        }
    }
}
