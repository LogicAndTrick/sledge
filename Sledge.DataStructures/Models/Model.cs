using System.Collections.Generic;

namespace Sledge.DataStructures.Models
{
    public class Model
    {
        public List<Bone> Bones { get; private set; }
        public List<Mesh> Meshes { get; private set; }
        public List<Animation> Animations { get; private set; }
        public bool BonesTransformMesh { get; set; }

        public Model()
        {
            Bones = new List<Bone>();
            Meshes = new List<Mesh>();
            Animations = new List<Animation>();
        }
    }
}