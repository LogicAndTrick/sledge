using Sledge.Common;

namespace Sledge.DataStructures.Models
{
    public class Texture
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Flags { get; set; }
        public System.Drawing.Bitmap Image { get; set; }
    }
}