using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using IGraphicsContext = OpenTK.Graphics.IGraphicsContext;

namespace Sledge.Graphics.Arrays
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

    public abstract class VBO<TIn, TOut> : IDisposable
        where TOut : struct
    {
        public ArraySpecification Specification { get; private set; }
        private int _size;

        private readonly List<TOut> _data;
        private readonly Dictionary<int, List<uint>> _indices;
        private readonly Dictionary<int, Tuple<int, object>> _subsetState;
        private readonly Dictionary<int, List<Subset>> _subsets;
        private readonly Dictionary<object, int> _offsets;

        public VBO(IEnumerable<TIn> data)
        {
            Specification = new ArraySpecification(typeof(TOut));
            _size = Marshal.SizeOf(typeof (TOut));

            _data = new List<TOut>();
            _indices = new Dictionary<int, List<uint>>();
            _subsetState = new Dictionary<int, Tuple<int, object>>();
            _subsets = new Dictionary<int, List<Subset>>();
            _offsets = new Dictionary<object, int>();
            _vertexArrays = new Dictionary<IGraphicsContext, VertexArray>();

            Update(data);
        }

        public void Update(IEnumerable<TIn> data)
        {
            Clear();
            CreateArray(data);
            Commit();
        }

        public void Render(IGraphicsContext context, BeginMode mode, Subset subset)
        {
            if (!_vertexArrays.ContainsKey(context)) CreateVertexArray(context);

            var arr = _vertexArrays[context];
            if (arr.Dirty) arr.Bind(Specification, _array, _elementArray);
            GL.BindVertexArray(arr.ID);
            GL.DrawElements(mode, subset.Count, DrawElementsType.UnsignedInt, subset.Start * sizeof (uint));
            GL.BindVertexArray(0);
        }

        private struct VertexArray
        {
            public int ID;
            public bool Dirty;

            public void Bind(ArraySpecification specification, int array, int elementArray)
            {
                GL.BindVertexArray(ID);

                GL.BindBuffer(BufferTarget.ArrayBuffer, array);
                var stride = specification.Stride;
                for (var j = 0; j < specification.Indices.Count; j++)
                {
                    var ai = specification.Indices[j];
                    GL.EnableVertexAttribArray(j);
                    GL.VertexAttribPointer(j, ai.Length, ai.Type, false, stride, ai.Offset);
                }
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementArray);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                Dirty = false;
            }
        }

        private void CreateVertexArray(IGraphicsContext context)
        {
            int id;
            GL.GenVertexArrays(1, out id);
            _vertexArrays.Add(context, new VertexArray {ID = id, Dirty = true});
        }

        public void Dispose()
        {
            //
        }

        private int _array = -1;
        private int _elementArray = -1;
        private Dictionary<IGraphicsContext, VertexArray> _vertexArrays;

        protected void Clear()
        {
            _data.Clear();
            _indices.Clear();
            _subsetState.Clear();
            _subsets.Clear();
            _offsets.Clear();

            if (_array >= 0) GL.DeleteBuffers(1, ref _array);
            if (_elementArray >= 0) GL.DeleteBuffers(1, ref _elementArray);
            foreach (var id in _vertexArrays)
            {
                
            }
        }

        protected void Update(int index, IEnumerable<TOut> data)
        {
            var d = data.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _array);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(index * _size), new IntPtr(d.Length * _size), d);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected void Commit()
        {
            var data = _data.ToArray();
            var elementData = new uint[_indices.Sum(x => x.Value.Count)];
            var idx = 0;
            foreach (var list in _indices)
            {
                list.Value.CopyTo(0, elementData, idx, list.Value.Count);
                if (idx > 0 && _subsets.ContainsKey(list.Key))
                {
                    foreach (var subset in _subsets[list.Key])
                    {
                        subset.Start += idx;
                    }
                }
                idx += list.Value.Count;
            }

            // Array buffer
            GL.GenBuffers(1, out _array);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _array);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * _size), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Element buffer
            GL.GenBuffers(1, out _elementArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementArray);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(elementData.Length * sizeof(uint)), elementData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _data.Clear();
            _indices.Clear();
            _subsetState.Clear();
        }

        protected abstract void CreateArray(IEnumerable<TIn> data);

        protected void StartSubset<T>(int groupId, T context) where T : class 
        {
            if (_subsetState.ContainsKey(groupId)) throw new Exception("Cannot start two subsets for the same group.");

            if (!_indices.ContainsKey(groupId)) _indices.Add(groupId, new List<uint>());
            var list = _indices[groupId];

            _subsetState.Add(groupId, Tuple.Create(list.Count, (object) context));
        }

        protected void PushSubset(int groupId)
        {
            if (!_subsetState.ContainsKey(groupId)) throw new Exception("No subset exists for this group.");

            var ss = _subsetState[groupId];
            _subsetState.Remove(groupId);

            if (!_indices.ContainsKey(groupId)) _indices.Add(groupId, new List<uint>());
            var list = _indices[groupId];

            var num = list.Count - ss.Item1;
            if (num == 0) return;

            if (!_subsets.ContainsKey(groupId)) _subsets.Add(groupId, new List<Subset>());
            _subsets[groupId].Add(new Subset(groupId, ss.Item2, ss.Item1, num));
        }

        protected IEnumerable<Subset> GetSubsets(int groupId)
        {
            if (!_subsets.ContainsKey(groupId)) return new List<Subset>();
            return _subsets[groupId];
        }

        protected IEnumerable<Subset> GetSubsets<T>(int groupId)
        {
            if (!_subsets.ContainsKey(groupId)) return new List<Subset>();
            return _subsets[groupId].Where(x => x.Instance is T);
        }

        protected void PushIndex(int groupId, uint start, IEnumerable<uint> indices)
        {
            if (!_indices.ContainsKey(groupId)) _indices.Add(groupId, new List<uint>());
            var list = _indices[groupId];
            list.AddRange(indices.Select(x => x + start));
        }

        protected void PushOffset<T>(T obj)
        {
            _offsets.Add(obj, _data.Count);
        }

        protected int GetOffset<T>(T obj)
        {
            if (!_offsets.ContainsKey(obj)) return -1;
            return _offsets[obj];
        }

        protected uint PushData(IEnumerable<TOut> data)
        {
            var index = _data.Count;
            _data.AddRange(data);
            return (uint) index;
        }

        protected static IEnumerable<uint> Triangulate(int num)
        {
            for (uint i = 1; i < num - 1; i++)
            {
                yield return 0;
                yield return i;
                yield return i + 1;
            }
        }

        protected static IEnumerable<uint> Linearise(int num)
        {
            for (uint i = 0; i < num; i++)
            {
                var ni = (uint)((i + 1) % num);
                yield return i;
                yield return ni;
            }
        }
    }
}
