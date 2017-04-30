using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;

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
            
            var shell = container.GetExport<Forms.Shell>().Value;
            Application.Run(shell);
        }
    }
}