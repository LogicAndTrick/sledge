using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
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
            var all = GetAllVisible(map.WorldSpawn);
            _array = new SolidVertexArray(all);
            _decalArray = new DecalFaceVertexArray(all);
        }

        private IList<MapObject> GetAllVisible(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).ToList();
        }

        private void FindRecursive(ICollection<MapObject> items, MapObject root, Predicate<MapObject> matcher)
        {
            if (!matcher(root)) return;
            items.Add(root);
            root.Children.ForEach(x => FindRecursive(items, x, matcher));
        }

        public void Update(Map map)
        {
            var all = GetAllVisible(map.WorldSpawn);
            _array.Update(all);
            _decalArray.Update(all);
        }

        public void UpdateDecals(Map map)
        {
            var all = GetAllVisible(map.WorldSpawn);
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

        public void DrawTextured(object context, Coordinate cameraLocation, ShaderProgram program)
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
            _array.Bind(context, 1);
            foreach (var subset in _array.TransparentSubsets.OrderByDescending(x => (cameraLocation - x.Instance.Origin).LengthSquared()))
            {
                var tf = subset.Instance;
                if (tf.Texture != null) tf.Texture.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tf.Texture != null);
                _array.Array.DrawElements(0, subset.Start, subset.Count);
            }
            _array.Unbind();
            GL.Disable(EnableCap.CullFace);
            _decalArray.Bind(context, 0);
            foreach (var subset in _decalArray.TextureSubsets)
            {
                var tex = subset.Instance;
                if (tex != null) tex.Bind();
                else TextureHelper.Unbind();
                program.Set("isTextured", tex != null);
                _decalArray.Array.DrawElements(0, subset.Start, subset.Count);
            }
            _decalArray.Unbind();
            GL.Enable(EnableCap.CullFace);
        }

        public void DrawWireframe(object context, ShaderProgram program)
        {
            _array.Bind(context, 2);
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
