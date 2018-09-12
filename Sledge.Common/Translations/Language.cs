namespace Sledge.Common.Translations
{
    /// <summary>
    /// A translation language
    /// </summary>
    public class Language
    {
        /// <summary>
        /// The language code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// A description of the language, ideally with both native and English text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The language code to inherit from, or null if language should not inherit.
        /// For non-English languages, this should be set to 'en' so untranslated strings are English instead of blank.
        /// </summary>
        public string Inherit { get; set; }

        /// <summary>
        /// The string collection for this language
        /// </summary>
        public TranslationStringsCollection Collection { get; }

        public Language(string code)
        {
            Code = code;
            Description = null;
            Inherit = null;
            Collection = new TranslationStringsCollection();
        }
    }
}