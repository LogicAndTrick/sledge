using System.Collections;
using System.Collections.Generic;

namespace Sledge.Rendering.Interfaces
{
    public interface ITextureProvider
    {
        bool Exists(string name);

        TextureDetails Fetch(string name);
        IEnumerable<TextureDetails> Fetch(IEnumerable<string> names);

        void Request(IEnumerable<string> names);
        IEnumerable<TextureDetails> PopRequestedTextures(int count);
    }
}