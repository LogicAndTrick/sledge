using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.Common.Shell.Menu;

namespace Sledge.BspEditor.Editing
{
    [Export(typeof(IMenuMetadataProvider))]
    public class MenuDataProvider : IMenuMetadataProvider
    {
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

            yield return new MenuGroup("Tools", "", "Evil", "B");
            yield return new MenuGroup("Tools", "", "Entity", "H");
            yield return new MenuGroup("Tools", "", "Transform", "L");
            yield return new MenuGroup("Tools", "", "Snap", "N");
            yield return new MenuGroup("Tools", "", "Align", "P");
            yield return new MenuGroup("Tools", "", "Flip", "R");
        }
    }
}