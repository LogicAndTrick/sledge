using System.Collections.Generic;

namespace Sledge.DataStructures.Models
{
    public class Texture
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Flags { get; set; }
        public System.Drawing.Bitmap Image { get; set; }
    }

    public class Model
    {
        public List<Bone> Bones { get; private set; }
        public List<Mesh> Meshes { get; private set; }
        public List<Animation> Animations { get; private set; }
        public List<Texture> Textures { get; set; }
        public bool BonesTransformMesh { get; set; }

        public Model()
        {
            Bones = new List<Bone>();
            Meshes = new List<Mesh>();
            Animations = new List<Animation>();
            Textures = new List<Texture>();
        }
    }
}