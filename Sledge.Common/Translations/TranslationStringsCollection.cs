using System;
using System.Collections.Generic;

namespace Sledge.Common.Translations
{
    public class TranslationStringsCollection
    {
        public Dictionary<string, string> Settings { get; set; }
        public Dictionary<string, string> Strings { get; set; }

        public TranslationStringsCollection()
        {
            Settings = new Dictionary<string, string>();
            Strings = new Dictionary<string, string>();
        }

        public string GetString(params string[] path)
        {
            var key = String.Join(".", path);
            return Strings.ContainsKey(key) ? Strings[key] : "";
        }
    }
}