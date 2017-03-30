using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Commands;
using Sledge.Common.Context;
using Sledge.Common.Hooks;
using Sledge.Common.Hotkeys;
using Sledge.Common.Settings;
using Sledge.Shell.Forms;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The hotkey register registers and handles hotkeys
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    internal class HotkeyRegister : IStartupHook, ISettingsContainer
    {
        // Store the context (the hotkey register is one of the few things that should need static access to the context)
        [Import] private IContext _context;
        [ImportMany] private IEnumerable<Lazy<ICommand>> _commands;

        public async Task OnStartup()
        {
            // Register all commands as hotkeys
            foreach (var export in _commands)
            {
                var ty = export.Value.GetType();
                var dha = ty.GetCustomAttributes(typeof(DefaultHotkeyAttribute), false).OfType<DefaultHotkeyAttribute>().FirstOrDefault();
                Add(new CommandHotkey(export.Value, defaultHotkey: dha?.Hotkey));
            }

            // Register this as the hotkey register for all base forms
            BaseForm.HotkeyRegister = this;
        }

        /// <summary>
        /// The list of all hotkeys by ID
        /// </summary>
        private readonly Dictionary<string, IHotkey> _hotkeys;

        /// <summary>
        /// The list of registered hotkeys by shortcut
        /// </summary>
        private readonly Dictionary<string, IHotkey> _registeredHotkeys;

        public HotkeyRegister()
        {
            _hotkeys = new Dictionary<string, IHotkey>();
            _registeredHotkeys = new Dictionary<string, IHotkey>();
        }

        /// <summary>
        /// Add a hotkey to the list but do not register it
        /// </summary>
        /// <param name="hotkey">The hotkey to add</param>
        private void Add(IHotkey hotkey)
        {
            _hotkeys[hotkey.ID] = hotkey;
        }

        /// <summary>
        /// Fire the hotkey (if any) that is registered on a shortcut
        /// </summary>
        /// <param name="keyData">The key event data</param>
        /// <returns>True if the key was registered and was in context</returns>
        internal bool Fire(Keys keyData)
        {
            var cmd = KeysToString(keyData);
            if (_registeredHotkeys.ContainsKey(cmd))
            {
                var hk = _registeredHotkeys[cmd];
                if (hk.IsInContext(_context))
                {
                    _registeredHotkeys[cmd].Invoke();
                    return true;
                }
            }
            return false;
        }

        // Settings provider
        // The settings provider is the one that registers hotkey shortcuts.
        // Even if no settings exist, it will register the default hotkeys.

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
                var hk = svs.ContainsKey(val) && svs[val] != null ? svs[val] : _hotkeys[val].DefaultHotkey;
                if (hk != null && !_registeredHotkeys.ContainsKey(hk)) _registeredHotkeys.Add(hk, _hotkeys[val]);
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
