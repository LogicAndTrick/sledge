using System;
using System.Drawing;

namespace Sledge.Common
{
    public interface ITexture : IDisposable
    {
        string Name { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Bitmap BitmapImage { get; set; }
        void Bind();
    }
}