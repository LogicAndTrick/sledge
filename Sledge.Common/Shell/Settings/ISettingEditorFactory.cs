namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// Creates settings editors
    /// </summary>
    public interface ISettingEditorFactory
    {
        /// <summary>
        /// A sort order for this factory
        /// </summary>
        string OrderHint { get; }

        /// <summary>
        /// Checks if this factory can create an editor for a particular key
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if this factory can create an editor for this key</returns>
        bool Supports(SettingKey key);

        /// <summary>
        /// Create an editor for a setting key
        /// </summary>
        /// <param name="key">The setting key</param>
        /// <returns>An editor for the key</returns>
        ISettingEditor CreateEditorFor(SettingKey key);
    }
}