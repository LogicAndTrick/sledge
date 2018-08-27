namespace Sledge.Common.Shell.Hooks
{
    /// <summary>
    /// A hook that runs when shutting down, on the UI thread.
    /// </summary>
    public interface IUIShutdownHook
    {
        /// <summary>
        /// Runs on shutdown
        /// </summary>
        /// <returns></returns>
        void OnUIShutdown();
    }
}