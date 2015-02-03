using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Arrays;

namespace Sledge.Rendering
{
    public class OpenGLRenderer : IRenderer
    {
        private readonly Dictionary<IViewport, ViewportData> _viewportData;

        public OpenGLRenderer()
        {
            _viewportData = new Dictionary<IViewport, ViewportData>();
        }

        public IViewport CreateViewport()
        {
            var view = new OpenGLViewport();
            view.Render += RenderViewport;
            return view;
        }

        private void RenderViewport(IViewport viewport, Frame frame)
        {
            var data = GetViewportData(viewport);

            // Set up FBO
            data.Framebuffer.Size = new Size(viewport.Control.Width, viewport.Control.Height);
            data.Framebuffer.Bind();

            // Set up camera
            GL.LoadIdentity();
            GL.Viewport(0, 0, viewport.Control.Width, viewport.Control.Height);

            GL.MatrixMode(MatrixMode.Projection);
            var vpMatrix = viewport.Camera.GetViewportMatrix(viewport.Control.Width, viewport.Control.Height);
            GL.LoadMatrix(ref vpMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            var camMatrix = viewport.Camera.GetCameraMatrix();
            GL.LoadMatrix(ref camMatrix);

            // Do actual render
            var colours = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < colours.Length; i++)
            {
                var a = i * 10;
                var b = (i + 1) * 10;
                GL.Color3(colours[i]);
                GL.Vertex3(a, 0, 0);
                GL.Vertex3(b, 0, 0);
                GL.Vertex3(0, a, 0);
                GL.Vertex3(0, b, 0);
                GL.Vertex3(0, 0, a);
                GL.Vertex3(0, 0, b);
            }
            GL.End();

            // Blit FBO
            data.Framebuffer.Unbind();
            data.Framebuffer.Render();
        }

        private ViewportData GetViewportData(IViewport viewport)
        {
            if (!_viewportData.ContainsKey(viewport))
            {
                var data = new ViewportData(new Framebuffer(viewport.Control.Width, viewport.Control.Height));
                _viewportData.Add(viewport, data);
            }
            return _viewportData[viewport];
        }

        private class ViewportData
        {
            public Framebuffer Framebuffer { get; set; }

            public ViewportData(Framebuffer framebuffer)
            {
                Framebuffer = framebuffer;
            }
        }
    }

    public class OpenGLElementArray<T> : IDisposable where T : struct 
    {
        public class Subset
        {
            public int GroupID { get; set; }
            public object Instance { get; set; }
            public int Start { get; set; }
            public int Count { get; set; }

            public Subset(int groupId, object instance, int start, int count)
            {
                GroupID = groupId;
                Instance = instance;
                Start = start;
                Count = count;
            }
        }

        private const int _structSize = sizeof (ushort);
        private readonly int _buffer;

        private int _arraySize;
        private OpenGLVertexBuffer<T> _vertexBuffer;
        private Dictionary<int, Func<IEnumerable<IGrouping<object, T>>>> _groups;

        public OpenGLElementArray(OpenGLVertexBuffer<T> vertexBuffer)
        {
            _vertexBuffer = vertexBuffer;
            _buffer = GL.GenBuffer();

            _arraySize = 1024;
            _groups = new Dictionary<int, Func<IEnumerable<IGrouping<object, T>>>>();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(_arraySize * _structSize), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void AddGroup(int id, Func<IEnumerable<IGrouping<object, T>>> groupingFunction)
        {
            _groups.Add(id, groupingFunction);
        }

        public void Stuff()
        {
            // Group 1 = textured
            // Get grouped data for group 1
            // { untextured => [...], texture_1 => [...], ... }
            // generate subsets for group 1
            // { untextured, [0,1,2] }, { texture_1, [3,4,5] }, ...
            int start = 0;
            foreach (var kv in _groups)
            {
                var sets = kv.Value();
                foreach (var set in sets)
                {
                    var list = set.Select(x => _vertexBuffer.IndexOf(x)).ToArray();
                    var ss = new Subset(kv.Key, set.Key, start, list.Length);
                    // push list onto buffer
                }
            }
        }

        private int GetNewSize(int required)
        {
            var ok = 1024;
            while (required > ok)
            {
                if (ok < 4096) ok *= 2;
                else ok += 4096;
            }
            return ok;
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_buffer);
        }
    }

    /// <summary>
    /// A vertex buffer that can store arbitrary data. Keeps the object indices in memory so it can return the index for use in an element array.
    /// </summary>
    /// <remarks>
    /// The buffer starts at 1024 vertices and doubles in size until it reaches 4096,
    /// after which it increases by 4096 until it reaches the maximum size of 65536 elements.
    /// The buffer can shrink by the same values.
    /// </remarks>
    /// <typeparam name="T">The vertex type struct to store in the buffer. Must be valid for a <code>ArraySpecification</code> object.</typeparam>
    public class OpenGLVertexBuffer<T> : IDisposable where T : struct 
    {
        private ArraySpecification Specification { get; set; }
        private readonly int _structSize;
        private readonly int _buffer;

        private ushort _nextIndex;
        private int _arraySize;
        private Dictionary<T, ushort> _elements;
        
        public OpenGLVertexBuffer()
        {
            Specification = new ArraySpecification(typeof(T));
            _structSize = Marshal.SizeOf(typeof(T));
            _buffer = GL.GenBuffer();

            _nextIndex = 0;
            _arraySize = 1024;
            _elements = new Dictionary<T, ushort>();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_arraySize * _structSize), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public ushort IndexOf(T obj)
        {
            return _elements[obj];
        }

        public void Bind()
        {
            // Note: Unlike buffers, VAOs cannot be shared across multiple opengl contexts!
            // That makes them really hard to deal with and probably not worth the effort...
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            var stride = Specification.Stride;
            for (var j = 0; j < Specification.Indices.Count; j++)
            {
                var ai = Specification.Indices[j];
                GL.EnableVertexAttribArray(j);
                GL.VertexAttribPointer(j, ai.Length, ai.Type, false, stride, ai.Offset);
            }
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void PushChanges(IEnumerable<T> add, IEnumerable<T> remove)
        {
            foreach (var obj in remove)
            {
                _elements.Remove(obj);
            }

            var index = _nextIndex;
            var list = add.ToList();
            foreach (var obj in list)
            {
                _elements.Add(obj, _nextIndex);
                _nextIndex++;
            }
            if (_nextIndex == index) return;

            if (_nextIndex > _arraySize) Rebuild();
            else Append(index, list);
        }

        private void Append(int startIndex, List<T> list)
        {
            var d = list.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(startIndex * _structSize), new IntPtr(d.Length * _structSize), d);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void Rebuild()
        {
            var data = _elements.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
            if (_arraySize < data.Length) _arraySize = GetNewSize(data.Length);
            _elements = data.Select((x, i) => new {x, i}).ToDictionary(x => x.x, x => (ushort) x.i);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_arraySize * _structSize), data, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private int GetNewSize(int required)
        {
            var ok = 1024;
            while (required > ok)
            {
                if (ok < 4096) ok *= 2;
                else ok += 4096;
            }
            return ok;
        }

        public void Dispose()
        {
            _elements.Clear();
            GL.DeleteBuffer(_buffer);
        }
    }
}