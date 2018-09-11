namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// A menu group is a sub-list within a menu section. Groups are separated by splitters.
    /// The group may be a sub-group, which will appear as a sub-menu.
    /// </summary>
    public class MenuGroup
    {
        /// <summary>
        /// The menu section for this group
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// The path to the sub-group this group falls within
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The name of this group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An order hint for this group
        /// </summary>
        public string OrderHint { get; set; }

        /// <summary>
        /// The description of this group
        /// </summary>
        public string Description { get; set; }

        public MenuGroup(string section, string path, string name, string orderHint)
        {
            Section = section;
            Path = path;
            Name = name;
            OrderHint = orderHint;
        }
    }
}