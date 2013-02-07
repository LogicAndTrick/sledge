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
        private readonly DecalFaceVertexArray _decalArray;

        public ArrayManager(Map map)
        {
            var all = map.WorldSpawn.FindAll();
            _array = new SolidVertexArray(all);
            _decalArray = new DecalFaceVertexArray(all);
            // Update(map);
        }

        public void Update(Map map)
        {
            var all = map.WorldSpawn.FindAll();
            _array.Update(all);
            _decalArray.Update(all);
        }

        public void UpdateDecals(Map map)
        {
            var all = map.WorldSpawn.FindAll();
            _decalArray.Update(all);
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
            // Todo abstract this out a bit, repeated code all over the place
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
            _decalArray.Bind(context, 0);
            foreach (var subset in _decalArray.TextureSubsets)
            {
                var tex = subset.Instance;
                if (tex != null) tex.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tex != null);
                _array.Array.DrawElements(0, subset.Start, subset.Count);
            }
            _decalArray.Unbind();
        }

        public void DrawWireframe(object context, ShaderProgram program)
        {
            _array.Bind(context, 1);
            foreach (var subset in _array.WireframeSubsets)
            {
                _array.Array.DrawElements(1, subset.Start, subset.Count);
            }
            _array.Unbind();
            _decalArray.Bind(context, 1);
            foreach (var subset in _decalArray.WireframeSubsets)
            {
                _decalArray.Array.DrawElements(1, subset.Start, subset.Count);
            }
            _decalArray.Unbind();
        }
    }
}
