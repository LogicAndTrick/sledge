using System;

namespace Sledge.Common.Shell.Menu
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MenuItemAttribute : Attribute
    {
        public string Section { get; }
        public string Path { get; }
        public string Group { get; }

        public MenuItemAttribute(string section, string path, string group)
        {
            Section = section;
            Path = path;
            Group = group;
        }
    }
}