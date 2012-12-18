using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.MapObjects
{
    public class Output
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public string Input { get; set; }
        public string Parameter { get; set; }
        public decimal Delay { get; set; }
        public bool OnceOnly { get; set; }

        public Output Clone()
        {
            return new Output
                       {
                           Name = Name,
                           Target = Target,
                           Input = Input,
                           Parameter = Parameter,
                           Delay = Delay,
                           OnceOnly = OnceOnly
                       };
        }
    }
}
