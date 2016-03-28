using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sledge.Editor.Documents;
using Sledge.Providers.Model;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

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

    public class ModelTextureProvider : ITextureProvider
    {
        public Document Document { get; private set; }

        public ModelTextureProvider(Document document)
        {
            Document = document;
        }

        private ModelReference GetModelForTexture(string name)
        {
            if (!name.StartsWith("Model::")) return null;
            name = name.Substring(7);
            var idx = name.IndexOf("::", StringComparison.InvariantCulture);
            if (idx < 0) return null;
            name = name.Substring(0, idx);
            return Document.ModelCollection.GetLoadedModel(name);
        }

        private string GetNameForTexture(string name)
        {
            var idx = name.LastIndexOf("::", StringComparison.InvariantCulture);
            if (idx >= 0) return name.Substring(idx + 2);
            return null;
        }

        public bool Exists(string name)
        {
            return GetModelForTexture(name) != null;
        }

        public TextureDetails Fetch(string name)
        {
            return Fetch(new[] { name }).FirstOrDefault();
        }

        public IEnumerable<TextureDetails> Fetch(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var model = GetModelForTexture(name);
                var texName = GetNameForTexture(name);
                if (model != null && texName != null)
                {
                    var tex = model.Model.Textures.FirstOrDefault(x => x.Index.ToString() == texName);
                    if (tex != null)
                    {
                        var td = new TextureDetails(name, tex.Image, tex.Width, tex.Height, TextureFlags.None);
                        yield return td;
                    }
                }
            }
        }

        private readonly ConcurrentQueue<TextureDetails> _textureQueue = new ConcurrentQueue<TextureDetails>();

        public void Request(IEnumerable<string> names)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var name in names)
                {
                    var model = GetModelForTexture(name);
                    var texName = GetNameForTexture(name);
                    if (model != null && texName != null)
                    {
                        var tex = model.Model.Textures.FirstOrDefault(x => x.Index.ToString() == texName);
                        if (tex != null)
                        {
                            var td = new TextureDetails(name, tex.Image, tex.Width, tex.Height, TextureFlags.None);
                            _textureQueue.Enqueue(td);
                        }
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