using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics;

namespace Sledge.Graphics.Arrays
{
    public class VBO<TIn, TOut> : IDisposable
        where TOut : struct
    {
        public ArraySpecification Specification { get; private set; }
        private int _size;

        public VBO()
        {
            Specification = new ArraySpecification(); // calculate it!
            _size = Marshal.SizeOf(typeof (TOut));
        }

        public void Render(IGraphicsContext context)
        {
            // render stuff!
        }

        public void Dispose()
        {
            //
        }

        protected void StartSubset<T>(int groupId, T context)
        {
        }

        protected void PushSubset(int groupId)
        {
        }

        protected void PushIndex(int groupId, params uint[] indices)
        {

        }

        protected void PushOffset<T>(int groupId, T obj)
        {
            
        }

        protected void CreateArray()
        {
            
        }

        protected IEnumerable<TOut> Convert(TIn obj)
        {
            return null;
        } 
    }
}
