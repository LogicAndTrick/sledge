namespace Sledge.DataStructures.Models
{
    public class BoneWeighting
    {
        public Bone Bone { get; private set; }
        public float Weight { get; private set; }

        public BoneWeighting(Bone bone, float weight)
        {
            Bone = bone;
            Weight = weight;
        }
    }
}