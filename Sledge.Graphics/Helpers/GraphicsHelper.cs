using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Helpers
{
    public class GraphicsHelper
    {
        public static void InitGL3D()
        {
            GL.ClearColor(Color.Black);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            TextureHelper.EnableTexturing();
        }

        public static void InitGL2D()
        {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public static void DisableDepthTesting()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public static void EnableDepthTesting()
        {
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
