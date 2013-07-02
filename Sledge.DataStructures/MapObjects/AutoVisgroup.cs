using System;
using System.Collections.Generic;

namespace Sledge.DataStructures.MapObjects
{
    public class AutoVisgroup : Visgroup
    {
        public bool IsHidden { get; set; }
        public Func<MapObject, bool> Filter { get; set; }
        public List<Visgroup> Children { get; set; }

        protected AutoVisgroup()
        {
            Children = new List<Visgroup>();
        }
    }
}