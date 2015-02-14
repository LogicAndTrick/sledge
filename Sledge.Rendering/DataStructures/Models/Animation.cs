using System.Collections.Generic;

namespace Sledge.Rendering.DataStructures.Models
{
    public class Animation
    {
        public List<AnimationFrame> Frames { get; set; }

        public Animation(List<AnimationFrame> frames)
        {
            Frames = frames;
        }
    }
}