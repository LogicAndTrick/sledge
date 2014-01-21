using System;
using System.Drawing;

namespace Sledge.Editor.UI.DockPanels
{
    public class OutputWord
    {
        public ConsoleColor Colour { get; set; }
        public string Text { get; set; }

        public OutputWord()
        {
            Colour = ConsoleColor.Black;
            Text = "";
        }

        public Color GetColour()
        {
            var c = Color.FromName(Colour.ToString());
            return c.IsEmpty ? Color.Black : c;
        }
    }
}
