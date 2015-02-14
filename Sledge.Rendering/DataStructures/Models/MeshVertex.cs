using System.Collections.Generic;
using OpenTK;

namespace Sledge.Rendering.DataStructures.Models
{
    public class MeshVertex
    {
        public Vector3 Location { get; set; }
        public Vector3 Normal { get; set; }
        public float TextureU { get; set; }
        public float TextureV { get; set; }
        public Dictionary<int, float> Weightings { get; set; }

        public MeshVertex(Vector3 location, Vector3 normal, float textureU, float textureV, Dictionary<int, float> weightings)
        {
            Location = location;
            Normal = normal;
            TextureU = textureU;
            TextureV = textureV;
            Weightings = weightings;
        }
    }
}