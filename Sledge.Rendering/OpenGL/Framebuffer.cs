using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL
{
    public class Framebuffer : IDisposable
    {
        private Size _size;
        private Size _internalSize;

        public Size Size
        {
            get { return _size; }
            set { Resize(value); }
        }

        private readonly int _texture;
        private readonly int _framebuffer;
        private readonly int _renderbuffer;

        public Framebuffer(int width, int height)
        {
            _size = new Size(width, height);
            _internalSize = new Size(NextPowerOfTwo(width), NextPowerOfTwo(height));

            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, _internalSize.Width, _internalSize.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            _framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture, 0);

            _renderbuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, _internalSize.Width, _internalSize.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _renderbuffer);

            var state = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (state != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception(state.ToString());
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteRenderbuffer(_renderbuffer);
            GL.DeleteTexture(_texture);
            GL.DeleteFramebuffer(_framebuffer);
        }

        private void Resize(Size size)
        {
            if (size.Width == _size.Width && size.Height == _size.Height) return;
            _size = size;

            size = new Size(NextPowerOfTwo(size.Width), NextPowerOfTwo(size.Height));
            if (size.Width == _internalSize.Width && size.Height == _internalSize.Height) return;

            _internalSize = size;

            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderbuffer);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, _internalSize.Width, _internalSize.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, _internalSize.Width, _internalSize.Height);
            
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render()
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _framebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(0, 0, _size.Width, _size.Height, 0, 0, _size.Width, _size.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

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
    }
}