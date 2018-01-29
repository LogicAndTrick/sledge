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
        public static void InvokeSync(this Control control, Action action)
        {
            if (control.InvokeRequired) control.Invoke(new MethodInvoker(() => InvokeSync(control, action)));
            else action();
        }

        public static async Task InvokeAsync(this Control control, Action action)
        {
            InvokeSync(control, action);
            //await Task.Factory.StartNew(() => InvokeSync(control, action)).ConfigureAwait(false);
        }

        public static async Task<DialogResult> ShowDialogAsync(this Form form)
        {
            await Task.Yield();
            return form.ShowDialog();
        }
    }
}
