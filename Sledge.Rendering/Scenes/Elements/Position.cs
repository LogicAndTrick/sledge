using OpenTK;

namespace Sledge.Rendering.Scenes.Elements
{
    public class Position
    {
        public PositionType Type { get; set; }
        public Vector3 Location { get; set; }
        public Vector3 Offset { get; set; }
        public bool Normalised { get; set; }

        public Position(PositionType type, Vector3 location)
        {
            Type = type;
            Location = location;
            Normalised = false;
            Offset = Vector3.Zero;
        }
    }
}