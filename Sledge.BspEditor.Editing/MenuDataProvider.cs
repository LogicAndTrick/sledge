using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing
{
    [AutoTranslate]
    [Export(typeof(IMenuMetadataProvider))]
    public class MenuDataProvider : IMenuMetadataProvider
    {
        public string Flip { get; set; } = "Flip";
        public string Align { get; set; } = "Align";

        public IEnumerable<MenuSection> GetMenuSections()
        {
            yield break;
        }

        public IEnumerable<MenuGroup> GetMenuGroups()
        {
            yield return new MenuGroup("Menu", "", "Build", "F");

            yield return new MenuGroup("Edit", "", "Properties", "V");

            yield return new MenuGroup("Map", "", "Texture", "F");
            yield return new MenuGroup("Map", "", "Properties", "N");
            yield return new MenuGroup("Map", "", "Pointfile", "P");

            yield return new MenuGroup("View", "", "Selection", "D");
            yield return new MenuGroup("View", "", "GoTo", "F");
            yield return new MenuGroup("View", "", "SplitView", "H");

            yield return new MenuGroup("Tools", "", "Evil", "B");
            yield return new MenuGroup("Tools", "", "Entity", "H");
            yield return new MenuGroup("Tools", "", "Transform", "L");
            yield return new MenuGroup("Tools", "", "Snap", "N");
            yield return new MenuGroup("Tools", "", "FlipAlign", "P");

            yield return new MenuGroup("Tools", "Flip", "Flip", "P1") { Description = Flip };
            yield return new MenuGroup("Tools", "Align", "Align", "P2") { Description = Align };
        }
    }
}