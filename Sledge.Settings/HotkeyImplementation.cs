namespace Sledge.Settings
{
    public class HotkeyImplementation
    {
        public HotkeyDefinition Definition { get; private set; }
        public string Hotkey { get; private set; }

        public HotkeyImplementation(HotkeyDefinition definition, string hotkey)
        {
            Definition = definition;
            Hotkey = hotkey;
        }
    }
}