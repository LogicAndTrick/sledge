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

            yield return new MenuGroup("Tools", "", "Evil", "B");
            yield return new MenuGroup("Tools", "", "Entity", "D");
            yield return new MenuGroup("Tools", "", "Transform", "H");
        }
    }
}