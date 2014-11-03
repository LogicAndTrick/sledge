using System.Collections.Generic;
using System.Linq;

namespace Sledge.EditorNew.UI.Menus
{
    public class RecentFilesMenuGroup : IMenuGroup
    {
        public string Path { get; private set; }

        public RecentFilesMenuGroup(string path)
        {
            Path = path;
        }

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            if (!Settings.SettingsManager.RecentFiles.Any())
            {
                return new[] {new RecentFileMenuItem(null)};
            }
            return Settings.SettingsManager.RecentFiles.Select(x => new RecentFileMenuItem(x));
        }
    }
}