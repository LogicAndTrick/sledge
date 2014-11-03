using System.Drawing;
using Sledge.EditorNew.Commands;

namespace Sledge.EditorNew.UI.Menus
{
    public class CommandMenuItem : IMenuItem
    {
        private readonly ICommand _command;
        public string TextKey { get { return _command.TextKey; } }
        public string Text { get; private set; }
        public Image Image { get; set; }
        public bool IsActive { get { return CommandManager.HasContext(_command.Context); } }
        public bool ShowInMenu { get; set; }
        public bool ShowInToolstrip { get; set; }

        public CommandMenuItem(ICommand command, Image image, bool showInMenu, bool showInToolstrip)
        {
            _command = command;
            Image = image;
            ShowInMenu = showInMenu;
            ShowInToolstrip = showInToolstrip;
        }

        public void Execute()
        {
            _command.Fire();
        }
    }
}