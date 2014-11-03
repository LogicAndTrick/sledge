using Sledge.EditorNew.Commands;
using Sledge.Settings;

namespace Sledge.EditorNew.Bootstrap
{
    public static class CommandBootstrapper
    {
        public static void Bootstrap()
        {
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileNew, DefaultCommandContexts.None, HotkeysMediator.FileNew));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileOpen, DefaultCommandContexts.None, HotkeysMediator.FileOpen));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileClose, DefaultCommandContexts.Document, HotkeysMediator.FileClose));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileSave, DefaultCommandContexts.Document, HotkeysMediator.FileSave));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileSaveAs, DefaultCommandContexts.Document, HotkeysMediator.FileSaveAs));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileExport, DefaultCommandContexts.Document, HotkeysMediator.FileExport));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileRun, DefaultCommandContexts.Document, HotkeysMediator.FileCompile));
            CommandManager.Register(new MediatorCommand(DefaultCommandGroups.GroupFile, DefaultCommands.FileExit, DefaultCommandContexts.None, HotkeysMediator.FileExport));
        }
    }
}