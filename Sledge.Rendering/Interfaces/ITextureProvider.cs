using System.Collections;
using System.Collections.Generic;

namespace Sledge.Rendering.Interfaces
{
    public interface ITextureProvider
    {
        bool Exists(string name);
        
        void Request(IEnumerable<string> names);
        IEnumerable<TextureDetails> PopRequestedTextures(int count);
    }
}