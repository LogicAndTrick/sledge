using System;
using Sledge.Common;

namespace Sledge.Rendering.OpenGL
{
    public class Texture : IDisposable
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public TextureFlags Flags { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Texture(int id, string name, TextureFlags flags)
        {
            ID = id;
            Name = name;
            Flags = flags;
        }

        public void Bind()
        {

        }

        public void Unbind()
        {

        }

        public void Dispose()
        {
            // 
        }
    }
}