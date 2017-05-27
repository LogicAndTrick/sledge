using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Shell
{
    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            if (!control.IsHandleCreated) return;

            if (control.InvokeRequired) control.Invoke((MethodInvoker) delegate { action(); });
            else action();
        }

        public static async Task<DialogResult> ShowDialogAsync(this Form form)
        {
            await Task.Yield();
            return form.ShowDialog();
        }
    }
}
