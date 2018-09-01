using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Sledge.Providers.Texture
{
    public class MultiTextureStreamSource : ITextureStreamSource
    {
        private readonly List<ITextureStreamSource> _streams;

        public MultiTextureStreamSource(IEnumerable<ITextureStreamSource> streams)
        {
            _streams = streams.ToList();
            _streams.Add(new NullTextureStreamSource());
        }

        public bool HasImage(string item)
        {
            return _streams.Any(x => x.HasImage(item));
        }

        public Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
        {
            return _streams.FirstOrDefault(x => x.HasImage(item))?.GetImage(item, maxWidth, maxHeight);
        }

        public void Dispose()
        {
            _streams.ForEach(x => x.Dispose());
        }
    }
}