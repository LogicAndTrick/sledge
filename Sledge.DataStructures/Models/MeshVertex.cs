using System.Collections.Generic;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Models
{
    public class MeshVertex
    {
        public CoordinateF Location { get; private set; }
        public IEnumerable<BoneWeighting> BoneWeightings { get; private set; }
        public float TextureU { get; private set; }
        public float TextureV { get; private set; }

        public MeshVertex(CoordinateF location, IEnumerable<BoneWeighting> boneWeightings, float textureU, float textureV)
        {
            Location = location;
            BoneWeightings = boneWeightings;
            TextureU = textureU;
            TextureV = textureV;
        }

        public MeshVertex(CoordinateF location,Bone bone, float textureU, float textureV)
        {
            Location = location;
            BoneWeightings = new List<BoneWeighting> {new BoneWeighting(bone, 1)};
            TextureU = textureU;
            TextureV = textureV;
        }
    }
}