using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLElementArray<T> : IDisposable where T : struct
    {
        private class Subset
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

        private const int StructSize = sizeof (ushort);
        private readonly int _buffer;

        private int _arraySize;
        private readonly OpenGLVertexBuffer<T> _vertexBuffer;
        private readonly Dictionary<int, PrimitiveType> _groupTypes;
        private readonly Dictionary<int, Func<IEnumerable<T>, IEnumerable<IGrouping<object, T>>>> _groups;
        private readonly Dictionary<int, List<Subset>> _subsets;

        public OpenGLElementArray(OpenGLVertexBuffer<T> vertexBuffer)
        {
            _vertexBuffer = vertexBuffer;
            _buffer = GL.GenBuffer();

            _arraySize = 1024;
            _groupTypes = new Dictionary<int, PrimitiveType>();
            _groups = new Dictionary<int, Func<IEnumerable<T>, IEnumerable<IGrouping<object, T>>>>();
            _subsets = new Dictionary<int, List<Subset>>();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(_arraySize * StructSize), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void AddGroup(int id, PrimitiveType type, Func<IEnumerable<T>, IEnumerable<IGrouping<object, T>>> groupingFunction)
        {
            _groupTypes.Add(id, type);
            _groups.Add(id, groupingFunction);
        }

        public void RemoveGroup(int id)
        {
            _groups.Remove(id);
        }

        /// <summary>
        /// Render the element buffer. The vertex buffer must be bound by the caller - it will not be bound by this function.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="instance"></param>
        public void Render(int groupId, object instance)
        {
            var mode = _groupTypes[groupId];
            var subset = _subsets[groupId].First(x => x.Instance == instance);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffer);
            GL.DrawElements(mode, subset.Count, DrawElementsType.UnsignedInt, subset.Start * sizeof(uint));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Update()
        {
            _subsets.Clear();
            var data = new List<ushort>();
            var start = 0;
            foreach (var kv in _groups)
            {
                _subsets.Add(kv.Key, new List<Subset>());
                var sets = kv.Value(_vertexBuffer.Elements);
                foreach (var set in sets)
                {
                    var list = set.Select(x => _vertexBuffer.IndexOf(x)).ToArray();
                    var ss = new Subset(kv.Key, set.Key, start, list.Length);
                    data.AddRange(list);
                    _subsets[kv.Key].Add(ss);
                    start += list.Length;
                }
            }
            Rebuild(data.ToArray());
        }

        private void Rebuild(ushort[] data)
        {
            if (_arraySize < data.Length) _arraySize = GetNewSize(data.Length);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(_arraySize * StructSize), data, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private static int GetNewSize(int required)
        {
            var ok = 1024;
            while (ok < required)
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
}