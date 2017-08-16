using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;

namespace Sledge.Shell.Translations
{
    [AutoTranslate]
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    [Export(typeof(ITranslationStringProvider))]
    public class TranslationsProvider : IStartupHook, ISettingsContainer, ITranslationStringProvider
    {
        [Import] private TranslationStringsCatalog _catalog;

        public string Language { get; set; } = "en";

        public Task OnStartup()
        {
            // Load language setting early, as most settings are loaded on initialise
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sledge", "Shell", Name + ".json");
            Dictionary<string, string> data = null;
            if (File.Exists(file))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                }
                catch
                {
                    data = null;
                }
            }
            if (data != null && data.ContainsKey("Language"))
            {
                Language = data["Language"];
            }
            _catalog.Initialise(Language);
            return Task.FromResult(0);
        }

        public void Translate(object target)
        {
            if (!_catalog.Languages.ContainsKey(Language)) return;
            _catalog.Translate(Language, target);
        }

        public string GetString(string key)
        {
            if (!_catalog.Languages.ContainsKey(Language)) return null;
            var lang = _catalog.Languages[Language];
            if (!lang.Strings.ContainsKey(key)) return null;
            return lang.Strings[key];
        }

        public string GetSetting(string key)
        {
            if (!_catalog.Languages.ContainsKey(Language)) return null;
            var lang = _catalog.Languages[Language];
            if (!lang.Settings.ContainsKey(key)) return null;
            return lang.Settings[key];
        }

        public string Name => "Sledge.Shell.Translations";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Interface", "Language", typeof(string));
        }

        public void LoadValues(ISettingsStore store)
        {
            Language = store.Get("Language", Language);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Language", Language);
        }
    }
}
