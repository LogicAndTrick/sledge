namespace Sledge.Rendering.Interfaces
{
    public interface ITextureDataSource
    {
        int Width { get; }
        int Height { get; }
        byte[] GetData();
    }
}