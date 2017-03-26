using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using Sledge.Common.Hooks;

namespace Sledge.Shell
{
    /// <summary>
    /// The class that bootstraps the whole thing.
    /// </summary>
    internal static class Bootstrapping
    {
        private static readonly CompositionContainer Container;
        
        static Bootstrapping()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new ApplicationCatalog());
            Container = new CompositionContainer(catalog);
        }

        /// <summary>
        /// Run all shell-startup and startup hooks
        /// </summary>
        /// <param name="shell">The shell</param>
        /// <returns>The running task</returns>
        public static async Task Startup(Forms.Shell shell)
        {
            foreach (var export in Container.GetExports<IShellStartupHook>())
            {
                await export.Value.OnStartup(shell, Container);
            }

            foreach (var export in Container.GetExports<IStartupHook>())
            {
                await export.Value.OnStartup(Container);
            }
        }

        /// <summary>
        /// Run all initialise hooks
        /// </summary>
        /// <param name="t">The continuation task</param>
        /// <returns>The running task</returns>
        public static async Task Initialise(Task t)
        {
            foreach (var export in Container.GetExports<IInitialiseHook>())
            {
                await export.Value.OnInitialise(Container);
            }
        }

        /// <summary>
        /// Run all shutting down hooks and stops if cancelled
        /// </summary>
        /// <returns>False if shutdown cannot continue</returns>
        public static async Task<bool> ShuttingDown()
        {
            foreach (var export in Container.GetExports<IShuttingDownHook>())
            {
                if (!await export.Value.OnShuttingDown()) return false;
            }
            return true;
        }

        /// <summary>
        /// Run all shutdown hooks
        /// </summary>
        /// <returns>The running task</returns>
        public static async Task Shutdown()
        {
            foreach (var export in Container.GetExports<IShutdownHook>())
            {
                await export.Value.OnShutdown();
            }
        }
    }
}