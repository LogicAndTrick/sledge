using Sledge.EditorNew.Commands;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI.Menus;

namespace Sledge.EditorNew.Bootstrap
{
    public static class MenuBootstrapper
    {
        public static void Bootstrap()
        {
            MenuManager.Add(new BasicMenuGroup("File")
            {
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileNew), Resources.Menu_New, true, true),
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileOpen), Resources.Menu_Open, true, true),
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileClose), Resources.Menu_Close, true, true),
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileSave), Resources.Menu_Save, true, true),
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileSaveAs), Resources.Menu_SaveAs, true, true),
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileExport), Resources.Menu_Export, true, true),
            });
            MenuManager.Add(new BasicMenuGroup("File")
            {
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileRun), Resources.Menu_Run, true, true),
            });
            MenuManager.Add(new RecentFilesMenuGroup("File"));
            MenuManager.Add(new BasicMenuGroup("File")
            {
                new CommandMenuItem(CommandManager.GetCommand(DefaultCommands.FileExit), null, true, false),
            });

            MenuManager.Build();
        }
    }
}