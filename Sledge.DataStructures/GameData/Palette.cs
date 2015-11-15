namespace Sledge.DataStructures.GameData
{
    public class Palette
    {
        public byte[] ByteArray { get; private set; }

        public Palette(byte[] pal)
        {
            ByteArray = pal;
        }
    }
}
