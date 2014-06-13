using System.Collections.Generic;
using Sledge.Common;
using Sledge.Graphics.Helpers;

namespace Sledge.Providers.Texture
{
    public class TextureItem
    {
        public TexturePackage Package { get; private set; }
        public string Name { get; private set; }
        public TextureFlags Flags { get; set; }

        public TextureSubItem PrimarySubItem
        {
            get { return _subItems.ContainsKey(TextureSubItemType.Base) ? _subItems[TextureSubItemType.Base] : null; }
        }

        private readonly Dictionary<TextureSubItemType, TextureSubItem> _subItems;

        public IEnumerable<TextureSubItem> AllSubItems
        {
            get { return _subItems.Values; }
        }

        public int Width { get { return PrimarySubItem.Width; } }
        public int Height { get { return PrimarySubItem.Height; } }

        public TextureItem(TexturePackage package, string name, TextureFlags flags, int width, int height)
        {
            Package = package;
            Name = name;
            Flags = flags;
            var baseItem = new TextureSubItem(TextureSubItemType.Base, this, name, width, height);
            _subItems = new Dictionary<TextureSubItemType, TextureSubItem> {{TextureSubItemType.Base, baseItem}};
        }

        public TextureItem(TexturePackage package, string name, TextureFlags flags, string primarySubItemName, int width, int height)
        {
            Package = package;
            Name = name;
            Flags = flags;
            var baseItem = new TextureSubItem(TextureSubItemType.Base, this, primarySubItemName, width, height);
            _subItems = new Dictionary<TextureSubItemType, TextureSubItem> {{TextureSubItemType.Base, baseItem}};
        }

        public TextureItem(TexturePackage package, string name, TextureFlags flags)
        {
            Package = package;
            Name = name;
            Flags = flags;
            _subItems = new Dictionary<TextureSubItemType, TextureSubItem>();
        }

        public TextureSubItem AddSubItem(TextureSubItemType type, string name, int width, int height)
        {
            var si = new TextureSubItem(type, this, name, width, height);
            _subItems.Add(type, si);
            return si;
        }

        public ITexture GetTexture()
        {
            if (!TextureHelper.Exists(Name.ToLowerInvariant()))
            {
                TextureProvider.LoadTextureItem(this);
            }
            return TextureHelper.Get(Name.ToLowerInvariant());
        }
    }
}
