using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Models
{
    public class Bone
    {
        public int BoneIndex { get; private set; }
        public int ParentIndex { get; private set; }
        public Bone Parent { get; private set; }
        public string Name { get; private set; }
        public CoordinateF DefaultPosition { get; private set; }
        public CoordinateF DefaultAngles { get; private set; }
        public CoordinateF DefaultPositionScale { get; private set; }
        public CoordinateF DefaultAnglesScale { get; private set; }
        public MatrixF Transform { get; private set; }

        public Bone(int boneIndex, int parentIndex, Bone parent, string name,
                    CoordinateF defaultPosition, CoordinateF defaultAngles,
                    CoordinateF defaultPositionScale, CoordinateF defaultAnglesScale)
        {
            BoneIndex = boneIndex;
            ParentIndex = parentIndex;
            Parent = parent;
            Name = name;
            DefaultPosition = defaultPosition;
            DefaultAngles = defaultAngles;
            DefaultPositionScale = defaultPositionScale;
            DefaultAnglesScale = defaultAnglesScale;
            Transform = QuaternionF.EulerAngles(DefaultAngles).GetMatrix().Translate(defaultPosition);
            if (parent != null) Transform *= parent.Transform;
        }
    }
}