using Sledge.Common;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering
{
    public class SolidVertexArraySubset
    {
        private readonly SolidVertexArray _array;

        public bool IsSelected { get; private set; }

        public int Start { get; private set; }
        public int Count { get; private set; }

        public ITexture Texture { get; private set; }

        public SolidVertexArraySubset(SolidVertexArray array, int start, int count, ITexture texture, bool isSelected)
        {
            _array = array;
            IsSelected = isSelected;
            Start = start;
            Count = count;
            Texture = texture;
        }

        public void Draw(ShaderProgram program)
        {
            _array.Array.Bind(0);
            _array.Array.DrawElements(0, Start, Count);
            _array.Array.Unbind();
        }

        public void DrawWireframe(ShaderProgram program)
        {
            _array.Array.Bind(1);
            _array.Array.DrawElements(1, Start, Count);
            _array.Array.Unbind();
        }

        private bool Equals(SolidVertexArraySubset other)
        {
            return IsSelected.Equals(other.IsSelected) && Equals(Texture, other.Texture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolidVertexArraySubset) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IsSelected.GetHashCode() * 397) ^ (Texture != null ? Texture.GetHashCode() : 0);
            }
        }
    }
}