using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
            foreach (var export in _startupHooks)
            {
                Log.Debug("Bootstrapper", "Startup hook: " + export.Value.GetType().FullName);
                await export.Value.OnStartup();
            }
        }

        /// <summary>
        /// Run all initialise hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Initialise()
        {
            foreach (var export in _initialiseHooks)
            {
                Log.Debug("Bootstrapper", "Initialise hook: " + export.Value.GetType().FullName);
                await export.Value.OnInitialise();
            }
        }

        /// <summary>
        /// Run all shutting down hooks and stops if cancelled
        /// </summary>
        /// <returns>False if shutdown cannot continue</returns>
        public async Task<bool> ShuttingDown()
        {
            foreach (var export in _shuttingDownHooks)
            {
                Log.Debug("Bootstrapper", "Shutting down hook: " + export.Value.GetType().FullName);
                if (!await export.Value.OnShuttingDown()) return false;
            }
            return true;
        }

        /// <summary>
        /// Run all shutdown hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Shutdown()
        {
            foreach (var export in _shutdownHooks)
            {
                Log.Debug("Bootstrapper", "Shutdown hook: " + export.Value.GetType().FullName);
                await export.Value.OnShutdown();
            }
        }
    }
}