using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    public interface ITransformable
    {
        /// <summary>
        /// Transforms members of this object, and all child elements.
        /// </summary>
        /// <param name="matrix">The transformation matrix</param>
        void Transform(Matrix matrix);
    }
}
