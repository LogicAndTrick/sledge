using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Microsoft.VisualBasic.ApplicationServices;

namespace Sledge.Shell
{
    public class SingleInstance : WindowsFormsApplicationBase
    {
        public SingleInstance(Form form)
        {
            IsSingleInstance = true;
            MainForm = form;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            e.BringToForeground = true;
            base.OnStartupNextInstance(e);
            Oy.Publish("Shell:InstanceOpened", e.CommandLine.ToList());
        }
    }
}
