using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

namespace Sledge.Shell
{
    /// <summary>
    /// Runs after the shell has been created and before any other hook.
    /// </summary>
    internal interface IShellStartupHook
    {
        /// <summary>
        /// Runs on shell startup
        /// </summary>
        /// <param name="shell">The shell instance</param>
        /// <param name="container">The container</param>
        /// <returns>The running task</returns>
        Task OnStartup(Forms.Shell shell, CompositionContainer container);
    }
}
