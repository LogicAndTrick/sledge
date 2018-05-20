using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Sledge.Common.Translations
{
    [Export]
    public class TranslationStringsCatalog
    {
        [ImportMany("AutoTranslate")] private IEnumerable<Lazy<object>> _autoTranslate;

        private List<string> _loaded;
        public Dictionary<string, Language> Languages { get; set; }

        public TranslationStringsCatalog()
        {
            Languages = new Dictionary<string, Language>();
            _loaded = new List<string>();
        }
        
        public void Initialise(string language)
        {
            foreach (var at in _autoTranslate)
            {
                Translate(language, at.Value);
            }
        }

        public void Translate(string language, object target)
        {
            if (target is IManualTranslate mt)
            {
                if (!Languages.ContainsKey(language)) return;
                var strings = Languages[language];
                mt.Translate(strings.Collection);
            }
            else
            {
                Inject(language, target);
            }
        }

        private void Inject(string language, object target)
        {
            if (target == null) return;
            var ty = target.GetType();
            Load(ty);

            if (!Languages.ContainsKey(language)) return;
            var strings = Languages[language];

            var props = ty.GetProperties().Where(x => x.CanWrite);
            foreach (var prop in props)
            {
                var path = ty.FullName + '.' + prop.Name;
                if (strings.Collection.Strings.ContainsKey(path))
                {
                    prop.SetValue(target, strings.Collection.Strings[path]);
                }
            }
        }

        private void Load(Type type)
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

            var strings = obj.Descendants()
                .OfType<JProperty>()
                .Where(x => x.Path[0] != '@')
                .Where(x => x.Value.Type == JTokenType.String);
            foreach (var st in strings)
            {
                language.Collection.Strings[basePath + st.Path] = st.Value?.ToString();
            }

            var settingsNode = obj["@Settings"] as JObject;
            if (settingsNode != null)
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