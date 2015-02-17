using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Sledge.Rendering.DataStructures
{
    public class Line
    {
        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }

        public Line(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }
}
