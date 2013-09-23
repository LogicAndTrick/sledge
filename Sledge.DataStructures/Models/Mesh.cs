using System.Collections.Generic;

namespace Sledge.DataStructures.Models
{
    public class Mesh
    {
        public int LOD { get; set; }
        public int SkinRef { get; set; }
        public List<MeshVertex> Vertices { get; private set; }

        public Mesh(int lod)
        {
            LOD = lod;
            Vertices = new List<MeshVertex>();
        }
    }
}