using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
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
        private readonly List<IStartupHook> _startupHooks;
        private readonly List<IUIStartupHook> _uiStartupHooks;
        private readonly List<IInitialiseHook> _initialiseHooks;
        private readonly List<IShuttingDownHook> _shuttingDownHooks;
        private readonly List<IUIShutdownHook> _uiShutdownHooks;
        private readonly List<IShutdownHook> _shutdownHooks;

        [ImportingConstructor]
        public Bootstrapper
        (
            [ImportMany] IEnumerable<Lazy<IStartupHook>> startupHooks,
            [ImportMany] IEnumerable<Lazy<IUIStartupHook>> uiStartupHooks,
            [ImportMany] IEnumerable<Lazy<IInitialiseHook>> initialiseHooks,
            [ImportMany] IEnumerable<Lazy<IShuttingDownHook>> shuttingDownHooks,
            [ImportMany] IEnumerable<Lazy<IUIShutdownHook>> uiShutdownHooks,
            [ImportMany] IEnumerable<Lazy<IShutdownHook>> shutdownHooks
        )
        {
            _startupHooks = startupHooks.Select(x => x.Value).ToList();
            _uiStartupHooks = uiStartupHooks.Select(x => x.Value).ToList();
            _initialiseHooks = initialiseHooks.Select(x => x.Value).ToList();
            _shuttingDownHooks = shuttingDownHooks.Select(x => x.Value).ToList();
            _uiShutdownHooks = uiShutdownHooks.Select(x => x.Value).ToList();
            _shutdownHooks = shutdownHooks.Select(x => x.Value).ToList();
        }

        private void Handle<T>(string description, T hook, Action<T> function)
        {
            try
            {
#if DEBUG
                Log.Debug("Bootstrapper", description + ": " + hook.GetType().FullName);
#endif
                function(hook);
            }
            catch (Exception e)
            {
                Oy.Publish("Shell:UnhandledException", e);
            }
        }

        private async Task Handle<T>(string description, T hook, Func<T, Task> function)
        {
            try
            {
#if DEBUG
                Log.Debug("Bootstrapper", description + ": " + hook.GetType().FullName);
#endif
                await function(hook);
            }
            catch (Exception e)
            {
                Oy.Publish("Shell:UnhandledException", e);
            }
        }

        /// <summary>
        /// Run all UI startup hooks
        /// </summary>
        public void UIStartup()
        {
            foreach (var hook in _uiStartupHooks.OrderBy(x => x.GetType().Name))
            {
                Handle("UI Startup hook", hook, h => h.OnUIStartup());
            }
        }

        /// <summary>
        /// Run all startup hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Startup()
        {
            foreach (var export in _startupHooks.OrderBy(x => x.GetType().FullName))
            {
                await Handle("Startup hook", export, h => h.OnStartup());
            }
        }

        /// <summary>
        /// Run all initialise hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Initialise()
        {
            foreach (var export in _initialiseHooks.OrderBy(x => x.GetType().FullName))
            {
                await Handle("Initialise hook", export, h => h.OnInitialise());
            }
        }

        /// <summary>
        /// Run all shutting down hooks and stops if cancelled
        /// </summary>
        /// <returns>False if shutdown cannot continue</returns>
        public async Task<bool> ShuttingDown()
        {
            foreach (var export in _shuttingDownHooks.OrderBy(x => x.GetType().FullName))
            {
                try
                {
                    Log.Debug("Bootstrapper", "Shutting down hook: " + export.GetType().FullName);
                    if (!await export.OnShuttingDown()) return false;
                }
                catch
                {
                    // We're shutting down anyway, so don't report this one
                }
            }
            return true;
        }

        /// <summary>
        /// Run all UI shutdown hooks
        /// </summary>
        public void UIShutdown()
        {
            foreach (var hook in _uiShutdownHooks.OrderBy(x => x.GetType().Name))
            {
                Handle("UI Shutdown hook", hook, h => h.OnUIShutdown());
            }
        }

        /// <summary>
        /// Run all shutdown hooks
        /// </summary>
        /// <returns>The running task</returns>
        public async Task Shutdown()
        {
            foreach (var export in _shutdownHooks.OrderBy(x => x.GetType().FullName))
            {
                await Handle("Shutdown hook", export, h => h.OnShutdown());
            }
        }
    }
}