using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;
using Sledge.Packages;
using Sledge.Packages.Wad;

namespace Sledge.Providers.Texture.Wad
{
    public class WadTextureStreamSourceProvider : SyncResourceProvider<ITextureStreamSource>
    {
        public override bool CanProvide(string location)
        {
            return File.Exists(location) && location.EndsWith(".wad");
        }

        public override IEnumerable<ITextureStreamSource> Fetch(string location, List<string> resources)
        {
            yield return new WadStreamSource(new FileInfo(location));
        }

        private class WadStreamSource : ITextureStreamSource
        {
            private readonly WadPackage _package;
            private readonly IPackageStreamSource _stream;

            public WadStreamSource(FileInfo location)
            {
                _package = new WadPackage(location);
                _stream = _package.GetStreamSource();
            }

            public bool HasImage(string item)
            {
                return _stream.HasFile(item);
            }

            public Task<Bitmap> GetImage(string item, int maxWidth, int maxHeight)
            {
                return Task.Factory.StartNew(() =>
                {
                    using (var s = _stream.OpenFile(item))
                    {
                        return new Bitmap(s);
                    }
                });
            }

            public void Dispose()
            {
                _stream.Dispose();
                _package.Dispose();
            }
        }
    }
}