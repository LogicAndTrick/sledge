using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Shell
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Invoke a delegate synchronously on the UI thread.
        /// Blocks the UI until complete. Use with caution.
        /// </summary>
        /// <param name="control">The control to invoke upon</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeSync(this Control control, Action action)
        {
            if (control.InvokeRequired) control.Invoke(new MethodInvoker(() => action()));
            else action();
        }
        
        /// <summary>
        /// Invoke a delegate asynchronously on the UI thread and return an awaitable task.
        /// Awaiting the task will block the UI until complete. Use with caution.
        /// </summary>
        /// <param name="control">The control to invoke upon</param>
        /// <param name="action">The delegate to run</param>
        /// <returns>The task that will resolve once the delegate is complete</returns>
        public static async Task InvokeAsync(this Control control, Action action)
        {
            var asr = control.BeginInvoke(action);
            await Task.Factory.FromAsync(asr, control.EndInvoke);
        }
        
        /// <summary>
        /// Invoke a delegate asynchronously on the UI thread and return immediately.
        /// The delegate will run at some unknown time. Use carefully.
        /// </summary>
        /// <param name="control">The control to invoke upon</param>
        /// <param name="action">The delegate to run</param>
        public static void InvokeLater(this Control control, Action action)
        {
            if (!control.IsHandleCreated) return;
            control.BeginInvoke(action);
        }
        
        public static Task InvokeLaterAsync(this Control control, Action action)
        {
            if (!control.IsHandleCreated) return Task.FromResult(0);
            var tcs = new TaskCompletionSource<int>();
            control.BeginInvoke((MethodInvoker)(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(0);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Opens a dialog and returns an awaitable task that will resolve once the dialog has closed.
        /// </summary>
        /// <param name="form">The dialog to open</param>
        /// <returns>The task that will resolve once the dialog is closed</returns>
        public static async Task<DialogResult> ShowDialogAsync(this Form form)
        {
            await Task.Yield();
            return form.ShowDialog();
        }
    }
}
