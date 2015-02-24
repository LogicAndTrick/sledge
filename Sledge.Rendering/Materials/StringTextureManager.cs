using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Materials
{
    public class StringTextureManager
    {
        private class FontKey : IDisposable
        {
            private Font _font;
            public string Name { get; private set; }
            public float Size { get; private set; }
            public FontStyle Style { get; private set; }

            public FontKey(string name, float size, FontStyle style)
            {
                Name = name;
                Size = size;
                Style = style;
                _font = new Font(name, size, Style, GraphicsUnit.Pixel);
            }

            public void Dispose()
            {
                _font.Dispose();
            }
        }
        
        public class StringCollection : IDisposable
        {
            private Dictionary<string, string> _strings;

            public StringCollection()
            {
                _strings = new Dictionary<string, string>();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        private readonly IRenderer _renderer;
        private Dictionary<FontKey, StringCollection> _strings;

        public StringTextureManager(IRenderer renderer)
        {
            _renderer = renderer;
        }
    }
}
