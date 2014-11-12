using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    [ControlImplementation("WinForms")]
    public class WinFormsDialogWindow : WinFormsWindow, IDialogWindow
    {
        public override void Open()
        {
            Form.ShowDialog(((WinFormsShell) UIManager.Manager.Shell).Form);
        }
    }
}