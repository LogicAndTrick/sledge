using System;

namespace Sledge.Common.Shell.Hotkeys
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DefaultHotkeyAttribute : Attribute
    {
        public string Hotkey { get; private set; }

        public DefaultHotkeyAttribute(string hotkey)
        {
            Hotkey = hotkey;
        }
    }
}