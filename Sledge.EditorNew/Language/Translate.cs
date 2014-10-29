using System.IO;
using Sledge.Gui.Resources;

namespace Sledge.EditorNew.Language
{
    public static class Translate
    {
        private static string _language;
        private static TranslationStringCollection _collection;
        private static IStringProvider _provider;

        public static IStringProvider StringProvider
        {
            get { return _provider; }
        }

        static Translate()
        {
            _provider = new TranslationStringProvider();
            SetLanguage("en");
        }

        public static void SetLanguage(string language)
        {
            _language = language;
            _collection = new TranslationStringCollection(_language, GetTranslationsFolder());
        }

        private static string GetTranslationsFolder()
        {
            var dir = Path.GetDirectoryName(typeof (Translate).Assembly.Location);
            return Path.Combine(dir, "Translations");
        }

        public static string Fetch(string reference)
        {
            if (_collection == null) return reference;
            return _collection.Fetch(reference) ?? reference;
        }

        private class TranslationStringProvider : IStringProvider
        {
            string IStringProvider.Fetch(string key)
            {
                return Fetch(key);
            }
        }
    }
}
