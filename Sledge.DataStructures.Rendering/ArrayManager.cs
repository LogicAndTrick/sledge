using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering
{
    /// <summary>
    /// The array manager controls the rendering of the map via the solid vertex arrays it maintains.
    /// </summary>
    public class ArrayManager
    {
        private readonly SolidVertexArray _array;
        public ArrayManager(Map map)
        {
            _array = new SolidVertexArray(map.WorldSpawn.FindAll());
            Update(map);
        }

        public void Update(Map map)
        {
            _array.Update(map.WorldSpawn.FindAll());
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            _array.UpdatePartial(objects);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            _array.UpdatePartial(faces);
        }

        public void DrawTextured(object context, ShaderProgram program)
        {
            _array.Bind(context, 0);
            foreach (var subset in _array.TextureSubsets)
            {
                var tex = subset.Instance;
                if (tex != null) tex.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tex != null);
                _array.Array.DrawElements(0, subset.Start, subset.Count);
            }
            _array.Unbind();
        }

        public void DrawWireframe(object context, ShaderProgram program)
        {
            _array.Bind(context, 1);
            foreach (var subset in _array.WireframeSubsets)
            {
                _array.Array.DrawElements(1, subset.Start, subset.Count);
            }
            _array.Unbind();
        }
    }
}
