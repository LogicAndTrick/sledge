using System;
using System.Drawing;

namespace Sledge.Common
{
    public interface ITexture : IDisposable
    {
        string Name { get; }
        int Width { get; }
        int Height { get; }
        bool HasTransparency { get; }
        void Bind();
        void Unbind();
    }
}