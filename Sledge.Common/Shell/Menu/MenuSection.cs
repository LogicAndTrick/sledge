namespace Sledge.Common.Shell.Menu
{
    public class MenuSection
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrderHint { get; set; }

        public MenuSection(string name, string description, string orderHint)
        {
            Name = name;
            Description = description;
            OrderHint = orderHint;
        }
    }
}