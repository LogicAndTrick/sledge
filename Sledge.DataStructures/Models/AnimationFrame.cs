using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Models
{
    public class AnimationFrame
    {
        public List<BoneAnimationFrame> Bones { get; private set; }

        public AnimationFrame()
        {
            Bones = new List<BoneAnimationFrame>();
        }

        public List<Matrix4> GetBoneTransforms(bool transformBones, bool applyDefaults)
        {
            return Bones.Select(bone => GetAnimationTransform(bone.Bone, transformBones, applyDefaults)).ToList();
        }

        public Matrix4 GetAnimationTransform(Bone b, bool transformBones, bool applyDefaults)
        {
            var m = transformBones ? Matrix4.Identity : GetDefaultBoneTransform(b).Inverted();
            while (b != null)
            {
                var ang = Bones[b.BoneIndex].Angles;
                var pos = Bones[b.BoneIndex].Position;
                if (applyDefaults)
                {
                    ang *= OpenTkExtensions.QuaternionFromEulerRotation(b.DefaultAngles);
                    pos += b.DefaultPosition;
                }
                m *= Matrix4.CreateFromQuaternion(ang) * Matrix4.CreateTranslation(pos);
                //var test = Bones[b.BoneIndex].Angles * QuaternionF.EulerAngles(b.DefaultAngles);
                //m *= test.GetMatrix().Translate(Bones[b.BoneIndex].Position + b.DefaultPosition);
                //m *= Bones[b.BoneIndex].Angles.GetMatrix().Translate(Bones[b.BoneIndex].Position);
                b = b.Parent;
            }
            return m;
        }

        private static Matrix4 GetDefaultBoneTransform(Bone b)
        {
            var m = Matrix4.Identity;
            while (b != null)
            {
                var q = OpenTkExtensions.QuaternionFromEulerRotation(b.DefaultAngles);
                var mat = Matrix4.CreateFromQuaternion(q);
                var transform = mat * Matrix4.CreateTranslation(b.DefaultPosition);
                m *= transform;
                //m *= QuaternionF.EulerAngles(b.DefaultAngles).GetMatrix().Translate(b.DefaultPosition);
                b = b.Parent;
            }
            return m;
        }
    }
}