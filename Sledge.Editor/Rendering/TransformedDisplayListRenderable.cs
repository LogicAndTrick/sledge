using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;

namespace Sledge.Editor.Rendering
{
    public class TransformedDisplayListRenderable : IRenderable
    {
        public String ListName { get; set; }
        public bool Enabled { get; set; }

        private Matrix4d _matrix;
        public Matrix4d Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public Color Colour { get; set; }

        public TransformedDisplayListRenderable(string listName, Matrix4d matrix)
        {
            Enabled = true;
            ListName = listName;
            Matrix = matrix;
            Colour = Color.Empty;
        }

        public void Render(object sender)
        {
            if (ListName == null || !DisplayList.Exists(ListName) || !Enabled) return;
            Graphics.Helpers.Matrix.Push();
            GL.MultMatrix(ref _matrix);
            if (!Colour.IsEmpty) GL.Color4(Colour);
            DisplayList.Call(ListName);
            Graphics.Helpers.Matrix.Pop();
        }
    }
}
