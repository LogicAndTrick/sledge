using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sledge.Gui.Interfaces.Shell
{
    public interface IMenuItem : IDisposable
    {
        string TextKey { get; set; }
        string Text { get; set; }
        Image Icon { set; }
        IList<IMenuItem> SubItems { get; }
        event EventHandler Clicked;
        IMenuItem AddSubMenuItem(string key, string text = null);
    }
}