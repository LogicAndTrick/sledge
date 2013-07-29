using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Helpers
{
    public static class Viewport
    {
        public static void Perspective(int x, int y, int width, int height, int fov, float near = 0.1f, float far = 50000)
        {
            Switch(x, y, width, height);
            var mode = Matrix.CurrentMode;
            Matrix.Set(MatrixMode.Projection);
            var ratio = width / (float) height;
            if (ratio <= 0) ratio = 1;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), ratio, near, far);
            GL.LoadMatrix(ref projection);
            Matrix.Set(mode);
        }

        public static void Orthographic(int x, int y, int width, int height, float near = -1, float far = 1)
        {
            Switch(x, y, width, height);
            var mode = Matrix.CurrentMode;
            Matrix.Set(MatrixMode.Projection);
            Matrix4 ortho = Matrix4.CreateOrthographic(width, height, near, far);
            GL.LoadMatrix(ref ortho);
            Matrix.Set(mode);
        }

        public static void Switch(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
            GL.Scissor(x, y, width, height);
        }
    }
}
