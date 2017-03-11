using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Vertex : ISerializable
    {
        public Coordinate Location { get; set; }

        public Face Parent { get; set; }

        public Vertex(Coordinate location, Face parent)
        {
            Location = location;
            Parent = parent;
        }

        protected Vertex(SerializationInfo info, StreamingContext context)
        {
            Location = (Coordinate) info.GetValue("Location", typeof (Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
        }

        public Vertex Clone()
        {
            return new Vertex(Location.Clone(), Parent);
        }
    }
}
