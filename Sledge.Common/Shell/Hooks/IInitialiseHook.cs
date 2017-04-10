using System.Threading.Tasks;

namespace Sledge.Common.Shell.Hooks
{
    /// <summary>
    /// A hook that runs just after startup
    /// </summary>
    public interface IInitialiseHook
    {
        /// <summary>
        /// Runs when initialised, after startup
        /// </summary>
        /// <returns></returns>
        Task OnInitialise();
    }
}