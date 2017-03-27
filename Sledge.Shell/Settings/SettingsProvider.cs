using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;
using LogicAndTrick.Oy;
using Newtonsoft.Json;
using Sledge.Common.Hooks;
using Sledge.Common.Settings;

namespace Sledge.Shell.Settings
{
    /// <summary>
    /// Manages all settings
    /// </summary>
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(IShutdownHook))]
    public class SettingsProvider : SyncResourceProvider<SettingValue>, IInitialiseHook, IShutdownHook
    {
        [ImportMany] private IEnumerable<Lazy<ISettingsContainer>> _settingsContainers;

        public async Task OnInitialise()
        {
            // Register all settings containers
            foreach (var export in _settingsContainers)
            {
                Add(export.Value);
            }

            // Load the settings
            await LoadSettings(null);

            // Listen for setting events
            Oy.Subscribe<string>("Settings:Load", LoadSettings);
            Oy.Subscribe<string>("Settings:Save", SaveSettings);
        }
        
        public async Task OnShutdown()
        {
            // Save settings on close
            await SaveSettings(null);
        }

        private readonly Dictionary<string, List<SettingValue>> _values;
        private readonly List<ISettingsContainer> _containers;

        public SettingsProvider()
        {
            _values = new Dictionary<string, List<SettingValue>>();
            _containers = new List<ISettingsContainer>();
        }

        /// <summary>
        /// Register a settings container
        /// </summary>
        /// <param name="settingsContainer">The settings container</param>
        private void Add(ISettingsContainer settingsContainer)
        {
            _containers.Add(settingsContainer);
        }

        /// <summary>
        /// Load settings from disk
        /// </summary>
        /// <param name="name">The name of the settings module to load, or null to load all</param>
        /// <returns>A task that completes when the load is complete</returns>
        private async Task LoadSettings(string name)
        {
            if (name == null) _values.Clear();
            else if (_values.ContainsKey(name)) _values.Remove(name);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sledge", "Shell");
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                var containerName = Path.GetFileNameWithoutExtension(file);

                if (containerName == null) continue;
                if (name != null && containerName != name) continue;

                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                _values[containerName] = data.Select(x => new SettingValue(x.Key, x.Value)).ToList();
            }

            foreach (var container in _containers)
            {
                if (name != null && container.Name != name) continue;
                container.SetValues(_values.ContainsKey(container.Name) ? _values[container.Name] : new List<SettingValue>());
            }
        }

        /// <summary>
        /// Save settings to disk
        /// </summary>
        /// <param name="name">The name of the settings module to save, or null to save all</param>
        /// <returns>A task that complete when the operation is complete</returns>
        private async Task SaveSettings(string name)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sledge", "Shell");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            foreach (var container in _containers)
            {
                if (name != null && container.Name != name) continue;
                var values = container.GetValues();
                File.WriteAllText(Path.Combine(path, container.Name + ".json"), JsonConvert.SerializeObject(values.ToDictionary(x => x.Name, x => x.Value), Formatting.Indented));
            }
        }

        // Resource provider

        public override bool CanProvide(string location)
        {
            return location.StartsWith("settings://");
        }

        public override IEnumerable<SettingValue> Fetch(string location, List<string> resources)
        {
            var loc = location.Substring(11);
            if (!_values.ContainsKey(loc)) return new SettingValue[0];

            if (resources == null)
            {
                return _values[loc];
            }
            else
            {
                var vals = _values[loc];
                return resources.Select(x => vals.FirstOrDefault(y => y.Name == x)).Where(x => x != null);
            }
        }
    }
}
