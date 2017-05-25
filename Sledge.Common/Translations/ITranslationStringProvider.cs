namespace Sledge.Common.Translations
{
    public interface ITranslationStringProvider
    {
        void Translate(object target);
        string Get(string key);
    }
}