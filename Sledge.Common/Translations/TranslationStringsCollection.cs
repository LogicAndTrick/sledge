using System.Collections.Generic;

namespace Sledge.Common.Translations
{
    /// <summary>
    /// A collection of translation strings and settings.
    /// </summary>
    public class TranslationStringsCollection
    {
        public Dictionary<string, string> Settings { get; set; }
        public Dictionary<string, string> Strings { get; set; }

        public TranslationStringsCollection()
        {
            Settings = new Dictionary<string, string>();
            Strings = new Dictionary<string, string>();
        }
    }
}