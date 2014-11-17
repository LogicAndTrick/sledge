using Sledge.EditorNew.Language;
using Sledge.EditorNew.UI;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;

namespace Sledge.EditorNew.Bootstrap
{
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            Settings.SettingsManager.Read();

            // todo language setting
            Translate.SetLanguage("en");
            UIManager.Manager.StringProvider = Translate.StringProvider;

            Shell.Bootstrap();
            CommandBootstrapper.Bootstrap();
            MenuBootstrapper.Bootstrap();
            ToolBootstrapper.Bootstrap();
            SidebarBootstrapper.Bootstrap();
            BrushBootstrapper.Bootstrap();
        }
    }
}
