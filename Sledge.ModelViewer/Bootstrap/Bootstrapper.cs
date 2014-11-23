using Sledge.Gui;
using Sledge.ModelViewer.UI;

namespace Sledge.ModelViewer.Bootstrap
{
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            Settings.SettingsManager.Read();

            // todo language setting
            //Translate.SetLanguage("en");
            //UIManager.Manager.StringProvider = Translate.StringProvider;

            Shell.Bootstrap();
        }
    }
}
