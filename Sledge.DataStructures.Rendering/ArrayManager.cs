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

        public void Draw(ShaderProgram program)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            program.Set("currentTexture", 0);
            foreach (var group in _cache)
            {
                var tex = group.Key.Texture;
                var sel = group.Key.IsSelected;
                if (tex != null) tex.Bind();
                program.Set("isTextured", tex != null);
                program.Set("isSelected", true);
                foreach (var ts in group) ts.Draw(program);
                if (true)
                {
                    program.Set("isWireframe", true);
                    foreach (var ts in group) ts.DrawWireframe(program);
                    program.Set("isWireframe", false);
                }
            }
        }
    }
}
