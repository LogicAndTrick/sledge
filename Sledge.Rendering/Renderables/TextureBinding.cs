using System;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public class TextureBinding : IDisposable
    {
        private readonly Texture _texture;

        internal TextureBinding(Texture texture)
        {
            _texture = texture;
            texture.Inc();
        }

        public void BindTo(CommandList cl, uint slot) => _texture.BindTo(cl, slot);

        public void Dispose()
        {
            _texture.Dec();
        }
    }
}