using System.Threading.Tasks;

namespace Sledge.Rendering.Interfaces
{
    public interface ITextureDataSource
    {
        TextureSampleType SampleType { get; }
        int Width { get; }
        int Height { get; }
        Task<byte[]> GetData();
    }

    public enum TextureSampleType
    {
        Standard,
        Point,
    }
}