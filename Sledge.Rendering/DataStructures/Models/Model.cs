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

        public Model(List<Mesh> meshes)
        {
            Meshes = meshes;
            BoundingBox = new Box(meshes.Select(x => x.BoundingBox));
        }

        public void Update(Frame frame)
        {
            // blah blah animation frames etc
        }
    }
}
