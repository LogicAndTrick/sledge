using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Shell.Menu;

namespace Sledge.Shell.Components
{
    [Export(typeof(IMenuMetadataProvider))]
    public class DefaultMenuMetadataProvider : IMenuMetadataProvider
    {
        public IEnumerable<MenuSection> GetMenuSections()
        {
            yield return new MenuSection("File", "B");
            yield return new MenuSection("Edit", "D");
            yield return new MenuSection("View", "F");
            yield return new MenuSection("Help", "Y");
        }

        public IEnumerable<MenuGroup> GetMenuGroups()
        {
            yield return new MenuGroup("File", "", "File", "B");
            yield return new MenuGroup("File", "", "Exit", "Y");

            yield return new MenuGroup("Help", "", "About", "Y");
        }
    }
}
