namespace Sledge.Gui.Structures
{
    public class Frame
    {
        public long Milliseconds { get; private set; }

        public Frame(long milliseconds)
        {
            Milliseconds = milliseconds;
        }
    }
}