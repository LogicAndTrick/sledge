using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.DataStructures.Rendering;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;

namespace Sledge.Editor.Rendering
{
    public class TransformedDisplayListRenderable : IRenderable
    {
        public String ListName { get; set; }
        public bool Enabled { get; set; }

        private Matrix4 _matrix;
        public Matrix4 Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public Color Colour { get; set; }

        public TransformedDisplayListRenderable(string listName, Matrix4 matrix)
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
