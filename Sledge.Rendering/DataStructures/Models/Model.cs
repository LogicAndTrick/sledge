using System.Collections.Generic;
using System.Linq;
using OpenTK;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.DataStructures.Models
{
    public class Model : IBounded, IUpdatable
    {
        public Vector3 Origin { get { return BoundingBox.Center; } }
        public Box BoundingBox { get; private set; }

        public List<Mesh> Meshes { get; set; }
        public Animation Animation { get; set; }

        private int _currentFrame;

        public Model(List<Mesh> meshes)
        {
            Meshes = meshes;
            BoundingBox = new Box(meshes.Select(x => x.BoundingBox));
        }

        private long _lastFrame = -1;
        public void Update(Frame frame)
        {
            return;
            if (Animation == null || Animation.Frames.Count <= 1) return;
            var millisecondsPerFrame = 1000f / Animation.FramesPerSecond;

            if (_lastFrame >= 0 && frame.Milliseconds - _lastFrame >= millisecondsPerFrame)
            {
                _currentFrame = (_currentFrame + 1) % Animation.Frames.Count;
                _lastFrame = frame.Milliseconds;
            }
            if (_lastFrame < 0) _lastFrame = frame.Milliseconds;
        }

        public Matrix4[] GetCurrentTransforms()
        {
            if (Animation == null) return new Matrix4[] { Matrix4.Identity };
            return Animation.Frames[_currentFrame].Transforms.ToArray();
        } 
    }
}
