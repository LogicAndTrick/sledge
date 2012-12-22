using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Helpers
{
    public static class Matrix
    {
        public static MatrixMode CurrentMode { get; private set; }

        public static void Set(MatrixMode mode)
        {
            if (CurrentMode != mode) {
                GL.MatrixMode(mode);
            }
        }

        public static void Pop()
        {
            GL.PopMatrix();
        }

        public static void Push()
        {
            GL.PushMatrix();
        }

        public static void Identity()
        {
            GL.LoadIdentity();
        }
    }
}
