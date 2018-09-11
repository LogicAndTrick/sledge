using System;

namespace Sledge.Common.Shell.Hotkeys
{
    /// <summary>
    /// A hotkey that can be attached to a class to indicate the default hotkey sequence.
    /// </summary>
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