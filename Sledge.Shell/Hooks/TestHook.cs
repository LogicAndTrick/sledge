using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Commands;
using Sledge.Common.Hooks;

namespace Sledge.Shell.Hooks
{
    [Export(typeof(IStartupHook))]
    [Export(typeof(IShutdownHook))]
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(IShuttingDownHook))]
    public class TestHook : IStartupHook, IShutdownHook, IInitialiseHook, IShuttingDownHook
    {
        public async Task OnStartup(CompositionContainer container)
        {
            Console.WriteLine("Startup");
        }

        public async Task OnInitialise(CompositionContainer container)
        {
            Console.WriteLine("Initialise");
        }

        public async Task OnShutdown()
        {
            Console.WriteLine("Shutdown");
        }

        public async Task<bool> OnShuttingDown()
        {
            return true;
        }
    }
}
