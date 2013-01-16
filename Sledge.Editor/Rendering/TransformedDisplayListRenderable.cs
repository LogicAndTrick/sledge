using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Documents;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    public class RenderManagerRenderable : IRenderable
    {
        private readonly ViewportBase _viewport;
        private readonly RenderManager _manager;
        private readonly bool _is3D;

        public RenderManagerRenderable(ViewportBase viewport, RenderManager manager)
        {
            _viewport = viewport;
            _manager = manager;
            _is3D = viewport is Viewport3D;
        }

        public void Render(object sender)
        {
            Matrix4 vm = _viewport.GetViewportMatrix(),
                    cm = _viewport.GetCameraMatrix(),
                    mm = _viewport.GetModelViewMatrix();
            if (_is3D) _manager.Draw3D(_viewport, vm, cm, mm);
            else _manager.Draw2D(_viewport, vm, cm, mm);
        }
    }

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
