using System.Collections.Generic;

namespace Sledge.Rendering.DataStructures.Models
{
    public class Animation
    {
        public int FramesPerSecond { get; private set; }
        public List<AnimationFrame> Frames { get; set; }

        public Animation(int fps, List<AnimationFrame> frames)
        {
            FramesPerSecond = fps;
            Frames = frames;
        }
    }
}