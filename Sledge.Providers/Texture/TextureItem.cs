using System.Collections.Generic;
using System.ComponentModel;
using Sledge.Common;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class TextureItem
    {
        public TexturePackage Package { get; private set; }
        public string Name { get; private set; }
        public TextureSubItem PrimarySubItem { get; private set; }

        private List<TextureSubItem> _allSubItems;
        public IEnumerable<TextureSubItem> AllSubItems
        {
            get { return _allSubItems; }
        }

        public int Width { get { return PrimarySubItem.Width; } }
        public int Height { get { return PrimarySubItem.Height; } }

        public TextureItem(TexturePackage package, string name, int width, int height)
        {
            Package = package;
            Name = name;
            PrimarySubItem = new TextureSubItem(this, name, width, height);
            _allSubItems = new List<TextureSubItem> { PrimarySubItem };
        }

        public TextureItem(TexturePackage package, string name, string primarySubItemName, int width, int height)
        {
            Package = package;
            Name = name;
            PrimarySubItem = new TextureSubItem(this, primarySubItemName, width, height);
            _allSubItems = new List<TextureSubItem> { PrimarySubItem };
        }

        public TextureItem(TexturePackage package, string name)
        {
            Package = package;
            Name = name;
            PrimarySubItem = null;
            _allSubItems = new List<TextureSubItem>();
        }

        public TextureSubItem AddSubItem(string name, int width, int height)
        {
            var si = new TextureSubItem(this, name, width, height);
            _allSubItems.Add(si);
            if (PrimarySubItem == null) PrimarySubItem = si;
            return si;
        }

        public ITexture GetTexture(ISynchronizeInvoke invokable)
        {
            if (!TextureHelper.Exists(Name.ToLowerInvariant()))
            {
                TextureProvider.LoadTextureItem(this);
            }
            return TextureHelper.Get(Name.ToLowerInvariant());
        }
    }
}
