using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture
{
    public interface ITextureStreamSource : IDisposable
    {
        bool HasImage(string item);
        Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight);
    }
}