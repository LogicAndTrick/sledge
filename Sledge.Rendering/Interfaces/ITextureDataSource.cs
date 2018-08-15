using System.Threading.Tasks;

namespace Sledge.Rendering.Interfaces
{
    public interface ITextureDataSource
    {
        int Width { get; }
        int Height { get; }
        Task<byte[]> GetData();
    }
}