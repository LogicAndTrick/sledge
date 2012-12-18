using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.GameData
{
    public class GameDataObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ClassType ClassType { get; set; }
        public List<string> BaseClasses { get; private set; }
        public List<Behaviour> Behaviours { get; private set; }
        public List<Property> Properties { get; private set; }
        public List<IO> InOuts { get; private set; }

        public GameDataObject(string name, string description, ClassType classType)
        {
            Name = name;
            Description = description;
            ClassType = classType;
            BaseClasses = new List<string>();
            Behaviours = new List<Behaviour>();
            Properties = new List<Property>();
            InOuts = new List<IO>();
        }
    }
}
