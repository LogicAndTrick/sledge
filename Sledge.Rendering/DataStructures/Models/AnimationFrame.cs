using System.Collections.Generic;
using OpenTK;

namespace Sledge.Rendering.DataStructures.Models
{
    public class AnimationFrame
    {
        public List<Matrix4> Transforms { get; set; }

        public AnimationFrame(List<Matrix4> transforms)
        {
            Transforms = transforms;
        }
    }
}