using System.Collections.Generic;
using OpenTK;

namespace Sledge.DataStructures.Models
{
    public class MeshVertex
    {
        public Vector3 Location { get; set; }
        public Vector3 Normal { get; set; }
        public IEnumerable<BoneWeighting> BoneWeightings { get; private set; }
        public float TextureU { get; set; }
        public float TextureV { get; set; }

        public MeshVertex(Vector3 location, Vector3 normal, IEnumerable<BoneWeighting> boneWeightings, float textureU, float textureV)
        {
            Location = location;
            Normal = normal;
            BoneWeightings = boneWeightings;
            TextureU = textureU;
            TextureV = textureV;
        }

        public MeshVertex(Vector3 location, Vector3 normal, Bone bone, float textureU, float textureV)
        {
            Location = location;
            Normal = normal;
            BoneWeightings = new List<BoneWeighting> {new BoneWeighting(bone, 1)};
            TextureU = textureU;
            TextureV = textureV;
        }
    }
}