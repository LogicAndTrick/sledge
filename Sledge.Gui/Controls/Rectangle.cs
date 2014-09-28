using System.Security.Policy;

namespace Sledge.Gui.Controls
{
    public struct Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }

        public Rectangle(int x, int y, int width, int height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}