using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Sledge.Graphics.Helpers
{
    /// <summary>
    /// Texture utilities. Borrows large amounts of code from
    /// the Texture Utility Library project for OpenTK:
    /// http://www.opentk.com/project/TexLib
    /// </summary>
    public static class TextureHelper
    {
        public static readonly Dictionary<string, GLTexture> Textures;

        private static PixelInternalFormat PixelInternalFormat { get; set; }

        public static bool EnableTransparency
        {
            get { return PixelInternalFormat == PixelInternalFormat.Rgba; }
            set { PixelInternalFormat = value ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb; }
        }

        static TextureHelper()
        {
            Textures = new Dictionary<string, GLTexture>();
            PixelInternalFormat = PixelInternalFormat.Rgba;
        }

        public static void ClearLoadedTextures()
        {
            Textures.Values.ToList().ForEach(x => x.Dispose());
            Textures.Clear();
        }


        #region Enable/Disable

        public static void EnableTexturing()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        }

        public static void DisableTexturing()
        {
            GL.Disable(EnableCap.Texture2D);
        }

        #endregion

        #region Create

        public static GLTexture Create(string name, Bitmap bitmap, bool hasTransparency)
        {
            if (Exists(name)) {
                Delete(name);
            }
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );
            var tex = CreateAndBindTexture();
            SetTextureParameters();
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat,
                data.Width, data.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0
                );
            bitmap.UnlockBits(data);
            var texobj = new GLTexture(tex, name) { Width = bitmap.Width, Height = bitmap.Height, HasTransparency = hasTransparency, BitmapImage = (Bitmap)bitmap.Clone() };
            Textures.Add(name, texobj);
            return texobj;
        }

        private static int CreateAndBindTexture()
        {
            var tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            return tex;
        }

        private static void SetTextureParameters()
        {
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
        }

        #endregion

        #region Bind

        public static void Bind(string name)
        {
            if (!Exists(name)) {
                throw new Exception("Texture " + name + " doesn't exist");
            }
            Bind(Get(name).Reference);
        }

        public static void Bind(int reference)
        {
            GL.BindTexture(TextureTarget.Texture2D, reference);
        }

        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion

        #region Get

        public static bool Exists(string name)
        {
            return Textures.ContainsKey(name);
        }

        public static GLTexture Get(string name)
        {
            return Exists(name) ? Textures[name] : null;
        }

        #endregion

        #region Delete

        public static void Delete(string name)
        {
            if (Textures.ContainsKey(name)) {
                DeleteTexture(Get(name).Reference);
                Textures.Remove(name);
            }
        }

        public static void DeleteAll()
        {
            foreach (var e in Textures) {
                DeleteTexture(e.Value.Reference);
            }
            Textures.Clear();
        }

        public static void DeleteTexture(int num)
        {
            GL.DeleteTexture(num);
        }

        #endregion
    }
}
