using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
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
    public class ArrayManager : IDisposable
    {
        private MoVbo _vbo;
        //private readonly SolidVertexArray _array;
        //private readonly DecalFaceVertexArray _decalArray;

        public ArrayManager(Map map)
        {
            var all = GetAllVisible(map.WorldSpawn);
            //_array = new SolidVertexArray(all);
            //_decalArray = new DecalFaceVertexArray(all);

            _vbo = new MoVbo(all);
        }

        public void Dispose()
        {
            //_array.Dispose();
            //_decalArray.Dispose();

            _vbo.Dispose();
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
            _vbo.Update(all);
            //_array.Update(all);
            //_decalArray.Update(all);
        }

        public void UpdateDecals(Map map)
        {
            var all = GetAllVisible(map.WorldSpawn);
            //_decalArray.Update(all);
        }

        public void UpdatePartial(IEnumerable<MapObject> objects)
        {
            //_array.UpdatePartial(objects);
            _vbo.UpdatePartial(objects);
        }

        public void UpdatePartial(IEnumerable<Face> faces)
        {
            //_array.UpdatePartial(faces);
            _vbo.UpdatePartial(faces);
        }

        public void DrawTextured(IGraphicsContext context, Coordinate cameraLocation, ShaderProgram program)
        {
            _vbo.RenderTextured(context, program);
            _vbo.RenderTransparent(context, program, cameraLocation);
            // todo decals
        }

        public void DrawWireframe(IGraphicsContext context, ShaderProgram program)
        {

            _vbo.RenderWireframe(context, program);
        }
    }
}
