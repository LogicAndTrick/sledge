using OpenTK;

namespace Sledge.DataStructures.Models
{
    public class BoneAnimationFrame
    {
        public Bone Bone { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Angles { get; private set; }

        public BoneAnimationFrame(Bone bone, Vector3 position, Quaternion angles)
        {
            Bone = bone;
            Position = position;
            Angles = angles;
        }
    }
}