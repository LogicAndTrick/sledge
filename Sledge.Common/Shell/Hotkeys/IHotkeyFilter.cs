namespace Sledge.Common.Shell.Hotkeys
{
    /// <summary>
    /// Hotkey filters provide a component a chance to consume a key before it is fired.
    /// </summary>
    public interface IHotkeyFilter
    {
        /// <summary>
        /// An ordering hint for the hotkey filter
        /// </summary>
        string OrderHint { get; }

        /// <summary>
        /// Filter the hotkey, returning true if the hotkey has been consumed by the filter.
        /// </summary>
        /// <param name="hotkey">The hotkey string, for example <code>Ctrl+C</code>.</param>
        /// <param name="keys">The Windows Keys value for the hotkey</param>
        /// <returns>True to consume the hotkey, false to continue</returns>
        bool Filter(string hotkey, int keys);
    }
}