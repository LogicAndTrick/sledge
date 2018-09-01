using System.Threading.Tasks;

namespace Sledge.Common.Shell.Hooks
{
    /// <summary>
    /// A hook that runs when a shutdown is requested, enabling cancellation
    /// </summary>
    public interface IShuttingDownHook
    {
        /// <summary>
        /// Runs when a shutdown is requested
        /// </summary>
        /// <returns>True if shutdown is ok, false otherwise</returns>
        Task<bool> OnShuttingDown();
    }
}