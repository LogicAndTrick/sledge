namespace Sledge.Common.Shell.Menu
{
    public class MenuSection
    {
        public string Name { get; set; }
        public string OrderHint { get; set; }

        public MenuSection(string name, string orderHint)
        {
            Name = name;
            OrderHint = orderHint;
        }
    }
}