using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

namespace Sledge.Shell
{
    internal interface IShellStartupHook
    {
        Task OnStartup(Forms.Shell shell, CompositionContainer container);
    }
}
