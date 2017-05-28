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

        public static Task InvokeAsync(this Control control, Action action)
        {
            if (!control.IsHandleCreated) return Task.FromResult(false);
            var tcs = new TaskCompletionSource<bool>();

            if (control.InvokeRequired)
            {
                var res = control.BeginInvoke(action);
                return Task.Factory.FromAsync(res, r => tcs.SetResult(true));
            }
            else
            {
                action();
                tcs.SetResult(true);
            }

            return tcs.Task;
        }

        public static async Task<DialogResult> ShowDialogAsync(this Form form)
        {
            await Task.Yield();
            return form.ShowDialog();
        }
    }
}
