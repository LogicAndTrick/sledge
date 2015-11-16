using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.GameData
{
    public class Option
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public bool On { get; set; }

        public string DisplayText()
        {
            return String.IsNullOrWhiteSpace(Description) ? Key : Description;
        }
    }
}
