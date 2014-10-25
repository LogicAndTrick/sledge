using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sledge.Gui.Interfaces.Shell
{
    public interface IMenuItem : IDisposable
    {
        string Identifier { get; set; }
        string Text { get; set; }
        Bitmap Icon { set; }
        IList<IMenuItem> SubItems { get; }
        event EventHandler Clicked;
        IMenuItem AddSubMenuItem(string identifier, string text);
    }
}