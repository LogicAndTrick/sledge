namespace Sledge.Gui.Structures
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }
}