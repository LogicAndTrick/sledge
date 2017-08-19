using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Materials
{
    public class StringTextureManager : IUpdatable, IDisposable
    {
        internal class FontKey
        {
            public string Name { get; private set; }
            public float Size { get; private set; }
            public FontStyle Style { get; private set; }

            public FontKey(string name, float size, FontStyle style)
            {
                Name = name;
                Size = size;
                Style = style;
            }

            public Font CreateFont()
            {
                return new Font(Name, Size, Style, GraphicsUnit.Pixel);
            }

            public override string ToString()
            {
                return string.Format("{0}//{1}//{2}", Name, Size, (int) Style);
            }

            protected bool Equals(FontKey other)
            {
                return string.Equals(Name, other.Name) && Size.Equals(other.Size) && Style == other.Style;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((FontKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Size.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int) Style;
                    return hashCode;
                }
            }
        }

        internal class StringTextureValue
        {
            public string Value { get; private set; }
            public string TextureName { get; private set; }
            public Rectangle Rectangle { get; private set; }
            public long Timestamp { get; set; }
            public bool IsRemoved { get; set; }

            public StringTextureValue(string text, string textureName, Rectangle rectangle)
            {
                Value = text;
                TextureName = textureName;
                Rectangle = rectangle;
                Timestamp = -1;
            }
        }

        internal class StringTexture : IDisposable, IUpdatable
        {
            private const int MinSize = 512;
            private const int MaxSize = 2048;
            private const int MaxFragmentation = 200;

            private static uint _textureId;
            private static string GenerateTextureName()
            {
                return "Internal::StringTextureManager::Texture_" + (_textureId++);
            }

            public string TextureName { get; private set; }
            public int Size { get; private set; }

            private readonly Font _font;
            private readonly Dictionary<string, StringTextureValue> _values;
            private readonly IRenderer _renderer;
            private readonly long _cacheMilliseconds;

            private Bitmap _image;
            private int _fragmentation;

            public StringTexture(IRenderer renderer, FontKey font, long cacheMilliseconds)
            {
                _renderer = renderer;
                _cacheMilliseconds = cacheMilliseconds;
                _font = font.CreateFont();
                TextureName = GenerateTextureName();
                Size = MinSize;
                _image = new Bitmap(Size, Size);
                using (var g = Graphics.FromImage(_image))
                {
                    g.Clear(Color.Transparent);
                }
                _values = new Dictionary<string, StringTextureValue>();

                UpdateTexture();
                _renderer.Materials.Add(Material.Texture(TextureName, true));
            }

            public bool IsEmpty()
            {
                return _values.Count == 0;
            }

            public StringTextureValue Get(string text)
            {
                return Contains(text) ? _values[text] : null;
            }

            public bool Contains(string text)
            {
                return _values.ContainsKey(text);
            }

            public RectangleF GetUVRectangle(string text)
            {
                if (!Contains(text)) return RectangleF.Empty;
                var val = _values[text];
                var s = (float) Size;
                return new RectangleF(val.Rectangle.X / s, val.Rectangle.Y / s, val.Rectangle.Width / s, val.Rectangle.Height / s);
            }

            public Size GetSize(string text)
            {
                if (!Contains(text)) return System.Drawing.Size.Empty;
                var val = _values[text];
                return new Size(val.Rectangle.Width, val.Rectangle.Height);
            }

            private int GetFreeX(int startY, int endY)
            {
                var x = 0;
                foreach (var rec in _values.Values.Select(v => v.Rectangle))
                {
                    if (rec.Bottom < startY || rec.Top > endY) continue;
                    x = Math.Max(x, rec.Right);
                }
                return x;
            }

            private Rectangle FindSpaceFor(int width, int height)
            {
                int x = 0, y = 0;
                while (y + height < Size)
                {
                    x = GetFreeX(y, y + height);
                    if (x + width < Size) return new Rectangle(x, y, width, height);
                    y += height + 1;
                }
                return Rectangle.Empty;
            }

            private bool HasRoomFor(int width, int height)
            {
                return !FindSpaceFor(width, height).IsEmpty;
            }

            private bool CanMakeRoomFor(int width, int height)
            {
                var spare = MaxSize - Size;
                return width <= spare && height <= spare;
            }

            private bool ExpandFor(int width, int height)
            {
                var req = Size + Math.Max(width, height);
                var newSize = Size;
                while (newSize < req) newSize *= 2;

                if (newSize > MaxSize) return false;
                if (newSize == Size) return true;

                Size = newSize;
                var img = new Bitmap(Size, Size);
                using (var g = Graphics.FromImage(img))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(_image, 0, 0);
                }
                _image.Dispose();
                _image = img;

                UpdateTexture();
                return true;
            }

            public StringTextureValue Add(string value)
            {
                var v = AddInternal(value);
                UpdateTexture();
                return v;
            }

            private void UpdateTexture()
            {
                _renderer.Textures.Create(TextureName, _image, Size, Size, TextureFlags.PixelPerfect | TextureFlags.Transparent);
            }

            private StringTextureValue AddInternal(string value)
            {
                if (Contains(value)) return _values[value];

                var size = TextRenderer.MeasureText(value, _font);
                size = new Size(size.Width, size.Height + 2);
                var rec = FindSpaceFor(size.Width, size.Height);

                if (rec.IsEmpty && CanMakeRoomFor(size.Width, size.Height) && ExpandFor(size.Width, size.Height))
                {
                    rec = FindSpaceFor(size.Width, size.Height);
                }

                if (rec.IsEmpty) return null;

                var stv = new StringTextureValue(value, TextureName, rec);
                using (var g = Graphics.FromImage(_image))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.None;

                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.FillRectangle(Brushes.Transparent, stv.Rectangle);

                    g.CompositingMode = CompositingMode.SourceOver;
                    g.DrawString(stv.Value, _font, Brushes.White, stv.Rectangle);
                }
                _values.Add(value, stv);

                return stv;
            }

            private bool Remove(string value)
            {
                if (!Contains(value)) return false;

                // Remove the value
                var removed = _values[value];
                removed.IsRemoved = true;
                var ret = _values.Remove(value);
                return ret;
            }

            private void IncreaseFragmentation(int num)
            {
                _fragmentation += num;

                // If we've hit the fragmentation limit, expire all textures
                if (_fragmentation > MaxFragmentation)
                {
                    _fragmentation = 0;
                    var v = _values.Keys.FirstOrDefault();
                    Clear();
                    if (v != null) Add(v);
                }
            }

            private void Clear()
            {
                foreach (var v in _values.Values) v.IsRemoved = true;
                _values.Clear();
            }

            private void Rebuild()
            {
                _fragmentation = 0;
                var values = _values.Keys.ToList();
                Clear();
                foreach (var v in values) AddInternal(v);
                UpdateTexture();
            }

            public void ResetTimestamp(string text)
            {
                if (_values.ContainsKey(text)) _values[text].Timestamp = -1;
            }

            public void Update(Frame frame)
            {
                var rem = 0;
                foreach (var val in _values.Values.ToArray())
                {
                    if (val.Timestamp < 0)
                    {
                        val.Timestamp = frame.Milliseconds;
                    }
                    else if (frame.Milliseconds - val.Timestamp > _cacheMilliseconds)
                    {
                        if (Remove(val.Value)) rem++;
                    }
                }
                IncreaseFragmentation(rem);
            }

            public void Dispose()
            {
                _image.Dispose();
                _font.Dispose();
            }
        }

        internal static readonly FontKey DefaultFontKey = new FontKey(SystemFonts.DefaultFont.Name, 12, FontStyle.Regular);

        private readonly IRenderer _renderer;
        private readonly Dictionary<FontKey, List<StringTexture>> _textures;
        private const long TimeoutMilliseconds = 5000; // 5 seconds

        public StringTextureManager(IRenderer renderer)
        {
            _renderer = renderer;
            _textures = new Dictionary<FontKey, List<StringTexture>>();
        }

        private StringTexture GetTexture(string text, FontKey key, bool addIfMissing = true)
        {
            var list = _textures.ContainsKey(key) ? _textures[key] : new List<StringTexture>();
            var tex = list.FirstOrDefault(x => x.Contains(text));
            if (tex == null && addIfMissing)
            {
                foreach (var t in list)
                {
                    var stv = t.Add(text);
                    if (stv != null)
                    {
                        tex = t;
                        break;
                    }
                }
                if (tex == null)
                {
                    var sc = new StringTexture(_renderer, key, TimeoutMilliseconds);
                    if (!_textures.ContainsKey(key)) _textures.Add(key, new List<StringTexture>());
                    _textures[key].Add(sc);
                    var stv = sc.Add(text);
                    if (stv != null) tex = sc;
                }
            }
            if (tex != null) tex.ResetTimestamp(text);
            return tex;
        }

        private FontKey GetFontKey(string fontName, float fontSize, FontStyle style)
        {
            return new FontKey(fontName ?? DefaultFontKey.Name, fontSize <= 0 ? DefaultFontKey.Size : fontSize, style);
        }

        internal StringTextureValue GetTextureValue(string text, string fontName = null, float fontSize = 0, FontStyle style = FontStyle.Regular)
        {
            var tex = GetTexture(text, GetFontKey(fontName, fontSize, style));
            return tex?.Get(text);
        }

        public Material GetMaterial(string text, string fontName = null, float fontSize = 0, FontStyle style = FontStyle.Regular)
        {
            var tex = GetTexture(text, GetFontKey(fontName, fontSize, style));
            return tex == null ? null : Material.Texture(tex.TextureName, true);
        }

        public FaceElement GetElement(string text, Color color, PositionType type, Vector3 position, float anchorX, float anchorY, string fontName = null, float fontSize = 0, FontStyle style = FontStyle.Regular)
        {
            var tex = GetTexture(text, GetFontKey(fontName, fontSize, style));
            if (tex == null) return null;

            var mat = new Material(MaterialType.Textured, color, tex.TextureName);
            var uv = tex.GetUVRectangle(text);
            var size = tex.GetSize(text);

            return new FaceElement(type, mat, new[]
            {
                new PositionVertex(new Position(position) { Offset = new Vector3((float) Math.Floor(-size.Width * anchorX),      (float) Math.Floor(-size.Height * anchorY),      0) }, uv.X, uv.Y),
                new PositionVertex(new Position(position) { Offset = new Vector3((float) Math.Floor(size.Width * (1 - anchorX)), (float) Math.Floor(-size.Height * anchorY),      0) }, uv.X + uv.Width, uv.Y),
                new PositionVertex(new Position(position) { Offset = new Vector3((float) Math.Floor(size.Width * (1 - anchorX)), (float) Math.Floor(size.Height * (1 - anchorY)), 0) }, uv.X + uv.Width, uv.Y + uv.Height),
                new PositionVertex(new Position(position) { Offset = new Vector3((float) Math.Floor(-size.Width * anchorX),      (float) Math.Floor(size.Height * (1 - anchorY)), 0) }, uv.X, uv.Y + uv.Height)
            })
            {
                AccentColor = color,
                RenderFlags = RenderFlags.Polygon
            };
        }

        public Size GetSize(string text, string fontName = null, float fontSize = 0, FontStyle style = FontStyle.Regular)
        {
            var tex = GetTexture(text, GetFontKey(fontName, fontSize, style));
            if (tex == null) return Size.Empty;
            return tex.GetSize(text);
        }

        public void Update(Frame frame)
        {
            foreach (var kv in _textures.ToList())
            {
                foreach (var t in kv.Value.ToArray())
                {
                    t.Update(frame);

                    if (t.IsEmpty())
                    {
                        t.Dispose();
                        kv.Value.Remove(t);
                    }
                }
                if (kv.Value.Count == 0) _textures.Remove(kv.Key);
            }
        }

        public void Dispose()
        {
            foreach (var texture in _textures.SelectMany(x => x.Value))
            {
                texture.Dispose();
            }
            _textures.Clear();
        }
    }
}
