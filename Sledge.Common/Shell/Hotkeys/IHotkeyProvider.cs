using System.Collections.Generic;

namespace Sledge.Common.Shell.Hotkeys
{
    /// <summary>
    /// An interface which provides hotkeys from a source.
    /// </summary>
    public interface IHotkeyProvider
    {
        /// <summary>
        /// Get the list of hotkeys
        /// </summary>
        /// <returns>Hotkey list</returns>
        IEnumerable<IHotkey> GetHotkeys();
    }
}