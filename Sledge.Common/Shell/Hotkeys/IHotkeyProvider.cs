using System.Collections.Generic;

namespace Sledge.Common.Shell.Hotkeys
{
    public interface IHotkeyProvider
    {
        IEnumerable<IHotkey> GetHotkeys();
    }
}