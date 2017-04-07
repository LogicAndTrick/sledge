using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.Resources
{
    public class EnvironmentTextureProvider : IEnvironmentData, ITextureProvider
    {
        public IEnvironment Environment { get; set; }
        private readonly int _idLen;

        private TextureCollection _textureCollection;
        private readonly ConcurrentQueue<TextureDetails> _textureQueue = new ConcurrentQueue<TextureDetails>();

        public EnvironmentTextureProvider(IEnvironment environment)
        {
            Environment = environment;
            _idLen = environment.ID.Length + 2;
        }

        public async Task Init()
        {
            _textureCollection = await Environment.GetTextureCollection();
        }

        public bool Exists(string name)
        {
            return name.Length > _idLen
                && name.Substring(0, _idLen - 2) == Environment.ID
                && _textureCollection.HasTexture(name.Substring(_idLen));
        }

        public async void Request(IEnumerable<string> names)
        {
            var n = names.Where(x => x.Length > _idLen && x.Substring(0, _idLen - 2) == Environment.ID).Select(x => x.Substring(_idLen)).ToList();
            await _textureCollection.Precache(n);
            var items = await _textureCollection.GetTextureItems(n);
            using (var ss = await _textureCollection.GetStreamSource())
            {
                foreach (var item in items)
                {
                    var td = new TextureDetails($"{Environment.ID}::{item.Name}", await ss.GetImage(item.Name, 1024, 1024), item.Width, item.Height, item.Flags);
                    _textureQueue.Enqueue(td);
                }
            }
        }

        public IEnumerable<TextureDetails> PopRequestedTextures(int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (_textureQueue.TryDequeue(out TextureDetails td))
                {
                    yield return td;
                }
                else break;
            }
        }
    }
}
