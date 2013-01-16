using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering
{
    /// <summary>
    /// The array manager controls the rendering of the map via the solid vertex arrays it maintains.
    /// </summary>
    public class ArrayManager
    {
        private readonly Dictionary<Solid, SolidVertexArray> _arrays;
        private List<IGrouping<SolidVertexArraySubset, SolidVertexArraySubset>> _cache;
        
        public ArrayManager(Map map)
        {
            _arrays = new Dictionary<Solid, SolidVertexArray>();
            Update(map);
        }

        public void Update(Map map)
        {
            foreach (Solid solid in map.WorldSpawn.Find(x => x is Solid))
            {
                if (!_arrays.ContainsKey(solid)) _arrays.Add(solid, new SolidVertexArray(solid));
                else _arrays[solid].Update(solid);
            }
            _cache = _arrays.Values.SelectMany(x => x.Subsets).GroupBy(x => x).ToList();
        }

        public void Draw3D(object context, ShaderProgram program)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            program.Set("currentTexture", 0);
            foreach (var group in _cache)
            {
                var tex = group.Key.Texture;
                var sel = group.Key.IsSelected;
                if (tex != null) tex.Bind();
                program.Set("isTextured", tex != null);
                program.Set("isSelected", sel);
                foreach (var ts in group) ts.DrawFilled(context, program);
                if (sel)
                {
                    program.Set("isWireframe", true);
                    foreach (var ts in group) ts.DrawWireframe(context, program);
                    program.Set("isWireframe", false);
                }
            }
        }

        public void Draw2D(object context, ShaderProgram program)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            program.Set("isTextured", false);
            program.Set("currentTexture", 0);
            program.Set("isWireframe", true);
            //selectedwireframecolour
            foreach (var group in _cache)
            {
                var sel = group.Key.IsSelected;
                program.Set("isSelected", sel);
                foreach (var ts in group) ts.DrawWireframe(context, program);
            }
            program.Set("isWireframe", false);
        }
    }
}
