using System;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using Sledge.Common.Hooks;

namespace Sledge.Shell
{
    internal static class Bootstrapping
    {
        private static readonly CompositionContainer Container;

        static Bootstrapping()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new ApplicationCatalog());
            Container = new CompositionContainer(catalog);
        }

        public static async Task Startup()
        {
            foreach (var export in Container.GetExports<IStartupHook>())
            {
                await export.Value.OnStartup(Container);
            }
        }

        public static async Task Initialise(Task t)
        {
            foreach (var export in Container.GetExports<IInitialiseHook>())
            {
                await export.Value.OnInitialise(Container);
            }
        }

        public static async Task<bool> ShuttingDown()
        {
            foreach (var export in Container.GetExports<IShuttingDownHook>())
            {
                if (!await export.Value.OnShuttingDown()) return false;
            }
            return true;
        }

        public static async Task Shutdown()
        {
            foreach (var export in Container.GetExports<IShutdownHook>())
            {
                await export.Value.OnShutdown();
            }
        }
    }
}