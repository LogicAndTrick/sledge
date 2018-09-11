namespace Sledge.Common.Translations
{
    public class Language
    {
        public string Code { get; }
        public string Description { get; set; }
        public string Inherit { get; set; }
        internal TranslationStringsCollection Collection { get; }

        public Language(string code)
        {
            Code = code;
            Description = null;
            Inherit = null;
            Collection = new TranslationStringsCollection();
        }
    }
}