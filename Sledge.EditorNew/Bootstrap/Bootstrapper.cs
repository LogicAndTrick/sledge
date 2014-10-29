using Sledge.EditorNew.Commands;
using Sledge.EditorNew.Language;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI.Menus;
using Sledge.Gui;
using Sledge.Settings;

namespace Sledge.EditorNew.Bootstrap
{
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            // todo language setting
            Translate.SetLanguage("en");
            UIManager.Manager.StringProvider = Translate.StringProvider;

            CommandBootstrapper.Bootstrap();
            MenuBootstrapper.Bootstrap();
        }
    }

    public static class CommandBootstrapper
    {
        public static void Bootstrap()
        {
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileNew, DefaultCommandContexts.None, HotkeysMediator.FileNew));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileOpen, DefaultCommandContexts.None, HotkeysMediator.FileOpen));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileSave, DefaultCommandContexts.None, HotkeysMediator.FileSave));
        }
    }

    public static class MenuBootstrapper
    {
        public static void Bootstrap()
        {
            MenuManager.Add(new BasicMenuItem(CommandManager.GetCommand(DefaultCommands.FileNew), Resources.Menu_New, true, true, true));
            MenuManager.Add(new BasicMenuItem(CommandManager.GetCommand(DefaultCommands.FileOpen), Resources.Menu_Open, true, true, true));
            MenuManager.Add(new BasicMenuItem(CommandManager.GetCommand(DefaultCommands.FileSave), Resources.Menu_Save, true, true, true));

            MenuManager.Build();
        }
    }
}
