using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Hooks;

namespace Sledge.Shell
{
    /// <summary>
    /// The class that bootstraps the whole thing.
    /// </summary>
    [Export]
    internal class Bootstrapper
    {
        [ImportMany] private IEnumerable<Lazy<IStartupHook>> _startupHooks;
        [ImportMany] private IEnumerable<Lazy<IInitialiseHook>> _initialiseHooks;
        [ImportMany] private IEnumerable<Lazy<IShuttingDownHook>> _shuttingDownHooks;
        [ImportMany] private IEnumerable<Lazy<IShutdownHook>> _shutdownHooks;

        /// <summary>
        /// Run all shell-startup and startup hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Startup()
        {
            foreach (var export in _startupHooks.Select(x => x.Value).OrderBy(x => x.GetType().FullName))
            {
                Log.Debug("Bootstrapper", "Startup hook: " + export.GetType().FullName);
                await export.OnStartup();
            }
        }

        /// <summary>
        /// Run all initialise hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Initialise()
        {
            foreach (var export in _initialiseHooks.Select(x => x.Value).OrderBy(x => x.GetType().FullName))
            {
                Log.Debug("Bootstrapper", "Initialise hook: " + export.GetType().FullName);
                await export.OnInitialise();
            }
        }

        /// <summary>
        /// Run all shutting down hooks and stops if cancelled
        /// </summary>
        /// <returns>False if shutdown cannot continue</returns>
        public async Task<bool> ShuttingDown()
        {
            foreach (var export in _shuttingDownHooks.Select(x => x.Value).OrderBy(x => x.GetType().FullName))
            {
                Log.Debug("Bootstrapper", "Shutting down hook: " + export.GetType().FullName);
                if (!await export.OnShuttingDown()) return false;
            }
            return true;
        }

        /// <summary>
        /// Run all shutdown hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Shutdown()
        {
            foreach (var export in _shutdownHooks.Select(x => x.Value).OrderBy(x => x.GetType().FullName))
            {
                Log.Debug("Bootstrapper", "Shutdown hook: " + export.GetType().FullName);
                await export.OnShutdown();
            }
        }
    }
}