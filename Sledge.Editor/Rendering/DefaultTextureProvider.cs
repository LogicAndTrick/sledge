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
            return Document.TextureCollection.GetItem(name) != null;
        }

        public TextureDetails Fetch(string name)
        {
            return Fetch(new[] {name}).FirstOrDefault();
        }

        public IEnumerable<TextureDetails> Fetch(IEnumerable<string> names)
        {
            using (var ss = Document.TextureCollection.GetStreamSource(1024, 1024))
            {
                foreach (var item in names.Select(name => Document.TextureCollection.GetItem(name)).Where(item => item != null))
                {
                    yield return new TextureDetails(item.Name, ss.GetImage(item), item.Width, item.Height, item.Flags);
                }
            }
        }

        private readonly ConcurrentQueue<TextureDetails> _textureQueue = new ConcurrentQueue<TextureDetails>();

        public void Request(IEnumerable<string> names)
        {
            Task.Factory.StartNew(() =>
            {
                using (var ss = Document.TextureCollection.GetStreamSource(1024, 1024))
                {
                    foreach (var item in names.Select(name => Document.TextureCollection.GetItem(name)).Where(item => item != null))
                    {
                        var td = new TextureDetails(item.Name, ss.GetImage(item), item.Width, item.Height, item.Flags);
                        _textureQueue.Enqueue(td);
                    }
                }
            });
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