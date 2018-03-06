using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;
using LogicAndTrick.Oy;

namespace Sledge.Shell
{
    /// <summary>
    /// Class to bootstrap the shell
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Run the shell as an application using a container from the application catalog
        /// </summary>
        public static void Run()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new ApplicationCatalog());
            var container = new CompositionContainer(catalog);

            Run(container);
        }

        /// <summary>
        /// Run the shell with a custom container
        /// </summary>
        /// <param name="container">The container</param>
        public static void Run(CompositionContainer container)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => UnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => UnhandledException(e.ExceptionObject as Exception);
            
            var shell = container.GetExport<Forms.Shell>().Value;
            var si = new SingleInstance(shell);
            
            si.UnhandledException += (s, e) => UnhandledException(e.Exception);
            
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