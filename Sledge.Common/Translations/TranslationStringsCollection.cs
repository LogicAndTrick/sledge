using System;
using System.Collections.Generic;

namespace Sledge.Common.Translations
{
    internal class TranslationStringsCollection
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