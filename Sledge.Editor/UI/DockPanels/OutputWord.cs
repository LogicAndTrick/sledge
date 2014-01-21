using System;
using System.Drawing;

namespace Sledge.Editor.UI.DockPanels
{
    public class OutputWord
    {
        public ConsoleColor Colour { get; set; }
        public string Text { get; set; }

        public OutputWord(string text)
        {
            Text = text ?? "";
            Colour = ConsoleColor.Black;
        }

        public OutputWord(string text, ConsoleColor color)
        {
            Colour = color;
            Text = text ?? "";
        }

        public Color GetColour()
        {
            var c = Color.FromName(Colour.ToString());
            return c.IsEmpty ? Color.Black : c;
        }
    }
}
