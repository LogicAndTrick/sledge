using System;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common;

namespace Sledge.Shell
{
    /// <summary>
    /// Class to bootstrap the shell
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Called before the composition container is created, but after the default catalog has been created.
        /// </summary>
        public static event EventHandler<AggregateCatalog> BuildCatalog;

        /// <summary>
        /// Run the shell as an application using a container from the application catalog
        /// </summary>
        public static void Run()
        {
            var catalog = new AggregateCatalog();
            
            catalog.Catalogs.Add(new ValidAssembliesInDirectoryContainer(AppDomain.CurrentDomain.BaseDirectory));
            BuildCatalog?.Invoke(null, catalog);

            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            Run(container);
        }

        /// <summary>
        /// Run the shell with a custom container
        /// </summary>
        /// <param name="container">The container</param>
        public static void Run(CompositionContainer container)
        {
            Common.Container.Initialise(container);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => UnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => UnhandledException(e.ExceptionObject as Exception);

            Oy.UnhandledException += (s, e) => UnhandledException(e.Exception);
            
            var shell = container.GetExport<Forms.Shell>().Value;
            var si = new SingleInstance(shell);
            
            si.UnhandledException += (s, e) =>
            {
                e.ExitApplication = false;
                UnhandledException(e.Exception);
            };
            
            si.Run(Environment.GetCommandLineArgs());
        }

        private static void UnhandledException(Exception ex)
        {
            if (ex == null) return;
            try
            {
                Oy.Publish("Shell:UnhandledException", ex);
            }
            catch
            {
                // Exception in an exception? Oh dear...
            }
        }
    }
}