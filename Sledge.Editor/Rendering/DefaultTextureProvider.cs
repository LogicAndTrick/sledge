using System.Collections.Generic;
using System.Linq;
using Sledge.Editor.Documents;
using Sledge.Rendering.Interfaces;

namespace Sledge.Editor.Rendering
{
    // todo this method of fetching textures kinda sucks
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
    }
}