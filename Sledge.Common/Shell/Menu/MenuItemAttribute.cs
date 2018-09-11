using System;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// An attribute that can be attached to a class to indicate that it should have a menu item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MenuItemAttribute : Attribute
    {
        public string Section { get; }
        public string Path { get; }
        public string Group { get; }
        public string OrderHint { get; }

        public MenuItemAttribute(string section, string path, string group, string orderHint)
        {
            Section = section;
            Path = path;
            Group = group;
            OrderHint = orderHint;
        }
    }
}