using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Camera
    {
        public Coordinate EyePosition { get; set; }
        public Coordinate LookPosition { get; set; }

        public Camera()
        {
            EyePosition = new Coordinate(0, 0, 0);
            LookPosition = new Coordinate(0, 0, 0);
        }

        public decimal Length
        {
            get { return (LookPosition - EyePosition).VectorMagnitude(); }
            set { LookPosition = EyePosition + Direction * value; }
        }

        public Coordinate Direction
        {
            get { return (LookPosition - EyePosition).Normalise(); }
            set { LookPosition = EyePosition + value.Normalise() * Length; }
        }
    }
}
