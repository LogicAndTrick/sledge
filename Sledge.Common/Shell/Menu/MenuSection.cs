namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// A section is a top-level menu. For example: File, Edit, Tools, etc.
    /// </summary>
    public class MenuSection
    {
        /// <summary>
        /// The name of the section
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The display name of the section
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// An order hint for the section
        /// </summary>
        public string OrderHint { get; set; }

        public MenuSection(string name, string description, string orderHint)
        {
            Name = name;
            Description = description;
            OrderHint = orderHint;
        }
    }
}