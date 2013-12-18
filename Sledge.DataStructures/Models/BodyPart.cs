using System.Collections.Generic;

namespace Sledge.DataStructures.Models
{
    public class BodyPart
    {
        public string Name { get; private set; }
        public Dictionary<int, List<Mesh>> Meshes { get; private set; }
        public int ActiveMesh { get; private set; }

        public BodyPart(string name)
        {
            Name = name;
            Meshes = new Dictionary<int, List<Mesh>>();
            ActiveMesh = 0;
        }

        public void AddMesh(int groupId, Mesh mesh)
        {
            if (!Meshes.ContainsKey(groupId)) Meshes.Add(groupId, new List<Mesh>());
            Meshes[groupId].Add(mesh);
        }

        public void Activate(int groupId)
        {
            ActiveMesh = groupId;
        }

        public IEnumerable<Mesh> GetActiveGroup()
        {
            return Meshes.ContainsKey(ActiveMesh) ? Meshes[ActiveMesh] : new List<Mesh>();
        }
    }
}