using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.GameData;
using Property = Sledge.DataStructures.MapObjects.Property;

namespace Sledge.Editor.UI.ObjectProperties
{
    internal class TableValue
    {
        public string Class { get; set; }
        public string OriginalKey { get; set; }
        public string NewKey { get; set; }
        public string Value { get; set; }
        public bool IsModified { get; set; }
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }

        public Color GetColour()
        {
            if (IsAdded) return Color.LightBlue;
            if (IsRemoved) return Color.LightPink;
            if (IsModified) return Color.LightGreen;
            return Color.Transparent;
        }

        public string DisplayText(GameData gd)
        {
            var cls = gd.Classes.FirstOrDefault(x => x.Name == Class);
            var prop = cls == null ? null : cls.Properties.FirstOrDefault(x => x.Name == NewKey);
            return prop == null ? NewKey : prop.DisplayText();
        }

        public string DisplayValue(GameData gd)
        {
            var cls = gd.Classes.FirstOrDefault(x => x.Name == Class);
            var prop = cls == null ? null : cls.Properties.FirstOrDefault(x => x.Name == OriginalKey && x.VariableType == VariableType.Choices);
            var opt = prop == null ? null : prop.Options.FirstOrDefault(x => x.Key == Value);
            return opt == null ? Value : opt.Description;
        }

        public static List<TableValue> Create(GameData gd, string className, List<Property> props)
        {
            var list = new List<TableValue>();
            var cls = gd.Classes.FirstOrDefault(x => x.Name == className);
            var gameDataProps = cls != null ? cls.Properties : new List<DataStructures.GameData.Property>();
            foreach (var gdProps in gameDataProps.Where(x => x.Name != "spawnflags").GroupBy(x => x.Name))
            {
                var gdProp = gdProps.First();
                var vals = props.Where(x => x.Key == gdProp.Name).Select(x => x.Value).Distinct().ToList();
                var value = vals.Count == 0 ? gdProp.DefaultValue : (vals.Count == 1 ? vals.First() : "<multiple values>" + String.Join(", ", vals));
                list.Add(new TableValue { Class = className, OriginalKey = gdProp.Name, NewKey = gdProp.Name, Value = value});
            }
            foreach (var group in props.Where(x => gameDataProps.All(y => x.Key != y.Name)).GroupBy(x => x.Key))
            {
                var vals = @group.Select(x => x.Value).Distinct().ToList();
                var value = vals.Count == 1 ? vals.First() : "<multiple values> - " + String.Join(", ", vals);
                list.Add(new TableValue { Class = className, OriginalKey = @group.Key, NewKey = @group.Key, Value = value });
            }
            return list;
        }
    }
}