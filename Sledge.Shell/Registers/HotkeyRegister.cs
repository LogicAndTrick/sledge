using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Commands;
using Sledge.Common.Hooks;
using Sledge.Common.Hotkeys;
using Sledge.Common.Settings;
using Sledge.Shell.Forms;

namespace Sledge.Shell.Registers
{
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HotkeyRegister : IStartupHook, ISettingsContainer
    {
        public async Task OnStartup(CompositionContainer container)
        {
            // Register all commands as hotkeys
            foreach (var export in container.GetExports<ICommand>())
            {
                var ty = export.Value.GetType();
                var dha = ty.GetCustomAttributes(typeof(DefaultHotkeyAttribute), false).OfType<DefaultHotkeyAttribute>().FirstOrDefault();
                Add(new CommandHotkey(export.Value, defaultHotkey: dha?.Hotkey));
            }

            BaseForm.HotkeyRegister = this;
        }

        private readonly Dictionary<string, IHotkey> _hotkeys;
        private readonly Dictionary<string, IHotkey> _registeredHotkeys;

        public HotkeyRegister()
        {
            _hotkeys = new Dictionary<string, IHotkey>();
            _registeredHotkeys = new Dictionary<string, IHotkey>();
        }

        private void Add(IHotkey hotkey)
        {
            _hotkeys[hotkey.ID] = hotkey;
        }

        internal bool Fire(Keys keyData)
        {
            var cmd = KeysToString(keyData);
            if (_registeredHotkeys.ContainsKey(cmd))
            {
                // todo !hotkeys check context?
                _registeredHotkeys[cmd].Invoke();
                return true;
            }
            return false;
        }
        
        // Settings provider
        public string Name => "Sledge.Shell.Hotkeys";

        public IEnumerable<SettingKey> GetKeys()
        {
            return _hotkeys.Select(x => new SettingKey(x.Value.ID, x.Value.Name, typeof(string)));
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            _registeredHotkeys.Clear();
            var svs = values.ToDictionary(x => x.Name, x => x.Value);
            foreach (var val in _hotkeys.Keys)
            {
                var hk = svs.ContainsKey(val) ? svs[val] : _hotkeys[val].DefaultHotkey;
                if (hk != null) _registeredHotkeys.Add(hk, _hotkeys[val]);
            }
        }

        public IEnumerable<SettingValue> GetValues()
        {
            var reg = _registeredHotkeys.ToDictionary(x => x.Value.ID, x => x.Key);
            foreach (var hk in _hotkeys)
            {
                if (!reg.ContainsKey(hk.Key)) reg[hk.Key] = hk.Value.DefaultHotkey;
            }
            return reg.Select(x => new SettingValue(x.Key, x.Value));
        }

        // Hotkey utilities
        private static readonly Dictionary<string, string> KeyStringReplacements;

        static HotkeyRegister()
        {
            KeyStringReplacements = new Dictionary<string, string>
                                        {
                                            {"Add", "+"},
                                            {"Oemplus", "+"},
                                            {"Subtract", "-"},
                                            {"OemMinus", "-"},
                                            {"Separator", "-"},
                                            {"Decimal", "."},
                                            {"OemPeriod", "."},
                                            {"Divide", "/"},
                                            {"OemQuestion", "/"},
                                            {"Multiply", "*"},
                                            {"OemBackslash", "\\"},
                                            {"Oem5", "\\"},
                                            {"OemCloseBrackets", "]"},
                                            {"Oem6", "]"},
                                            {"OemOpenBrackets", "["},
                                            {"OemPipe", "|"},
                                            {"OemQuotes", "'"},
                                            {"Oem7", "'"},
                                            {"OemSemicolon", ";"},
                                            {"Oem1", ";"},
                                            {"Oemcomma", ","},
                                            {"Oemtilde", "`"},
                                            {"Back", "Backspace"},
                                            {"Return", "Enter"},
                                            {"Next", "PageDown"},
                                            {"Prior", "PageUp"},
                                            {"D1", "1"},
                                            {"D2", "2"},
                                            {"D3", "3"},
                                            {"D4", "4"},
                                            {"D5", "5"},
                                            {"D6", "6"},
                                            {"D7", "7"},
                                            {"D8", "8"},
                                            {"D9", "9"},
                                            {"D0", "0"},
                                            {"Delete", "Del"}
                                        };
        }

        private static string KeysToString(Keys key)
        {
            // KeysConverter seems to ignore the invariant culture, manually replicate the results
            var mods = key & Keys.Modifiers;
            var keycode = key & Keys.KeyCode;
            if (keycode == Keys.None) return "";

            var str = keycode.ToString();
            if (KeyStringReplacements.ContainsKey(str)) str = KeyStringReplacements[str];

            // Modifier order: Ctrl+Alt+Shift+Key
            return (mods.HasFlag(Keys.Control) ? "Ctrl+" : "")
                   + (mods.HasFlag(Keys.Alt) ? "Alt+" : "")
                   + (mods.HasFlag(Keys.Shift) ? "Shift+" : "")
                   + str;
        }
    }
}
