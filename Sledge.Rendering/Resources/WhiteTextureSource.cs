using System.Threading.Tasks;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Resources
{
    public class WhiteTextureSource : ITextureDataSource
    {
        public int Width => 1;
        public int Height => 1;

        public Task<byte[]> GetData()
        {
            return Task.FromResult(new byte[] {255, 255, 255, 255});
        }
    }
}