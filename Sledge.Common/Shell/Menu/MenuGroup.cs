namespace Sledge.Common.Shell.Menu
{
    public class MenuGroup
    {
        public string Section { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string OrderHint { get; set; }

        public MenuGroup(string section, string path, string name, string orderHint)
        {
            Section = section;
            Path = path;
            Name = name;
            OrderHint = orderHint;
        }
    }
}