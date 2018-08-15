using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Resources
{
    public class WhiteTextureSource : ITextureDataSource
    {
        public int Width => 1;
        public int Height => 1;

        public byte[] GetData()
        {
            return new byte[] {255, 255, 255, 255};
        }
    }
}