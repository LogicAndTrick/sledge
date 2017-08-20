using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.Shell.Components
{
    [AutoTranslate]
    [Export(typeof(IMenuMetadataProvider))]
    public class DefaultMenuMetadataProvider : IMenuMetadataProvider
    {
        public string File { get; set; } = "File";
        public string Edit { get; set; } = "Edit";
        public string View { get; set; } = "View";
        public string Tools { get; set; } = "Tools";
        public string Help { get; set; } = "Help";

        public IEnumerable<MenuSection> GetMenuSections()
        {
            yield return new MenuSection("File", File, "B");
            yield return new MenuSection("Edit", Edit, "D");
            yield return new MenuSection("View", View, "F");
            yield return new MenuSection("Tools", Tools, "R");
            yield return new MenuSection("Help", Help, "Y");
        }

        public IEnumerable<MenuGroup> GetMenuGroups()
        {
            yield return new MenuGroup("File", "", "File", "B");
            yield return new MenuGroup("File", "", "Recent", "W");
            yield return new MenuGroup("File", "", "Exit", "Y");

            yield return new MenuGroup("Tools", "", "Settings", "Y");

            yield return new MenuGroup("Help", "", "About", "Y");
        }
    }
}
