using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Helpers;

namespace Sledge.Editor.Rendering
{
    public class WidgetLinesRenderable : IRenderable
    {
        public void Render(object sender)
        {
            TextureHelper.DisableTexturing();
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);
            GL.Color3(Color.FromArgb(0, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 100, 0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 100);
            GL.End();
            TextureHelper.EnableTexturing();
        }
    }
}
