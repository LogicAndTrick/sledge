using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.Resources
{
    public class EnvironmentTextureSource : ITextureDataSource
    {
        private readonly TextureItem _item;
        private readonly IEnvironment _environment;

        public TextureSampleType SampleType => TextureSampleType.Standard;
        public int Width => _item.Width;
        public int Height => _item.Height;

        public EnvironmentTextureSource(IEnvironment environment, TextureItem item)
        {
            _environment = environment;
            _item = item;
        }

        public async Task<byte[]> GetData()
        {
            var textureCollection = await _environment.GetTextureCollection();

            using (var bitmap = await textureCollection.GetStreamSource().GetImage(_item.Name, 512, 512))
            {
                var lb = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var data = new byte[lb.Stride * lb.Height];
                Marshal.Copy(lb.Scan0, data, 0, data.Length);
                bitmap.UnlockBits(lb);
                return data;
            }
        }
    }
}