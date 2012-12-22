using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.MapObjects
{
    public class Property
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Property Clone()
        {
            return new Property
                       {
                           Key = Key,
                           Value = Value
                       };
        }
    }
}
