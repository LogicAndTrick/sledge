using System;
using System.Drawing;

namespace Sledge.Common
{
    public interface ITexture : IDisposable
    {
        TextureFlags Flags { get; }
        string Name { get; }
        int Width { get; }
        int Height { get; }
        void Bind();
        void Unbind();
    }

    public static class TextureExtensions
    {
        public static bool HasTransparency(this ITexture texture)
        {
            return texture.Flags.HasFlag(TextureFlags.Transparent);
        }
    }
}