using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Sledge.Common.Translations
{
    /// <summary>
    /// A catalog which loads and stores all languages and translation strings.
    /// </summary>
    [Export]
    public class TranslationStringsCatalog
    {
        private readonly List<string> _loaded;
        public Dictionary<string, Language> Languages { get; }

        [ImportingConstructor]
        public TranslationStringsCatalog()
        {
            Languages = new Dictionary<string, Language>();
            _loaded = new List<string>();

#if DEBUG
            Languages["debug_en"] = new Language("debug_en") { Description = "Debug (english fallback)", Inherit = "en" };
            Languages["debug_keys"] = new Language("debug_keys") { Description = "Debug (keys)" };
            Languages["debug_keys_long"] = new Language("debug_keys_long") { Description = "Debug (long keys)" };
            Languages["debug_blank"] = new Language("debug_blank") { Description = "Debug (no fallback)" };
#endif
        }

        public string GetString(string language, string key)
        {
            // Basic loop prevention
            for (var i = 0; i < 4; i++)
            {
                if (!Languages.ContainsKey(language)) break;
                var lang = Languages[language];
                if (lang.Collection.Strings.ContainsKey(key)) return lang.Collection.Strings[key];
                if (String.IsNullOrWhiteSpace(lang.Inherit)) break;
                language = lang.Inherit;
            }
            return null;
        }

        public string GetSetting(string language, string key)
        {
            // Basic loop prevention
            for (var i = 0; i < 4; i++)
            {
                if (!Languages.ContainsKey(language)) break;
                var lang = Languages[language];
                if (lang.Collection.Settings.ContainsKey(key)) return lang.Collection.Settings[key];
                if (String.IsNullOrWhiteSpace(lang.Inherit)) break;
                language = lang.Inherit;
            }
            return null;
        }

        public void Load(Type type)
        {
            var loc = type.Assembly.Location ?? "";
            if (_loaded.Contains(loc)) return;

            _loaded.Add(loc);
            if (!File.Exists(loc)) return;

            var dir = Path.Combine(Path.GetDirectoryName(loc) ?? "", "Translations");
            if (!Directory.Exists(dir)) return;

            foreach (var file in Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
            {
                LoadFile(file);
            }
        }

        private void LoadFile(string file)
        {
            var text = File.ReadAllText(file, Encoding.UTF8);
            var obj = JObject.Parse(text);

            var meta = obj["@Meta"];
            if (meta == null) return;

            var lang = Convert.ToString(meta["Language"]);
            if (String.IsNullOrWhiteSpace(lang)) return;

            var basePath = Convert.ToString(meta["Base"]) ?? "";
            if (!String.IsNullOrWhiteSpace(basePath)) basePath += ".";
            
            Language language;
            if (!Languages.ContainsKey(lang))
            {
                language = new Language(lang);
                Languages[lang] = language;
            }
            language = Languages[lang];
            
            var langDesc = Convert.ToString(meta["LanguageDescription"]);
            if (!string.IsNullOrWhiteSpace(langDesc) && string.IsNullOrWhiteSpace(language.Description)) language.Description = langDesc;

            var inherit = Convert.ToString(meta["Inherit"]);
            if (!string.IsNullOrWhiteSpace(inherit) && string.IsNullOrWhiteSpace(language.Inherit)) language.Inherit = inherit;

            var strings = obj.Descendants()
                .OfType<JProperty>()
                .Where(x => x.Path[0] != '@')
                .Where(x => x.Value.Type == JTokenType.String);
            foreach (var st in strings)
            {
#if DEBUG
                Languages["debug_blank"].Collection.Strings[basePath + st.Path] = "--";
                Languages["debug_keys"].Collection.Strings[basePath + st.Path] = "[" + (basePath + st.Path).Split('.').LastOrDefault() + "]";
                Languages["debug_keys_long"].Collection.Strings[basePath + st.Path] = "[" + (basePath + st.Path) + "]";
#endif
                language.Collection.Strings[basePath + st.Path] = st.Value?.ToString();
            }

            if (obj["@Settings"] is JObject settingsNode)
            {
                var settings = settingsNode.Descendants()
                    .OfType<JProperty>()
                    .Where(x => x.Value.Type == JTokenType.String);
                foreach (var se in settings)
                {
                    if (se.Name.StartsWith("@")) language.Collection.Settings[se.Name] = se.Value?.ToString();
                    else language.Collection.Settings[basePath + GetSettingPath(se)] = se.Value?.ToString();
                }
            }
        }

        private string GetSettingPath(JToken token)
        {
            var l = new List<string>();
            while (token != null)
            {
                if (token is JProperty)
                {
                    var name = ((JProperty) token).Name;
                    if (name.StartsWith("@")) break;
                    l.Add(name);
                }
                token = token.Parent;
            }
            l.Reverse();
            return String.Join(".", l);
        }
    }
}