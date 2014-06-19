using System;
using System.Linq;
using Sledge.DataStructures.MapObjects.VisgroupFilters;

namespace Sledge.DataStructures.MapObjects
{
    public class AutoVisgroup : Visgroup
    {
        public bool IsHidden { get; set; }
        public Func<MapObject, bool> Filter { get; set; }

        public override bool IsAutomatic { get { return true; } }

        public override Visgroup Clone()
        {
            return new AutoVisgroup
            {
                ID = ID,
                Name = Name,
                Visible = Visible,
                Colour = Colour,
                Children = Children.Select(x => x.Clone()).ToList(),
                IsHidden = IsHidden,
                Filter = Filter
            };
        }

        public static AutoVisgroup GetDefaultAutoVisgroup()
        {
            var filters = typeof (IVisgroupFilter).Assembly.GetTypes()
                .Where(x => typeof (IVisgroupFilter).IsAssignableFrom(x))
                .Where(x => !x.IsInterface)
                .Select(Activator.CreateInstance)
                .OfType<IVisgroupFilter>();
            var i = -1;
            var auto = new AutoVisgroup
                           {
                               ID = i--,
                               IsHidden = false,
                               Name = "Auto",
                               Visible = true
                           };
            foreach (var f in filters)
            {
                var parent = auto.Children.OfType<AutoVisgroup>().FirstOrDefault(x => x.Name == f.Group);
                if (parent == null)
                {
                    parent = new AutoVisgroup
                                 {
                                     ID = i--,
                                     IsHidden = false,
                                     Name = f.Group,
                                     Visible = true,
                                     Parent = auto
                                 };
                    auto.Children.Add(parent);
                }
                var vg = new AutoVisgroup
                             {
                                 ID = i--,
                                 Filter = f.IsMatch,
                                 IsHidden = false,
                                 Name = f.Name,
                                 Visible = true,
                                 Parent = parent
                             };
                parent.Children.Add(vg);
            }
            return auto;
        }
    }
}