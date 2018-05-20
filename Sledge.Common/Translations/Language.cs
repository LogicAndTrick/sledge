namespace Sledge.Common.Translations
{
    public class Language
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public TranslationStringsCollection Collection { get; set; }

        public Language(string code)
        {
            Code = code;
            Description = null;
            Collection = new TranslationStringsCollection();
        }
    }
}