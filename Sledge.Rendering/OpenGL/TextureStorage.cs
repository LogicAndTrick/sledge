using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Sledge.Rendering.OpenGL
{
    public class TextureStorage : IDisposable, ITextureStorage
    {
        private static bool? _supportsNpot;

        public static bool SupportsNonPowerOfTwo()
        {
            if (!_supportsNpot.HasValue)
            {
                var extensions = GL.GetString(StringName.Extensions);
                _supportsNpot = extensions.Contains("GL_ARB_texture_non_power_of_two");
            }
            return _supportsNpot.Value;
        }

        public static Bitmap WhitePixel { get; private set; }
        public static Bitmap DebugTexture { get; private set; }

        static TextureStorage()
        {
            WhitePixel = new Bitmap(1, 1);
            using (var g = Graphics.FromImage(WhitePixel))
            {
                g.Clear(Color.White);
            }

            DebugTexture = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(DebugTexture))
            {
                g.Clear(Color.White);
                g.FillRectangle(Brushes.Blue, 10, 10, 80, 80);
                g.FillRectangle(Brushes.Orange, 20, 20, 60, 60);
                g.FillRectangle(Brushes.Purple, 30, 30, 40, 40);
                g.FillRectangle(Brushes.Yellow, 40, 40, 20, 20);
            }
        }

        public Dictionary<string, Texture> Textures { get; private set; }

        private PixelInternalFormat PixelInternalFormat { get; set; }

        public bool EnableTransparency
        {
            get { return PixelInternalFormat == PixelInternalFormat.Rgba; }
            set { PixelInternalFormat = value ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb; }
        }

        public bool ForceNonPowerOfTwoResize { get; set; }

        public bool DisableTextureFiltering { get; set; }

        public TextureStorage()
        {
            Textures = new Dictionary<string, Texture>();
            PixelInternalFormat = PixelInternalFormat.Rgba;
        }

        // Todo: init these in a better place?
        public void Initialise()
        {
            foreach (var it in Internal.InternalTextures.GetInternalTextures())
            {
                Create(it.Key, it.Value, it.Value.Width, it.Value.Height, TextureFlags.None);
            }
        }

        #region Create

        // http://stackoverflow.com/questions/5525122/c-sharp-math-question-smallest-power-of-2-bigger-than-x
        // http://aggregate.org/MAGIC/#Next%20Largest%20Power%20of%202
        private static int NextPowerOfTwo(int num)
        {
            var x = (uint)num;
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (int)(x + 1);
        }

        public Texture Create(string name)
        {
            if (Exists(name))
            {
                Delete(name);
            }

            var tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            SetTextureParameters(TextureFlags.None);

            // The white pixel
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat, 1, 1, 0, PixelFormat.Bgra, PixelType.UnsignedByte, new[] { 0xFFFFFFFF });

            var texture = new Texture(tex, name, TextureFlags.None);
            Textures.Add(name, texture);
            return texture;
        }

        public Texture Create(string name, Bitmap bitmap, int width, int height, TextureFlags flags)
        {
            var actualBitmap = bitmap;
            if (ForceNonPowerOfTwoResize || !SupportsNonPowerOfTwo())
            {
                var w = NextPowerOfTwo(bitmap.Width);
                var h = NextPowerOfTwo(bitmap.Height);
                if (w != bitmap.Width || h != bitmap.Height) actualBitmap = new Bitmap(bitmap, w, h);
            }
            var data = actualBitmap.LockBits(
                new Rectangle(0, 0, actualBitmap.Width, actualBitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            Texture texobj;

            if (Exists(name))
            {
                texobj = Get(name);
                GL.BindTexture(TextureTarget.Texture2D, texobj.ID);
            }
            else
            {
                var tex = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, tex);
                SetTextureParameters(flags);

                texobj = new Texture(tex, name, flags) {Width = width, Height = height};
                Textures.Add(name, texobj);
            }

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat,
                data.Width,
                data.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0
                );

            actualBitmap.UnlockBits(data);
            if (actualBitmap != bitmap)
            {
                actualBitmap.Dispose();
            }

            return texobj;
        }

        private void SetTextureParameters(TextureFlags flags)
        {
            var minFilter = flags.HasFlag(TextureFlags.PixelPerfect) ? TextureMinFilter.Nearest
                                           : DisableTextureFiltering ? TextureMinFilter.Linear
                                                                     : TextureMinFilter.LinearMipmapLinear;
            var magFilter = flags.HasFlag(TextureFlags.PixelPerfect) ? TextureMagFilter.Nearest : TextureMagFilter.Linear;

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int) TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
        }

        #endregion

        #region Bind

        public void Bind(int textureIndex, string name)
        {
            if (!Exists(name))
            {
                throw new Exception("Texture " + name + " doesn't exist");
            }
            Bind(textureIndex, Get(name).ID);
        }

        public static void Bind(int textureIndex, int reference)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
            GL.BindTexture(TextureTarget.Texture2D, reference);
        }

        public static void Unbind(int textureIndex)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion

        #region Get

        public bool Exists(string name)
        {
            return Textures.ContainsKey(name);
        }

        public Texture Get(string name)
        {
            return Exists(name) ? Textures[name] : null;
        }

        #endregion

        #region Delete

        public void Delete(string name)
        {
            if (Textures.ContainsKey(name))
            {
                DeleteTexture(Get(name).ID);
                Textures.Remove(name);
            }
        }

        public void DeleteAll()
        {
            foreach (var e in Textures)
            {
                DeleteTexture(e.Value.ID);
            }
            Textures.Clear();
        }

        private static void DeleteTexture(int num)
        {
            GL.DeleteTexture(num);
        }

        #endregion

        public void Dispose()
        {
            DeleteAll();
        }
    }
}