using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Forms;
using Sledge.Shell.Input;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The hotkey register registers and handles hotkeys
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    [Export]
    internal class HotkeyRegister : IStartupHook, ISettingsContainer
    {
        // Store the context (the hotkey register is one of the few things that should need static access to the context)
        private readonly IContext _context;
        private readonly IEnumerable<Lazy<ICommand>> _commands;
        private readonly IEnumerable<Lazy<IHotkeyProvider>> _hotkeyProviders;
        private readonly IEnumerable<Lazy<IHotkeyFilter>> _hotkeyFilters;

        /// <summary>
        /// The list of all hotkeys by ID
        /// </summary>
        private readonly Dictionary<string, IHotkey> _hotkeys;

        /// <summary>
        /// The list of registered hotkeys by shortcut
        /// </summary>
        private readonly Dictionary<string, IHotkey> _registeredHotkeys;

        /// <summary>
        /// The list of registered hotkey filters
        /// </summary>
        private readonly List<IHotkeyFilter> _registeredFilters;

        [ImportingConstructor]
        public HotkeyRegister(
            [Import] IContext context, 
            [ImportMany] IEnumerable<Lazy<ICommand>> commands, 
            [ImportMany] IEnumerable<Lazy<IHotkeyProvider>> hotkeyProviders, 
            [ImportMany] IEnumerable<Lazy<IHotkeyFilter>> hotkeyFilters
        )
        {
            _context = context;
            _commands = commands;
            _hotkeyProviders = hotkeyProviders;
            _hotkeyFilters = hotkeyFilters;

            _hotkeys = new Dictionary<string, IHotkey>();
            _registeredHotkeys = new Dictionary<string, IHotkey>();
            _registeredFilters = new List<IHotkeyFilter>();
        }

        public Task OnStartup()
        {
            // Register all commands as hotkeys
            foreach (var export in _commands)
            {
                var ty = export.Value.GetType();
                var dha = ty.GetCustomAttributes(typeof(DefaultHotkeyAttribute), false).OfType<DefaultHotkeyAttribute>().FirstOrDefault();
                Add(new CommandHotkey(export.Value, defaultHotkey: dha?.Hotkey));
            }

            foreach (var hotkey in _hotkeyProviders.SelectMany(x => x.Value.GetHotkeys()))
            {
                Add(hotkey);
            }

            _registeredFilters.AddRange(_hotkeyFilters.Select(x => x.Value));

            Oy.Subscribe<IHotkeyFilter>("Hotkeys:AddFilter", f => _registeredFilters.Add(f));
            Oy.Subscribe<IHotkeyFilter>("Hotkeys:RemoveFilter", f => _registeredFilters.Remove(f));

            // Register this as the hotkey register for all base forms
            BaseForm.HotkeyRegister = this;

            return Task.CompletedTask;
        }

        public IEnumerable<IHotkey> GetHotkeys()
        {
            return _hotkeys.Values;
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
        /// Get a hotkey by its id
        /// </summary>
        public IHotkey GetHotkey(string id)
        {
            return _hotkeys.ContainsKey(id) ? _hotkeys[id] : null;
        }

        /// <summary>
        /// Get the shortcut string for a hotkey, if it's defined
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns></returns>
        public string GetHotkeyString(IHotkey hotkey)
        {
            if (hotkey == null) return null;
            var keys = _registeredHotkeys.Where(x => x.Value == hotkey).Select(x => x.Key).ToList();
            if (keys.Count == 0) return null;

            return keys.Contains(hotkey.DefaultHotkey) ? hotkey.DefaultHotkey : keys.FirstOrDefault();
        }

        /// <summary>
        /// Fire the hotkey (if any) that is registered on a shortcut
        /// </summary>
        /// <param name="keyData">The key event data</param>
        /// <returns>True if the key was registered and was in context</returns>
        internal bool Fire(Keys keyData)
        {
            var cmd = KeyboardState.KeysToString(keyData);
            var keys = (int) keyData;
            if (_registeredFilters.OrderBy(x => x.OrderHint).Any(f => f.Filter(cmd, keys))) return false;

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
            yield return new SettingKey("Hotkeys", "Bindings", typeof(HotkeyBindings));
        }

        public void LoadValues(ISettingsStore store)
        {
            _registeredHotkeys.Clear();
            if (store.Contains("Bindings"))
            {
                var bindings = store.Get("Bindings", new HotkeyBindings());
                foreach (var val in _hotkeys.Keys)
                {
                    var hk = bindings.ContainsKey(val) && bindings[val] != null ? bindings[val] : _hotkeys[val].DefaultHotkey;
                    if (hk != null && !_registeredHotkeys.ContainsKey(hk)) _registeredHotkeys.Add(hk, _hotkeys[val]);
                }
            }
        }

        public void StoreValues(ISettingsStore store)
        {
            var bindings = new HotkeyBindings();
            foreach (var rh in _registeredHotkeys)
            {
                bindings.Add(rh.Value.ID, rh.Key);
            }
            foreach (var hk in _hotkeys)
            {
                if (!bindings.ContainsKey(hk.Key)) bindings[hk.Key] = hk.Value.DefaultHotkey;
            }
            store.Set("Bindings", bindings);
        }

        public class HotkeyBindings : Dictionary<string, string>
        {
            public HotkeyBindings Clone()
            {
                var b = new HotkeyBindings();
                foreach (var kv in this) b[kv.Key] = kv.Value;
                return b;
            }
        }
    }
}
