using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Editor.Documents;
using Sledge.Rendering.Interfaces;

namespace Sledge.Editor.Rendering
{
    public class DefaultTextureProvider : ITextureProvider
    {
        public Document Document { get; private set; }

        public DefaultTextureProvider(Document document)
        {
            Document = document;
        }
        public bool Exists(string name)
        {
            return Document.TextureCollection.HasTexture(name);
        }

        private readonly ConcurrentQueue<TextureDetails> _textureQueue = new ConcurrentQueue<TextureDetails>();

        public async void Request(IEnumerable<string> names)
        {
            var n = names.ToList();
            await Document.TextureCollection.Precache(n);
            var items = await Document.TextureCollection.GetTextureItems(n);
            using (var ss = await Document.TextureCollection.GetStreamSource())
            {
                foreach (var item in items)
                {
                    var td = new TextureDetails(item.Name, await ss.GetImage(item.Name, 1024, 1024), item.Width, item.Height, item.Flags);
                    _textureQueue.Enqueue(td);
                }
            }
        }

        public IEnumerable<TextureDetails> PopRequestedTextures(int count)
        {
            for (var i = 0; i < count; i++)
            {
                TextureDetails td;
                if (_textureQueue.TryDequeue(out td)) yield return td;
                else break;
            }
        }
    }
}