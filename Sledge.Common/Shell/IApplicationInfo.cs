namespace Sledge.Common.Shell
{
    /// <summary>
    /// Exposes various application info to the shell
    /// </summary>
    public interface IApplicationInfo
    {
        /// <summary>
        /// Get the path to the app settings folder.
        /// This is typcailly somewhere in appdata.
        /// </summary>
        /// <param name="subfolder">A path to a subfolder, can be null</param>
        /// <returns>A path to the app settings folder</returns>
        string GetApplicationSettingsFolder(string subfolder);
    }
}
