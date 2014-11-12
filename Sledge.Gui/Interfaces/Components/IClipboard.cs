using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Components
{
    [ControlInterface]
    public interface IClipboard
    {
        void SetText(string text);
        string GetText();
        bool HasText();
    }
}
