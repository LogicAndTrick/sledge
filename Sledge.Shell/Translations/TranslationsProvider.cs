using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;

namespace Sledge.Shell.Translations
{
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    public class TranslationsProvider : IStartupHook, ISettingsContainer
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

        public string Name => "Sledge.Shell.Translations";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Language", typeof(string));
        }

        public void SetValues(ISettingsStore store)
        {
            Language = store.Get("Language", Language);
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("Language", Language);
        }
    }
}
