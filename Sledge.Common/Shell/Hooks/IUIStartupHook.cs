namespace Sledge.Common.Shell.Hooks
{
    /// <summary>
    /// A hook that runs at startup. Will run synchronously on the UI thread.
    /// </summary>
    public interface IUIStartupHook
    {
        /// <summary>
        /// Runs on startup
        /// </summary>
        /// <returns></returns>
        void OnUIStartup();
    }
}