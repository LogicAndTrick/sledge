using System.Drawing;
using System.Threading.Tasks;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Wad.Format;

namespace Sledge.Providers.Texture.Wad
{
    public class WadStreamSource : ITextureStreamSource
    {
        private readonly WadPackageStreamSource _stream;
        private readonly WadPackage _package;

        public WadStreamSource(IFile file)
        {
            _package = new WadPackage(file);
            _stream = new WadPackageStreamSource(_package);
        }

        public bool HasImage(string item)
        {
            return _stream.HasEntry(item);
        }

        public async Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
        {
            var entry = _stream.GetEntry(item);
            if (entry == null) return null;

            return await Task.Factory.StartNew(() =>
            {
                using (var s = _stream.OpenEntry(entry))
                {
                    return new Bitmap(s);
                }
            });
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}