using OpenTK;

namespace Sledge.Rendering.Scenes.Elements
{
    public class Position
    {
        public Vector3 Location { get; set; }
        public Vector3 Offset { get; set; }
        public bool Normalised { get; set; }

        public Position(Vector3 location)
        {
            Location = location;
            Normalised = false;
            Offset = Vector3.Zero;
        }

        public Position Clone()
        {
            return new Position(Location) {Offset = Offset, Normalised = Normalised};
        }
    }
}