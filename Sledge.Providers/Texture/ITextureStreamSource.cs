using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sledge.Providers.Texture
{
    public interface ITextureStreamSource : IDisposable
    {
        bool HasImage(TextureItem item);
        Bitmap GetImage(TextureItem item);
    }

    public class MultiTextureStreamSource : ITextureStreamSource
    {
        private readonly List<ITextureStreamSource> _streams;

        public MultiTextureStreamSource(IEnumerable<ITextureStreamSource> streams)
        {
            _streams = streams.ToList();
        }

        public bool HasImage(TextureItem item)
        {
            return _streams.Any(x => x.HasImage(item));
        }

        public Bitmap GetImage(TextureItem item)
        {
            foreach (var ss in _streams)
            {
                if (ss.HasImage(item)) return ss.GetImage(item);
            }
            return null;
        }

        public void Dispose()
        {
            _streams.ForEach(x => x.Dispose());
        }
    }
}