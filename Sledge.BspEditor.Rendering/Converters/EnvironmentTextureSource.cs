using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.Converters
{
    public class EnvironmentTextureSource : ITextureDataSource
    {
        public int Width => _item.Width;
        public int Height => _item.Height;

        private readonly TextureCollection _textureCollection;
        private readonly TextureItem _item;

        public EnvironmentTextureSource(IEnvironment environment, string name)
        {
            _textureCollection = environment.GetTextureCollection().Result;
            _item = _textureCollection.GetTextureItem(name).Result;
        }

        public byte[] GetData()
        {
            using (var bitmap = _textureCollection.GetStreamSource().GetImage(_item.Name, 512, 512).Result)
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