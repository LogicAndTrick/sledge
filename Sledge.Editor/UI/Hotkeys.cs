using System;
using System.Reflection;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.UI;

namespace Sledge.Editor.UI
{
    public static class Hotkeys
    {
        public static bool HotkeyDown(Keys keyData)
        {
            var keyCombination = KeyboardState.KeysToString(keyData);
            var hotkeyDefinition = Sledge.Settings.Hotkeys.GetHotkeyFor(keyCombination);
            if (hotkeyDefinition != null)
            {
                Mediator.Publish(hotkeyDefinition.Action, hotkeyDefinition.Parameter);
                return true;
            }
            return false;
        }
    }
}
