namespace Sledge.Gui.Interfaces
{
    public struct Padding
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Padding(int top, int left, int bottom, int right) : this()
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }
    }
}